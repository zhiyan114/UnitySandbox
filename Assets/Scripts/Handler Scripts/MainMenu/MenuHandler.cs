using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public GameObject DelSavebtn;
    private static bool Loaded = false;
    private void Start()
    {
        if (!Loaded)
        {
            Loaded = true;
            SaveManager.SavePath = Application.persistentDataPath + @"/UnitySandboxSave.bin";
            SaveManager.SetKey = Encoding.UTF8.GetBytes("FeelFreeToModifyTheSaveAndCheat!");
            if (SaveManager.SaveFileExist())
                if (SaveManager.LoadFromDisk())
                    DelSavebtn.SetActive(true);
            switch (SaveManager.Data.ScreenResolution)
            {
                case ResolutionType.FullScreen:
                    Screen.SetResolution(Screen.width, Screen.height, true);
                    break;
                case ResolutionType.FullHD:
                    Screen.SetResolution(1920, 1080, false);
                    break;
                case ResolutionType.HD:
                    Screen.SetResolution(1280, 720, false);
                    break;
                case ResolutionType.SD:
                    Screen.SetResolution(852, 480, false);
                    break;
            } 
        }
        else
            DelSavebtn.SetActive(true);
    }
    public void StartBtn_Handler()
    {
        SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
    }
    public void QuitBtn_Handler()
    {
        Application.Quit();
    }
}
