using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    public List<Transform> SpawnPlaces = new List<Transform>();
    public GameObject enemyPrefab;

    private bool triggered = false;

    void Start()
    {
        // Tự động tìm enemy và SpawnPlace con
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Enemy"))
                enemies.Add(child.gameObject);
            if (child.CompareTag("SpawnPlace"))
                SpawnPlaces.Add(child);
        }
    }

    void Update()
    {
        if (!triggered && AllEnemiesDead())
        {
            triggered = true;
            StartCoroutine(TriggerSpawn());
        }
    }

    bool AllEnemiesDead()
    {
        foreach (var e in enemies)
        {
            if (e != null) return false; // Còn enemy sống
        }
        return true;
    }

    IEnumerator TriggerSpawn()
    {
        yield return new WaitForSeconds(1f); // delay nhỏ
        foreach (var point in SpawnPlaces)
        {
            Instantiate(enemyPrefab, point.position, Quaternion.identity);
        }

        // Sau khi spawn xong, tự hủy script này
        Destroy(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform sp in transform)
        {
            if (sp.CompareTag("SpawnPlace"))
                Gizmos.DrawWireSphere(sp.position, 0.3f);
        }
    }
}
