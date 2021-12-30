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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using ProtoBuf;
using System.Reflection;
using UnityEngine;

[ProtoContract]
public class SaveData
{
    [ProtoMap(DisableMap = true)]
    [ProtoMember(1)]
    public Dictionary<string,float> Position { get; set; } = new Dictionary<string,float>();
    [ProtoMember(2)]
    public bool isOutdoor { get; set; } = false;
    [ProtoMember(3)]
    public int Balance { get; set; } = 0;
    [ProtoMember(4)]
    public ResolutionType ScreenResolution { get; set; } = ResolutionType.FullScreen; // ResolutionType exist under SettingsHandler.cs
}
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
    static public SaveData Data = new SaveData { };

    /*
     * Description: Save the data into into a save file
     * Return: bool - If save is successful or not
     */
    static public bool SaveToDisk()
    {
        try
        {
            File.Delete(FilePath);
            using (FileStream SaveFile = new FileStream(FilePath, FileMode.Create))
            {
                byte[] RandIV = new byte[12];
                new System.Random().NextBytes(RandIV);
                SaveFile.Write(RandIV, 0, RandIV.Length);
                BufferedAeadBlockCipher buffblockcipher = new BufferedAeadBlockCipher(new GcmBlockCipher(new AesEngine()));
                buffblockcipher.Init(true, new AeadParameters(new KeyParameter(AesKey), 128, RandIV));
                using (CipherStream cryptstream = new CipherStream(SaveFile, null, buffblockcipher))
                {
                    Serializer.Serialize(cryptstream, Data);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
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
                    byte[] IV = new byte[12];
                    SaveFile.Read(IV, 0, IV.Length);
                    BufferedAeadBlockCipher buffblockcipher = new BufferedAeadBlockCipher(new GcmBlockCipher(new AesEngine()));
                    buffblockcipher.Init(false, new AeadParameters(new KeyParameter(AesKey), 128, IV));
                    SaveData internalDat;
                    using (CipherStream cryptstream = new CipherStream(SaveFile, buffblockcipher, null))
                        internalDat = Serializer.Deserialize<SaveData>(cryptstream);
                    foreach(PropertyInfo prop in internalDat.GetType().GetProperties()) {
                        Data.GetType().GetProperty(prop.Name).SetValue(Data, prop.GetValue(internalDat, null));
                    }
                }
            }
        }
        catch(InvalidCipherTextException ex)
        {
            //DeleteSave(true);
            Debug.LogException(ex);
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }
        return true;
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
            Data = new SaveData { };
            status = true;
        }
        if (!SaveFileExist())
            return status;
        File.Delete(FilePath);
        return true;
    }
}