using UnityEngine;

namespace _Project._Scripts.Gameplay
{
    public class Chest : MonoBehaviour, IInteractable
    {
        public bool _IsOpened { get; private set; }
        public string _ChestID { get; private set; }

        [Header("Property của một chest trong game")]
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Sprite _openedSprite;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Dùng Script Global Helper để generate id 
            _ChestID ??= GlobalHelper.GenerateChestUniqueID(gameObject);
        }

        public bool CanInteract()
        {
            //Nếu chest này chưa được mở thì có thể tương tác để mở
            return !_IsOpened;
        }

        public void Interact()
        {
            if (!CanInteract()) return;

            OpenChest();
        }

        void OpenChest()
        {
            //Chuyển Sprite
            SetOpened(true);

            //Drop item từ rương, update trong tương lai
            if(_itemPrefab != null)
            {
                GameObject item = Instantiate(_itemPrefab, transform.position + Vector3.down, Quaternion.identity);
                item.GetComponent<BounceEffect>().Bounce();
            }
        }

        public void SetOpened(bool opened)
        {
            //Nếu rương đã được mở (= true) thì sẽ chuyển sprite sang sprite rương mở
            if(_IsOpened = opened)
            {
                GetComponent<SpriteRenderer>().sprite = _openedSprite;
            }
        }
    }
}
