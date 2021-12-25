using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMessageManager : MonoBehaviour
{
    public TextMeshProUGUI MessageText;
    public bool isVisible
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }
    public string setText
    {
        set { MessageText.text = value; }
    }
}
