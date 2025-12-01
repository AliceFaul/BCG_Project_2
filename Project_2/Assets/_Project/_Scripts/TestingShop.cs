using _Project._Scripts.UI;
using UnityEngine;

public class TestingShop : MonoBehaviour
{
    [SerializeField] private ShopData _testData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_testData == null || ShopUIController.Instance == null) return;

        if(collision.CompareTag("Player"))
        {
            ShopUIController.Instance.OpenShop(_testData);
        }
    }
}
