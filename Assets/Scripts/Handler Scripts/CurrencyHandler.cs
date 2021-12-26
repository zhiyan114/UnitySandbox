using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyHandler : MonoBehaviour
{
    public TextMeshProUGUI BalanceText;
    void Start()
    {
        BalanceText.text = SaveManager.Data.Balance.ToString();
    }
    private void Update()
    {
        BalanceText.text = SaveManager.Data.Balance.ToString();
    }

}
