using UnityEngine;
public class PauseMenu: MonoBehaviour
{
    public static bool IsGamePaused { get; private set; } = false;

    [SerializeField] private GameObject pausePanel;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void TogglePause()
    {
        if (IsGamePaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        IsGamePaused = true;

        
        if (pausePanel != null)
            pausePanel.SetActive(true);

        
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsGamePaused = false;

  
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}


