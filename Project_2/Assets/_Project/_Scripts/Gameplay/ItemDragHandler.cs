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
        [SerializeField] private float _alphaAmount = .6f;

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

            Slot dropSlot = eventData?.pointerEnter.GetComponent<Slot>(); //Slot mà muốn bỏ item vào, lấy theo chuột là eventData
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
                transform.SetParent(_ogTransform); //Nếu không có slot bỏ vào thì sẽ quay về slot ban đầu
                Debug.Log("Không có slot nào, quay về slot ban đầu");
            }

            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
}
