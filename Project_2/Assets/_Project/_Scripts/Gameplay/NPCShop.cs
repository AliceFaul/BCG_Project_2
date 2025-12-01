using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Gameplay
{
    public class NPCShop : MonoBehaviour, IInteractable
    {
        [SerializeField] ShopData _shopData;

        public bool CanInteract()
        {
            return true;
        }

        public void Interact()
        {
            ShopUIController.Instance.OpenShop(_shopData);
        }
    }
}
