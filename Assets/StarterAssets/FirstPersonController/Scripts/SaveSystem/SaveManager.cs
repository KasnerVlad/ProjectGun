using System;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
using UnityEngine;
using System.Collections.Generic;
using CustomDelegats;
using CInvoke;
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
    private List<InventorySlots> slots;
    private HotBarManager _hotBarManager;
    private Vm<int> _m;
    
    public void InitializeSlots(List<InventorySlots> _slots)=>slots = _slots;
    public void InitializeHotBarManager(HotBarManager _hotBarManager, Vm<int> m){this._hotBarManager = _hotBarManager; _m = m;}
    private void Awake(){        
        if (_GameSaveManager == null) { _GameSaveManager = this; } 
        else { Destroy(gameObject); } 
        DontDestroyOnLoad(gameObject);
        InventoryEvents.InvokeOnInit();
    }
    public void OnSave()
    {
        SaveName();
        _presenter.OnSave();
    }
    private void OnDestroy()
    {
        SavePlayerPosition();
        OnSave();
    }
    private void SaveName()
    {
        _presenter.UpdateName((int.Parse(_gameData.SaveName) + 1).ToString());
    }
    public void SavePlayerPosition()
    {
        if (playerPosition != null)
        {
            _presenter.UpdatePosition(playerPosition.position);
            _presenter.UpdateRotation(playerPosition.rotation);
        }
    }
    public void SaveSlots()
    {
        _presenter.UpdateSlotsItems(_inventorySystem2.GetSlotsItems());
        _presenter.UpdateSlotItemsAmount(_inventorySystem2.GetSlotsItemAmount());
    }

    public void SaveCurrentHotBarSlot()
    {
        _presenter.UpdateCurrentHotBarSlot(_hotBarManager.currentHotBarSlot);
    }
    public void OnLoad()=>_presenter.OnLoad();
    private void Start()
    {
        _inventorySystem2 = GetComponent<InventorySystem2>();
        _view = GetComponent<View>();
        _saveSystem = new JsonSaveSystem();
        StartGameData();
        _presenter = new Presenter(_view, _saveSystem, _gameData, slots, _m, playerPosition);

            _presenter.OnLoad();
            _=CustomInvoke.Invoke(InventoryEvents.InvokeInventoryUpdated, 10); 
            InventoryEvents.InvokeLoad();
        
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
