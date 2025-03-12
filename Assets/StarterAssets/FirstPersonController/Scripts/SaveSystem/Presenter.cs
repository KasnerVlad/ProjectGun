using UnityEngine;
public class Presenter
{
    private View _view;
    private ISaveSystem _saveSystem;
    private GameData _gameData;

    public Presenter(View view, ISaveSystem saveSystem, GameData gameData)
    {
        _view = view;
        _saveSystem = saveSystem;
        _gameData = gameData;
        view.Initialize(this);
    }

    public void UpdateName(string name) => _gameData.SaveName = name;
    
    public void UpdateVector(Vector3 vector) => _gameData.PlayerPosition = vector;
    public void UpdateRotation(Quaternion quaternion) => _gameData.PlayerRotation = quaternion;
    public void OnLoad()
    {
        _gameData = _saveSystem.Load<GameData>();
        SaveManager._GameSaveManager.playerPosition.position = _gameData.PlayerPosition;
        SaveManager._GameSaveManager.playerPosition.rotation = _gameData.PlayerRotation;
        _view.Display(_gameData);
    }
    public void OnSave() => _saveSystem.Save(_gameData); 
}
