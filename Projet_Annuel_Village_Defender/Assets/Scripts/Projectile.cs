using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject target;
    private int damage;

    public void Initialize(GameObject targetObject, int damageAmount)
    {
        target = targetObject;
        damage = damageAmount;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject == target)
        {
            // Appliquer les dégâts ici si nécessaire
            Destroy(gameObject);
        }
    }
}