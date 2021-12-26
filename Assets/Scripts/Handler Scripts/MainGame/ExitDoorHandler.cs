using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorHandler : MonoBehaviour
{
    public GameObject PlayerCamera;
    public Rigidbody2D PlayerPhysic;
    public GameObject DefaultCamera;
    private JObject UserData = (JObject)SaveManager.Data.GetValue("PlayerData");

    void Start()
    {
        if (UserData.TryGetValue("isOutdoor", out JToken isOutdoor_Token))
        {
            bool isOutdoor = isOutdoor_Token.ToObject<bool>();
            if (isOutdoor)
            {
                PlayerCamera.SetActive(true);
                DefaultCamera.SetActive(false);
            }
        }
    }
    private void AutoSetCam()
    {
        if (PlayerPhysic.velocity.x > 0)
        {
            // Player Exits the base
            DefaultCamera.SetActive(false);
            PlayerCamera.SetActive(true);
            UserData["isOutdoor"] = true;
        }
        else if (PlayerPhysic.velocity.x < 0)
        {
            // Player Enters the base
            DefaultCamera.SetActive(true);
            PlayerCamera.SetActive(false);
            UserData["isOutdoor"] = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AutoSetCam();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        AutoSetCam();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        AutoSetCam();
    }
}