using UnityEngine;

public interface IGamepad
{
    int Id { get; set; }
    string Type { get; set; }
    void SetGamepadEventHandler(IGamepadEventHandler eventHandler);
    bool IsConnected();
    bool IsButtonPressed(IGamepad.Key key, IGamepad.PressureType pressure);
    Vector2 GetAnalogMovement(IGamepad.Analog analog);
    Vector2 GetAnalogMovement(IGamepad.Analog analog, Vector2 friction);
    void InvertAnalog(IGamepad.Analog analog);
    void InvertAnalog(IGamepad.Analog analog, IGamepad.Orientation orientation);
    string ControllerSpecificStatus
    {
        get;
    }
    string Status
    {
        get { return (IsConnected()) ? $"Gamepad {Id} of type {Type} is connected with status {ControllerSpecificStatus}" : $"Gamepad {Id} of type {Type} is connected, but there's an error. Actual status: {ControllerSpecificStatus}"; }
    }

    public enum Key
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
    public enum Analog
    {
        Left,
        Right
    }
    public enum PressureType
    {
        Single,
        Continue
    }
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

}
