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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Item"))
            {
                Item item = collision.GetComponent<Item>();
                if(item != null)
                {
                    //Add to Inventory
                }
            }
        }
    }
}
