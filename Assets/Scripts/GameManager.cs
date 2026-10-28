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

    [Header("UI (optional)")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject gameOverPanel;

    public float Speed { get; private set; }
    public bool IsGameOver { get; private set; }

    float score;

    void Awake()
    {
        Instance = this;
        Speed = startSpeed;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
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

        // Score = distance travelled
        score += Speed * Time.deltaTime;
        if (scoreText != null) scoreText.text = Mathf.FloorToInt(score).ToString("D5");
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
