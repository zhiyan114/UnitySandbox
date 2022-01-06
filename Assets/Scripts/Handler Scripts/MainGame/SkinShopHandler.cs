using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using UnityEngine.InputSystem;

public class SkinShopHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerMessageManager MessageManager;
    [SerializeField]
    private GameObject ShopUI;
    // Object for shop
    [SerializeField]
    private SpriteRenderer PlayerSkin;
    [SerializeField]
    private Transform ShopDisplay;
    [SerializeField]
    private Transform ShopTemplate;
    [SerializeField]
    private Transform shopDetailDisplay;
    private bool isTouchingShop = false;
    private string SelectedShopSkin;
    PlayerInput PlrInput;
    private void Awake()
    {
        PlrInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        PlayerSkin.sprite = Resources.Load<Sprite>("Skins/" + SaveManager.Data.CurrentSkin);
        RectTransform LastClone = null;
        foreach (KeyValuePair<string,int> skin in Economy.Manager.AvailableSkins)
        {
            // Need to work on auto scaling and positioning once more skins are added to automate this.
            Transform Template = Instantiate(ShopTemplate,ShopDisplay);
            Template.name = skin.Key+"_Skin";
            Template.Find("Title").GetComponent<TextMeshProUGUI>().text = skin.Key;
            Template.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Skins/" + skin.Key);
            Template.Find("Price").GetComponent<TextMeshProUGUI>().text = skin.Value.ToString();
            Template.GetComponent<Button>().onClick.AddListener(delegate { ShopDetailDisplay_Handler(skin.Key);  });
            RectTransform Template_Rect = Template.GetComponent<RectTransform>();
            if (!(LastClone is null))
            {
                RectTransform Default_Rect = ShopTemplate.GetComponent<RectTransform>();
                float xdiff = Mathf.Abs(Default_Rect.anchorMax.x - Default_Rect.anchorMin.x);
                float ydiff = Mathf.Abs(Default_Rect.anchorMax.y - Default_Rect.anchorMin.y);
                if (LastClone.anchorMax.x < 0.8)
                {
                    Template_Rect.anchorMin = new Vector2((float)(LastClone.anchorMax.x + 0.02), LastClone.anchorMin.y);
                    float xValCalc = (float)(LastClone.anchorMax.x + xdiff + 0.02);
                    Template_Rect.anchorMax = new Vector2((float)(xValCalc >= 1 ? 1 : xValCalc), LastClone.anchorMax.y);
                }
                else
                {
                    // Needs to change Y position to fill in
                    
                    Template_Rect.anchorMin = new Vector2(Default_Rect.anchorMin.x, (float)((Default_Rect.anchorMin.y - ydiff) - 0.02));
                    Template_Rect.anchorMax = new Vector2(Default_Rect.anchorMax.x, (float)(Default_Rect.anchorMin.y - 0.02));
                }
                //Template_Rect.anchorMin = new Vector2((float)((LastClone.anchorMin.x * 2)+0.02), (float)(LastClone.anchorMin.x > 0.95 ? (LastClone.anchorMin.y*2)+0.02 : LastClone.anchorMin.y));
            }
            LastClone = Template.GetComponent<RectTransform>();

            Template.gameObject.SetActive(true);
        }
        // Select current skin by default
        ShopDetailDisplay_Handler(SaveManager.Data.CurrentSkin);
    }
    private void ShopDetailDisplay_Handler(string skinName)
    {
        shopDetailDisplay.Find("Title").GetComponent<TextMeshProUGUI>().text = skinName;
        shopDetailDisplay.Find("Img").GetComponent<Image>().sprite = Resources.Load<Sprite>("Skins/" + skinName);
        shopDetailDisplay.Find("Cost").GetComponent<TextMeshProUGUI>().text = Economy.Manager.AvailableSkins[skinName].ToString();
        TextMeshProUGUI ActionBtnText = shopDetailDisplay.Find("ActionBtn").Find("ActionName").GetComponent<TextMeshProUGUI>();
        SelectedShopSkin = skinName;
        Button ActionBtn = shopDetailDisplay.Find("ActionBtn").GetComponent<Button>();
        if (SaveManager.Data.CurrentSkin == skinName)
        {
            // User already owned the skin and equipped
            ActionBtn.interactable = false;
            ActionBtnText.text = "Equipped";
        } else if(SaveManager.Data.OwnedSkins.Contains(skinName))
        {
            // User already owned the skin but didn't equip it
            ActionBtn.interactable = true;
            ActionBtnText.text = "Equip";
        } else
        {
            // User neither owned the skin or equip it
            ActionBtn.interactable = true;
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
            if (!Economy.Manager.SetBalance(-Economy.Manager.AvailableSkins[SelectedShopSkin]))
            {
                // Not enough balance was detected
                StartCoroutine(LowBalanceMsg());
                return;
            };
            SaveManager.Data.OwnedSkins.Add(SelectedShopSkin);
        }
        // Equip the skin if the user already owns it or after the purchase
        Transform ActionBtn = shopDetailDisplay.Find("ActionBtn");
        PlayerSkin.sprite = Resources.Load<Sprite>("Skins/" + SelectedShopSkin);
        SaveManager.Data.CurrentSkin = SelectedShopSkin;
        ActionBtn.GetComponent<Button>().interactable = false;
        ActionBtn.Find("ActionName").GetComponent<TextMeshProUGUI>().text = "Equipped";
    }
    private IEnumerator LowBalanceMsg()
    {
        Button ActionBtn = shopDetailDisplay.Find("ActionBtn").GetComponent<Button>();
        TextMeshProUGUI ActionBtnText = shopDetailDisplay.Find("ActionBtn").Find("ActionName").GetComponent<TextMeshProUGUI>();
        ActionBtn.interactable = false;
        ActionBtnText.text = "Not Enough Money";
        yield return new WaitForSeconds(3);
        ActionBtn.interactable = true;
        ActionBtnText.text = "Buy";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MessageManager.setText = "To open the shop, press the e key.";
        MessageManager.isVisible = true;
        isTouchingShop = true;
    }
    public void InteractShop(InputAction.CallbackContext cb)
    {
        if (isTouchingShop)
        {
            if (cb.phase == InputActionPhase.Started)
            {
                ShopUI.SetActive(!ShopUI.activeSelf);
                MessageManager.isVisible = !ShopUI.activeSelf;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        ShopUI.SetActive(false);
        MessageManager.isVisible = false;
        isTouchingShop = false;
    }
}
