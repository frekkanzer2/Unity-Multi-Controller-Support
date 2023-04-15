public interface IGamepadEventHandler
{
    void OnGamepadConnected();
    void OnGamepadDeconnected();
    void OnButtonSinglePression(IGamepad.Key key);
    void OnButtonContinuePression(IGamepad.Key key);
}
