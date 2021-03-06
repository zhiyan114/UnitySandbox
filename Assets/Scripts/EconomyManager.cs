using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Economy
{

    public static class Manager
    {
        static public Dictionary<string,int> AvailableSkins = new Dictionary<string, int>()
        {
            {"Default", 0},
            {"Angry",100 },
            {"Ape Smile",128 },
            {"Ape Pirate", 256 },
            {"Ape Devil", 420 },
            {"Ape Army", 690 },
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

