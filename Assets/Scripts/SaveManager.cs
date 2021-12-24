/*
 * FileName: SaveManager.cs
 * Author: zhiyan114
 * Description: This file handles all the save data in the game.
 * 
 * Supported Object: Byte Array (automatic base64 conversion), int/double, String, Boolean, Array, and Dictionary.
 * Warning: SaveManager.Data MUST BE JObject. Changing it to JArray will break the internal (which is automatic byte array converter)
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

static public class SaveManager
{
    static private string FilePath = "Default_Save.dat";
    static public string SavePath
    {
        set { FilePath = value; }
        get { return FilePath; }
    }
    static private byte[] AesKey = new byte[16];
    static public byte[] SetKey
    {
        set
        {
            switch(value.Length)
            {
                case 16:
                    break;
                case 24:
                    break;
                case 32:
                    break;
                default:
                    throw new CryptographicException("AES Key size must be either 16, 24, 32 byte long.");
            }
            AesKey = value;
        }
    }
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
                    AesAlg.Key = AesKey;
                    AesAlg.Mode = CipherMode.CBC;
                    ICryptoTransform encryptor = AesAlg.CreateEncryptor();
                    SaveFile.Write(AesAlg.IV, 0, AesAlg.IV.Length);
                    using (CryptoStream EncryptStream = new CryptoStream(SaveFile, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter PlainTextSave = new StreamWriter(EncryptStream))
                        {
                            JObject OrganizedSave = (JObject)Data.DeepClone();
                            PrepareSerialize(OrganizedSave,"");
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
                    AesAlg.Key = AesKey;
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
                            PrepareDeserialize(Data,"");
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
    static private void PrepareSerialize(JObject dict,string QueryVal="")
    {
        foreach (KeyValuePair<string, JToken> item in (QueryVal != "") ? (JObject)dict.SelectToken(QueryVal.Substring(0,QueryVal.Length-1)) : dict)
        {
            switch(item.Value.Type)
            {
                case JTokenType.Bytes:
                    {
                        item.Value.Replace("base64:" + Convert.ToBase64String((byte[])item.Value));
                        break;
                    }
                case JTokenType.Object:
                    {
                        PrepareSerialize(dict, QueryVal+item.Key+".");
                        break;
                    }
                case JTokenType.Array:
                    {
                        PrepareSerialize(dict, QueryVal + item.Key,true);
                        break;
                    }
            }
        }
    }
    static private void PrepareSerialize(JObject dict, string QueryVal = "", bool isArray=true)
    {
        for (int i=0; i<(dict.SelectToken(QueryVal) as JArray).Count; i++)
        {
            JToken item = dict.SelectToken(QueryVal + "["+i+"]");
            switch (item.Type)
            {
                case JTokenType.Bytes:
                    {
                        item.Replace("base64:" + Convert.ToBase64String((byte[])item));
                        break;
                    }
                case JTokenType.Object:
                    {
                        PrepareSerialize(dict, QueryVal + "[" + i + "]" + ".");
                        break;
                    }
                case JTokenType.Array:
                    {
                        PrepareSerialize(dict, QueryVal + "[" + i + "]", true);
                        break;
                    }
            }
        }
    }

    /*
     * Description: Prepare Object for use
     */
    static private void PrepareDeserialize(JObject dict, string QueryVal = "")
    {
        foreach (KeyValuePair<string, JToken> item in (QueryVal != "") ? (JObject)dict.SelectToken(QueryVal.Substring(0, QueryVal.Length - 1)) : dict)
        {
            if(item.Value.Type == JTokenType.String && item.Value.ToString().StartsWith("base64:"))
            {
                dict.SelectToken(QueryVal + item.Key).Replace(Convert.FromBase64String(item.Value.ToString().Substring(7)));
            }
            switch (item.Value.Type)
            {
                case JTokenType.Object:
                    {
                        PrepareDeserialize(dict, QueryVal+item.Key + ".");
                        break;
                    }
                case JTokenType.Array:
                    {
                        PrepareDeserialize(dict, QueryVal + item.Key, true);
                        break;
                    }
            }
        }
    }
    static private void PrepareDeserialize(JObject dict, string QueryVal = "", bool isArray = true)
    {
        for (int i = 0; i < (dict.SelectToken(QueryVal) as JArray).Count; i++)
        {
            JToken item = dict.SelectToken(QueryVal + "[" + i + "]");
            if (item.Type == JTokenType.String && item.ToString().StartsWith("base64:"))
            {
                item.Replace(Convert.FromBase64String(item.ToString().Substring(7)));
            }
            switch (item.Type)
            {
                case JTokenType.Object:
                    {
                        PrepareDeserialize(dict, QueryVal + "[" + i + "]" + ".");
                        break;
                    }
                case JTokenType.Array:
                    {
                        PrepareDeserialize(dict, QueryVal + "[" + i + "]", true);
                        break;
                    }
            }
        }
    }
    /*
     * Description: Check if the save file exists
     * Return: bool - if the file exist or not
     */
    static public bool SaveFileExist()
    {
        return File.Exists(FilePath);
    }
    /*
     * Description: Delete the save file with the option to delete the loaded save as well
     * Args: DeleteLoadedSave - Weather or not to delete the loaded JObject save.
     * Return:
     *  true - the files has been successfully 
     *  false - The save file isn't existed when DeleteLoadedSave is false
     */
    static public bool DeleteSave(bool DeleteLoadedSave = false)
    {
        bool status = false;
        if (DeleteLoadedSave)
        {
            Data.RemoveAll();
            status = true;
        }
        if (!SaveFileExist())
            return status;
        File.Delete(FilePath);
        return true;
    }
}