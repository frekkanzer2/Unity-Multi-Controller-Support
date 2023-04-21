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
        Vector2 values = Vector2.zero;
        Vector2 stick = _reference.GetStick();
        if (_reference.isLeft)
            values = new Vector2(stick.y, stick.x * -1);
        else
            values = new Vector2(stick.y * -1, stick.x);
        if (analog == IGamepad.Analog.Left && _reference.isLeft)
        {
            if (invertedLeftHorizontal) values.x *= -1;
            if (invertedLeftVertical) values.y *= -1;
        }
        else if (analog == IGamepad.Analog.Right && !_reference.isLeft)
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
