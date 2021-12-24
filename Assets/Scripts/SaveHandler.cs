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
using Newtonsoft.Json.Linq;

static public class SaveHandler
{
    static private string FilePath = Application.persistentDataPath + @"/UnitySandboxSave.dat";
    static private string AesKey = "FeelFreeToModifyTheSaveAndCheat!";
    static public JObject Data = new JObject();

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
                    SaveFile.Write(AesAlg.IV, 0, AesAlg.IV.Length);
                    using (CryptoStream EncryptStream = new CryptoStream(SaveFile, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter PlainTextSave = new StreamWriter(EncryptStream))
                        {
                            JObject OrganizedSave = (JObject)Data.DeepClone();
                            PrepareSerialize(OrganizedSave);
                            PlainTextSave.Write(OrganizedSave.ToString());
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
                            Data = JObject.Parse(JsonString);
                            PrepareDeserialize(Data);
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
     * Description: Prepare the object for disk saving
     */
    static private void PrepareSerialize(JObject dict)
    {
        foreach (KeyValuePair<string, JToken> item in dict)
        {
            switch(item.Value.Type)
            {
                case JTokenType.Bytes:
                    {
                        dict[item.Key] = new JObject();
                        dict[item.Key]["RealType"] = "base64";
                        dict[item.Key]["Value"] = Convert.ToBase64String((byte[])item.Value);
                        break;
                    }
                case JTokenType.Object:
                    {
                        PrepareSerialize((JObject)item.Value);
                        break;
                    }
                case JTokenType.Array:
                    {
                        PrepareSerialize((JArray)item.Value);
                        break;
                    }
            }
        }
    }
    static private void PrepareSerialize(JArray arr)
    {
        
        for (int i = 0; i < arr.Count; i++)
        {
            JToken item = arr[i];
            switch (item.Type)
            {
                case JTokenType.Bytes:
                    {
                        arr[i] = new JObject();
                        arr[i]["RealType"] = "base64";
                        arr[i]["Value"] = Convert.ToBase64String((byte[])item);
                        break;
                    }
                case JTokenType.Object:
                    {
                        PrepareSerialize((JObject)item);
                        break;
                    }
                case JTokenType.Array:
                    {
                        PrepareSerialize((JArray)item);
                        break;
                    }
            }
        }
    }
    /*
     * Description: Prepare Object for use
     */
    static private void PrepareDeserialize(JObject dict)
    {
        foreach (KeyValuePair<string, JToken> item in dict)
        {
            switch (item.Value.Type)
            {
                case JTokenType.Object:
                    {
                        JObject Obj = (JObject)item.Value;

                        if(Obj.TryGetValue("RealType",out JToken data))
                        {
                            if((string)data == "base64")
                            {
                                dict[item.Key] = Convert.FromBase64String((string)Obj["Value"]);
                            }
                            break;
                        }
                        PrepareDeserialize((JObject)Obj);
                        break;
                    }
                case JTokenType.Array:
                    {
                        PrepareDeserialize((JArray)item.Value);
                        break;
                    }
            }
        }
    }
    static private void PrepareDeserialize(JArray arr)
    {
        for (int i = 0; i < arr.Count; i++)
        {
            JToken item = arr[i];
            switch (item.Type)
            {
                case JTokenType.Object:
                    {
                        JObject Obj = (JObject)item;

                        if (Obj.TryGetValue("RealType", out JToken data))
                        {
                            if ((string)data == "base64")
                            {
                                arr[i] = Convert.FromBase64String((string)Obj["Value"]);
                            }
                            break;
                        }
                        PrepareDeserialize((JObject)Obj);
                        break;
                    }
                case JTokenType.Array:
                    {
                        PrepareDeserialize((JArray)item);
                        break;
                    }
            }
        }
    }
    static public bool SaveFileExist()
    {
        return File.Exists(FilePath);
    }
}