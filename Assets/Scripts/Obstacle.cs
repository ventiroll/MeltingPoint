using UnityEngine;

// Put on each obstacle prefab (cactus, rock, etc). Moves left and cleans itself up.
public class Obstacle : MonoBehaviour
{
    [SerializeField] float destroyX = -15f; // off the left edge of the screen

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        transform.Translate(Vector2.left * GameManager.Instance.Speed * Time.deltaTime);

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}
