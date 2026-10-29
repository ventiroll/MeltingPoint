using System.Collections.Generic;
using UnityEngine;

// Visual-only ground scrolling. Place the ground sprites side by side,
// starting at the camera's left edge, and add this script to each one.
// When a tile is completely off-screen on the left, it snaps onto the right end
// of whichever tile is currently last, so the row can never drift apart.
[RequireComponent(typeof(SpriteRenderer))]
public class GroundScroller : MonoBehaviour
{
    [SerializeField] float overlap = 0.02f; // tiny overlap between tiles to hide seams

    static readonly List<GroundScroller> allTiles = new List<GroundScroller>();

    Camera cam;
    SpriteRenderer sr;

    void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()  { allTiles.Add(this); }
    void OnDisable() { allTiles.Remove(this); }

    // Move in Update...
    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        transform.Translate(Vector2.left * GameManager.Instance.Speed * Time.deltaTime);
    }

    // ...and wrap in LateUpdate, after every tile has moved this frame,
    // so the wrapped tile lines up exactly with the others.
    void LateUpdate()
    {
        // Left edge of the camera's view at this tile's depth
        float depth = transform.position.z - cam.transform.position.z;
        float cameraLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, depth)).x;

        if (sr.bounds.max.x >= cameraLeft) return; // still visible

        // Find the right edge of the last tile in the row
        float rightmostEdge = float.MinValue;
        foreach (GroundScroller tile in allTiles)
        {
            rightmostEdge = Mathf.Max(rightmostEdge, tile.sr.bounds.max.x);
        }

        // Snap this tile's left edge onto it (slightly overlapping)
        float shift = (rightmostEdge - overlap) - sr.bounds.min.x;
        transform.position += new Vector3(shift, 0f, 0f);
    }
}