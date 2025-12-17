using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform[] mutantWaypoints;

    [Header("Enemy Prefabs")]
    public GameObject mutant1Prefab;   // "Mutant 1" - big guy
    public GameObject mutant2Prefab;   // "Mutant 2" - medium
    public GameObject zombiePrefab;    // "Zombie" - small

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnDelay = 0.3f;

    private int currentRound = 0;
    private bool isSpawning = false;

    private struct WaveConfig
    {
        public int mutant1Count;
        public int mutant2Count;
        public int zombieCount;

        public WaveConfig(int m1, int m2, int z)
        {
            mutant1Count = m1;
            mutant2Count = m2;
            zombieCount = z;
        }
    }

    private WaveConfig[] waves;

    void Awake()
    {
        // Gradual difficulty:
        // Round 1: super easy
        // Round 2–4: slowly add more and tougher enemies

        waves = new WaveConfig[]
        {
            // Round 1: VERY EASY
            // No big Mutant 1 yet. Just 1 Mutant 2 and a few zombies.
            new WaveConfig(
                0,  // Mutant 1
                1,  // Mutant 2
                4   // Zombies
            ),

            // Round 2: EASY
            new WaveConfig(
                1,  // Mutant 1
                1,  // Mutant 2
                6   // Zombies
            ),

            // Round 3: MEDIUM
            new WaveConfig(
                1,  // Mutant 1
                2,  // Mutant 2
                8   // Zombies
            ),

            // Round 4: HARD
            new WaveConfig(
                2,   // Mutant 1
                3,   // Mutant 2
                10   // Zombies
            ),
        };
    }

    void Start()
    {
        StartNextRound();
    }

    void Update()
    {
        if (!isSpawning && NoEnemiesAlive())
        {
            StartNextRound();
        }
    }

    void StartNextRound()
    {
        currentRound++;
        Debug.Log("Starting Round: " + currentRound);

        // Rounds 1–4 = normal mixed waves
        if (currentRound >= 1 && currentRound <= 4)
        {
            WaveConfig wave = waves[currentRound - 1];
            StartCoroutine(SpawnWave(wave));
        }
        // Round 5 = "Boss" wave: lots of Mutant 1, still some others
        else if (currentRound == 5)
        {
            WaveConfig bossWave = new WaveConfig(
                5,   // Mutant 1 (main threat)
                4,   // Mutant 2
                14   // Zombies
            );

            StartCoroutine(SpawnWave(bossWave));
        }
        else
        {
            Debug.Log("All waves complete! You WIN!");
        }
    }

    bool NoEnemiesAlive()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    IEnumerator SpawnWave(WaveConfig wave)
    {
        isSpawning = true;

        List<GameObject> toSpawn = new List<GameObject>();

        for (int i = 0; i < wave.mutant1Count; i++)
            toSpawn.Add(mutant1Prefab);

        for (int i = 0; i < wave.mutant2Count; i++)
            toSpawn.Add(mutant2Prefab);

        for (int i = 0; i < wave.zombieCount; i++)
            toSpawn.Add(zombiePrefab);

        foreach (GameObject enemy in toSpawn)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject obj = Instantiate(enemy, sp.position, sp.rotation);

            // ✅ Assign waypoints to any enemy that uses EnemyAI
            EnemyAI ai = obj.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.AssignWaypoints(mutantWaypoints);
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }
}
