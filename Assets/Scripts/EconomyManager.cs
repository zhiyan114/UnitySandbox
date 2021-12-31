using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Economy
{

    public static class Manager
    {
        static public ShopManager.Skins[] AvailableSkins = new ShopManager.Skins[]
        {
            new ShopManager.Skins { Name = "Default", Price = 0 }
        };
        public static int GetBalance
        {
            get => SaveManager.Data.Balance;
        }
        public static bool SetBalance(int newBalance)
        {
            newBalance = GetBalance + newBalance;
            if (newBalance > 99999999 || newBalance < 0)
                return false;

            BalanceChangedEventArgs BalEventArg = new BalanceChangedEventArgs
            {
                oldBalance = SaveManager.Data.Balance,
                newBalance = newBalance
            };
            if (BalanceChanged != null) BalanceChanged(null, BalEventArg);
            SaveManager.Data.Balance = newBalance;
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

