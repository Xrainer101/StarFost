using System.Collections.Generic;
using UnityEngine;

public class RepeatingLayout : MonoBehaviour
{
    public Transform player;
    public List<GameObject> chunkPrefabs;

    public int chunksOnScreen = 4;
    public float chunkLength = 300f;

    private List<GameObject> activeChunks = new List<GameObject>();
    private float spawnZ;

    void Start()
    {
        // Anchor system around player
        spawnZ = player.position.z - chunkLength * 2;

        for (int i = 0; i < chunksOnScreen; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        float despawnZ = player.position.z - chunkLength * 3;

        if (activeChunks.Count > 0)
        {
            GameObject firstChunk = activeChunks[0];

            if (firstChunk.transform.position.z < despawnZ)
            {
                DeleteChunk();
                SpawnChunk();
            }
        }
    }

    void SpawnChunk()
    {
        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];

        Vector3 spawnPos = new Vector3(0, 0, spawnZ);
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);

        activeChunks.Add(go);

        spawnZ += chunkLength;
    }

    void DeleteChunk()
    {
        Destroy(activeChunks[0]);
        activeChunks.RemoveAt(0);
    }
}