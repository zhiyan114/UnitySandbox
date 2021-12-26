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
        if (SaveManager.SaveFileExist())
            if (SaveManager.LoadFromDisk())
                DelSavebtn.SetActive(true);
        
    }
    public void StartBtn_Handler()
    {
        SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
    }
}
