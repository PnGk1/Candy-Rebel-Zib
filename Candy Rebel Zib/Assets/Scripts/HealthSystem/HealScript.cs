using Baseplate.HealthSystem;
using UnityEngine;

public class HealScript : MonoBehaviour
{
    [SerializeField] float timeBetweenHeal = 1;
    private float timeSinceLastHeal;
    private bool CanHeal = true;
    private void OnCollisionStay(Collision hit)
    {
        IDamageable target = hit.collider.GetComponent<IDamageable>();
        if (target != null && CanHeal == true)
        {
            target.Heal(1);
            CanHeal = false;
        }
    }
    private void Update()
    {
        if (CanHeal == false)
        {
            timeSinceLastHeal += Time.deltaTime;
            if (timeSinceLastHeal >= timeBetweenHeal)
            {
                CanHeal = true;
                timeSinceLastHeal = 0f;
            }
        }
    }
}
