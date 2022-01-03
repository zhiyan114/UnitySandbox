using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ProtoBuf;

[ProtoContract]
public enum ResolutionType
{
    [ProtoEnum]
    FullScreen = 0,
    [ProtoEnum]
    Borderless = 1,
    [ProtoEnum]
    FullHD = 2,
    [ProtoEnum]
    HD = 3,
    [ProtoEnum]
    SD = 4,
}
public class SettingsHandler : MonoBehaviour
{
    private SaveData UserData = SaveManager.Data;
    public GameObject MainSection;
    public TMP_Dropdown ResolutionSelect;
    public void SettingsBtn_Handler()
    {
        ResolutionSelect.value = (int)UserData.ScreenResolution;
        MainSection.SetActive(false);
        gameObject.SetActive(true);

    }
    public void Backbtn_Handler()
    {
        gameObject.SetActive(false);
        MainSection.SetActive(true);
    }
    public void Savebtn()
    {
        
        switch(ResolutionSelect.value)
        {
            case 0:
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.ExclusiveFullScreen);
                UserData.ScreenResolution = ResolutionType.FullScreen;
                break;
            case 1:
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
                UserData.ScreenResolution = ResolutionType.Borderless;
                break;
            case 2:
                Screen.SetResolution(1920, 1080, false);
                UserData.ScreenResolution = ResolutionType.FullHD;
                break;
            case 3:
                Screen.SetResolution(1280, 720, false);
                UserData.ScreenResolution = ResolutionType.HD;
                break;
            case 4:
                Screen.SetResolution(852, 480, false);
                UserData.ScreenResolution = ResolutionType.SD;
                break;
        }
        SaveManager.SaveToDisk();
        Backbtn_Handler();
    }
    
}
