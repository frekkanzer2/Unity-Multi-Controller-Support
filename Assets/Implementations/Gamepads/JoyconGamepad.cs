using UnityEngine;

public class JoyconGamepad : IGamepad
{

    private readonly Joycon _reference;
    private IPlayer player;

    public JoyconGamepad(int id, Joycon reference)
    {
        Id = id;
        Type = "Joycon";
        _reference = reference;
    }

    public int Id { get ; set ; }
    public string Type { get; set; }

    public string ControllerSpecificStatus => $"{_reference.state}";

    public Vector2 GetAnalogMovement()
    {
        return _reference.GetStick();
    }

    public bool HasPressedButton1()
    {
        if (_reference.isLeft)
            return _reference.GetButton(Joycon.Button.DPAD_LEFT);
        return _reference.GetButton(Joycon.Button.DPAD_RIGHT);
    }

    public bool HasPressedButton2()
    {
        if (_reference.isLeft)
            return _reference.GetButton(Joycon.Button.DPAD_DOWN);
        return _reference.GetButton(Joycon.Button.DPAD_UP);
    }

    public bool HasPressedButton3()
    {
        if (_reference.isLeft)
            return _reference.GetButton(Joycon.Button.DPAD_RIGHT);
        return _reference.GetButton(Joycon.Button.DPAD_LEFT);
    }

    public bool HasPressedButton4()
    {
        if (_reference.isLeft)
            return _reference.GetButton(Joycon.Button.DPAD_UP);
        return _reference.GetButton(Joycon.Button.DPAD_DOWN);
    }

    public bool IsConnected()
    {
        bool isConnected = _reference.state == Joycon.state_.IMU_DATA_OK;
        if (player != null && !isConnected) player.SetDead();
        return isConnected;
    }

    public bool IsPressingButton1()
    {
        throw new System.NotImplementedException();
    }

    public bool IsPressingButton2()
    {
        throw new System.NotImplementedException();
    }

    public bool IsPressingButton3()
    {
        throw new System.NotImplementedException();
    }

    public bool IsPressingButton4()
    {
        throw new System.NotImplementedException();
    }

    public void SetPlayerReference(IPlayer player)
    {
        this.player = player;
    }

    public bool IsStartPressed()
    {
        return _reference.GetButton(Joycon.Button.PLUS) || _reference.GetButton(Joycon.Button.MINUS);
    }
}
