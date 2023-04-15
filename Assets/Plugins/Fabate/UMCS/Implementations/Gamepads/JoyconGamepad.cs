using System.Collections.Generic;
using UnityEngine;

public class JoyconGamepad : IGamepad
{

    private readonly Joycon _reference;
    private IGamepadEventHandler eventHandler;
    private List<IGamepad.Key> pressedButtons = new List<IGamepad.Key>();

    public JoyconGamepad(int id, Joycon reference)
    {
        Id = id;
        Type = "Joycon";
        _reference = reference;
    }
    public JoyconGamepad(int id, Joycon reference, IGamepadEventHandler eventHandler)
    {
        Id = id;
        Type = "Joycon";
        _reference = reference;
        SetGamepadEventHandler(eventHandler);
    }

    public int Id { get ; set ; }
    public string Type { get; set; }

    public string ControllerSpecificStatus => $"{_reference.state}";

    public Vector2 GetAnalogMovement(IGamepad.Analog analog)
    {
        Vector2 stick = _reference.GetStick();
        if (_reference.isLeft)
            return new Vector2(stick.y, stick.x * -1);
        return new Vector2(stick.y * -1, stick.x);
    }

    public bool IsButtonPressed(IGamepad.Key key, IGamepad.PressureType pressure)
    {
        Joycon.Button button;
        switch(key)
        {
            case IGamepad.Key.ActionButtonDown:
                button = (_reference.isLeft) ? Joycon.Button.DPAD_LEFT : Joycon.Button.DPAD_RIGHT;
                break;
            case IGamepad.Key.ActionButtonRight:
                button = (_reference.isLeft) ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP;
                break;
            case IGamepad.Key.ActionButtonUp:
                button = (_reference.isLeft) ? Joycon.Button.DPAD_RIGHT : Joycon.Button.DPAD_LEFT;
                break;
            case IGamepad.Key.ActionButtonLeft:
                button = (_reference.isLeft) ? Joycon.Button.DPAD_UP : Joycon.Button.DPAD_DOWN;
                break;
            case IGamepad.Key.Start:
            case IGamepad.Key.Select:
            case IGamepad.Key.Center:
                button = (_reference.isLeft) ? Joycon.Button.MINUS : Joycon.Button.PLUS;
                break;
            default:
                throw new System.NotImplementedException($"Key not implemented for gamepad {Type}");
        }

        if (eventHandler != null && pressure == IGamepad.PressureType.Single) eventHandler.OnButtonSinglePression(key);
        else if (eventHandler != null && pressure == IGamepad.PressureType.Continue) eventHandler.OnButtonContinuePression(key);

        if (!_reference.GetButton(button))
        {
            pressedButtons.RemoveAll(btn => btn.Equals(key));
            return false;
        }
        else if (pressure == IGamepad.PressureType.Single)
        {
            if (pressedButtons.Contains(key)) return false;
            pressedButtons.Add(key);
            return true;
        }
        return true; // Continue mode
    }

    public bool IsConnected()
    {
        bool isConnected = _reference.state == Joycon.state_.IMU_DATA_OK;
        if (eventHandler != null && isConnected) eventHandler.OnGamepadConnected();
        if (eventHandler != null && !isConnected) eventHandler.OnGamepadDeconnected();
        return isConnected;
    }

    public void SetGamepadEventHandler(IGamepadEventHandler eventHandler) => this.eventHandler = eventHandler;
}
