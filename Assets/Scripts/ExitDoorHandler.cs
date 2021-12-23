using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorHandler : MonoBehaviour
{
    public GameObject PlayerCamera;
    public Rigidbody2D PlayerPhysic;
    public GameObject DefaultCamera;
    private Collider2D ExitDoorCollide;

    void Start()
    {
        ExitDoorCollide = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ExitDoorCollide.enabled = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (PlayerPhysic.velocity.x > 0)
        {
            // Player Exits the base
            DefaultCamera.SetActive(false);
            PlayerCamera.SetActive(true);
        }
        else if (PlayerPhysic.velocity.x < 0)
        {
            // Player Enters the base
            DefaultCamera.SetActive(true);
            PlayerCamera.SetActive(false);
        }
        ExitDoorCollide.enabled = true;
    }
}
