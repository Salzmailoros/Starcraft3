using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected UnitStats stats;
    public int currentHealth;
    public int currentDamage;       // for future use incase there are buffs or upgrades we can add proper handling.

    public virtual void Initialize(UnitStats stats)
    {
        this.stats = stats;
        GetComponent<SpriteRenderer>().sprite = stats.unitSprite;
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        transform.localScale = new Vector3(stats.size.x, stats.size.y, 1);
        currentHealth = stats.health;
        currentDamage = stats.damage;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void DealDamage(IDamageable DamageableTarget)
    {
        DamageableTarget.TakeDamage(currentDamage);
    }

    public virtual void Die()
    {
        Debug.Log($"{stats.unitName} died.");
        Destroy(gameObject);
    }


    public void OnLeftClick()
    {
        Debug.Log($"Clicked on {stats.unitName} ");
    }

    public void OnRightClick()
    {
        //
    }

    public void MoveTo(Vector2Int newPosition)
    {
        transform.position = new Vector3(newPosition.x, newPosition.y, 0);
        Debug.Log($"{stats.unitName} moved to {newPosition}");
    }

    private Vector2Int GetTargetTilePosition()
    {
        // Implement logic to convert mouse position to tile position
        return Vector2Int.zero;  // Placeholder
    }

    public UnitStats ReturnInfoPanelInfo()
    {
        return stats;
    }
}
