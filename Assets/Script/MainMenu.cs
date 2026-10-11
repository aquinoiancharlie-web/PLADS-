using UnityEngine;
using UnityEngine.SceneManagement; // Kailangan para sa scene transitions

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI Panels (Optional)")]
    [SerializeField] private GameObject howToPlayPanel;

    private void Start()
    {
        // Siguraduhing nakatago ang How To Play panel sa simula
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Lilipat mula Main Menu papuntang Game Scene
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Bubuksan ang How To Play / Instructions Panel
    /// </summary>
    public void OpenHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Isasara ang How To Play / Instructions Panel
    /// </summary>
    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Isasara ang laro (Gumagana lang sa actual Build, hindi sa Editor)
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("QUIT GAME: Isinasara ang laro...");
        Application.Quit();
    }
}