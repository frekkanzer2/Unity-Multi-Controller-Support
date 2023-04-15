using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadManager : MonoBehaviour
{

    private List<IGamepad> gamepads;
    private List<Joycon> joyconsFromExternalManager;

    private List<PlayerControllerDto> associations;

    void Start()
    {
        gamepads = new();
        associations = new();
        joyconsFromExternalManager = JoyconManager.Instance.j;
        int lastGamepadId = 1;
        if (!joyconsFromExternalManager.IsEmpty())
            foreach (Joycon j in joyconsFromExternalManager)
            {
                gamepads.Add(new JoyconGamepad(lastGamepadId, j));
                lastGamepadId++;
            }
        DisplayGamepadsStatus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            DisplayGamepadsStatus();
    }

    public void DisplayGamepadsStatus()
    {
        if (gamepads.IsEmpty()) Debug.LogWarning("GamepadManager > No gamepad is connected");
        else foreach (IGamepad gamepad in gamepads) Debug.Log("GamepadManager > " + gamepad.Status);
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
