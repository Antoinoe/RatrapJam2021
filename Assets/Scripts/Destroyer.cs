using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public LayerMask toDestroy;

    private void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((toDestroy.value & (1 << collision.gameObject.layer)) > 0)  // Comparaison : Is 'gameObject.layer' Inside of 'toDestroy' LayerMask
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((toDestroy.value & (1 << collision.gameObject.layer)) > 0)
        {
            Destroy(collision.gameObject);
        }
    }
}
