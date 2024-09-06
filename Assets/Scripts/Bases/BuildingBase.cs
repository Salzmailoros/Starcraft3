using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    protected BuildingStats buildingStats;
    public float currentHealth;
    public Vector2 gridPos;
    public Material materialForHPBar;

    public virtual void Initialize(BuildingStats stats)
    {
        buildingStats = stats;
        GetComponent<SpriteRenderer>().sprite = stats.buildingSprite;
        currentHealth = stats.health;
        materialForHPBar = GetComponent<SpriteRenderer>().material;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        materialForHPBar.SetFloat("_HP",currentHealth/ buildingStats.health);
        if (buildingStats.health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{buildingStats.buildingName} has been Destroyed.");
        Destroy(this.gameObject);
    }

    public BuildingStats BuildingStats()
    {
        return buildingStats;
    }
}
