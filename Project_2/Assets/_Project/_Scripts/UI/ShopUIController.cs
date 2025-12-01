using _Project._Scripts.Core;
using _Project._Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI
{
    public class ShopUIController : MonoBehaviour
    {
        public static ShopUIController Instance { get; private set; }

        [Header("Shop UI và Prefab")]
        [SerializeField] private Transform _shopListContent;
        [SerializeField] private GameObject _shopPanel;
        [SerializeField] private RectTransform _itemDescContent;
        [SerializeField] private TMP_Text _coinText;

        [Header("Prefab bảng thông tin của các item")]
        [SerializeField] private GameObject _stockEntryPrefab;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _closeButton;

        ShopData _currentData;
        ItemSO _currentItem;

        private void Awake()
        {
            if(Instance == null) Instance = this;
            else Destroy(gameObject);

            _shopPanel.SetActive(false);
            _itemDescContent.gameObject.SetActive(false);

            if(_closeButton != null)
                _closeButton.onClick.AddListener(CloseShop);

            if(_coinText != null)
                PlayerWallet.Instance.OnCoinChanged += OnCoinChangedHandler;
        }

        private void Update()
        {
            if(!_itemDescContent.gameObject.activeSelf) return;

            if(_itemDescContent != null && Input.GetMouseButtonDown(0))
            {
                if(!RectTransformUtility.RectangleContainsScreenPoint(_itemDescContent, 
                    Input.mousePosition, null))
                {
                    CloseDetail();
                }
            }

            if(_shopPanel.gameObject.activeSelf || _itemDescContent.gameObject.activeSelf)
            {
                if (PlayerInput.Instance == null) return;

                if(PlayerInput.Instance._pauseInput)
                {
                    CloseShop();
                }
            }
        }

        private void OnDestroy()
        {
            PlayerWallet.Instance.OnCoinChanged -= OnCoinChangedHandler;
        }

        public void OpenShop(ShopData data)
        {
            _currentData = data;

            HUDController.Instance.HidePlayerHUD(true);
            PauseController.SetPaused(true);

            PlayerWallet.Instance.RefreshCoinUI();

            foreach(Transform child in _shopListContent)
            {
                if (child.gameObject.name == "Info") continue;

                Destroy(child.gameObject);
            }

            foreach(ItemSO item in data._items)
            {
                CreateItemButton(item.ItemImage, item.Name, item._price.ToString(), item);
            }

            _shopPanel.SetActive(true);
        }

        void CreateItemButton(Sprite itemSprite, string itemName, string itemPrice, ItemSO itemData)
        {
            GameObject stockEntry = Instantiate(_stockEntryPrefab, _shopListContent);

            stockEntry.transform.Find("ItemSprite").GetComponent<Image>().sprite = itemSprite;
            stockEntry.transform.Find("ItemName").GetComponent<TMP_Text>().text = $"{itemName}";
            stockEntry.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = $"{itemPrice}";

            Button b = stockEntry.GetComponent<Button>();
            if(b == null) b = stockEntry.AddComponent<Button>();
            b.transition = Selectable.Transition.None;

            b.onClick.AddListener(() => ShowDetail(itemData));
        }

        void ShowDetail(ItemSO item)
        {
            _currentItem = item;

            _itemDescContent.gameObject.SetActive(true);

            _itemDescContent.transform.Find("DetailSprite").GetComponent<Image>().sprite = item.ItemImage;
            _itemDescContent.transform.Find("DetailName").GetComponent<TMP_Text>().text = item.Name;
            _itemDescContent.transform.Find("DetailDesc").GetComponent<TMP_Text>().text = item.Description;
            _itemDescContent.transform.Find("DetailPrice").GetComponent<TMP_Text>().text = item._price.ToString();

            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(BuyItem);
        }

        void CloseDetail() => _itemDescContent.gameObject.SetActive(false);
        
        public void CloseShop()
        {
            _itemDescContent.gameObject.SetActive(false);
            _shopPanel.SetActive(false);

            HUDController.Instance.HidePlayerHUD(false);
            PauseController.SetPaused(false);
        }

        void BuyItem()
        {
            if(_currentItem == null) return;

            if(!PlayerWallet.Instance.SpendCoin(_currentItem._price))
            {
                Debug.LogWarning("Not have enough money");
                return;
            }

            InventoryController.Instance.AddItem(_currentItem._itemPrefab);
            CloseDetail();
            Debug.Log("Bought successful");
        }

        void UpdateShopCoin()
        {
            if(_coinText != null)
                _coinText.text = PlayerWallet.Instance.Coins.ToString();
        }

        void OnCoinChangedHandler(int x) => UpdateShopCoin();
    }
}
