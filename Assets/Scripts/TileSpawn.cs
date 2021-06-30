using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawn : MonoBehaviour
{
    private static TileSpawn inst;
    public static TileSpawn Get { get { return inst; } }

    // Pool
    public SpawnPool pool;

    // Spawn behaviour
    public float tileHeight = 60;  // 15 Tiles assets = ...m ?
    float lastSpawn = 0;


    // Start is called before the first frame update
    void Awake()
    {
        if (inst == null) inst = this;
    }

    public void Spawn()
    {
        Instantiate(pool.GetTile(), new Vector2(0, lastSpawn + 2 * tileHeight), Quaternion.identity);
        lastSpawn += tileHeight;
    }

    public void ReachNextSpawn(float highestPoint)
    {
        if (highestPoint >= lastSpawn + tileHeight)
            Spawn();
    }
}
