using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShopManager
{
    [ProtoContract]
    public class Item
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public string Description;
        [ProtoMember(3)]
        public int Price;
        [ProtoMember(4)]
        public int MaxQuantity;
        [ProtoMember(5)]
        public GameObject PlayerReceive;

    }
}
namespace Economy
{
    public static class Manager
    {
        static SaveData UserData = SaveManager.Data;
        static public Dictionary<string, ShopManager.Item> ShopItems = new Dictionary<string, ShopManager.Item>();

        public static int GetBalance
        {
            get => UserData.Balance;
        }
        public static bool SetBalance(int newBalance)
        {
            newBalance = GetBalance + newBalance;
            if (newBalance > 99999999 || newBalance < 0)
                return false;

            BalanceChangedEventArgs BalEventArg = new BalanceChangedEventArgs
            {
                oldBalance = UserData.Balance,
                newBalance = newBalance
            };
            if (BalanceChanged != null) BalanceChanged(null, BalEventArg);
            UserData.Balance = newBalance;
            return true;
        }
        static public event EventHandler<BalanceChangedEventArgs> BalanceChanged;
    }
    public class BalanceChangedEventArgs : EventArgs
    {
        public int oldBalance;
        public int newBalance;
    }
}

