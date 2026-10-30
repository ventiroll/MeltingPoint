using System.Collections;
using UnityEngine;

// Which element an obstacle is. The player must press the matching key to hit it.
public enum Element { Fire, Water, Air, Earth }

// Put on each obstacle prefab (fire, water, air, earth).
// Moves left, can be attacked with the matching key, and penalises the player if touched while still alive.
public class Obstacle : MonoBehaviour
{
    [Header("Element")]
    [SerializeField] Element element = Element.Fire; // set this on each prefab

    [Header("Combat")]
    [SerializeField] int hitsToKill = 1;     // hits needed to defeat this obstacle
    [SerializeField] int pointsOnKill = 25;  // awarded once, when the obstacle is defeated
    [SerializeField] int touchPenalty = 25;  // lost if the player runs into it before it's killed

    [Header("Sound")]
    [SerializeField] AudioClip defeatSound;  // plays when this obstacle is removed

    [Header("Hit flash")]
    [SerializeField] Color hitFlashColor = Color.red;
    [SerializeField] float flashTime = 0.1f;

    [Header("Cleanup")]
    [SerializeField] float destroyX = -15f;  // off the left edge of the screen

    int hitsTaken;
    bool penaltyApplied;
    SpriteRenderer sr;
    Color originalColor;

    public Element ElementType => element;
    public bool IsDead => hitsTaken >= hitsToKill;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        transform.Translate(Vector2.left * GameManager.Instance.Speed * Time.deltaTime);

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }

    // Called by the player when an attack with the correct key lands
    public void TakeHit()
    {
        if (IsDead) return;

        hitsTaken++;

        if (IsDead)
        {
            GameManager.Instance.AddScore(pointsOnKill);
            GameManager.Instance.PlaySound(defeatSound);
            Destroy(gameObject);
            return;
        }

        if (sr != null)
        {
            StopAllCoroutines();
            StartCoroutine(Flash());
        }
    }

    // Called by the player when it runs into this obstacle
    public void OnPlayerTouched()
    {
        if (IsDead || penaltyApplied) return; // only penalise once per obstacle

        penaltyApplied = true;
        GameManager.Instance.AddScore(-touchPenalty);
    }

    IEnumerator Flash()
    {
        sr.color = hitFlashColor;
        yield return new WaitForSeconds(flashTime);
        sr.color = originalColor;
    }
}