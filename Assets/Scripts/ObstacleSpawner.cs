using UnityEngine;

// Put on an empty object just off the right edge of the screen, at ground height.
public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] obstaclePrefabs;
    [SerializeField] float minDelay = 1.0f;
    [SerializeField] float maxDelay = 2.5f;

    float timer;

    void Start()
    {
        timer = Random.Range(minDelay, maxDelay);
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver || obstaclePrefabs.Length == 0) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            Instantiate(prefab, transform.position, Quaternion.identity);
            timer = Random.Range(minDelay, maxDelay);
        }
    }
}
