using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs & References")]
    public RoundManager roundPrefab;        // Prefab of one full round (contains all 4 stages)
    public Transform roundParent;           // Parent object to keep hierarchy clean
    public Sprite[] photoPool;              // Pool of character sprites

    [Header("Game Settings")]
    [Tooltip("Horizontal distance between each round prefab")]
    public float roundSpacing = 30f;

    [Tooltip("Sliding speed of each round prefab (units per second)")]
    public float moveSpeed = 3f;

    [Tooltip("Delay before next round spawns (seconds)")]
    public float spawnDelay = 1.5f;

    private int currentRoundIndex = 0;      // For positioning each new round
    private bool isSpawning = false;

    void Start()
    {
        Debug.Log("GameManager Start() running...");
        SpawnNewRound();
    }

    /// <summary>
    /// Spawns a new full round prefab and initializes it.
    /// </summary>
    public void SpawnNewRound()
    {
        if (isSpawning) return;
        isSpawning = true;

        Debug.Log($"[GameManager] Spawning round {currentRoundIndex + 1}...");

        // Instantiate a new RoundManager prefab
        RoundManager newRound = Instantiate(roundPrefab, roundParent);

        // Position offset (based on index * spacing)
        Vector3 startPos = new Vector3(currentRoundIndex * roundSpacing, 0, 0);
        newRound.transform.localPosition = startPos;

        // Apply movement speed to round
        newRound.moveSpeed = moveSpeed;

        // Initialize with character pool and reference to GameManager
        newRound.Init(photoPool, this);

        currentRoundIndex++;
        isSpawning = false;
    }

    /// <summary>
    /// Called from RoundManager after one round finishes.
    /// </summary>
    public void OnRoundComplete()
    {
        Debug.Log("[GameManager] Round complete. Scheduling next round...");
        StartCoroutine(SpawnNextAfterDelay());
    }

    private System.Collections.IEnumerator SpawnNextAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnNewRound();
    }
}
