using UnityEngine;
using System.Collections.Generic;

namespace _Project._Scripts.Core
{
    public class ItemDictionary : MonoBehaviour
    {
        [Header("Danh sách các item trong game")]
        [Tooltip("List này chứa toàn bộ item trong game và gán ID tăng dần từ 1")]
        [SerializeField] public List<Item> _itemPrefabs;
        private Dictionary<int, GameObject> _itemDictionary;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if(_itemPrefabs.Count <= 0)
            {
                Debug.LogWarning("Hãy bỏ ít nhất một item vào List");
                return;
            }

            _itemDictionary = new Dictionary<int, GameObject>();

            //Tăng tự động ID của các item từ 1
            for(int i = 0; i < _itemPrefabs.Count; i++)
            {
                if (_itemPrefabs[i] != null)
                {
                    _itemPrefabs[i]._itemID = i + 1;
                }
            }

            //Bỏ các item trong List vào Dictionary
            foreach(Item item in _itemPrefabs)
            {
                _itemDictionary[item._itemID] = item.gameObject;
            }
        }

        //Hàm này sử dụng để lấy prefab item tương ứng với ID được gán với nó
        public GameObject GetItemPrefab(int itemID)
        {
            //TryGetValue để từ itemID sẽ cho ra prefab item có ID là itemID
            _itemDictionary.TryGetValue(itemID, out GameObject prefab);
            if(prefab == null)
            {
                Debug.LogWarning($"Item với {itemID} không có trong dictionary");
            }

            //Return prefab item
            return prefab;
        }
    }
}
