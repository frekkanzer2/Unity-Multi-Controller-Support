using UnityEngine;

public interface IGamepad
{
    int Id { get; set; }
    string Type { get; set; }
    void SetGamepadEventHandler(IGamepadEventHandler eventHandler);
    bool IsConnected();
    bool IsButtonPressed(IGamepad.Key key, IGamepad.PressureType pressure);
    Vector2 GetAnalogMovement(IGamepad.Analog analog);
    string ControllerSpecificStatus
    {
        get;
    }
    string Status
    {
        get { return (IsConnected()) ? $"Gamepad {Id} of type {Type} is connected with status {ControllerSpecificStatus}" : $"Gamepad {Id} of type {Type} is connected, but there's an error. Actual status: {ControllerSpecificStatus}"; }
    }

    enum Key
    {
        ActionButtonDown,
        ActionButtonRight,
        ActionButtonUp,
        ActionButtonLeft,
        Select,
        Start,
        Center,
        MovementButtonDown,
        MovementButtonRight,
        MovementButtonUp,
        MovementButtonLeft,
        LeftAnalog,
        RightAnalog,
        LeftShoulder,
        RightShoulder,
        LeftTrigger,
        RightTrigger
    }
    enum Analog
    {
        Left,
        Right
    }
    enum PressureType
    {
        Single,
        Continue
    }

}
