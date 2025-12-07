using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CamSwitch2Graveyard : MonoBehaviour
{
    public CinemachineVirtualCamera VcamBar;
    public CinemachineVirtualCamera VcamGraveyard;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            VcamBar.Priority = 10;
            VcamGraveyard.Priority = 5;
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            VcamGraveyard.Priority = 10;
            VcamBar.Priority = 5;
        }
    }
}
