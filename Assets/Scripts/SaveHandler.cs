/*
 * FileName: SaveHandler.cs
 * Author: zhiyan114
 * Description: This file handles all the save data in the game. Should not be called in a quick manner (i.e. calling every frame).
 * ToDo: Better byte[] handling and automatic JObject to Dictionary Conversion
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

static public class SaveHandler
{
    static private string FilePath = Application.persistentDataPath + @"/UnitySandboxSave.dat";
    static private string AesKey = "FeelFreeToModifyTheSaveAndCheat!";
    static public Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();

    /*
     * Description: Save the data into into a save file
     * Return: bool - If save is successful or not
     */
    static public bool SaveToDisk()
    {
        try
        {
            using (Aes AesAlg = Aes.Create())
            {
                File.Delete(FilePath);
                using (FileStream SaveFile = new FileStream(FilePath, FileMode.Create))
                {
                    byte[] RandIV = new byte[16];
                    new System.Random().NextBytes(RandIV);
                    AesAlg.IV = RandIV;
                    AesAlg.Key = Encoding.UTF8.GetBytes(AesKey);
                    AesAlg.Mode = CipherMode.CBC;
                    ICryptoTransform encryptor = AesAlg.CreateEncryptor();
                    string SaveJson = JsonConvert.SerializeObject(Data, Formatting.None);
                    SaveFile.Write(AesAlg.IV, 0, AesAlg.IV.Length);
                    using (CryptoStream EncryptStream = new CryptoStream(SaveFile, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter PlainTextSave = new StreamWriter(EncryptStream))
                        {
                            PlainTextSave.Write(SaveJson);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
    /*
     * Description: Loads the save data from the disk and update SaveData dictionary accordingly. Probably should only be ran once per game session.
     * Return:
     *  true - the data has been successfully loaded
     *  false - The data cannot be loaded
     */
    static public bool LoadFromDisk()
    {
        try
        {
            using (Aes AesAlg = Aes.Create())
            {
                using (FileStream SaveFile = new FileStream(FilePath, FileMode.Open))
                {
                    AesAlg.Key = Encoding.UTF8.GetBytes(AesKey);
                    byte[] IV = new byte[16];
                    SaveFile.Read(IV, 0, IV.Length);
                    AesAlg.IV = IV;
                    AesAlg.Mode = CipherMode.CBC;
                    ICryptoTransform decryptor = AesAlg.CreateDecryptor();
                    using (CryptoStream DecryptStream = new CryptoStream(SaveFile, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader PlainTextLoad = new StreamReader(DecryptStream))
                        {
                            string JsonString = "";
                            while (!PlainTextLoad.EndOfStream)
                            {
                                JsonString += PlainTextLoad.ReadLine();
                            }
                            Data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonString);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
    static public bool SaveFileExist()
    {
        return File.Exists(FilePath);
    }
}