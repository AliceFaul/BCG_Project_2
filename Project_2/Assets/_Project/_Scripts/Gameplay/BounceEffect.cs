using System.Collections;
using UnityEngine;

namespace _Project._Scripts.Gameplay
{
    public class BounceEffect : MonoBehaviour
    {
        [Header("Property của Bounce")]
        [Tooltip("Độ cao khi Bounce")]
        [SerializeField] private float _bounceHeigh = .3f;
        [Tooltip("Thời gian mỗi Time.deltaTime từ điểm start đến điểm height, dùng cho việc nội suy")]
        [SerializeField] private float _bounceDuration = .4f;
        [SerializeField] private int _bounceCount = 2;

        public void Bounce()
        {
            StartCoroutine(BounceHandler());
        }

        IEnumerator BounceHandler()
        {
            Vector3 startPos = transform.position;
            float localHeight = _bounceHeigh;
            float localDuration = _bounceDuration;

            for(int i = 0; i < _bounceCount; i++)
            {
                yield return BounceCoroutine(startPos, localHeight, localDuration / 2);
                localHeight *= .5f;
                localDuration *= .8f;
            }

            transform.position = startPos;
        }

        IEnumerator BounceCoroutine(Vector3 start, float height, float duration)
        {
            Vector3 peak = start + Vector3.up * height;

            float elapsedTime = 0f;
            //Nội suy di chuyển item đi lên
            while(elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(start, peak, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;

            //Nội suy di chuyển đi xuống
            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(peak, start, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
