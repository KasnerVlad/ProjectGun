using System;
using System.IO;
using UnityEngine;
public class JsonSaveSystem : ISaveSystem
{
    private readonly string _filePath;

    public JsonSaveSystem()
    {
        _filePath = Application.persistentDataPath + "/save.json";
        Debug.Log(_filePath);
    }
    public void Save<T>(T saveData)
    {
        var json = JsonUtility.ToJson(saveData);
        using (var writer = new StreamWriter(_filePath))
        {
            writer.Write(json);
        }
    }
    public T Load<T>()
    {
        string json = "";
        using (var reader = new StreamReader(_filePath))
        {
            string line;
            while ((line = reader.ReadLine())!=null)
            {
                json += line;
            }
        }
        if (string.IsNullOrEmpty(json))
        {
            return Activator.CreateInstance<T>();
        }
        Debug.Log(json);
        /*BildDebug.inistate.Log(json);*/
        return JsonUtility.FromJson<T>(json);
    }
}
