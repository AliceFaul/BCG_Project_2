using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapCounter : MonoBehaviour
{
    [ContextMenu("Count all Tilemaps")]
    void CountAllTilemaps()
    {
        Tilemap[] _allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        int total = 0;

        foreach(Tilemap tile in _allTilemaps)
        {
            int count = 0;
            BoundsInt bounds = tile.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
                if (tile.HasTile(pos)) count++;

            Debug.Log($"Tilemap {tile.name}: {count} tiles");
            total += count;
        }

        Debug.Log($"Total tile: {total} tile");
    }
}
