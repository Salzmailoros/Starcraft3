using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected UnitStats stats;
    public float currentHealth;
    public float currentDamage;
    public Material materialForHpBar;
    protected GridManager gridManager;
    private Vector2Int currentGridPos;

    public virtual void Initialize(UnitStats stats)
    {
        this.stats = stats;
        GetComponent<SpriteRenderer>().sprite = stats.unitSprite;
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        transform.localScale = new Vector3(stats.size.x, stats.size.y, 1);
        currentHealth = stats.health;
        currentDamage = stats.damage;
        gridManager = GridManager.Instance; // Cache reference to GridManager

        materialForHpBar = GetComponent<SpriteRenderer>().material;
        currentGridPos = gridManager.WorldPositionToGrid(transform.position);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        materialForHpBar.SetFloat("_HP", currentHealth / stats.health);

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

    public void MoveTo(Vector2Int newPosition)
    {
        currentGridPos = gridManager.WorldPositionToGrid(transform.position);
        Debug.Log("MYcurrent position :" + currentGridPos);
        StartCoroutine(MoveAlongPath(newPosition));
    }

    private IEnumerator MoveAlongPath(Vector2Int targetGridPos)
    {
        List<Vector2Int> path = AStarPathfinder.Instance.FindPath(currentGridPos, targetGridPos);
        Debug.Log("BOOP");
        if (path == null || path.Count == 0)
        {
            Debug.Log("No valid path found.");
            yield break;
        }

        while (path.Count > 0)
        {
            Vector2Int nextStep = path[0];
            path.RemoveAt(0);

            // Move towards the next tile
            Vector3 targetPos = gridManager.GridToWorldPosition(nextStep);
            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, stats.movementSpeed * Time.deltaTime);
                yield return null;
            }

            // Update tile occupation
            gridManager.SetTile(currentGridPos, null); // Free old tile
            gridManager.SetTile(nextStep, gameObject); // Occupy new tile
            currentGridPos = nextStep; // Update current grid position
        }
    }

    public UnitStats ReturnInfoPanelInfo()
    {
        return stats;
    }
}
