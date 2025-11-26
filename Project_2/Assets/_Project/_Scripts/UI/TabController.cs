using _Project._Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI
{
    public class TabController : MonoBehaviour
    {
        [Header("Menu Tabs, Pages")]
        [Tooltip("Tham chiếu các Tabs và Pages vào")]
        [SerializeField] private Image[] _tabImages; //Một array chứa các tab trong menu (Update trong tương lai)
        [SerializeField] private GameObject[] _pages; //Array chứa các trang của tab ở trên


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ActiveTabs(0);
        }

        public void ActiveTabs(int tabSelected)
        {
            for(int i = 0; i < _pages.Length; i++)
            {
                _pages[i].SetActive(false);
                _tabImages[i].color = Color.gray;
            }

            _pages[tabSelected].SetActive(true);
            _tabImages[tabSelected].color = Color.white;
        }

        //Hàm này dùng để tạo âm thanh khi nhấn chuyển các tab và gọi trong event trigger của các tab
        public void PlaySfx()
        {
            SoundEffectManager.Instance.Play("SwitchTab");
        }
    }
}
