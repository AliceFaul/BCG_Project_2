using TMPro;
using UnityEngine;

namespace _Project._Scripts.UI
{
    public class DamagePopup : MonoBehaviour
    {
        [Header("Khởi tạo các biến Damage Popup")]
        private TextMeshPro _tmpText; //Component Text của popup
        private const float DISAPPEAR_TIMER_MAX = 3f; //Dùng const để lưu giá trị ở runtime
        private float _disappearTimer; //Bộ đếm thời gian biến mất của Popup
        private Color _textColor; //Màu sắc của text
        private Vector3 _moveVector; //Tạo hiệu ứng di chuyển của popup
        [Tooltip("Càng nhỏ thì thời gian tồn tại của Popup càng lâu")]
        [SerializeField] private float _disappearTime = 3f; //Thời gian tồn tại của popup
        private static int _sortingOrder; //Lớp sắp xếp của popup

        private void Awake()
        {
            _tmpText = transform.GetComponent<TextMeshPro>();
        }

        private void Update()
        {
            transform.position += _moveVector * Time.deltaTime;
            _moveVector -= _moveVector * 8f * Time.deltaTime;

            if(_disappearTimer > 0)
                _disappearTimer -= Time.deltaTime;

            //Tạo hiệu ứng phóng to khi xuất hiện và nhỏ dần sau đó biến mất bằng localScale
            if(_disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
            {
                //Vòng đời đầu tiên của popup
                float increaseScaleAmount = 1f;
                transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            }
            else
            {
                //Vòng đời sau đó của popup
                float decreaseScaleAmount = 1f;
                transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            }

            _textColor.a -= _disappearTime * Time.deltaTime; //Mờ dần theo giá trị _disappearTime
            _tmpText.color = _textColor; //Chỉnh màu text của popup
            if (_textColor.a < 0)
                Destroy(gameObject); //Nếu alpha của popup về 0 thì sẽ destroy gameobject
        }

        //Hàm tạo ra Damage Popup, dùng static để có thể gọi ở bất kì đâu
        public static DamagePopup CreatePopup(Vector3 position, float damageAmount, bool isCritical)
        {
            Transform damagePopupTransform = Instantiate(HUDController.Instance._pfDamagePopup, position, Quaternion.identity);

            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            if(damagePopup != null)
            {
                damagePopup.Setup(damageAmount, isCritical);
            }

            return damagePopup;
        }

        //Hàm setup của popup
        void Setup(float damage, bool isCritical)
        {
            _tmpText.SetText(damage.ToString());
            if(!isCritical)
            {
                _tmpText.fontSize = 3;
                _textColor = Color.white;
            }
            else
            {
                _tmpText.fontSize = 6;
                _textColor = Color.red;
            }
            _tmpText.color = _textColor;
            _disappearTimer = DISAPPEAR_TIMER_MAX;

            _sortingOrder++;
            _tmpText.sortingOrder = _sortingOrder;

            _moveVector = new Vector3(1, 1) * 10f;
        }
    }
}
