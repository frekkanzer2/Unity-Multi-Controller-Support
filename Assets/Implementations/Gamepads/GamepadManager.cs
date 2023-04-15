using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadManager : MonoBehaviour
{

    private List<IGamepad> gamepads;
    private List<PlayerControllerDto> associations;
    [SerializeField]
    private bool debugEnabled;
    private string debugPrefix = "GamepadManager >";

    void Start()
    {
        gamepads = new();
        associations = new();
        ReloadAvailableGamepads();
        DisplayGamepadsStatus();
        StartCoroutine(ReloadAvailableGamepadsCoroutine(60));
    }

    void Update()
    {
        GamepadPressDetection();
        if (Input.GetKeyDown(KeyCode.G))
            DisplayGamepadsStatus();
    }

    public void DisplayGamepadsStatus()
    {
        if (gamepads.IsEmpty()) Debug.LogWarning($"{debugPrefix} No gamepad is connected");
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

    #region Private methods

    private IEnumerator ReloadAvailableGamepadsCoroutine(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReloadAvailableGamepads();
        StartCoroutine(ReloadAvailableGamepadsCoroutine(seconds));
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
    }

    private void GamepadPressDetection()
    {
        if (gamepads.IsEmpty() || !debugEnabled) return;
        foreach(IGamepad gamepad in gamepads)
        {
            string gamepadPrefix = $"on gamepad {gamepad.Id}";
            if (gamepad.HasPressedButton1())
            {
                Debug.Log($"{debugPrefix} Button 1 pressed {gamepadPrefix}");
            }
            if (gamepad.HasPressedButton2())
            {
                Debug.Log($"{debugPrefix} Button 2 pressed {gamepadPrefix}");
            }
            if (gamepad.HasPressedButton3())
            {
                Debug.Log($"{debugPrefix} Button 3 pressed {gamepadPrefix}");
            }
            if (gamepad.HasPressedButton4())
            {
                Debug.Log($"{debugPrefix} Button 4 pressed {gamepadPrefix}");
            }
            if (gamepad.GetAnalogMovement() != Vector2.zero)
            {
                Vector2 movement = gamepad.GetAnalogMovement();
                Debug.Log($"{debugPrefix} Movement (x:{movement.x},y:{movement.y}) {gamepadPrefix}");
            }
        }

    }

    #endregion
}
