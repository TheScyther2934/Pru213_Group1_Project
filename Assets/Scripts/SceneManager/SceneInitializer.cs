using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.SpawnPlayerAndCamera();
    }
}
