using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = transform.position;
        }
    }
}
