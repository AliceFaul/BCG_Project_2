using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerItemCollector : MonoBehaviour
    {
        private InventoryController _inventoryController;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _inventoryController = FindAnyObjectByType<InventoryController>();
        }

        //Update trong tương lai
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Item"))
            {
                Item item = collision.GetComponent<Item>();
                if(item != null)
                {
                    //Kiểm tra inventory và thêm item vào
                    bool itemAdded = _inventoryController.AddItem(collision.gameObject);

                    //Nếu có thì sẽ nhặt item và xóa gameObject
                    if(itemAdded)
                    {
                        item.Pickup();
                        Destroy(collision.gameObject);
                    }
                }
            }
        }
    }
}
