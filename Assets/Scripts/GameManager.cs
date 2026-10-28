using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

// Put on an empty "GameManager" object. Controls speed, score, game over and restart.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Speed")]
    [SerializeField] float startSpeed = 6f;
    [SerializeField] float speedIncreasePerSecond = 0.1f;
    [SerializeField] float maxSpeed = 20f;

    [Header("Score")]
    [SerializeField] int startingScore = 100;   // raise this to give the player a buffer
    [SerializeField] float resetDelay = 1f;   // seconds before the scene resets (0 = instant)

    [Header("UI (optional)")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject gameOverPanel;

    public float Speed { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }

    void Awake()
    {
        Instance = this;
        Speed = startSpeed;
        Score = startingScore;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateScoreText();
    }

    void Update()
    {
        if (IsGameOver)
        {
            var kb = Keyboard.current;
            var pointer = Pointer.current;
            if ((kb != null && (kb.spaceKey.wasPressedThisFrame || kb.rKey.wasPressedThisFrame))
                || (pointer != null && pointer.press.wasPressedThisFrame))
            {
                Restart();
            }
            return;
        }

        // Gradually speed up, like the dino game
        Speed = Mathf.Min(Speed + speedIncreasePerSecond * Time.deltaTime, maxSpeed);
    }

    // Positive amount = points gained, negative = penalty
    public void AddScore(int amount)
    {
        if (IsGameOver) return;

        Score += amount;
        UpdateScoreText();

        // Reset when a penalty drops the score to 0 or below
        if (amount < 0 && Score <= 0)
        {
            GameOver();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null) scoreText.text = "Score: " + Mathf.Max(Score, 0).ToString("D5");
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Invoke(nameof(Restart), resetDelay);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}