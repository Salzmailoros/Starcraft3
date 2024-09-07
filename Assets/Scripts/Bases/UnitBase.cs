using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected UnitStats stats;
    public float currentHealth;
    public float currentDamage;
    private IDamageable currentTarget = null;
    private bool isMoving = false;
    private Coroutine moveCoroutine;
    private float lastAttackTime = 0f;
    public bool isCommandOverride = false; // Prevents unnecessary target reset
    private bool hasNewCommand = false; // Detects new commands

    protected GridManager gridManager;
    private Vector2Int currentGridPos;

    private Material materialForHPBar;

    public virtual void Initialize(UnitStats stats)
    {
        this.stats = stats;
        GetComponent<SpriteRenderer>().sprite = stats.unitSprite;
        transform.localScale = new Vector3(stats.size.x, stats.size.y, 1);
        currentHealth = stats.health;
        currentDamage = stats.damage;
        gridManager = GridManager.Instance;
        materialForHPBar = GetComponent<SpriteRenderer>().material;
        currentGridPos = gridManager.WorldPositionToGrid(transform.position);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        materialForHPBar.SetFloat("_HP", currentHealth / stats.health);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        PoolManager.Instance.ReturnObjectToPool(stats.name, gameObject);
        gridManager.SetTile(currentGridPos, null); // Free the tile upon death
        Debug.Log("Returned me to :" + stats.name + " pool");
        Destroy(gameObject);
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            MonoBehaviour target = currentTarget as MonoBehaviour;
            if (target == null)
            {
                ResetTarget();
                return;
            }

            float distance = Vector3.Distance(transform.position, target.transform.position);

            // If the target is within attack range and movement is finished, attack
            if (distance <= stats.range && !isMoving)
            {
                if (Time.time >= lastAttackTime + (1 / stats.attackSpeed))
                {
                    lastAttackTime = Time.time;
                    currentTarget.TakeDamage(currentDamage);
                    Debug.Log("BOOP");
                }
            }
            else if (!isMoving && !hasNewCommand)  // Move only if there's no new command
            {
                MoveTo(gridManager.WorldPositionToGrid(target.transform.position));
            }
        }
    }

    public void Attack(IDamageable damageableTarget)
    {
        hasNewCommand = true;  // Flag for new command
        if (currentTarget != damageableTarget)
        {
            ResetTarget();  // Reset current actions before switching targets
        }

        currentTarget = damageableTarget;

        MonoBehaviour target = currentTarget as MonoBehaviour;
        if (target != null)
        {
            Vector2Int targetGridPos = gridManager.WorldPositionToGrid(target.transform.position);
            MoveTo(targetGridPos);  // Move to the target's position
        }

        hasNewCommand = false;  // New command is handled
    }

    public void MoveTo(Vector2Int newPosition)
    {
        hasNewCommand = true;  // Mark new command
        ResetTarget();  // Stop any current action and reset target

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveAlongPath(newPosition));

        hasNewCommand = false;  // New command is handled
    }

    private IEnumerator MoveAlongPath(Vector2Int goalPosition)
    {
        // Free the old tile before starting movement
        gridManager.SetTile(currentGridPos, null);
        currentGridPos = gridManager.WorldPositionToGrid(transform.position);

        List<Vector2Int> path = AStarPathfinder.Instance.FindPath(currentGridPos, goalPosition);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No valid path found!");
            ResetTarget();  // If no valid path, reset target
            yield break;
        }

        isMoving = true;
        MonoBehaviour target = currentTarget as MonoBehaviour;

        foreach (Vector2Int nextStep in path)
        {
            Vector3 targetPos = gridManager.GridToWorldPosition(nextStep);

            // Move towards the next tile
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                if (hasNewCommand)  // Interrupt movement if a new command is issued
                {
                    // Ensure the tile is occupied even if movement is interrupted
                    gridManager.SetTile(currentGridPos, gameObject);
                    isMoving = false;
                    yield break;
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPos, stats.movementSpeed * Time.deltaTime);
                yield return null;
            }

            // Occupy the new tile after completing the step
            gridManager.SetTile(currentGridPos, null); // Free old tile
            gridManager.SetTile(nextStep, gameObject); // Occupy new tile
            currentGridPos = nextStep;

            // Check if the target is now within range after the step
            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (distanceToTarget <= stats.range)
                {
                    Debug.Log("Reached target tile, ready to attack.");
                    isMoving = false;
                    yield break;  // Stop moving, start attacking
                }
            }
        }

        isMoving = false;

        // After completing the path, reset the target if still out of range
        if (currentTarget != null && Vector3.Distance(transform.position, (currentTarget as MonoBehaviour).transform.position) > stats.range)
        {
            Debug.Log("Target too far to walk to or attack.");
            ResetTarget();
        }
    }

    private void ResetTarget()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);  // Stop any movement
        gridManager.SetTile(currentGridPos, gameObject);  // Ensure tile occupation is updated
        currentTarget = null;
        isMoving = false;
        isCommandOverride = false;  // Reset command override flag
        hasNewCommand = false;  // Reset new command flag
    }

    public UnitStats ReturnInfoPanelInfo()
    {
        return stats;
    }
}
