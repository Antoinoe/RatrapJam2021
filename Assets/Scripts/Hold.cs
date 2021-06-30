using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    public bool inUse = false;
    LayerMask holdLayer;

    Collider2D col2D;

    // Start is called before the first frame update
    void Start()
    {
        holdLayer = gameObject.layer;
        col2D = GetComponent<Collider2D>();   
    }

    public void Holding()
    {
        gameObject.layer = 0;
    }

    public void Release()
    {
        gameObject.layer = holdLayer;
    }
}
