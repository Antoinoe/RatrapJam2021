using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    public bool inUse = false;
    protected LayerMask holdLayer;
    protected Collider2D col2D;
    protected bool started = false;

    private void Start()
    {
        if (!started) Init();
    }

    protected virtual void Init()
    {
        holdLayer = gameObject.layer;
        col2D = GetComponent<Collider2D>();

        started = true;
    }

    public virtual void Holding()
    {
        inUse = true;
        gameObject.layer = 0;
    }

    public void Release()
    {
        inUse = false;
        gameObject.layer = holdLayer;
    }
}
