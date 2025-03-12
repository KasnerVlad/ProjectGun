using System.Net.Mime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class View: MonoBehaviour
{
    private Presenter _presenter;
    public Text saveNameText; 
    public void Initialize(Presenter presenter) => _presenter = presenter; 
    public void Display(GameData gameData) => saveNameText.text = gameData.SaveName; 
}
