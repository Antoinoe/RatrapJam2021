using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector2 offset;
    
    public bool lockX;
    public bool lockY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float X = (lockX) ? transform.position.x : target.position.x + offset.x;
        float Y = (lockY) ? transform.position.y : target.position.y + offset.y;

        transform.position = new Vector2(X, Y);
    }
}
