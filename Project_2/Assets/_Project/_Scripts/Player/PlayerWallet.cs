using System;
using UnityEngine;

namespace _Project._Scripts.Player
{
    /// <summary>
    /// Script dùng để quản lý túi tiền (coin) của người chơi
    /// </summary>
    public class PlayerWallet : MonoBehaviour
    {
        public static PlayerWallet Instance { get; private set; }

        public int Coins { get; private set; }

        public event Action<int> OnCoinChanged;

        private void Awake()
        {
            Instance = this;

            Coins = PlayerPrefs.GetInt("COINS", 0);
            OnCoinChanged?.Invoke(Coins);
        }

        public bool SpendCoin(int amount)
        {
            if(Coins < amount) return false;

            Coins -= amount;
            OnCoinChanged?.Invoke(Coins);
            PlayerPrefs.SetInt("COINS", Coins);
            PlayerPrefs.Save();

            return true;
        }

        [ContextMenu("Add Coin")]
        public void AddCoin(int amount = 1)
        {
            Coins += amount;
            OnCoinChanged?.Invoke(Coins);
            PlayerPrefs.SetInt("COINS", Coins);
            PlayerPrefs.Save();
        }

        public void RefreshCoinUI()
        {
            OnCoinChanged?.Invoke(Coins);
        }
    }
}
