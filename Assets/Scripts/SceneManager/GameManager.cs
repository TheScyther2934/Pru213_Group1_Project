using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject playerPrefab;
    public GameObject cameraPrefab;

    private GameObject playerInstance;
    private GameObject cameraInstance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartNewGame()
    {
        // If a player/camera still exist from previous run, remove them
        if (playerInstance != null) Destroy(playerInstance);
        if (cameraInstance != null) Destroy(cameraInstance);

        // Start first map
        SceneManager.LoadScene("Map1");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map1")
        {
            SpawnPlayerAndCamera();
            StartCoroutine(ShowIntroDialogue());
        }
        
    }
    
    private IEnumerator ShowIntroDialogue()
    {
        yield return new WaitForSeconds(1f); // small delay to allow PlayerDialogue to initialize

        List<string> lines = new List<string>
        {
            "Finally, I manage to get inside this dungeon",
            "The World Tree Heart that got stolen from us is somewhere inside this place",
            "The World Tree missing its heart will make everything fallen into chaos",
            "I need to hurry, the fate of the whole world depends on me"
        };

        if (PlayerDialogue.Instance != null)
            PlayerDialogue.Instance.ShowDialogue(lines);
        else
            Debug.LogWarning("PlayerDialogue instance not found!");
    }
    public void SpawnPlayerAndCamera()
    {
        // Only spawn if not already present
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
            DontDestroyOnLoad(playerInstance);
        }

        if (cameraInstance == null)
        {
            cameraInstance = Instantiate(cameraPrefab);
            DontDestroyOnLoad(cameraInstance);
        }

        // Reassign camera target
        CameraFollow follow = cameraInstance.GetComponent<CameraFollow>();
        if (follow != null)
        {
            follow.target = playerInstance.transform;
        }
    }

    public void BackToMainMenu()
    {
        // Destroy persistent objects (player, camera)
        if (playerInstance != null)
        {
            Destroy(playerInstance);
            playerInstance = null;
        }

        if (cameraInstance != null)
        {
            Destroy(cameraInstance);
            cameraInstance = null;
        }

        Destroy(gameObject);
        Instance = null;
        SceneManager.LoadScene("MainMenu");
    }

}
