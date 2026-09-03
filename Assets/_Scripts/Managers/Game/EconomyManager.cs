using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    // Singleton Setup
    public static EconomyManager Instance { get; private set; }

    private int _currentScore = 0;

    // ----------------------------------------------------------

    private void Awake()
    {
        // Singleton Setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Ensure UI starts at 0 when the scene loads
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(_currentScore);
        }
    }

    // Method for increasing the player's score
    public void AddScore(int points)
    {
        _currentScore += points;

        if (UIManager.Instance != null)
        {
            // Update the UI to show the score
            UIManager.Instance.UpdateScore(_currentScore);
        }
    }

    // Getter for the current score
    public int GetCurrentScore()
    {
        return _currentScore;
    }
}
