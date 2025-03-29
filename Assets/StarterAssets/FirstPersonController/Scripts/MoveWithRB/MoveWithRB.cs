using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SurfaceSlider))]
public class MoveWithRB : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SurfaceSlider _surfaceSlider;

    public void Move(Vector3 direction)
    {
        Vector3 slopeDirection = _surfaceSlider.Project(direction.normalized);
    
        // Рассчитываем целевую скорость с учетом наклона
        Vector3 targetVelocity = slopeDirection * moveSpeed;
    
        // Сохраняем текущую вертикальную скорость (гравитация/прыжок)
        targetVelocity.y = rb.velocity.y;
    
        // Плавно изменяем скорость
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 10f);
    }
}