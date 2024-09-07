public interface IDamageable
{
    void TakeDamage(float damage);
    void Die();
    int TeamID { get; set; }
}
