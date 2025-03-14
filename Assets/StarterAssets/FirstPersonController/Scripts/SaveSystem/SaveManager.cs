using System;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager _GameSaveManager;
    [SerializeField] private bool _createNullData;
    public Transform playerPosition;
    private View _view;
    private Presenter _presenter;
    private ISaveSystem _saveSystem;
    private GameData _gameData;
    private InventorySystem2 _inventorySystem2;
    public void OnSave()
    {
        _presenter.UpdateName((int.Parse(_gameData.SaveName) + 1).ToString());
        _presenter.UpdatePosition(playerPosition.position);
        _presenter.UpdateRotation(playerPosition.rotation);
        _presenter.UpdateSlotsItems(_inventorySystem2.GetSlotsItems());
        _presenter.UpdateSlotItemsAmount(_inventorySystem2.GetSlotsItemAmount());
        _presenter.OnSave();
    }
    private void Start()
    {
        if (_GameSaveManager == null) { _GameSaveManager = this; } 
        else { Destroy(gameObject); } 
        DontDestroyOnLoad(gameObject);
        
        _inventorySystem2 = GetComponent<InventorySystem2>();
        _view = GetComponent<View>();
        _saveSystem = new JsonSaveSystem();
        StartGameData();
        _presenter = new Presenter(_view, _saveSystem, _gameData, _inventorySystem2.slots);
        _presenter.OnLoad();
        InventoryEvents.InvokeInventoryUpdated(_inventorySystem2.slots);
    }

    private void StartGameData()
    {
        _gameData = _saveSystem.Load<GameData>();
        if (_gameData==null||_gameData.SaveName==""||_createNullData) {           
            _gameData = new GameData()
            {
                SaveName = "0",
                PlayerPosition = playerPosition.position,
                PlayerRotation = playerPosition.rotation, 
                items = _inventorySystem2.GetSlotsItems(),
                itemsAmount = _inventorySystem2.GetSlotsItemAmount()
            };
            _saveSystem.Save(_gameData);
        }

    }
}
