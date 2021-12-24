using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    private void Start()
    {
        SaveManager.SavePath = Application.persistentDataPath + @"/UnitySandboxSave.dat";
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
