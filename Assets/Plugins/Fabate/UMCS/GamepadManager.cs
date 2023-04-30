using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
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

    public List<IGamepad> GetGamepads() => this.gamepads;

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

    public bool IsButtonPressedFromAnyGamepad(IGamepad.Key key, IGamepad.PressureType pressure)
    {
        foreach (IGamepad g in this.gamepads)
            if (g.IsButtonPressed(key, pressure)) return true;
        return false;
    }
    public List<IGamepad> GetGamepadsByPressingButton(IGamepad.Key key, IGamepad.PressureType pressure)
    {
        List<IGamepad> list = new();
        foreach (IGamepad g in this.gamepads)
            if (g.IsButtonPressed(key, pressure))
                list.Add(g);
        return list;
    }

    public void ReloadAvailableGamepads()
    {
        if (debugEnabled) Debug.Log("Reloading gamepads");
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
        if (debugEnabled) Debug.Log($"Found {gamepadsFromInputSystem.Length} gamepads from Input System");
        foreach (Gamepad gp in gamepadsFromInputSystem)
        {
            if (gp is XInputController)
            {
                if (debugEnabled) Debug.Log($"Found Xbox controller: {gp.name}-{gp.displayName}|{gp.device}-{gp.deviceId}|{gp.description}");
                gamepads.Add(new XboxGamepad(lastGamepadId, (XInputController)gp));
                lastGamepadId++;
            }
            else if (gp is DualShockGamepad)
            {
                if (debugEnabled) Debug.Log($"Found PlayStation controller: {gp.name}-{gp.displayName}|{gp.device}-{gp.deviceId}|{gp.description}");
                gamepads.Add(new PS4Gamepad(lastGamepadId, (DualShockGamepad)gp));
                lastGamepadId++;
            }
            else
            if (debugEnabled) Debug.Log($"Detected not managed gamepad: {gp.name}-{gp.displayName}|{gp.device}-{gp.deviceId}|{gp.description}");
        }
        Joystick[] joysticksFromInputSystem = Joystick.all.ToArray();
        if (debugEnabled) Debug.Log($"Found {gamepadsFromInputSystem.Length} joysticks from Input System");
        foreach (Joystick j in joysticksFromInputSystem)
        {
            if (j.name.Contains("ZhiXu"))
            {
                if (debugEnabled) Debug.Log($"Detected ZhiXu Pro Controller gamepad: {j.name}-{j.displayName}|{j.device}-{j.deviceId}|{j.description}");
                gamepads.Add(new ZhiXuGamepad(lastGamepadId, j));
                lastGamepadId++;
            }
            else
            if (debugEnabled) Debug.Log($"Detected not managed gamepad: {j.name}-{j.displayName}|{j.device}-{j.deviceId}|{j.description}");
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
