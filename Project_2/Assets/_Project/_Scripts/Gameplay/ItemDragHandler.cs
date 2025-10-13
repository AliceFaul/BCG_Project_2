using UnityEngine;
using UnityEngine.EventSystems;


namespace _Project._Scripts.Gameplay
{
    /// <summary>
    /// Script gán vào prefab item dùng để kéo thả trong UI
    /// </summary>
    public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
                if(dropSlot._currentItem != null) //Kiểm tra nếu slot đó có item khác thì đổi chỗ
                {
                    dropSlot._currentItem.transform.SetParent(ogSlot.transform);
                    ogSlot._currentItem = dropSlot._currentItem;
                    dropSlot._currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    Debug.Log("Slot có item, tráo đổi item");
                }
                else //Nếu không có item nào ở slot muốn bỏ vào
                {
                    ogSlot._currentItem = null;
                    Debug.Log("Đã bỏ item vào slot");
                }

                transform.SetParent(dropSlot.transform);
                dropSlot._currentItem = gameObject;
            }
            else
            {
                if(!CheckMouseWithinInventory(eventData.position)) //Nếu chuột không nằm trong inventory page
                {
                    //Drop item khỏi inventory page
                    DropItem(ogSlot);
                }
                else //Nếu chuột vẫn nằm trong inventory và không có slot để bỏ vào
                {
                    transform.SetParent(_ogTransform); //Nếu không có slot bỏ vào thì sẽ quay về slot ban đầu
                    Debug.Log("Không có slot nào, quay về slot ban đầu");
                }
            }

            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

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
            ogSlot._currentItem = null;

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
            GameObject item = Instantiate(gameObject, dropPosition, Quaternion.identity);
            item.GetComponent<BounceEffect>().Bounce();

            //Xóa item trong UI 
            Destroy(gameObject);
        }
    }
}
