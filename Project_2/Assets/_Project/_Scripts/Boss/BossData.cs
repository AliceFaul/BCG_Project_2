using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "Game/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("Basic Info")]
    public string bossName;
    public float maxHealth;
    public float respawnTime; // thời gian hồi sinh

    [Header("Loot")]
    public LootItem[] lootTable; // mảng item rớt
}

[System.Serializable]
public class LootItem
{
    public string itemName; // tên item
    public GameObject itemPrefab; // prefab rớt ra
    public float dropChance; // tỉ lệ % rớt (0-100)
}
