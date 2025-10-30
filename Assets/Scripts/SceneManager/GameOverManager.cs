using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    private PlayerStats playerStats;

    void Start()
    {
        // Tìm player và lấy component PlayerStats
        playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats != null)
        {
            // Đăng ký sự kiện lắng nghe OnDeath
            playerStats.OnDeath.AddListener(ShowGameOverUI);
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerStats not found in scene.");
        }
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnDeath.RemoveListener(ShowGameOverUI);
        }
    }

    private void ShowGameOverUI()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Debug.Log("💀 Game Over triggered");
        }
        else
        {
            Debug.LogWarning("⚠️ GameOverUI is not assigned in GameOverHandler!");
        }
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
