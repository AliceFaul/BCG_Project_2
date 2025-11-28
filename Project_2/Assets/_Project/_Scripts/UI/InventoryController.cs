using UnityEngine;
using System.Collections.Generic;
using _Project._Scripts.Core;
using System;
using UnityEngine.UI;
using TMPro;
using System.Data.Common;

namespace _Project._Scripts.UI
{
    /// <summary>
    /// Script này gán vào GameManager để Update UI Inventory
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        [Header("UI hiển thị mô tả Item")]
        [SerializeField] private Image itemImage;          // Kéo object "ItemImage" vào đây
        [SerializeField] private TMP_Text itemNameText;    // Kéo "ItemName"
        [SerializeField] private TMP_Text itemDescText;    // Kéo "ItemDescription"
        [SerializeField] private GameObject descriptionPanel; // Kéo "ItemDescriptionPanel" hoặc panel cha

        //Singleton của InventoryController
        public static InventoryController Instance { get; private set; }

        private ItemDictionary _itemDictionary;

        [Header("Inventory Page")]
        public GameObject _inventoryPanel; //Inventory Page trong Menu

        [Space(10)]

        [Header("Các thành phần trong Inventory Page")]
        [Tooltip("Prefab Slot chứa script Slot")]
        [SerializeField] private GameObject _slotPrefab; //Prefab slot
        [Tooltip("Số lượng slot trong Inventory Panel")]
        [SerializeField] private int _slotCount; //Số lượng slot trong inventory panel

        Dictionary<int, int> _itemCountCache = new();
        public event Action OnInventoryChanged;

        #region Unity Life Cycle

        private void Awake()
        {
            //Thiết lập Singleton
            if(Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _itemDictionary = FindAnyObjectByType<ItemDictionary>();
            RebuildItemCounts();
        }

        #endregion

        #region Inventory Control Setting

        public void SetupDescriptionPage(ItemSO data)
        {
            if (data == null)
            {
                Debug.LogWarning("ItemSO bị null!");
                return;
            }

            // Gán dữ liệu vào UI
            itemImage.sprite = data.ItemImage;
            itemNameText.text = data.Name;
            itemDescText.text = data.Description;

            // Bật panel mô tả
            if(descriptionPanel != null)
            {
                SetDescription(true);
                if (data.Description != null)
                {
                    descriptionPanel.GetComponentInChildren<TMP_Text>().text = data.Description;
                }
            }
        }
        public void SetDescription(bool isActive)
        {
            descriptionPanel.SetActive(isActive);
        }
        //Hàm này giúp kiểm tra inventory và thêm prefab item vào inventory khi người chơi nhặt item, gọi ở PlayerItemCollector
        public bool AddItem(GameObject itemPrefab)
        {
            //Lúc này sẽ bắt đầu xem để tăng số lượng của item
            Item itemToAdd = itemPrefab.GetComponent<Item>();
            if (itemToAdd == null) return false;

            //Duyệt qua inventory page và kiểm tra từng slot
            foreach(Transform slotTransform in _inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if (slot != null && slot._currentItem != null) //nếu như slot đó đã có item
                {
                    Item slotItem = slot._currentItem.GetComponent<Item>();
                    if(slotItem != null && slotItem._itemID == itemToAdd._itemID) //Kiểm tra xem item trong slot đó có trùng ID với item vừa nhặt không
                    {
                        //Nếu trùng thì sẽ tăng số lượng lên
                        slotItem.AddToStack();
                        RebuildItemCounts();
                        return true;
                    }
                }
            }

            //Lúc này nếu không tìm thấy item giống với item vừa nhặt thì sẽ tạo mới trong slot mới
            //Dùng foreach để tìm kiếm slot trống
            foreach(Transform slotTransform in _inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if(slot != null && slot._currentItem == null) //Nếu slot không có item
                {
                    //Tạo item mới bằng itemPrefab được nhặt
                    GameObject newItem = Instantiate(itemPrefab, slotTransform);
                    newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot._currentItem = newItem;
                    RebuildItemCounts();
                    return true;
                }
            }

            //Nếu không có slot nào trống trong inventory thì tạo log
            Debug.Log("Inventory is full");
            return false;
        }

        /// <summary>
        /// Hàm này chủ yếu giúp kích hoạt event OnInventoryChanged khi inventory có thay đổi gì đó
        /// như là nhặt, drop, ...
        /// </summary>
        public void RebuildItemCounts()
        {
            //Dọn sạch dictionary (xóa dữ liệu cũ và lưu dữ liệu mới)
            _itemCountCache.Clear();

            //Duyệt qua trong inventory
            foreach(Transform slotTransform in _inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if(slot._currentItem != null)
                {
                    Item item = slot._currentItem.GetComponent<Item>();
                    if(item != null)
                    {
                        //Lưu vào dictionary với key là itemID và value là quantity (số lượng)
                        //Nếu có số lượng trước đó thì sẽ cộng thêm với số lượng của item vừa thêm
                        _itemCountCache[item._itemID] = _itemCountCache.GetValueOrDefault(item._itemID, 0) + item._quantity;
                    }
                }
            }

            //Kích hoạt event OnInventoryChanged, có nghĩa là inventory vừa thay đổi
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Hàm chỉ đơn giản là lấy Dictionary với ID và Quantity (Số lượng)
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> GetItemCounts() => _itemCountCache;

        /// <summary>
        /// Hàm này sẽ xóa item khỏi Inventory bằng itemID và số lượng sẽ xóa
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="amount"></param>
        public void RemoveItemsFromInventory(int itemID, int amount)
        {
            //Đầu tiên tất nhiên là duyệt qua inventory
            foreach(Transform slotTransform in _inventoryPanel.transform)
            {
                if (amount <= 0) break; //Nếu số lượng cần xóa là 0 thì thoát khỏi foreach

                Slot slot = slotTransform.GetComponent<Slot>();
                if(slot?._currentItem?.GetComponent<Item>() is Item item && item._itemID == itemID)
                {
                    //Dùng Mathf.Min để lấy số lượng ít nhất để xóa số lượng của item (tránh số âm)
                    int removed = Mathf.Min(amount, item._quantity);
                    item.RemoveFromStack(removed); //Trừ số lượng
                    amount -= removed;

                    //Nếu số lượng của item dưới 0 thì xóa item trong inventory
                    if(item._quantity <= 0)
                    {
                        Destroy(slot._currentItem);
                        slot._currentItem = null;
                    }
                }
            }

            //Kích hoạt event cho biết có thay đổi trong inventory
            RebuildItemCounts();
        }

        #endregion

        #region Inventory Saving System

        //Hàm này dùng để saving các item lại
        public List<InventorySaveData> GetInventoryItems()
        {
            List<InventorySaveData> invData = new List<InventorySaveData>();

            foreach(Transform slotTransform in _inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if(slot._currentItem != null)
                {
                    Item item = slot._currentItem.GetComponent<Item>();
                    invData.Add(new InventorySaveData { 
                        _itemID = item._itemID, 
                        _slotIndex = slotTransform.GetSiblingIndex(),
                        _quantity = item._quantity
                    });
                }
            }

            return invData;
        }

        //Hàm này dùng để loading các item được lưu lại trong InventorySaveData
        public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
        {
            //Làm trống inventory page, tránh duplicate
            foreach(Transform child in _inventoryPanel.transform)
            {
                Destroy(child.gameObject);
            }

            //Tạo lại các slot trong inventory page
            for(int i = 0; i < _slotCount; i++)
            {
                Instantiate(_slotPrefab, _inventoryPanel.transform);
            }

            //Lúc này sẽ setup lại item trong InventorySaveData 
            foreach(InventorySaveData data in inventorySaveData)
            {
                if(data._slotIndex < _slotCount) //Kiểm tra slotIndex phải nhỏ hơn số lượng slot thì mới tiếp tục
                {
                    //Lấy slot tương ứng với slotIndex chứa item đã lưu
                    Slot slot = _inventoryPanel.transform.GetChild(data._slotIndex).GetComponent<Slot>();
                    GameObject itemPrefab = _itemDictionary.GetItemPrefab(data._itemID); //Lấy prefab item bằng itemID qua Dictionary
                    if(itemPrefab != null)
                    {
                        GameObject item = Instantiate(itemPrefab, slot.transform); //Instantiate prefab trong slot
                        //Setup vị trí nằm giữa slot và lưu currentItem
                        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                        //Tạo một biến item dùng để load lại số lượng
                        Item itemComponent = item.GetComponent<Item>();
                        if(itemComponent != null && data._quantity > 1)
                        {
                            itemComponent._quantity = data._quantity;
                            itemComponent.UpdateQuantityDisplay();
                        }

                        slot._currentItem = item;
                    }
                }
            }

            RebuildItemCounts();
        }

        #endregion
    }
}
