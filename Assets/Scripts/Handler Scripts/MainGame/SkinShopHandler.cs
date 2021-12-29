using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkinShopHandler : MonoBehaviour
{
    public PlayerMessageManager MessageManager;
    public GameObject ShopUI;
    private bool isTouchingShop = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MessageManager.setText = "To open the shop, press the e key.";
        MessageManager.isVisible = true;
        isTouchingShop = true;
    }
    private void Update()
    {
        if(isTouchingShop)
            if (Input.GetKeyDown(KeyCode.E))
            {
                ShopUI.SetActive(!ShopUI.activeSelf);
                MessageManager.isVisible = !ShopUI.activeSelf;
            }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        ShopUI.SetActive(false);
        MessageManager.isVisible = false;
        isTouchingShop = false;
    }
}
