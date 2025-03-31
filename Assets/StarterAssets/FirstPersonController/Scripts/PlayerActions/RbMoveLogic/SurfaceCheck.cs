using UnityEngine;
using System.Collections.Generic;
using StarterAssets.FirstPersonController.Scripts;
namespace Rb.Move
{
    public class SurfaceCheck : ISurfaceCheck
    {
        
        private class TimedContact
        {
            public Vector3 Normal;
            public float TimeAlive;
        }

        private readonly List<TimedContact> _contacts = new List<TimedContact>();
        private readonly float _maxSlopeAngle;
        private const float ContactLifetime = 0.2f;

        public bool HasGroundContacts => _contacts.Count > 0;

        public SurfaceCheck(FPSControllerBase fpsController)
        {
            _maxSlopeAngle = fpsController.MaxSlopeAngle;
        }

        public void UpdateContacts()
        {
            for (int i = _contacts.Count - 1; i >= 0; i--)
            {
                _contacts[i].TimeAlive += Time.deltaTime;
                if (_contacts[i].TimeAlive > ContactLifetime)
                {
                    _contacts.RemoveAt(i);
                }
            }
        }

        public Vector3 CalculateAverageNormal()
        {
            if (_contacts.Count == 0) return Vector3.up;
            
            Vector3 sum = Vector3.zero;
            foreach (var contact in _contacts)
            {
                sum += contact.Normal;
            }
            return (sum / _contacts.Count).normalized;
        }

        public void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (IsValidSlope(contact.normal))
                {
                    AddContact(contact.normal);
                }
            }
        }

        private bool IsValidSlope(Vector3 normal) => 
            Vector3.Angle(normal, Vector3.up) <= _maxSlopeAngle;

        private void AddContact(Vector3 normal)
        {
            foreach (var existing in _contacts)
            {
                if (Vector3.Distance(existing.Normal, normal) < 0.01f)
                {
                    existing.TimeAlive = 0;
                    return;
                }
            }
            _contacts.Add(new TimedContact { Normal = normal, TimeAlive = 0 });
        }

        public void ClearContactNormals() => _contacts.Clear();
    }

    public interface ISurfaceCheck
    {
        public bool HasGroundContacts { get; }
        void OnCollisionStay(Collision collision);
        Vector3 CalculateAverageNormal();
        void ClearContactNormals();
        void UpdateContacts();
    }
}