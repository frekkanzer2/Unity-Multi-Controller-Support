using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadManager : MonoBehaviour
{

    private List<IGamepad> gamepads;
    private List<PlayerControllerDto> associations;
    [SerializeField]
    private bool debugEnabled;
    private string debugPrefix = "GamepadManager >";
    private int numberOfDetectedGamepads = 0;

    void Start()
    {
        gamepads = new();
        associations = new();
        //StartCoroutine(ReloadAvailableGamepadsCoroutine(60));
    }

    void Update()
    {
        JoyconManager.Instance._OnUpdate(); // Necessary update for joycons
        FetchGamepads();
        if (Input.GetKeyDown(KeyCode.G))
            DisplayGamepadsStatus();
        if (Input.GetKeyDown(KeyCode.R))
            ReloadAvailableGamepads();
    }

    void FixedUpdate()
    {
        GamepadPressDetection();
    }

    public void DisplayGamepadsStatus()
    {
        if (gamepads.IsEmpty()) Debug.LogWarning($"{debugPrefix} No supported gamepad is connected");
        else foreach (IGamepad gamepad in gamepads) Debug.Log($"{debugPrefix} {gamepad.Status}");
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

    private void OnApplicationQuit()
    {
        JoyconManager.Instance.OnApplicationQuit();
    }

    #region Private methods

    private void FetchGamepads()
    {
        int actualNumberOfGamepads = Gamepad.all.Count;
        if (this.numberOfDetectedGamepads != actualNumberOfGamepads)
        {
            this.numberOfDetectedGamepads = actualNumberOfGamepads;
            ReloadAvailableGamepads();
        }
    }

    private void ReloadAvailableGamepads()
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
        Debug.Log($"{this.debugPrefix} Found {gamepads.Count} supported gamepads\nPress 'G' to see every supported connected gamepad");
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
