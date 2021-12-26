using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public TextMeshProUGUI BalanceText;
    void Start()
    {
        SaveManager.SetDefaultValue("Balance", 0, "PlayerData");
        BalanceText.text = SaveManager.Data["PlayerData"]["Balance"].ToString();
    }
    public int GetBalance()
    {
        return (int)SaveManager.Data["PlayerData"]["Balance"];
    }
    public bool SetBalance(int Total)
    {
        int NewBalance = GetBalance()+Total;
        if (NewBalance < 0 || NewBalance > 99999999)
            return false;
        SaveManager.Data["PlayerData"]["Balance"].Replace(NewBalance);
        BalanceText.text = NewBalance.ToString();
        return true;
    }

}
