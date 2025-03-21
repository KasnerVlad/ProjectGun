using UnityEngine;
using System;
public class DropDown : PropertyAttribute
{
    public Type TargetType { get; }
    public string MethodName { get; }

    public DropDown(Type targetType, string methodName)
    {
        TargetType = targetType;
        MethodName = methodName;
    }
}