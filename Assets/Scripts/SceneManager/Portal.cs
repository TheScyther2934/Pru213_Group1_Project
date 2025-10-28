using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Portal : MonoBehaviour
{
    public string targetScene; // Scene name to load
    public TextMeshPro textAbovePortal; // assign the "Press E" text (child object)

    private bool isPlayerNearby = false;

    void Start()
    {
        if (textAbovePortal != null)
            textAbovePortal.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextScene();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (textAbovePortal != null)
                textAbovePortal.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (textAbovePortal != null)
                textAbovePortal.gameObject.SetActive(false);
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("No target scene assigned for portal!");
        }
    }
}
