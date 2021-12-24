using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject SaveMsg;
    void Start()
    {
        InvokeRepeating("DoSave", 30, 15);
    }
    IEnumerator ShowSaveMsg()
    {
        SaveMsg.SetActive(true);
        yield return new WaitForSeconds(3);
        SaveMsg.SetActive(false);
    }
    public void DoSave()
    {
        StartCoroutine(ShowSaveMsg());
        SaveManager.SaveToDisk();
    }
    private void OnApplicationQuit()
    {
        SaveManager.SaveToDisk();
    }
}
