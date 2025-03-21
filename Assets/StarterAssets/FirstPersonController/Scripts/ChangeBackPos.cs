using System;
using UnityEngine;

public class ChangeBackPos : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private AnimationCurve interpolationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 startPosition = new Vector3(0, 1.646f, 0);
    [SerializeField] private Vector3 endPosition = new Vector3(0, 1.337f, 0.268f);
    private void Update() => UpdateLogic();

    private void UpdateLogic()
    {
        float angleX = playerCamera.transform.localEulerAngles.x;
        angleX = (angleX > 180) ? angleX - 360 : angleX;
        
        // Ограничиваем угол в диапазоне [0, 85]
        float clampedAngle = Mathf.Clamp(angleX, 0, 85);
        
        // Нормализуем угол в интервал [0, 1]
        float t = clampedAngle / 85f;
        
        // Получаем значение из кривой
        float curveT = interpolationCurve.Evaluate(t);
        
        // Применяем интерполяцию позиции
        target.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curveT);
    }

}