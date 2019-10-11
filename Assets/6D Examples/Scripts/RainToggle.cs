using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainToggle : MonoBehaviour
{
    public GameObject rainRef;

    private GameObject rainInstance;
    private bool raining = false;
    
    public void Toggle()
    {
        if (!rainRef)
        {
            return;
        }
        raining = !raining;
        if (raining)
        {
            rainInstance = Instantiate(rainRef, transform.position, transform.rotation);
        } else if (rainInstance)
        {
            Destroy(rainInstance);
        }
    }
}
