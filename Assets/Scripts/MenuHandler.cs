using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public GameObject DelSavebtn;
    private void Start()
    {
        SaveManager.SavePath = Application.persistentDataPath + @"/UnitySandboxSave.dat";
        SaveManager.SetKey = Encoding.UTF8.GetBytes("FeelFreeToModifyTheSaveAndCheat!");
        if(SaveManager.SaveFileExist())
        {
            DelSavebtn.SetActive(true);
        }
    }
    public void DelSavebtn_Handler()
    {
        DelSavebtn.SetActive(false);
        SaveManager.DeleteSave(true);
    }
    public void StartBtn_Handler()
    {
        if(SaveManager.SaveFileExist())
        {
            SaveManager.LoadFromDisk();
        }
        // Load the default settings
        if(!SaveManager.Data.TryGetValue("PlayerData",out JToken _))
        {
            SaveManager.Data.Add("PlayerData", new JObject());
        }
        SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
    }
}
