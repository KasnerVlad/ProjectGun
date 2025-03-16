using System.Net.Mime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class View: MonoBehaviour
{
    public Text saveNameText; 
    public void Display(GameData gameData) => saveNameText.text = gameData.SaveName; 
}
