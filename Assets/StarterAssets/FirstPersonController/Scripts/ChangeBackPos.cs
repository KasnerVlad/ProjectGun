using System;
using UnityEngine;

public class ChangeBackPos : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private AnimationCurve interpolationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 startPosition = new Vector3(0, 1.646f, 0);
    [SerializeField] private Vector3 midPosition = new Vector3(0, 1.646f, 0);
    [SerializeField] private Vector3 endPosition = new Vector3(0, 1.337f, 0.268f);
    [SerializeField] private float minAngle =-89;
    [SerializeField] private float maxAngle = 85;
    [SerializeField] private float midAngle = 0;
    private void Update() => UpdateLogic();

    private void UpdateLogic()
    {
        float angleX = playerCamera.transform.localEulerAngles.x;
        angleX = (angleX > 180) ? angleX - 360 : angleX;

        float clampedAngle = Mathf.Clamp(angleX, minAngle, maxAngle);
        float totalRange = maxAngle - minAngle;

        // Инвертируем 't', чтобы maxAngle соответствовал startPosition (наклон назад)
        float t = 1 - (clampedAngle - minAngle) / totalRange;
        float curveT = interpolationCurve.Evaluate(t);

        // Нормализованный midAngle
        float tMid = 1 - (midAngle - minAngle) / totalRange;

        Vector3 targetPos;
        if (curveT <= tMid)
        {
            // Интерполяция от endPosition (наклон вперед) к midPosition (нейтрально)
            float segmentT = curveT / tMid;
            targetPos = Vector3.Lerp(endPosition, midPosition, segmentT);
        }
        else
        {
            // Интерполяция от midPosition (нейтрально) к startPosition (наклон назад)
            float segmentT = (curveT - tMid) / (1 - tMid);
            targetPos = Vector3.Lerp(midPosition, startPosition, segmentT);
        }

        target.transform.localPosition = targetPos;
    }

}