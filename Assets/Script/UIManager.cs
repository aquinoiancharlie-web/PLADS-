using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // KAILANGAN PARA SA SCENE TRANSITIONS

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Mission Metrics")]
    [SerializeField] private TextMeshProUGUI rescueText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI alertBannerText;
    [SerializeField] private TextMeshProUGUI waterLevelText;

    [Header("End Game Panels")]
    [SerializeField] private GameObject victoryPanel;   // I-drag dito ang Victory UI Panel
    [SerializeField] private GameObject gameOverPanel;  // (Opsyonal) I-drag dito ang Game Over UI Panel

    [Header("Announcement Settings")]
    [SerializeField] private float alertDuration = 5f;

    [Header("Boat Stats")]
    [SerializeField] private Slider hullSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color damagedColor = Color.red;

    [Header("Mission Parameters")]
    [SerializeField] private float missionTimeSeconds = 180f;
    [SerializeField] private int totalVictimsToRescue = 8;

    [Header("Rescue Time Bonus")]
    [SerializeField] private float minBonusSeconds = 15f;
    [SerializeField] private float maxBonusSeconds = 30f;

    private int currentRescuedCount = 0;
    private float currentHullHealth = 100f;
    private bool isTimerRunning = true;
    private bool hasWarnedOneMinute = false;

    private Coroutine alertCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Siguraduhing nakatago muna ang Victory at Game Over panels sa simula
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        UpdateRescueUI();
        UpdateHullUI(100f);
        ShowAlert("NDRRMC ALERT: Flood levels rising. Locate and rescue stranded survivors!", alertDuration);
    }

    private void Update()
    {
        HandleTimer();
    }

    private void HandleTimer()
    {
        if (!isTimerRunning) return;

        if (missionTimeSeconds > 0)
        {
            missionTimeSeconds -= Time.deltaTime;

            if (missionTimeSeconds <= 60f && !hasWarnedOneMinute)
            {
                hasWarnedOneMinute = true;
                ShowAlert("NDRRMC WARNING: 1 MINUTE REMAINING! Evacuate remaining survivors immediately!", alertDuration);
            }

            if (missionTimeSeconds <= 0f)
            {
                missionTimeSeconds = 0f;
                isTimerRunning = false;
                ShowAlert("TIME EXPIRED: Flood waters have breached safety levels!", 0f);
                TriggerGameOver();
            }

            DisplayTime(missionTimeSeconds);
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay = Mathf.Max(0f, timeToDisplay);
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Para sa pag-update ng Water Level Text
    public void UpdateWaterLevelUI(float currentHeight, float maxHeight)
    {
        if (waterLevelText != null)
        {
            waterLevelText.text = $"Water Level: {currentHeight:F1}m / {maxHeight:F1}m";
        }
    }

    public void AddRescuedVictim()
    {
        currentRescuedCount++;
        UpdateRescueUI();

        if (currentRescuedCount >= totalVictimsToRescue)
        {
            isTimerRunning = false;

            if (RisingFlood.Instance != null)
            {
                RisingFlood.Instance.StopFlood();
            }

            ShowAlert("MISSION SUCCESS: All survivors rescued safely!", 0f);
            TriggerVictory();
        }
        else
        {
            int randomBonus = Mathf.RoundToInt(Random.Range(minBonusSeconds, maxBonusSeconds));
            missionTimeSeconds += randomBonus;

            if (missionTimeSeconds > 60f)
            {
                hasWarnedOneMinute = false;
            }

            ShowAlert($"SURVIVOR RESCUED! (+{randomBonus}s Emergency Extra Time)", 3f);
        }
    }

    private void UpdateRescueUI()
    {
        if (rescueText != null)
        {
            rescueText.text = $"Rescued: {currentRescuedCount} / {totalVictimsToRescue}";
        }
    }

    public void UpdateHullUI(float newHealthPercentage)
    {
        currentHullHealth = Mathf.Clamp(newHealthPercentage, 0f, 100f);

        if (hullSlider != null)
        {
            hullSlider.value = currentHullHealth / 100f;
        }

        if (fillImage != null && hullSlider != null)
        {
            fillImage.color = Color.Lerp(damagedColor, healthyColor, hullSlider.value);
        }
    }

    public void ShowAlert(string message, float duration = 5f)
    {
        if (alertBannerText == null) return;

        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
        }

        alertBannerText.text = message;
        alertBannerText.gameObject.SetActive(true);

        if (duration > 0f)
        {
            alertCoroutine = StartCoroutine(HideAlertAfterDelay(duration));
        }
    }

    private IEnumerator HideAlertAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (alertBannerText != null)
        {
            alertBannerText.text = "";
            alertBannerText.gameObject.SetActive(false);
        }
    }

    // VICTORY CONDITION TRIGGER
    private void TriggerVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        UnlockCursorAndPause();
    }

    // GAME OVER CONDITION TRIGGER
    private void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        UnlockCursorAndPause();
    }

    private void UnlockCursorAndPause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // I-pause ang laro
    }

    // PUBLIC FUNCTION: Para sa Main Menu Button
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Ibalik muna ang oras sa normal
        SceneManager.LoadScene("MainMenu");
    }

    // PUBLIC FUNCTION: Para sa Retry / Play Again Button
    public void RestartMission()
    {
        Time.timeScale = 1f; // Ibalik muna ang oras sa normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}