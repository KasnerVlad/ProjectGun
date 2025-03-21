using System;
using System.Collections.Generic;
using UnityEngine;
using CustomDelegats;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
public class Presenter
{
    private readonly View _view;
    private readonly ISaveSystem _saveSystem;
    private GameData _gameData;
    private readonly List<InventorySlots> _slots;
    private readonly Vm<int> _m;
    public Presenter(View view, ISaveSystem saveSystem, GameData gameData, List<InventorySlots> slots, Vm<int> m)
    {
        _view = view;
        _saveSystem = saveSystem;
        _gameData = gameData;
        _slots = slots;
        _m = m;
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
        SaveManager._GameSaveManager.playerPosition.position = _gameData.PlayerPosition;
        SaveManager._GameSaveManager.playerPosition.rotation = _gameData.PlayerRotation;
        if(_m!=null){ _m.Invoke(_gameData.currentHotBarSlot);}
        else {Debug.LogError("Nothing to load"); }
        UpdateSlotsItems();
        _view.Display(_gameData);
        InventoryEvents.InvokeSlotsItemChanged();
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
