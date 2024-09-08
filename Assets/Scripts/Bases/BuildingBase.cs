using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    protected BuildingStats buildingStats;
    public float currentHealth;
    public Vector2Int gridPos;
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
        materialForHPBar.SetFloat("_HP", currentHealth / buildingStats.health);  // Update HP bar based on current health

        if (currentHealth <= 0)  // Check current health, not the max health
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        GridManager.Instance.SetTile(gridPos, null);
        Debug.Log("I died" + this.name);
        Destroy(this.gameObject);
    }

    public BuildingStats BuildingStats()
    {
        return buildingStats;
    }
}
