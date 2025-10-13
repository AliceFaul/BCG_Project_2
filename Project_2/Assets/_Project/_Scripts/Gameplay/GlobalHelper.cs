using UnityEngine;

/// <summary>
/// Script này giúp tạo một Unique ID cho Chest, giúp cho việc saving
/// </summary>
public static class GlobalHelper
{
    public static string GenerateChestUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}";
    }
}
