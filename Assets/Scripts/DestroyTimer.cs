using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [Min(0)] public float lifespan;
    public bool deactivate;
    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        if(timer < lifespan)
            timer += Time.deltaTime;
        else
        {
            if (deactivate) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
    }
}
