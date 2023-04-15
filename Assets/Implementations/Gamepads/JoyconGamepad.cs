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
        throw new System.NotImplementedException();
    }

    public bool HasPressedButton1()
    {
        throw new System.NotImplementedException();
    }

    public bool HasPressedButton2()
    {
        throw new System.NotImplementedException();
    }

    public bool HasPressedButton3()
    {
        throw new System.NotImplementedException();
    }

    public bool HasPressedButton4()
    {
        throw new System.NotImplementedException();
    }

    public bool IsConnected()
    {
        bool isConnected = _reference.state != Joycon.state_.DROPPED;
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

    public bool IsStartPressed()
    {
        throw new System.NotImplementedException();
    }

    public void SetPlayerReference(IPlayer player)
    {
        this.player = player;
    }
}
