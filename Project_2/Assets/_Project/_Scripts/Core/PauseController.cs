using UnityEngine;

namespace _Project._Scripts.Core
{
    /// <summary>
    /// Script giúp pause bằng cách ngắt input của người chơi thay vì dừng bằng Time.timeScale
    /// </summary>
    public class PauseController : MonoBehaviour
    {
        //Biến này sẽ được gọi để kiểm tra xem game có đang pause hay không
        public static bool IsGamePaused { get; private set; }

        //Hàm sẽ dùng trong những event mà bắt buộc phải dừng input của người chơi
        public static void SetPaused(bool pause)
        {
            IsGamePaused = pause;
        }
    }
}
