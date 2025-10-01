using System.Collections.Generic;
using UnityEngine;

public class OceanGenerator : MonoBehaviour
{
    public GameObject oceanTilePrefab;
    public int radius = 5;// Ocean Radius
    public float height = 0f;// Where the ocean is placed

    private List<GameObject> tilePool = new List<GameObject>();
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        GeneratePool();
        UpdateTiles();
    }

    private void Update()
    {
        UpdateTiles();
    }

    // Generates the pool of OceanTiles
    private void GeneratePool()
    {
        int totalTiles = (radius * 2 + 1) * (radius * 2 + 1);
        for (int i = 0; i < totalTiles; i++)
        {
            GameObject tile = Instantiate(oceanTilePrefab, Vector3.zero, Quaternion.identity);
            tilePool.Add(tile);
        }
    }


    //  Moves tiles in the pool that are where the player is heading away from to where the player is going
    private void UpdateTiles()
    {
        int index = 0;
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector3 pos = new Vector3(
                    Mathf.Floor(player.position.x / 10 + x) * 10,
                    height,
                    Mathf.Floor(player.position.z / 10 + z) * 10
                );
                tilePool[index].transform.position = pos;
                index++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius * 10);
    }
}
