using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ArrowTrap : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite holeSprite;           // lỗ trống
    public Sprite holeWithTipSprite;    // lỗ + mũi tên ló ra (prepare)

    [Header("Fire Settings")]
    public GameObject arrowPrefab;      // prefab ArrowProjectile
    public Transform firePoint;         // vị trí spawn (child)
    public float fireInterval = 2.0f;   // tổng thời gian giữa 2 phát
    public float prepareTime = 0.25f;   // thời gian hiện holeWithTip trước khi bắn

    [Header("Projectile overrides (optional)")]
    public float arrowSpeedOverride = 0f;
    public float arrowLifeOverride = 0f;

    [Header("Order / Visual (must match ArrowProjectile)")]
    public int arrowOrderWhenBehind = 0;  // set arrow prefab's "behind" order
    public int arrowOrderWhenFront = 25;  // set arrow prefab's "front" order (should > trap order)
    public float arrowDistanceToSwitch = 0.25f;

    [Header("Top-down direction (set in Inspector)")]
    public Vector2 fireDirection = Vector2.right; // use (1,0), (-1,0), (0,1), (0,-1)

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && holeSprite != null) sr.sprite = holeSprite;
    }

    void Start()
    {
        StartCoroutine(FireLoop());
    }

    IEnumerator FireLoop()
    {
        while (true)
        {
            // idle state
            if (sr != null && holeSprite != null) sr.sprite = holeSprite;
            yield return new WaitForSeconds(Mathf.Max(0.01f, fireInterval - prepareTime));

            // prepare (show tip)
            if (sr != null && holeWithTipSprite != null) sr.sprite = holeWithTipSprite;
            yield return new WaitForSeconds(Mathf.Max(0.01f, prepareTime));

            // fire
            if (sr != null && holeSprite != null) sr.sprite = holeSprite;
            SpawnArrow();
        }
    }

    void SpawnArrow()
    {
        if (arrowPrefab == null || firePoint == null) return;

        GameObject a = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        var proj = a.GetComponent<ArrowProjectile>();
        if (proj != null)
        {
            // override visual order settings
            proj.orderWhenBehind = arrowOrderWhenBehind;
            proj.orderWhenFront = arrowOrderWhenFront;
            proj.distanceToSwitch = arrowDistanceToSwitch;

            // override speed/lifetime optionally
            if (arrowSpeedOverride > 0f) proj.speed = arrowSpeedOverride;
            if (arrowLifeOverride > 0f) proj.lifetime = arrowLifeOverride;

            proj.Launch(fireDirection.normalized);
        }
    }
}
