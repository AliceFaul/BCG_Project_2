using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using _Project._Scripts.Core;

namespace _Project._Scripts.UI
{
    public class HotbarController : MonoBehaviour
    {
        [Header("Tham chiếu thanh Hotbar vào")]
        private ItemDictionary _itemDictionary;
        [Tooltip("Thanh Hotbar trong UI PlayerHUD")]
        public GameObject _hotbarPanel;

        [Header("Property của Hotbar")]
        [Tooltip("Prefab Hotbar slot, khác với Slot trong invenory page")]
        [SerializeField] private GameObject _slotPrefab;
        [Tooltip("Số lượng slot trong Hotbar, sử dụng key từ 1 - 0 trên bàn phím để sử dụng item")]
        [SerializeField] private int _slotCount = 10; //Là từ 1 - 0 trên bàn phím

        private KeyCode[] _hotbarKeys; //Chứa các Input 1 - 0 trên bàn phím để sử dụng item trong slot

        private void Awake()
        {
            _itemDictionary = FindAnyObjectByType<ItemDictionary>();

            _hotbarKeys = new KeyCode[_slotCount];
            for(int i = 0; i < _slotCount; i++)
            {
                //Lưu các KeyCode từ 1 - 0 vào _hotbarKeys
                _hotbarKeys[i] = i < 9 ? (KeyCode)((int)KeyCode.Alpha1 + i) : KeyCode.Alpha0;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Testing
            for(int i = 0; i < _slotCount; i++)
            {
                if (Input.GetKeyDown(_hotbarKeys[i]))
                {
                    UseItem(i);
                }
            }
        }

        void UseItem(int index)
        {
            Slot hotbarSlot = _hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
            if(hotbarSlot != null && hotbarSlot._currentItem != null)
            {
                Item item = hotbarSlot._currentItem.GetComponent<Item>();
                item.UseItem();
            }
            else
            {
                Debug.Log($"Not have item in slot {index + 1}");
            }
        }

        #region HotBar Saving System

        /// <summary>
        /// Hai hàm saving dưới đây chỉ copy từ Inventory Saving
        /// </summary>
        /// <returns></returns>

        //Hàm này dùng để saving các item lại
        public List<InventorySaveData> GetHotBarItems()
        {
            List<InventorySaveData> hbData = new List<InventorySaveData>();

            foreach (Transform slotTransform in _hotbarPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if (slot._currentItem != null)
                {
                    Item item = slot._currentItem.GetComponent<Item>();
                    hbData.Add(new InventorySaveData { _itemID = item._itemID, _slotIndex = slotTransform.GetSiblingIndex() });
                }
            }

            return hbData;
        }

        //Hàm này dùng để loading các item được lưu lại trong InventorySaveData
        public void SetHotBarItems(List<InventorySaveData> hotbarSaveData)
        {
            //Làm trống inventory page, tránh duplicate
            foreach (Transform child in _hotbarPanel.transform)
            {
                Destroy(child.gameObject);
            }

            //Tạo lại các slot trong inventory page
            for (int i = 0; i < _slotCount; i++)
            {
                Instantiate(_slotPrefab, _hotbarPanel.transform);
            }

            //Lúc này sẽ setup lại item trong InventorySaveData 
            foreach (InventorySaveData data in hotbarSaveData)
            {
                if (data._slotIndex < _slotCount) //Kiểm tra slotIndex phải nhỏ hơn số lượng slot thì mới tiếp tục
                {
                    //Lấy slot tương ứng với slotIndex chứa item đã lưu
                    Slot slot = _hotbarPanel.transform.GetChild(data._slotIndex).GetComponent<Slot>();
                    GameObject itemPrefab = _itemDictionary.GetItemPrefab(data._itemID); //Lấy prefab item bằng itemID qua Dictionary
                    if (itemPrefab != null)
                    {
                        GameObject item = Instantiate(itemPrefab, slot.transform); //Instantiate prefab trong slot
                        //Setup vị trí nằm giữa slot và lưu currentItem
                        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        slot._currentItem = item;
                    }
                }
            }
        }

        #endregion
    }
}
