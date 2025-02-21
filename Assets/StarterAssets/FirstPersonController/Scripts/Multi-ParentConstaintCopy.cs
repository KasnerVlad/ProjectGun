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
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(target.transform.position - soursObject.transform.position),
            smoothSpeed * Time.deltaTime
        );
        // Плавное перемещение (если нужно)
        transform.position = Vector3.Lerp(
            soursObject.transform.position,
            target.transform.position,
            smoothSpeed * Time.deltaTime
        );
    }
}
