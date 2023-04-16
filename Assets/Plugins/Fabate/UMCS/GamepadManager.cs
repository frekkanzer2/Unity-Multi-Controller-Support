using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class GamepadManager : MonoBehaviour
{

    private List<IGamepad> gamepads;
    private List<PlayerControllerAssociationDto> associations;
    [SerializeField]
    private bool debugEnabled;
    private string debugPrefix = "GamepadManager >";

    private static GamepadManager _instance = null;
    public static GamepadManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Start()
    {
        if (_instance == null) _instance = this;
        gamepads = new();
        associations = new();
        ReloadAvailableGamepads();
        if (this.debugEnabled)
            Debug.Log($"{this.debugPrefix} Press 'G' to see every supported connected gamepad\nPress 'R' to force gamepads detection");
    }

    void Update()
    {
        FetchGamepads();
        JoyconManager.Instance._OnUpdate(); // Necessary update for joycons
        if (Input.GetKeyDown(KeyCode.G))
            DisplayGamepadsStatus();
        if (Input.GetKeyDown(KeyCode.R))
            ReloadAvailableGamepads();
        GamepadPressDetection();
    }

    void FixedUpdate()
    {
    }

    public void DisplayGamepadsStatus()
    {
        if (gamepads.IsEmpty()) Debug.LogWarning($"{debugPrefix} No supported gamepad is connected");
        else foreach (IGamepad gamepad in gamepads) Debug.Log($"{debugPrefix} {gamepad.Status}");
    }

    public List<PlayerControllerAssociationDto> Associations
    {
        get
        {
            return associations;
        }
    }

    public void AddAssociation(int playerNumber, int gamepadId) => this.associations.Add(new PlayerControllerAssociationDto() { PlayerNumber = playerNumber, ControllerId = gamepadId });
    public PlayerControllerAssociationDto? GetAssociationByPlayerNumber(int playerNumber) => this.associations.Find(association => association.PlayerNumber == playerNumber);
    public PlayerControllerAssociationDto? GetAssociationByGamepadId(int gamepadId) => this.associations.Find(association => association.ControllerId == gamepadId);

    public void RemoveAssociation(int playerNumber) => this.associations.RemoveAll(association => association.PlayerNumber == playerNumber);

    public IGamepad? GetGamepadById(int id) => this.gamepads.Find(gamepad => gamepad.Id == id);
    public IGamepad? GetGamepadByIndex(int index) => this.gamepads[index];
    public IGamepad? GetGamepadByAssociation(PlayerControllerAssociationDto pcaDto) => this.gamepads.Find(gamepad => gamepad.Id == pcaDto.ControllerId);

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
        Gamepad[] gamepadsFromInputSystem = Gamepad.all.ToArray();
        foreach (Gamepad gp in gamepadsFromInputSystem)
        {
            if (gp is XInputController)
            {
                gamepads.Add(new XboxGamepad(lastGamepadId, (XInputController)gp));
                lastGamepadId++;
            }
            else if (gp is DualShockGamepad)
            {

            }
        }
    }

    private void OnApplicationQuit()
    {
        JoyconManager.Instance.OnApplicationQuit();
    }

    #region Private methods

    private void FetchGamepads()
    {
        InputSystem.onDeviceChange +=
        (device, change) =>
        {
            ReloadAvailableGamepads();
        };
    }

    private void GamepadPressDetection()
    {
        if (gamepads.IsEmpty() || !debugEnabled) return;
        foreach(IGamepad gamepad in gamepads)
        {
            string gamepadPrefix = $"on gamepad {gamepad.Id}";
            if (gamepad.IsButtonPressed(IGamepad.Key.ActionButtonDown, IGamepad.PressureType.Single))
            {
                Debug.Log($"{debugPrefix} Button 1 pressed {gamepadPrefix}");
            }
            if (gamepad.IsButtonPressed(IGamepad.Key.ActionButtonRight, IGamepad.PressureType.Single))
            {
                Debug.Log($"{debugPrefix} Button 2 pressed {gamepadPrefix}");
            }
            if (gamepad.IsButtonPressed(IGamepad.Key.ActionButtonUp, IGamepad.PressureType.Single))
            {
                Debug.Log($"{debugPrefix} Button 3 pressed {gamepadPrefix}");
            }
            if (gamepad.IsButtonPressed(IGamepad.Key.ActionButtonLeft, IGamepad.PressureType.Single))
            {
                Debug.Log($"{debugPrefix} Button 4 pressed {gamepadPrefix}");
            }
        }

    }

    #endregion
}
