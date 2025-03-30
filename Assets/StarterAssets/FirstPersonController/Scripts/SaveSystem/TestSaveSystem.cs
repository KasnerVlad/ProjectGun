using UnityEngine;
using System.Threading.Tasks;
using CInvoke;
public class TestSaveSystem : MonoBehaviour
{
    [Tooltip("In milliseconds")]
    [SerializeField] private int timeMiddleTask;
    [SerializeField] private bool work;
    /*[SerializeField] private bool work2;*/
    private void Start()
    {
        _=CustomInvoke.Invoke(Test, 100);
    }
    private async Task Test()
    {
        while (work&&Application.isPlaying)
        {
            await Task.Delay(timeMiddleTask);
            SaveManager._GameSaveManager.SavePlayerPosition();
            SaveManager._GameSaveManager.SaveSlots();
            SaveManager._GameSaveManager.SaveCurrentHotBarSlot();
            SaveManager._GameSaveManager.OnSave();
            Debug.Log(SaveManager._GameSaveManager.playerPosition);
            Debug.Log("Saved");
            await Task.Delay(timeMiddleTask);
            SaveManager._GameSaveManager.OnLoad();
            await Task.Yield();
        }
    }
}