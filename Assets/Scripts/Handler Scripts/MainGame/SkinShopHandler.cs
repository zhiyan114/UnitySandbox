using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ShopManager
{
    [ProtoContract]
    public class Skins
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public int Price { get; set; }

    }
}


public class SkinShopHandler : MonoBehaviour
{
    public PlayerMessageManager MessageManager;
    public GameObject ShopUI;
    // Object for shop
    public SpriteRenderer PlayerSkin;
    public Transform ShopDisplay;
    public Transform ShopTemplate;
    private bool isTouchingShop = false;
    private void Start()
    {
        PlayerSkin.sprite = Resources.Load<Sprite>("Skins/" + SaveManager.Data.CurrentSkin.Name);
        foreach (ShopManager.Skins skin in Economy.Manager.AvailableSkins)
        {
            Transform Template = Instantiate(ShopTemplate,ShopDisplay);
            Template.Find("Title").GetComponent<TextMeshProUGUI>().text = skin.Name;
            Template.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Skins/" + skin.Name);
            Template.Find("Price").GetComponent<TextMeshProUGUI>().text = skin.Price.ToString();
            Template.gameObject.SetActive(true);
        }
    }
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
