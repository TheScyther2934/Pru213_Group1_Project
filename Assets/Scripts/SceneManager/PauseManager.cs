using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseManager : MonoBehaviour
{
    public GameObject pauseUI;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                OnEnterPauseUI();
        }
    }

    public void OnEnterPauseUI()
    {
        isPaused = true;
        pauseUI.SetActive(true);
        Time.timeScale = 0f; // ⏸️ Freeze the game
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseUI.SetActive(false);
        Time.timeScale = 1f; // ▶️ Resume the game
    }

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f; // Unpause first

        // ✅ Let GameManager handle cleanup and scene transition
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BackToMainMenu();
        }
        else
        {
            // fallback (if somehow GameManager missing)
            SceneManager.LoadScene("MainMenu");
        }
    }
}
