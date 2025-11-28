using System.Collections.Generic;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance { get; private set; }

        //Các thông số input
        public Vector2 _moveInput { get; private set; }
        public bool _attackInput { get; private set; }
        public bool _menuInput { get; private set; }
        public bool _pauseInput { get; private set; }
        public bool _infoInput { get; private set; }
        public bool _interactInput { get; private set; }
        public bool _runningInput { get; private set; }
        public Dictionary<string, KeyCode> _skillInputDictionary = new Dictionary<string, KeyCode>();

        private void Awake()
        {
            //Tạo Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandlePCInput();
        }

        #region Input Handler
        public void HandlePCInput()
        {
            _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _attackInput = Input.GetMouseButtonDown(0);
            _menuInput = Input.GetKeyDown(KeyCode.B);
            _infoInput = Input.GetKeyUp(KeyCode.F);
            _pauseInput = Input.GetKeyDown(KeyCode.Escape);
            _interactInput = Input.GetKeyDown(KeyCode.E);
            _runningInput = Input.GetKey(KeyCode.LeftShift);
            HandlerSkillInput();
        }

        void HandlerSkillInput()
        {
            foreach(var input in _skillInputDictionary)
            {
                if (Input.GetKeyDown(input.Value))
                {
                    HandleSkillInputByName(input.Key);
                }
            }
        }

        public bool HandleSkillInputByName(string name)
        {
            int slotIndex = -1;

            switch (name)
            {
                case "Skill_Z": slotIndex = 0; break;
                case "Skill_X": slotIndex = 1; break;
                case "Skill_C": slotIndex = 2; break;
                case "Skill_V": slotIndex = 3; break;
                case "Skill_G": slotIndex = 4; break;
                case "Skill_T": slotIndex = 5; break;
            }

            if (slotIndex == -1) return false;

            // Gọi sang UI manager hoặc HUD skill
            SkillIcon icon = UIManager.Instance.hudSlots[slotIndex];

            if (icon != null)
            {
                icon.OnClickSkill();  // dùng skill!
                return true;
            }

            return false;
        }


        public void InsertSkillKeyCode()
        {
            _skillInputDictionary["Skill_Z"] = KeyCode.Z;
            _skillInputDictionary["Skill_X"] = KeyCode.X;
            _skillInputDictionary["Skill_C"] = KeyCode.C;
            _skillInputDictionary["Skill_V"] = KeyCode.V;
            _skillInputDictionary["Skill_G"] = KeyCode.G;
            _skillInputDictionary["Skill_T"] = KeyCode.T;
        }
        #endregion
    }
}
