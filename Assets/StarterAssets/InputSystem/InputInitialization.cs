using System;
using UnityEngine;
public class InputInitialization : MonoBehaviour
{
    public static InputInitialization instance;
    public KeyCode aiming;
    public KeyCode singleFire;
    public KeyCode multipleFire;
    public KeyCode reload;
    public KeyCode take;
    public KeyCode hide;
    public KeyCode toggleFireMode;
    public KeyCode addItem;
    public KeyCode removeItem;
    public KeyCode clearInventory;
    public KeyCode toggleInventory;
    public KeyCode startDragging;
    public KeyCode endDragging; 
    public KeyCode jump;
    public KeyCode sprint;
    public KeyCode toggleHotbar;
    [DropDown(typeof(Axis), "GetAxes")]
    public string scrollAxis;
    [DropDown(typeof(Axis), "GetAxes")]
    public string lookX;
    [DropDown(typeof(Axis), "GetAxes")]
    public string lookY;
    [DropDown(typeof(Axis), "GetAxes")]
    public string moveX;
    [DropDown(typeof(Axis), "GetAxes")]
    public string moveY;
    private void Start() => Initialize();

    private void Initialize()
    {
        if (instance == null){instance = this;}
        else { Destroy(this); }
        DontDestroyOnLoad(this);
        KeyCodeManager.aiming = aiming;
        KeyCodeManager.singleFire = singleFire;
        KeyCodeManager.multipleFire = multipleFire;
        KeyCodeManager.reload = reload;
        KeyCodeManager.take = take;
        KeyCodeManager.hide = hide;
        KeyCodeManager.toggleFireMode = toggleFireMode;
        KeyCodeManager.addItem = addItem;
        KeyCodeManager.removeItem = removeItem;
        KeyCodeManager.clearInventory = clearInventory;
        KeyCodeManager.toggleInventory = toggleInventory;
        KeyCodeManager.startDragging = startDragging;
        KeyCodeManager.endDragging = endDragging;
        KeyCodeManager.scrollAxis = scrollAxis;
        KeyCodeManager.jump = jump;
        KeyCodeManager.toggleHotbar = toggleHotbar;
        KeyCodeManager.sprint = sprint;
        KeyCodeManager.moveX = moveX;
        KeyCodeManager.moveY = moveY;
        KeyCodeManager.lookX = lookX;
        KeyCodeManager.lookY = lookY;
        Input2.analogMovement = false;
    }
}

public static class KeyCodeManager
{
    public static KeyCode aiming;
    public static KeyCode singleFire;
    public static KeyCode multipleFire;
    public static KeyCode reload;
    public static KeyCode take;
    public static KeyCode hide;
    public static KeyCode toggleFireMode;
    public static KeyCode addItem;
    public static KeyCode removeItem;
    public static KeyCode clearInventory;
    public static KeyCode toggleInventory;
    public static KeyCode startDragging;
    public static KeyCode endDragging;
    public static KeyCode jump;
    public static KeyCode sprint;
    public static KeyCode toggleHotbar;
    public static string scrollAxis;
    public static string lookX;
    public static string lookY;
    public static string moveX;
    public static string moveY;
}
[Serializable]
public static class Axis
{
    public static string scroll { get; private set; } = "Mouse ScrollWheel";
    public static string mouseX { get; private set; } = "Mouse X";
    public static string mouseY { get; private set; } = "Mouse Y";
    public static string horizontal { get; private set; } = "Horizontal";
    public static string vertical { get; private set; } = "Vertical";
    public static string[] GetAxes() => new[] { scroll, mouseX, mouseY, horizontal, vertical};
}

public static class Input2
{
    public static bool Aiming => Input.GetKey(KeyCodeManager.aiming);
    public static bool ToggleFireMode => Input.GetKeyDown(KeyCodeManager.toggleFireMode);
    public static bool SingleFire => Input.GetKeyDown(KeyCodeManager.singleFire);
    public static bool MultipleFire => Input.GetKey(KeyCodeManager.multipleFire);
    public static bool Reload => Input.GetKeyDown(KeyCodeManager.reload);
    public static bool Take => Input.GetKeyDown(KeyCodeManager.take);
    public static bool Hide => Input.GetKeyDown(KeyCodeManager.hide);
    public static bool PressedG => Input.GetKeyDown(KeyCodeManager.addItem);
    public static bool PressedR => Input.GetKeyDown(KeyCodeManager.removeItem);
    public static bool PressedC => Input.GetKeyDown(KeyCodeManager.clearInventory);
    public static bool PressedE => Input.GetKeyDown(KeyCodeManager.toggleInventory);
    public static bool StartDragging => Input.GetKeyDown(KeyCodeManager.startDragging);
    public static bool EndDragging => Input.GetKeyUp(KeyCodeManager.endDragging);
    public static bool MiddleClick => Input.GetKeyDown(KeyCodeManager.toggleHotbar);
    public static bool Jump => Input.GetKey(KeyCodeManager.jump);
    public static float Scroll => Input.GetAxis(KeyCodeManager.scrollAxis);
    public static Vector2 Look =>canLook ? new Vector2(Input.GetAxis(KeyCodeManager.lookX), Input.GetAxis(KeyCodeManager.lookY)*-1):Vector2.zero;
    public static Vector2 mousePos => new Vector2(Input.GetAxis(KeyCodeManager.lookX), Input.GetAxis(KeyCodeManager.lookY));
    public static Vector2 Move => new Vector2(Input.GetAxisRaw(KeyCodeManager.moveX), Input.GetAxisRaw(KeyCodeManager.moveY));
    public static bool Sprint => Input.GetKey(KeyCodeManager.sprint);
    public static bool canLook;
    public static bool analogMovement;
}