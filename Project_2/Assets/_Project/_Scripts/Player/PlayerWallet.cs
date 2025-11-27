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

        private void Awake()
        {
            Instance = this;

            Coins = PlayerPrefs.GetInt("COINS", 0);
        }

        public bool SpendCoin(int amount)
        {
            if(Coins < amount) return false;

            Coins -= amount;
            PlayerPrefs.SetInt("COINS", Coins);
            PlayerPrefs.Save();

            return true;
        }

        public void AddCoin(int amount)
        {
            Coins += amount;
            PlayerPrefs.SetInt("COINS", Coins);
            PlayerPrefs.Save();
        }
    }
}
