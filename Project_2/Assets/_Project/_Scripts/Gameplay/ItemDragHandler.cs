using _Project._Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;


namespace _Project._Scripts.Gameplay
{
    /// <summary>
    /// Script gán vào prefab item dùng để kéo thả trong UI
    /// </summary>
    public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        private Transform _ogTransform;
        private CanvasGroup _itemCG;

        [Header("Property khi nắm kéo của item")]
        [SerializeField] private float _alphaAmount = .6f;
        [Tooltip("Random ngẫu nhiên trong khoảng cách từ min đến max")]
        private const float _minDropDistance = 2f;
        private const float _maxDropDistance = 3f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _itemCG = GetComponent<CanvasGroup>();
        }

        #region Drag, Drop System

        //Lúc bắt đầu kéo thả vật phẩm trong inventory page
        public void OnBeginDrag(PointerEventData eventData)
        {
            _ogTransform = transform.parent; //Lưu lại slot ban đầu nó đang nằm
            transform.SetParent(transform.root); //Cho item đứng trên canvas
            _itemCG.blocksRaycasts = false;
            _itemCG.alpha = _alphaAmount; //Làm cho item mờ

            Debug.Log("Bắt đầu kéo thả item");
        }

        //Lúc đang kéo thả item
        public void OnDrag(PointerEventData eventData)
        {
            //Cho item đi theo chuột lúc đang nắm kéo
            transform.position = eventData.position;
            Debug.Log("Item đang đi thep chuột");
        }

        //Hàm xử lý khi ngừng kéo thả
        public void OnEndDrag(PointerEventData eventData)
        {
            //Bật lại các property trong canvas group
            _itemCG.blocksRaycasts = true;
            _itemCG.alpha = 1f;

            Slot dropSlot = eventData?.pointerEnter?.GetComponent<Slot>(); //Slot mà muốn bỏ item vào, lấy theo chuột là eventData
            if(dropSlot == null)
            {
                GameObject item = eventData.pointerEnter;
                if(item != null)
                {
                    dropSlot = item.GetComponentInParent<Slot>();
                }
            }

            Slot ogSlot = _ogTransform.GetComponent<Slot>();

            if(dropSlot != null)
            {
                if(dropSlot._currentItem != null)
                {
                    //Nếu bỏ vào slot có item cùng ID với item mình đang cầm thì tăng số lượng lên
                    Item draggedItem = GetComponent<Item>();
                    Item targetItem = dropSlot._currentItem.GetComponent<Item>();

                    //Kiểm tra ID của item mình đang cầm với item trong slot
                    if(draggedItem._itemID == targetItem._itemID)
                    {
                        targetItem.AddToStack(draggedItem._quantity);
                        ogSlot._currentItem = null;
                        Destroy(gameObject); //Xóa item đã kéo
                    }
                    else
                    {
                        //Kiểm tra nếu slot đó có item khác thì đổi chỗ
                        dropSlot._currentItem.transform.SetParent(ogSlot.transform);
                        ogSlot._currentItem = dropSlot._currentItem;
                        dropSlot._currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                        transform.SetParent(dropSlot.transform);
                        dropSlot._currentItem = gameObject;

                        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                        Debug.Log("Slot có item, tráo đổi item");
                    }
                }
                else //Nếu không có item nào ở slot muốn bỏ vào
                {
                    ogSlot._currentItem = null;
                    transform.SetParent(dropSlot.transform);
                    dropSlot._currentItem = gameObject;
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    Debug.Log("Đã bỏ item vào slot");
                }
            }
            else //Kiểm tra nếu người chơi kéo item bên ngoài slot
            {
                if(!CheckMouseWithinInventory(eventData.position)) //Nếu chuột không nằm trong inventory page
                {
                    //Drop item khỏi inventory page
                    DropItem(ogSlot);
                }
                else //Nếu chuột vẫn nằm trong inventory và không có slot để bỏ vào
                {
                    transform.SetParent(_ogTransform); //Nếu không có slot bỏ vào thì sẽ quay về slot ban đầu
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    Debug.Log("Không có slot nào, quay về slot ban đầu");
                }
            }
        }

        //Đây là interface pointer event để khi người chơi click vào item trong UI
        //Sẽ sử dụng để hiện thông tin của item bằng ScriptableObject của Item (Update trong tương lai)
        public void OnPointerClick(PointerEventData eventData)
        {
            // Lấy component Item đang được click
            Item item = GetComponent<Item>();
            if (item == null || item._itemSO == null)
            {
                Debug.LogWarning("Không có dữ liệu ItemSO để hiển thị");
                return;
            }

            // Gọi hàm có sẵn trong InventoryController để hiển thị mô tả
            InventoryController.Instance.SetupDescriptionPage(item._itemSO);

            Debug.Log($"Click item: {item._name}");
        }


        #endregion

        //Hàm bool kiểm tra xem chuột có đang nằm trong Inventory Page không
        bool CheckMouseWithinInventory(Vector2 mousePosition)
        {
            //Lấy Inventory Page thông qua cha của slot
            RectTransform inventoryPanel = _ogTransform.parent.GetComponent<RectTransform>();
            //Dùng RectTransformUtility để kiểm tra chuột có đang nằm trong Inventory Page không
            return RectTransformUtility.RectangleContainsScreenPoint(inventoryPanel, mousePosition);
        }

        void DropItem(Slot ogSlot)
        {
            //Lúc này sẽ kiểm tra số lượng item
            //Nếu số lượng lớn hơn 1 thì sẽ trừ số lượng còn không sẽ xóa item khỏi inventory
            Item item = GetComponent<Item>();
            int quantity = item._quantity;
            
            if(quantity > 1)
            {
                item.RemoveFromStack();

                transform.SetParent(_ogTransform);
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                quantity = 1;
            }
            else
            {
                ogSlot._currentItem = null;
            }

            //Tìm người chơi
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if(player == null)
            {
                Debug.LogWarning("Không tìm thấy người chơi");
                return;
            }
            //Drop item ngẫu nhiên xung quanh người chơi
            Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(_minDropDistance, _maxDropDistance);
            Vector2 dropPosition = (Vector2)player.transform.position + dropOffset;

            //Tạo item drop ngoài scene
            GameObject itemPrefab = Instantiate(gameObject, dropPosition, Quaternion.identity);
            Item dropItem = itemPrefab.GetComponent<Item>();
            dropItem._quantity = 1;
            itemPrefab.GetComponent<BounceEffect>().Bounce();

            //Xóa item trong UI 
            if(quantity <= 1 && ogSlot._currentItem == null)
            {
                Destroy(gameObject);
            }

            InventoryController.Instance.RebuildItemCounts();
        }
    }
}
