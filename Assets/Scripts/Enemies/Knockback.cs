using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool gettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private LayerMask wallLayerMask = 1; // Default layer mask
    [SerializeField] private float checkDistance = 0.1f; // Distance to check for walls ahead

    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        gettingKnockedBack = true;
        Vector2 knockDirection = (transform.position - damageSource.position).normalized;
        
        // Use velocity instead of AddForce to have better control
        // Divide by mass to prevent massive objects from flying too far
        float finalForce = knockBackThrust / rb.mass;
        rb.velocity = knockDirection * finalForce;
        
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        float elapsed = 0f;
        Vector2 startVelocity = rb.velocity;
        
        while (elapsed < knockBackTime)
        {
            // Decelerate the knockback over time
            float progress = elapsed / knockBackTime;
            rb.velocity = Vector2.Lerp(startVelocity, Vector2.zero, progress);
            
            // Check for wall collision to stop early
            if (CheckWallAhead())
            {
                rb.velocity = Vector2.zero;
                break;
            }
            
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        rb.velocity = Vector2.zero;
        gettingKnockedBack = false;
    }

    private bool CheckWallAhead()
    {
        if (col == null) return false;

        // Check for walls in the direction of movement
        Vector2 movementDir = rb.velocity.normalized;
        Vector2 checkOrigin = (Vector2)transform.position + movementDir * (checkDistance + col.bounds.extents.magnitude);
        
        // Use OverlapCircle to check if there's a wall at the destination
        Collider2D hitCollider = Physics2D.OverlapCircle(checkOrigin, checkDistance, wallLayerMask);
        
        // Also do raycast for more precise detection
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movementDir, 
            checkDistance + col.bounds.extents.magnitude, wallLayerMask);
        
        return hitCollider != null || hit.collider != null;
    }
}
