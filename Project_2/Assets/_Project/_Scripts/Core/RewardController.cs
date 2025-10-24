using _Project._Scripts.Core;
using _Project._Scripts.Gameplay;
using _Project._Scripts.UI;
using UnityEngine;

public class RewardController : MonoBehaviour
{
    public static RewardController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GiveQuestReward(Quest quest)
    {
        if(quest?._rewards == null) return;

        foreach(var reward in quest._rewards)
        {
            switch(reward._type)
            {
                case RewardType.Item:
                    GiveItemReward(reward._rewardID, reward._amount);
                    break;
                case RewardType.Exp:
                    break;
                case RewardType.Gold:
                    break;
                case RewardType.Custom:
                    break;
            }
        }
    }

    void GiveItemReward(int itemID, int amount)
    {
        var itemPrefab = FindAnyObjectByType<ItemDictionary>()?.GetItemPrefab(itemID);

        if(itemPrefab == null) return;

        for(int i = 0; i < amount; i++)
        {
            if(!InventoryController.Instance.AddItem(itemPrefab))
            {
                GameObject dropItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
                dropItem.GetComponent<BounceEffect>()?.Bounce();
            }
            else
            {
                itemPrefab.GetComponent<Item>()?.Pickup();
            }
        }
    }
}
