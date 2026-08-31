using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Health UI")]
    [SerializeField] private Slider _forestHealthBar;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Endgame Screens")]
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;

    // ---------------------------------------------------------

    private void Awake()
    {
        // Ensure time is flowing
        Time.timeScale = 1f;
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // --- HUD UPDATES ---
    public void UpdateScore(int currentScore)
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {currentScore}";
        }
    }

    public void UpdateWaveText(string waveName)
    {
        if (_waveText != null)
        {
            _waveText.text = waveName;
        }
    }

    public void UpdateTimerText(float timeLeft)
    {
        // Display float as an integer
        if (_timerText != null)
        {
            if (timeLeft <= 0)
            {
                _timerText.text = "";
            }
            else
            {
                _timerText.text = Mathf.CeilToInt(timeLeft).ToString();
            }
        }
    }

    // --- HEALTH ---
    public void UpdateForestBar(float currentHealth, float maxHealth)
    {
        if (_forestHealthBar != null)
        {
            _forestHealthBar.maxValue = maxHealth;
            _forestHealthBar.value = currentHealth;
        }
    }

    // --- END GAME ---

    public void ShowVictoryScreen()
    {
        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ShowDefeatScreen()
    {
        if (_defeatPanel != null)
        {
            _defeatPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
