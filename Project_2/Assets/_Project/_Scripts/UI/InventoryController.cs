using UnityEngine;
using System.Collections.Generic;
using _Project._Scripts.Core;

namespace _Project._Scripts.UI
{
    public class InventoryController : MonoBehaviour
    {
        private ItemDictionary _itemDictionary;

        [Header("Inventory Page")]
        public GameObject _inventoryPanel; //Inventory Page trong Menu

        [Space(10)]

        [Header("Các thành phần trong Inventory Page")]
        [Tooltip("Prefab Slot chứa script Slot")]
        [SerializeField] private GameObject _slotPrefab; //Prefab slot
        [Tooltip("Số lượng slot trong Inventory Panel")]
        [SerializeField] private int _slotCount; //Số lượng slot trong inventory panel
        [SerializeField] private GameObject[] _itemPrefabs;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _itemDictionary = FindAnyObjectByType<ItemDictionary>();

            //for (int i = 0; i < _slotCount; i++)
            //{
            //    Slot slot = Instantiate(_slotPrefab, _inventoryPanel.transform).GetComponent<Slot>();

            //    if (i < _itemPrefabs.Length)
            //    {
            //        GameObject item = Instantiate(_itemPrefabs[i], slot.transform);
            //        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            //        slot._currentItem = item;
            //    }
            //}
        }

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
                    invData.Add(new InventorySaveData { _itemID = item._itemID, _slotIndex = slotTransform.GetSiblingIndex() });
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
                        slot._currentItem = item;
                    }
                }
            }
        }
    }
}
