using Baseplate.HealthSystem;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision hit)
    {
        IDamageable target = hit.collider.GetComponent<IDamageable>();
        if (target != null)
        {
            target.Damage(1);
        }
    }
}
