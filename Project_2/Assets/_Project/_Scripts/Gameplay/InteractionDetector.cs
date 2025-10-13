using UnityEngine;

namespace _Project._Scripts.Gameplay
{
    public class InteractionDetector : MonoBehaviour
    {
        private IInteractable _interactable = null; //Sẽ gán giá trị khi object có cùng interface nằm trong collider
        [SerializeField] private GameObject _interactIcon;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _interactIcon.SetActive(false);
        }

        //Hàm này gọi trong PlayerMovement để sử dụng input 
        public void OnInteract()
        {
            _interactable.Interact();
            if(!_interactable.CanInteract())
            {
                _interactIcon.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Kiểm tra object nằm trong collider có implement interface IInteractable không và có thể tương tác không
            if(collision.TryGetComponent(out IInteractable interactable) && 
                interactable.CanInteract())
            {
                _interactable = interactable;
                _interactIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //Nếu đi xa khỏi object có interface
            if (collision.TryGetComponent(out IInteractable interactable) &&
                interactable == _interactable)
            {
                _interactable = null;
                _interactIcon.SetActive(false);
            }
        }
    }
}
