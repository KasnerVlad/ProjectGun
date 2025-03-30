using UnityEngine;

public class LockInTarger : MonoBehaviour
{
    [SerializeField] private GameObject targer;
    [SerializeField] private Vector3 _ofsset;
    [SerializeField] private bool _locked, _localPos;
    void Update()
    {
        if (transform.position!=targer.transform.position+_ofsset&&_locked&&!_localPos)
        {
            transform.position = targer.transform.position+_ofsset;
        }
        else if(transform.localPosition!=targer.transform.localPosition+_ofsset&&_locked&&_localPos)
        {
            transform.localPosition = targer.transform.localPosition+_ofsset;
        }
    }
}
