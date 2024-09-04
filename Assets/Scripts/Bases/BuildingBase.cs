using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    protected BuildingStats stats;
    public int currentHealth;

    public virtual void Initialize(BuildingStats stats)
    {
        this.stats = stats;
        GetComponent<SpriteRenderer>().sprite = stats.buildingSprite;
        currentHealth = stats.health;
    }

    public virtual void TakeDamage(int damage)
    {
        stats.health -= damage;
        if (stats.health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{stats.buildingName} has been Destroyed.");
        Destroy(gameObject);
    }

    public BuildingStats ReturnInfoPanelInfo()
    {
        return stats;
    }
}
