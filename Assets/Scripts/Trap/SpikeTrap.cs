using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Sprite retractedSprite;
    public Sprite halfExtendedSprite;
    public Sprite fullExtendedSprite;

    public float retractedDuration = 2f;
    public float extendingDuration = 0.3f;
    public float extendedDuration = 2f;
    public float retractingDuration = 0.3f;

    public bool offsetPhase;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private float timer;
    private int phase; // 0=retracted, 1=extending, 2=extended, 3=retracting
    private bool playerInside;
    private PlayerController player;
    private PlayerStats playerStats;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;

        if (offsetPhase)
            timer = Random.Range(0f, retractedDuration + extendedDuration);

        phase = 0;
        UpdateTrapState();
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (phase)
        {
            case 0: if (timer >= retractedDuration) NextPhase(1); break;
            case 1: if (timer >= extendingDuration) NextPhase(2); break;
            case 2: if (timer >= extendedDuration) NextPhase(3); break;
            case 3: if (timer >= retractingDuration) NextPhase(0); break;
        }
    }

    private void NextPhase(int newPhase)
    {
        phase = newPhase;
        timer = 0;
        UpdateTrapState();
    }

    private void UpdateTrapState()
    {
        switch (phase)
        {
            case 0: // retracted
                spriteRenderer.sprite = retractedSprite;
                boxCollider.size = new Vector2(1f, 1f);
                boxCollider.offset = Vector2.zero;
                boxCollider.isTrigger = true; // cho đi qua
                break;

            case 1: // extending
                spriteRenderer.sprite = halfExtendedSprite;
                boxCollider.size = new Vector2(1f, 1f);
                boxCollider.isTrigger = true;
                break;

            case 2: // fully extended
                spriteRenderer.sprite = fullExtendedSprite;
                boxCollider.size = new Vector2(1f, 2f);
                boxCollider.offset = new Vector2(0, 0.5f);
                boxCollider.isTrigger = false; // cản player

                // 🟢 Ép Unity refresh collider để OnCollisionEnter2D được kích hoạt
                boxCollider.enabled = false;
                boxCollider.enabled = true;
                break;


            case 3: // retracting
                spriteRenderer.sprite = halfExtendedSprite;
                boxCollider.size = new Vector2(1f, 1f);
                boxCollider.offset = Vector2.zero;
                boxCollider.isTrigger = true; // cho đi ra dễ
                if (playerInside)
                {
                    playerInside = false;
                    player?.FreezeMovement(false);
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (phase == 2 && collision.collider.CompareTag("Player"))
        {
            playerStats = collision.collider.GetComponent<PlayerStats>();
            player = collision.collider.GetComponent<PlayerController>();
            playerStats.TakeDamage(10);
            player.FreezeMovement(true);
            playerInside = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && playerInside)
        {
            playerInside = false;
            player.FreezeMovement(false);
        }
    }
}
