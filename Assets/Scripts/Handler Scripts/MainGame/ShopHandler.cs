using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopHandler : MonoBehaviour
{
    public PlayerMessageManager MessageManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MessageManager.setText = "To open the shop, press the ??? key. (It incompleted if you can't tell lol)";
        MessageManager.isVisible = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        MessageManager.isVisible = false;
    }
}
