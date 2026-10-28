using UnityEngine;

// Visual-only ground scrolling. Place the ground sprites side by side with no gaps,
// starting at the camera's left edge, and add this script to each one.
// A tile wraps to the back of the line only once it is completely off-screen on the left.
[RequireComponent(typeof(SpriteRenderer))]
public class GroundScroller : MonoBehaviour
{
    [SerializeField] float tileWidth = 0f; // leave at 0 to read it from the sprite automatically
    [SerializeField] int tileCount = 3;    // how many ground sprites are in the loop

    Camera cam;
    SpriteRenderer sr;

    void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();

        if (tileWidth <= 0f)
        {
            tileWidth = sr.bounds.size.x;
        }
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        transform.Translate(Vector2.left * GameManager.Instance.Speed * Time.deltaTime);

        // Left edge of the camera's view at this tile's depth
        // (works for both orthographic and perspective cameras)
        float depth = transform.position.z - cam.transform.position.z;
        float cameraLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, depth)).x;

        // The sprite's actual right edge, whatever its pivot point is
        float tileRight = sr.bounds.max.x;

        if (tileRight < cameraLeft)
        {
            transform.position += new Vector3(tileWidth * tileCount, 0f, 0f);
        }
    }
}