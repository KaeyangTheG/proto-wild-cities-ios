using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBehavior : MonoBehaviour
{
    public GameObject treeReference;

    static bool planted = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (planted)
        {
            return;
        }
        Destroy(gameObject);
        
        GameObject tree =
            Instantiate(treeReference, transform.position, Quaternion.identity);
        planted = true;
    }
}
