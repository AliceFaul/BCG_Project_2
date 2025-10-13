/// <summary>
/// Interface này sẽ định nghĩa những object nào player có thể tương tác được 
/// </summary>
public interface IInteractable
{
    void Interact(); //Hàm này sẽ gọi event tương tác trong object được tương tác
    bool CanInteract(); //Hàm này sẽ kiểm tra người chơi có thể gọi hàm tương tác ở trên được không
}
