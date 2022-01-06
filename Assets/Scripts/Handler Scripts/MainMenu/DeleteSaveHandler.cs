using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveHandler : MonoBehaviour
{
    [SerializeField]
    private Transform BtnCollection;
    public void DelSavebtn_Handler()
    {
        foreach (Transform btn in BtnCollection)
            btn.GetComponent<Button>().interactable = false;
        gameObject.SetActive(true);
        
    }
    public void Declinebtn_Handler()
    {
        gameObject.SetActive(false);
        foreach (Transform btn in BtnCollection)
            btn.GetComponent<Button>().interactable = true;
    }
    public void Confirmbtn_Handler()
    {
        SaveManager.DeleteSave(true);
        gameObject.SetActive(false);
        BtnCollection.Find("DelSaveBtn").gameObject.SetActive(false);
        foreach (Transform btn in BtnCollection)
            btn.GetComponent<Button>().interactable = true;
    }
}
