using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XInput;

public class XboxGamepad : IGamepad
{

    private readonly XInputController _reference;
    private IGamepadEventHandler eventHandler;
    private List<IGamepad.Key> pressedButtons = new List<IGamepad.Key>();

    public XboxGamepad(int id, XInputController xboxGamepad)
    {
        this.Id = id;
        this.Type = "Xbox";
        this._reference = xboxGamepad;
    }
    public XboxGamepad(int id, XInputController xboxGamepad, IGamepadEventHandler eventHandler)
    {
        this.Id = id;
        this.Type = "Xbox";
        this._reference = xboxGamepad;
        SetGamepadEventHandler(eventHandler);
    }

    public int Id { get; set; }
    public string Type { get; set; }

    public string ControllerSpecificStatus => this._reference.enabled ? "ONLINE" : "OFFLINE";

    public Vector2 GetAnalogMovement(IGamepad.Analog analog)
    {
        if (analog == IGamepad.Analog.Left) return _reference.leftStick.ReadValue();
        else return _reference.rightStick.ReadValue();
    }

    public bool IsButtonPressed(IGamepad.Key key, IGamepad.PressureType pressure)
    {
        ButtonControl button;
        switch (key)
        {
            case IGamepad.Key.ActionButtonDown:
                button = _reference.aButton;
                break;
            case IGamepad.Key.ActionButtonRight:
                button = _reference.bButton;
                break;
            case IGamepad.Key.ActionButtonUp:
                button = _reference.yButton;
                break;
            case IGamepad.Key.ActionButtonLeft:
                button = _reference.xButton;
                break;
            case IGamepad.Key.Start:
                button = _reference.startButton;
                break;
            case IGamepad.Key.Select:
                button = _reference.selectButton;
                break;
            case IGamepad.Key.LeftAnalog:
                button = _reference.leftStickButton;
                break;
            case IGamepad.Key.RightAnalog:
                button = _reference.rightStickButton;
                break;
            default:
                throw new System.NotImplementedException($"Key not implemented for gamepad {Type}");
        }

        if (eventHandler != null && pressure == IGamepad.PressureType.Single) eventHandler.OnButtonSinglePression(key);
        else if (eventHandler != null && pressure == IGamepad.PressureType.Continue) eventHandler.OnButtonContinuePression(key);

        if (!button.isPressed)
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
        bool isConnected = this._reference.enabled;
        if (eventHandler != null && isConnected) eventHandler.OnGamepadConnected();
        if (eventHandler != null && !isConnected) eventHandler.OnGamepadDeconnected();
        return isConnected;
    }

    public void SetGamepadEventHandler(IGamepadEventHandler eventHandler) => this.eventHandler = eventHandler;
}
