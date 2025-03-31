using UnityEngine;
using System.Collections.Generic;
using StarterAssets.FirstPersonController.Scripts;
namespace Rb.Move
{
    public class SurfaceCheck : ISurfaceCheck
    {
        
        private readonly List<Vector3> _contactNormals = new List<Vector3>();
        private readonly FPSControllerBase _fpsController;
        public bool HasGroundContacts { get { return _contactNormals.Count > 0; } private set { } }
        public SurfaceCheck(FPSControllerBase fpsController) { _fpsController = fpsController; }

        public void ClearContactNormals()=> _contactNormals.Clear();
        
        public Vector3 CalculateAverageNormal()
        {
            if (_contactNormals.Count == 0) return Vector3.up;
            
            Vector3 sum = Vector3.zero;
            foreach (Vector3 normal in _contactNormals)
            {
                sum += normal;
            }
            return (sum / _contactNormals.Count).normalized;
        }
        public void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Angle(contact.normal, Vector3.up) <= _fpsController.MaxSlopeAngle)
                {
                    _contactNormals.Add(contact.normal);
                }
            }
        } 
    }

    public interface ISurfaceCheck
    {
        public bool HasGroundContacts { get; }
        void OnCollisionStay(Collision collision);
        Vector3 CalculateAverageNormal();
        void ClearContactNormals();
    }
}