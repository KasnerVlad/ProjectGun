using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CustomDelegats;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
using CTSCancelLogic;
public class Presenter
{
    private readonly View _view;
    private readonly ISaveSystem _saveSystem;
    private GameData _gameData;
    private readonly List<InventorySlots> _slots;
    private readonly Vm<int> _m;
    private readonly Transform _playerTransform;
    public Presenter(View view, ISaveSystem saveSystem, GameData gameData, List<InventorySlots> slots, Vm<int> m, Transform playerTransform)
    {
        _view = view;
        _saveSystem = saveSystem;
        _gameData = gameData;
        _slots = slots;
        _m = m;
        _playerTransform = playerTransform;
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void UpdateName(string name) => _gameData.SaveName = name;
    public void UpdatePosition(Vector3 vector) => _gameData.PlayerPosition = vector;
    public void UpdateRotation(Quaternion quaternion) => _gameData.PlayerRotation = quaternion;
    public void UpdateSlotsItems(List<Item> slots) => _gameData.items = slots;
    public void UpdateSlotItemsAmount(List<int> amount) => _gameData.itemsAmount = amount;
    public void UpdateCurrentHotBarSlot(int num)=>_gameData.currentHotBarSlot = num;
    public void OnLoad()
    {
        _gameData = _saveSystem.Load<GameData>();
        
        Debug.Log($"name {_gameData.SaveName}, pos {_gameData.PlayerPosition}, rot {_gameData.PlayerRotation}, items {_gameData.items}, amount {_gameData.itemsAmount}, currentHotBarSlot {_gameData.currentHotBarSlot}");
        _playerTransform.position = _gameData.PlayerPosition;
        _playerTransform.rotation = _gameData.PlayerRotation;
        if(_m!=null){ _m.Invoke(_gameData.currentHotBarSlot);}
        else {Debug.LogError("Nothing to load"); }
        UpdateSlotsItems();
        _view.Display(_gameData);
        InventoryEvents.InvokeSlotsItemChanged();
        LoadingScreenLogic.inistate.cts.Cancel();
        LoadingScreenLogic.endloadingM.Invoke(1f, LoadingScreenLogic.sliderReturen.Invoke().gameObject, CancelAndRestartTokens.GiveDictinary(new []{LoadingScreenLogic.sliderReturen.Invoke().gameObject}, new []{new CancellationTokenSource()}), LoadingScreenLogic.speedReturen.Invoke());
    }
    private void UpdateSlotsItems()
    {
        foreach (var slot in _slots)
        {
            int index = _slots.IndexOf(slot);
            slot.ClearSlot();
            if (_gameData.items.Count >= index && _gameData.itemsAmount.Count >= index){ slot.AddItem(_gameData.items[index], _gameData.itemsAmount[index]);}
            else { throw new Exception($"Failed to update slot items. index: {index}, _gameData.items.Count {_gameData.items.Count}"); }
        }
    }
    public void OnSave() => _saveSystem.Save(_gameData); 
}
