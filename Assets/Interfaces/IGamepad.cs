using UnityEngine;

public interface IGamepad
{
    int Id { get; set; }
    string Type { get; set; }
    void SetPlayerReference(IPlayer player);
    bool IsConnected();
    bool HasPressedButton1();
    bool HasPressedButton2();
    bool HasPressedButton3();
    bool HasPressedButton4();
    bool IsPressingButton1();
    bool IsPressingButton2();
    bool IsPressingButton3();
    bool IsPressingButton4();
    bool IsStartPressed();
    
    Vector2 GetAnalogMovement();
    string Status
    {
        get { return (IsConnected()) ? $"Gamepad {Id} of type {Type} is connected" : $"Gamepad {Id} of type {Type} is offline"; }
    }

}
