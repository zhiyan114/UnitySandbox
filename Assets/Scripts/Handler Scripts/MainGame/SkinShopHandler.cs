using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ShopManager
{
    [ProtoContract]
    public class Skin
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
    public Transform shopDetailDisplay;
    private bool isTouchingShop = false;
    private ShopManager.Skin SelectedShopSkin;
    private void Start()
    {
        PlayerSkin.sprite = Resources.Load<Sprite>("Skins/" + SaveManager.Data.CurrentSkin.Name);
        foreach (ShopManager.Skin skin in Economy.Manager.AvailableSkins)
        {
            // Need to work on auto scaling and positioning once more skins are added to automate this.
            Transform Template = Instantiate(ShopTemplate,ShopDisplay);
            Template.name = skin.Name+"_Skin";
            Template.Find("Title").GetComponent<TextMeshProUGUI>().text = skin.Name;
            Template.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Skins/" + skin.Name);
            Template.Find("Price").GetComponent<TextMeshProUGUI>().text = skin.Price.ToString();
            Template.GetComponent<Button>().onClick.AddListener(delegate { ShopDetailDisplay_Handler(skin);  });
            Template.gameObject.SetActive(true);
        }
        // Select current skin by default
        ShopDetailDisplay_Handler(SaveManager.Data.CurrentSkin);
    }
    private void ShopDetailDisplay_Handler(ShopManager.Skin skin)
    {
        shopDetailDisplay.Find("Title").GetComponent<TextMeshProUGUI>().text = skin.Name;
        shopDetailDisplay.Find("Img").GetComponent<Image>().sprite = Resources.Load<Sprite>("Skins/" + skin.Name);
        shopDetailDisplay.Find("Cost").GetComponent<TextMeshProUGUI>().text = skin.Price.ToString();
        TextMeshProUGUI ActionBtnText = shopDetailDisplay.Find("ActionBtn").Find("ActionName").GetComponent<TextMeshProUGUI>();
        SelectedShopSkin = skin;
        if(SaveManager.Data.CurrentSkin == skin)
        {
            // User already owned the skin and equipped
            shopDetailDisplay.Find("ActionBtn").GetComponent<Button>().interactable = false;
            ActionBtnText.text = "Equipped";
        } else if(SaveManager.Data.OwnedSkins.Contains(skin))
        {
            // User already owned the skin but didn't equip it
            ActionBtnText.text = "Equip";
        } else
        {
            // User neither owned the skin or equip it
            ActionBtnText.text = "Buy";
        }
    }
    public void ShopDetailAction_Handler()
    {
        // If any of those returns then there is something wrong with the game. It added there just in-case something does go wrong.
        if (SelectedShopSkin is null) return;
        if (SelectedShopSkin == SaveManager.Data.CurrentSkin) return;
        if (!SaveManager.Data.OwnedSkins.Contains(SelectedShopSkin))
        {
            // Perform skin purchase
            if (!Economy.Manager.SetBalance(-SelectedShopSkin.Price))
            {
                // Not enough balance was detected
                StartCoroutine(LowBalanceMsg());
                return;
            };
        }
        // Equip the skin if the user already owns it or after the purchase
        Transform ActionBtn = shopDetailDisplay.Find("ActionBtn");
        PlayerSkin.sprite = Resources.Load<Sprite>("Skins/" + SelectedShopSkin.Name);
        SaveManager.Data.CurrentSkin = SelectedShopSkin;
        ActionBtn.GetComponent<Button>().interactable = false;
        ActionBtn.Find("ActionName").GetComponent<TextMeshProUGUI>().text = "Equipped";
    }
    private IEnumerator LowBalanceMsg()
    {
        TextMeshProUGUI ActionBtnText = shopDetailDisplay.Find("ActionBtn").Find("ActionName").GetComponent<TextMeshProUGUI>();
        ActionBtnText.text = "Not Enough Money";
        yield return new WaitForSeconds(3);
        ActionBtnText.text = "Buy";
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
