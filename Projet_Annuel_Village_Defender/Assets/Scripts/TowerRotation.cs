using UnityEngine;

public class TowerRotation : MonoBehaviour
{
    [Header("Cible")]
    [SerializeField] GameObject targetEnemy;
    [SerializeField] float detectionRange = 10f;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 5f;

    private Transform currentTarget;

    private void Update()
    {
        if (targetEnemy != null && targetEnemy.activeSelf)
        {
            currentTarget = targetEnemy.transform;
            RotateTowardsTarget();
        }
    }

    private void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public bool HasTarget()
    {
        return targetEnemy != null && targetEnemy.activeSelf;
    }
}