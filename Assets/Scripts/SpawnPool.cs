using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPool : MonoBehaviour
{
    public List<GameObject> pool;

    public GameObject GetTile()
    {
        return pool[Random.Range(0, pool.Count)];
    }
}
