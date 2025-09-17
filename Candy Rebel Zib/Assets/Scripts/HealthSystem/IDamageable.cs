namespace Baseplate.HealthSystem
{
    public interface IDamageable
    {
        public void Damage(float Hitpoint);
        public void Heal(float Hitpoints);
    }
}
