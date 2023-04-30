using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;

public class PS4Gamepad : IGamepad
{

    private readonly DualShockGamepad _reference;
    private IGamepadEventHandler eventHandler;
    private List<IGamepad.Key> pressedButtons = new List<IGamepad.Key>();

    public PS4Gamepad(int id, DualShockGamepad psGamepad)
    {
        this.Id = id;
        this.Type = "PS4";
        this._reference = psGamepad;
    }
    public PS4Gamepad(int id, DualShockGamepad psGamepad, IGamepadEventHandler eventHandler)
    {
        this.Id = id;
        this.Type = "PS4";
        this._reference = psGamepad;
        SetGamepadEventHandler(eventHandler);
    }

    public int Id { get; set; }
    public string Type { get; set; }

    public string ControllerSpecificStatus => this._reference.enabled ? "ONLINE" : "OFFLINE";

    public Vector2 GetAnalogMovement(IGamepad.Analog analog)
    {
        Vector2 values = Vector2.zero;
        if (analog == IGamepad.Analog.Left) values = _reference.leftStick.ReadValue();
        else values = _reference.rightStick.ReadValue();
        if (analog == IGamepad.Analog.Left)
        {
            if (invertedLeftHorizontal) values.x *= -1;
            if (invertedLeftVertical) values.y *= -1;
        }
        else if (analog == IGamepad.Analog.Right)
        {
            if (invertedRightHorizontal) values.x *= -1;
            if (invertedRightVertical) values.y *= -1;
        }
        return values;
    }

    public Vector2 GetAnalogMovement(IGamepad.Analog analog, Vector2 friction)
    {
        friction = new Vector2(Mathf.Abs(friction.x), Mathf.Abs(friction.y));
        if (friction.x > 1 || friction.y > 1) throw new System.ArgumentException("Friction values must be in range 0, 1");
        Vector2 analogData = GetAnalogMovement(analog);
        Vector2 analogDataToCheck = new Vector2(Mathf.Abs(analogData.x), Mathf.Abs(analogData.y));
        if (analogDataToCheck.x < friction.x) analogData.x = 0;
        if (analogDataToCheck.y < friction.y) analogData.y = 0;
        return analogData;
    }

    private bool invertedLeftHorizontal = false;
    private bool invertedLeftVertical = false;
    private bool invertedRightHorizontal = false;
    private bool invertedRightVertical = false;

    public void InvertAnalog(IGamepad.Analog analog)
    {
        InvertAnalog(analog, IGamepad.Orientation.Horizontal);
        InvertAnalog(analog, IGamepad.Orientation.Vertical);
    }

    public void InvertAnalog(IGamepad.Analog analog, IGamepad.Orientation orientation)
    {
        if (analog == IGamepad.Analog.Left)
        {
            if (orientation == IGamepad.Orientation.Horizontal)
                invertedLeftHorizontal = !invertedLeftHorizontal;
            if (orientation == IGamepad.Orientation.Vertical)
                invertedLeftVertical = !invertedLeftVertical;
        }
        else if (analog == IGamepad.Analog.Right)
        {
            if (orientation == IGamepad.Orientation.Horizontal)
                invertedRightHorizontal = !invertedRightHorizontal;
            if (orientation == IGamepad.Orientation.Vertical)
                invertedRightVertical = !invertedRightVertical;
        }
    }

    public bool IsButtonPressed(IGamepad.Key key, IGamepad.PressureType pressure)
    {
        ButtonControl button;
        switch (key)
        {
            case IGamepad.Key.ActionButtonDown:
                button = _reference.crossButton;
                break;
            case IGamepad.Key.ActionButtonRight:
                button = _reference.circleButton;
                break;
            case IGamepad.Key.ActionButtonUp:
                button = _reference.triangleButton;
                break;
            case IGamepad.Key.ActionButtonLeft:
                button = _reference.squareButton;
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
            case IGamepad.Key.MovementButtonRight:
                button = _reference.dpad.right;
                break;
            case IGamepad.Key.MovementButtonLeft:
                button = _reference.dpad.left;
                break;
            case IGamepad.Key.MovementButtonDown:
                button = _reference.dpad.down;
                break;
            case IGamepad.Key.MovementButtonUp:
                button = _reference.dpad.up;
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
