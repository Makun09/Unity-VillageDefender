using UnityEngine;

public class BasicFire : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float projectileSpeed = 20f;

    [Header("Tir")]
    [SerializeField] float fireRate = 1f;
    private float lastFireTime;

    [Header("Dégâts")]
    [SerializeField] int projectileDamage = 10;

    [Header("Cible")]
    [SerializeField] GameObject targetEnemy;

    private TowerRotation towerRotation;

    private void Start()
    {
        towerRotation = GetComponent<TowerRotation>();
    }

    private void Update()
    {
        if (targetEnemy != null && targetEnemy.activeSelf && Time.time >= lastFireTime + fireRate)
        {
            Fire();
            lastFireTime = Time.time;
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null || targetEnemy == null) return;

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (targetEnemy.transform.position - spawnPosition).normalized;
            rb.linearVelocity = direction * projectileSpeed;
        }

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.Initialize(targetEnemy, projectileDamage);
        }
    }
}