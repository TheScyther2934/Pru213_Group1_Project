using UnityEngine;

public class ItemTester : MonoBehaviour
{
    public Consumable item;
    PlayerStats ps;

    void Start() => ps = FindObjectOfType<PlayerStats>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log($"🧪 Dùng item: {item.displayName}");
            item.UseOn(ps);
        }
    }
}
