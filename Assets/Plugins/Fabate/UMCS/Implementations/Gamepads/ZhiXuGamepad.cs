using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ZhiXuGamepad : IGamepad
{
    private readonly Joystick _reference;
    private IGamepadEventHandler eventHandler;
    private List<IGamepad.Key> pressedButtons = new List<IGamepad.Key>();

    public ZhiXuGamepad(int id, Joystick hidJoystick)
    {
        this.Id = id;
        this.Type = "ZhiXu HID Gamepad";
        this._reference = hidJoystick;
    }
    public ZhiXuGamepad(int id, Joystick hidJoystick, IGamepadEventHandler eventHandler)
    {
        this.Id = id;
        this.Type = "ZhiXu HID Gamepad";
        this._reference = hidJoystick;
        SetGamepadEventHandler(eventHandler);
    }
    public int Id { get; set; }
    public string Type { get; set; }

    public string ControllerSpecificStatus => this._reference.enabled ? "ONLINE" : "OFFLINE";

    public Vector2 GetAnalogMovement(IGamepad.Analog analog)
    {
        Vector2 values = _reference.stick.ReadValue();
        if (invertedHorizontal) values.x *= -1;
        if (invertedVertical) values.y *= -1;
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

    private bool invertedHorizontal = false;
    private bool invertedVertical = false;

    public void InvertAnalog(IGamepad.Analog analog)
    {
        InvertAnalog(analog, IGamepad.Orientation.Horizontal);
        InvertAnalog(analog, IGamepad.Orientation.Vertical);
    }

    public void InvertAnalog(IGamepad.Analog analog, IGamepad.Orientation orientation)
    {
        if (orientation == IGamepad.Orientation.Horizontal)
            invertedHorizontal = !invertedHorizontal;
        if (orientation == IGamepad.Orientation.Vertical)
            invertedVertical = !invertedVertical;
    }

    public bool IsButtonPressed(IGamepad.Key key, IGamepad.PressureType pressure)
    {
        List<InputControl> all = new();
        all.AddRange(_reference.allControls.ToArray());
        InputControl button;
        switch (key)
        {
            case IGamepad.Key.ActionButtonDown:
                button = all.Find(i => i.name.ToUpper().Equals("TRIGGER"));
                break;
            case IGamepad.Key.ActionButtonRight:
                button = all.Find(i => i.name.ToUpper().Equals("BUTTON2"));
                break;
            case IGamepad.Key.ActionButtonUp:
                button = all.Find(i => i.name.ToUpper().Equals("BUTTON5"));
                break;
            case IGamepad.Key.ActionButtonLeft:
                button = all.Find(i => i.name.ToUpper().Equals("BUTTON4"));
                break;
            case IGamepad.Key.Start:
                button = all.Find(i => i.name.ToUpper().Equals("BUTTON12"));
                break;
            case IGamepad.Key.Select:
                button = all.Find(i => i.name.ToUpper().Equals("BUTTON11"));
                break;
            case IGamepad.Key.LeftAnalog:
                button = all.Find(i => i.name.ToUpper().Equals("BUTTON14"));
                break;
            case IGamepad.Key.RightAnalog:
                button = all.Find(i => i.name.ToUpper().Equals("BUTTON15"));
                break;
            default:
                throw new System.NotImplementedException($"Key not implemented for gamepad {Type}");
        }

        if (eventHandler != null && pressure == IGamepad.PressureType.Single) eventHandler.OnButtonSinglePression(key);
        else if (eventHandler != null && pressure == IGamepad.PressureType.Continue) eventHandler.OnButtonContinuePression(key);

        if (button.IsPressed())
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
