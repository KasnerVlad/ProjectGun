using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager _GameSaveManager;
    public Transform playerPosition;
    private View _view;
    private Presenter _presenter;
    private ISaveSystem _saveSystem;
    private GameData _gameData;
    private void OnEnable() { _GameSaveManager = this; }
    private void OnDestroy() => OnSave(); 
    private void OnSave()
    {
        _presenter.UpdateName((int.Parse(_gameData.SaveName) + 1).ToString());
        _presenter.UpdateVector(playerPosition.position);
        _presenter.UpdateRotation(playerPosition.rotation);
        _presenter.OnSave();
    }
    private void Start()
    {
        _view = GetComponent<View>();
        _saveSystem = new JsonSaveSystem();
        StartGameData();
        _presenter = new Presenter(_view, _saveSystem, _gameData);
        _presenter.OnLoad();
    }

    private void StartGameData()
    {
        _gameData = _saveSystem.Load<GameData>();
        if (_gameData==null) {           
            _gameData = new GameData()
            {
                SaveName = "0",
                PlayerPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, playerPosition.position.z)
            };
            _saveSystem.Save(_gameData);
                                          
        }

    }
}
