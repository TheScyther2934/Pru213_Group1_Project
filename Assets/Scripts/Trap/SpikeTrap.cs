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

    [Tooltip("Thời gian giữa mỗi lần gây sát thương khi Player đứng trên bẫy")]
    public float damageInterval = 1f;
    public bool offsetPhase;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private float timer;
    private int phase; // 0=retracted, 1=extending, 2=extended, 3=retracting
    //private bool playerInside;
    //private PlayerController player;
    private PlayerStats playerStatsInside;
    public int damageAmount = 8;
    private float nextDamageTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;

        if (offsetPhase)
            //timer = Random.Range(0f, retractedDuration + extendedDuration);
            timer = 0f;

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

        if (phase == 2 && playerStatsInside != null && Time.time >= nextDamageTime)
        {
            // Gây sát thương cho người chơi
            playerStatsInside.TakeDamage(damageAmount, transform.position);

            // Đặt lại thời điểm gây sát thương tiếp theo
            nextDamageTime = Time.time + damageInterval;
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
        boxCollider.size = new Vector2(0.7f, 0.7f);
        boxCollider.offset = new Vector2(0, 0.5f);

        switch (phase)
        {
            case 0: spriteRenderer.sprite = retractedSprite; break;
            case 1: spriteRenderer.sprite = halfExtendedSprite; break;
            case 2: spriteRenderer.sprite = fullExtendedSprite; break;
            case 3: spriteRenderer.sprite = halfExtendedSprite; break;
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (phase == 2 && collision.collider.CompareTag("Player"))
    //    {
    //        playerStats = collision.collider.GetComponent<PlayerStats>();
    //        player = collision.collider.GetComponent<PlayerController>();
    //        playerStats.TakeDamage(10);
    //        player.FreezeMovement(true);
    //        playerInside = true;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerStatsInside = other.GetComponent<PlayerStats>();
            nextDamageTime = 0;
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Player") && playerInside)
    //    {
    //        playerInside = false;
    //        player.FreezeMovement(false);
    //    }
    //}

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerStatsInside = null;
        }
    }
}
