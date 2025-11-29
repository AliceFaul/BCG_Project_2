using _Project._Scripts.Core;
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

        [Header("Property của một chest trong Dungeon")]
        [SerializeField] private DungeonData _dungeonData;
        [SerializeField] private PolygonCollider2D _dungeonBoundary;

        public bool _isDungeonChest = false;

        private Sprite _closedSprite;
         
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Dùng Script Global Helper để generate id 
            _ChestID ??= GlobalHelper.GenerateChestUniqueID(gameObject);

            _closedSprite = GetComponent<SpriteRenderer>().sprite;
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
            SoundEffectManager.Instance.Play("Success");

            //Drop item từ rương, update trong tương lai
            if(_itemPrefab != null)
            {
                GameObject item = Instantiate(_itemPrefab, transform.position + Vector3.down, Quaternion.identity);
                item.GetComponent<BounceEffect>().Bounce();
            }

            if(DungeonController.Instance != null)
            {
                Debug.Log("Opened chest and clear dungeon");

                if(_isDungeonChest)
                {
                    DungeonController.Instance.OnChestOpened(this, _dungeonBoundary, _dungeonData);
                    GetComponent<SpriteRenderer>().sprite = _closedSprite;
                    SetOpened(false);
                }
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
