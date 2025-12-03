using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport2Bar : MonoBehaviour
{
    public Transform barEntrance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = barEntrance.position;
        }
    }
}
