using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiParentConstaintCopy : MonoBehaviour
{
    [SerializeField] private GameObject soursObject;
    [SerializeField] private GameObject target;
    [SerializeField] private float smoothSpeed = 0.5f;

    private void LateUpdate()
    {
        soursObject.transform.position = target.transform.position;
        soursObject.transform.rotation = target.transform.rotation;
        /*soursObject.transform.rotation = Quaternion.Slerp(
            soursObject.transform.rotation,
            Quaternion.LookRotation(target.transform.position - soursObject.transform.position),
            smoothSpeed * Time.deltaTime
        );
        // Плавное перемещение (если нужно)
        soursObject.transform.position = Vector3.Lerp(
            soursObject.transform.position,
            target.transform.position,
            smoothSpeed * Time.deltaTime
        );*/
    }
}
