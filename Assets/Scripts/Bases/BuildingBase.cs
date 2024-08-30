
using System.Runtime;
using UnityEngine;

public abstract class BuildingBase : MonoBehaviour, IClickable, IDamageable, IObjectSpawner
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

    void IDamageable.Die()
    {
        Die();
    }

    void IClickable.OnLeftClick()
    {
        Debug.Log($" Selected Building of type : {stats.buildingName}");
        // update the UI to show this buildings menu/stats/stuff
    }

    void IClickable.OnRightClick()
    {
        Debug.Log($"Targeted Building : {stats.buildingName}");
    }

    void IObjectSpawner.produceUnit()
    {
        throw new System.NotImplementedException();
    }
    public BuildingStats ReturnInfoPanelInfo()
    {
        return stats;
    }
}
