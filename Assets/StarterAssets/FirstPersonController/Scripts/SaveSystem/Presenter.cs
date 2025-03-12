using System.Collections.Generic;
using UnityEngine;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
public class Presenter
{
    private View _view;
    private ISaveSystem _saveSystem;
    private GameData _gameData;
    private List<InventorySlots> _slots;
    public Presenter(View view, ISaveSystem saveSystem, GameData gameData, List<InventorySlots> slots)
    {
        _view = view;
        _saveSystem = saveSystem;
        _gameData = gameData;
        view.Initialize(this);
        _slots = slots;
    }

    public void UpdateName(string name) => _gameData.SaveName = name;
    public void UpdatePosition(Vector3 vector) => _gameData.PlayerPosition = vector;
    public void UpdateRotation(Quaternion quaternion) => _gameData.PlayerRotation = quaternion;
    public void UpdateSlotsItems(List<Item> slots) => _gameData.items = slots;
    public void UpdateSlotItemsAmount(List<int> amount) => _gameData.itemsAmount = amount;
    public void OnLoad()
    {
        _gameData = _saveSystem.Load<GameData>();
        SaveManager._GameSaveManager.playerPosition.position = _gameData.PlayerPosition;
        SaveManager._GameSaveManager.playerPosition.rotation = _gameData.PlayerRotation;
        UpdateSlotsItems();
        _view.Display(_gameData);
    }
    private void UpdateSlotsItems()
    {
        foreach (var slot in _slots)
        {
            int currentSlotItem = _slots.IndexOf(slot);
            slot.ClearSlot();
            slot.AddItem(_gameData.items[currentSlotItem], _gameData.itemsAmount[currentSlotItem]);
        }
    }

    public void OnSave() => _saveSystem.Save(_gameData); 
}
