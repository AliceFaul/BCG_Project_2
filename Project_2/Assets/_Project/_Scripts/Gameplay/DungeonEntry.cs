using _Project._Scripts.Core;
using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Gameplay
{
    public class DungeonEntry : MonoBehaviour, IInteractable
    {
        [SerializeField] private DungeonData _data;
        [SerializeField] private PortalType _type;
        [SerializeField] private Transform _entryPoint;
        [SerializeField] private Transform _exitPoint;

        public PolygonCollider2D _dungeonBoundary;
        public PolygonCollider2D _overworldBoundary;

        public bool CanInteract() => true;

        public void Interact()
        {
            if (_type == PortalType.Enter)
            {
                if (_data.IsOnCooldown())
                {
                    Debug.Log($"{_data._dungeonName} in cooldown");
                    return;
                }

                DungeonUIController.Instance.ShowDungeonConfirmUI(_data, _dungeonBoundary, _entryPoint.position);
            }
            else if (_type == PortalType.Exit)
                DungeonController.Instance.ExitDungeon(_data, _overworldBoundary, _exitPoint.position);
        }
    }

    public enum PortalType { Enter, Exit }
}
