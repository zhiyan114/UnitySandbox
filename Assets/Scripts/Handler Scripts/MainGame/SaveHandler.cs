using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject SaveMsg;
    private Button SaveBtn;
    void Start()
    {
        SaveBtn = gameObject.GetComponent<Button>();
        InvokeRepeating("DoSave", 30, 15);
    }
    IEnumerator ShowSaveMsg()
    {
        SaveMsg.SetActive(true);
        yield return new WaitForSeconds(3);
        SaveMsg.SetActive(false);
        SaveBtn.interactable = true;
    }
    public void DoSave()
    {
        if (!SaveBtn.interactable) return;
        SaveBtn.interactable = false;
        StartCoroutine(ShowSaveMsg());
        SaveManager.SaveToDisk();
    }
    private void OnApplicationQuit()
    {
        SaveManager.SaveToDisk();
    }
}
