using UnityEngine;
using System.Collections.Generic;
public class SurfaceSlider : MonoBehaviour
{
    private readonly Dictionary<Collider, List<Vector3>> _activeCollisions = new Dictionary<Collider, List<Vector3>>();

    public Vector3 Project(Vector3 direction)
    {
        Vector3 averageNormal = CalculateAverageNormal();
        return direction - Vector3.Dot(direction, averageNormal) * averageNormal;
    }

    private Vector3 CalculateAverageNormal()
    {
        Vector3 sum = Vector3.zero;
        int totalContacts = 0;

        foreach (var entry in _activeCollisions)
        {
            foreach (Vector3 normal in entry.Value)
            {
                sum += normal;
                totalContacts++;
            }
        }

        return totalContacts > 0 
            ? (sum / totalContacts).normalized 
            : Vector3.up;
    }
    private void OnCollisionEnter(Collision collision)
    {
        UpdateCollisionNormals(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_activeCollisions.ContainsKey(collision.collider))
        {
            _activeCollisions.Remove(collision.collider);
        }
    }

    private void UpdateCollisionNormals(Collision collision)
    {
        List<Vector3> normals = new List<Vector3>();
        
        foreach (ContactPoint contact in collision.contacts)
        {
            normals.Add(contact.normal);
        }
        
        _activeCollisions[collision.collider] = normals;
    }

    private void OnDrawGizmos() 
    { 
        Gizmos.color = Color.white; 
        Gizmos.DrawLine(transform.position, transform.position + CalculateAverageNormal() * 3); 
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Project(transform.forward));
    }
}