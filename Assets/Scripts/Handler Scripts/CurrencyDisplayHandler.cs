using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyDisplayHandler : MonoBehaviour
{
    public TextMeshProUGUI BalanceText;
    void Start()
    {
        BalanceText.text = Economy.Manager.GetBalance.ToString();
        Economy.Manager.BalanceChanged += Manager_BalanceChanged;
    }

    private void Manager_BalanceChanged(object sender, Economy.BalanceChangedEventArgs e)
    {
        BalanceText.text = e.newBalance.ToString();
    }
}
