#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
[CustomPropertyDrawer(typeof(DropDown))]
public class DynamicDropdownDrawer : PropertyDrawer
{
    private static readonly Dictionary<CacheKey, string[]> _methodCache = 
        new Dictionary<CacheKey, string[]>();

    private struct CacheKey
    {
        public Type TargetType;
        public string MethodName;

        public CacheKey(Type type, string method)
        {
            TargetType = type;
            MethodName = method;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (DropDown)attribute;
        var key = new CacheKey(attr.TargetType, attr.MethodName);

        // Пытаемся получить данные из кэша
        if (!_methodCache.TryGetValue(key, out string[] options))
        {
            options = GetOptionsViaReflection(attr, position);
            if (options == null) return;

            // Сохраняем в кэш
            _methodCache[key] = options;
        }

        // Отрисовка dropdown
        DrawDropdown(position, property, label, options);
    }

    private string[] GetOptionsViaReflection(DropDown attr, Rect position)
    {
        MethodInfo method = attr.TargetType.GetMethod(
            attr.MethodName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
        );

        if (method == null)
        {
            EditorGUI.HelpBox(position, $"Метод {attr.MethodName} не найден!", MessageType.Error);
            return null;
        }

        object result = method.Invoke(null, null);
        return ConvertResult(result);
    }

    private void DrawDropdown(Rect position, SerializedProperty property, GUIContent label, string[] options)
    {
        if (options == null || options.Length == 0)
        {
            EditorGUI.HelpBox(position, "Нет доступных опций!", MessageType.Warning);
            return;
        }

        GUIContent[] guiOptions = options.Select(opt => new GUIContent(opt)).ToArray();
        int selectedIndex = Mathf.Clamp(Array.IndexOf(options, property.stringValue), 0, options.Length - 1);
        
        selectedIndex = EditorGUI.Popup(
            position: position,
            label: label,
            selectedIndex: selectedIndex,
            displayedOptions: guiOptions
        );

        property.stringValue = options[selectedIndex];
    }

    private string[] ConvertResult(object result)
    {
        switch (result)
        {
            case string[] arr: return arr;
            case List<string> list: return list.ToArray();
            case IEnumerable<string> enumerable: return enumerable.ToArray();
            default: return null;
        }
    }
}
#endif