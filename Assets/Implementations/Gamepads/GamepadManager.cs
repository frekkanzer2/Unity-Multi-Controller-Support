using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadManager : MonoBehaviour
{

    private List<IGamepad> gamepads;
    private List<PlayerControllerDto> associations;

    void Start()
    {
        gamepads = new();
        associations = new();
        ReloadAvailableGamepads();
        DisplayGamepadsStatus();
    }

    void Update()
    {
        ReloadAvailableGamepads();
        if (Input.GetKeyDown(KeyCode.G))
            DisplayGamepadsStatus();
    }

    public void DisplayGamepadsStatus()
    {
        if (gamepads.IsEmpty()) Debug.LogWarning("GamepadManager > No gamepad is connected");
        else foreach (IGamepad gamepad in gamepads) Debug.Log("GamepadManager > " + gamepad.Status);
    }

    public void ReloadAvailableGamepads()
    {
        gamepads = new();
        associations = new();
        int lastGamepadId = 1;
        JoyconManager.Instance.ReloadJoycons();
        if (!JoyconManager.Instance.j.IsEmpty())
            foreach (Joycon j in JoyconManager.Instance.j)
            {
                gamepads.Add(new JoyconGamepad(lastGamepadId, j));
                lastGamepadId++;
            }
    }

    public List<PlayerControllerDto> Associations
    {
        get
        {
            return associations;
        }
    }

    public void AddAssociation(int playerNumber, int gamepadId) => this.associations.Add(new PlayerControllerDto() { PlayerNumber = playerNumber, ControllerId = gamepadId });

    public void RemoveAssociation(int playerNumber) => this.associations.RemoveAll(association => association.PlayerNumber == playerNumber);

    public IGamepad? MainGamepad
    {
        get
        {
            if (gamepads.IsEmpty()) return null;
            for (int i = 0; i < gamepads.Count; i++)
                if (gamepads[i].IsConnected()) return gamepads[i];
            return null;
        }
    }
}
