// ObjectPooler.cs
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // Sử dụng Singleton pattern để dễ dàng truy cập từ mọi nơi
    public static ObjectPooler Instance;

    [Tooltip("Prefab của đối tượng cần pool")]
    public GameObject objectToPool;
    [Tooltip("Số lượng đối tượng khởi tạo ban đầu")]
    public int amountToPool;

    private List<GameObject> pooledObjects;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false); // Tắt đối tượng đi
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        // Tìm một đối tượng đang không được sử dụng trong pool
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Nếu tất cả đều đang được dùng, tạo thêm một cái mới và thêm vào pool
        // Điều này giúp hệ thống linh hoạt hơn
        GameObject newObj = Instantiate(objectToPool);
        newObj.SetActive(false);
        pooledObjects.Add(newObj);
        Debug.LogWarning("Object Pool is expanding. Consider increasing the initial pool size.");
        return newObj;
    }
}