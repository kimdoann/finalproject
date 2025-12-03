using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamSwitch2Bar : MonoBehaviour
{
    public CinemachineVirtualCamera VcamBar;
    public CinemachineVirtualCamera VcamGraveyard;
    // Start is called before the first frame update
    void Start()
    {
        VcamBar.Priority = 5;
        VcamGraveyard.Priority = 10;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            VcamGraveyard.Priority = 5;
            VcamBar.Priority = 10;
        }
    }
}
