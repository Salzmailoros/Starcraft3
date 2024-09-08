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
    private Coroutine moveCoroutine;  // Keep track of the movement coroutine
    private float lastAttackTime = 0f;
    private bool hasNewCommand = false;

    protected GridManager gridManager;
    private Vector2Int currentGridPos;
    private Material materialForHPBar;
    private IDamageable myDamageableComponent;

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
        myDamageableComponent = GetComponent<IDamageable>();
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
        //Debug.Log("Returned to " + stats.name + " pool");
        PoolManager.Instance.ReturnObjectToPool(stats.name, gameObject);
        gridManager.SetTile(currentGridPos, null);
        currentHealth = stats.health;
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            MonoBehaviour target = currentTarget as MonoBehaviour;
            if (target == null)
            {
                //Debug.LogWarning("Target is null, resetting.");
                ResetTarget();
                return;
            }

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= stats.range)
            {
                isMoving = false;
                if (myDamageableComponent != null && currentTarget.TeamID != myDamageableComponent.TeamID)
                {
                    if (Time.time >= lastAttackTime + (1 / stats.attackSpeed))
                    {
                        lastAttackTime = Time.time;
                        currentTarget.TakeDamage(currentDamage);
                        //Debug.Log("Attacking target.");
                    }
                }
                else
                {
                    Debug.LogWarning("Target is friendly. Cannot attack.");
                }
            }
            else if (!isMoving && !hasNewCommand)
            {
                StartMovingTowardsTarget(gridManager.WorldPositionToGrid(target.transform.position));
            }
        }
    }

    public void Attack(IDamageable damageableTarget)
    {
        hasNewCommand = true;

        // Stop current movement coroutine if it's still running
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            isMoving = false;
            moveCoroutine = null;
        }

        if (currentTarget != damageableTarget) ResetTarget();

        if (damageableTarget == myDamageableComponent)
        {
            Debug.LogWarning("Cannot attack self");
            ResetTarget();
            return;
        }

        currentTarget = damageableTarget;
        MonoBehaviour target = currentTarget as MonoBehaviour;

        if (target != null)
        {
            Vector2Int targetGridPos = gridManager.WorldPositionToGrid(target.transform.position);
            if (myDamageableComponent != null && currentTarget.TeamID == myDamageableComponent.TeamID)
            {
                Debug.LogWarning("Friendly target. Resetting.");
                ResetTarget();
            }
            else
            {
                //Debug.Log("Target is an enemy. Moving to engage.");
                StartMovingTowardsTarget(targetGridPos);
            }
        }

        hasNewCommand = false;
    }

    // New method to handle movement towards target
    private void StartMovingTowardsTarget(Vector2Int goalPosition)
    {
        // Stop any existing movement before starting a new one
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // Start new movement coroutine
        moveCoroutine = StartCoroutine(MoveAlongPath(goalPosition));
    }

    private IEnumerator MoveAlongPath(Vector2Int goalPosition)
    {
        if (currentGridPos == goalPosition)
        {
            Debug.Log("Already at the target position, no movement needed.");
            yield break;
        }

        gridManager.SetTile(currentGridPos, null);
        currentGridPos = gridManager.WorldPositionToGrid(transform.position);
        List<Vector2Int> path = AStarPathfinder.Instance.FindPath(currentGridPos, goalPosition);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No valid path to target, moving to closest available position.");
            Tile closestTile = gridManager.ReturnClosestEmptyTile(goalPosition);
            if (closestTile != null)
            {
                goalPosition = closestTile.GridPosition;
                path = AStarPathfinder.Instance.FindPath(currentGridPos, goalPosition);
            }

            if (path == null || path.Count == 0)
            {
                Debug.LogError("No valid path to target or nearest available position.");
                gridManager.SetTile(currentGridPos, gameObject);
                ResetTarget();
                yield break;
            }
        }

        isMoving = true;
        MonoBehaviour target = currentTarget as MonoBehaviour;

        foreach (Vector2Int nextStep in path)
        {
            Tile nextTile = gridManager.GetTile(nextStep);
            if (nextTile.IsOccupied)
            {
                Debug.LogWarning($"Next tile at {nextStep} is occupied, recalculating path.");
                Tile closestTile = gridManager.ReturnClosestEmptyTile(goalPosition);
                if (closestTile != null)
                {
                    goalPosition = closestTile.GridPosition;
                    path = AStarPathfinder.Instance.FindPath(currentGridPos, goalPosition);
                }

                if (path == null || path.Count == 0)
                {
                    Debug.LogError("No valid path found after recalculating.");
                    gridManager.SetTile(currentGridPos, gameObject);
                    ResetTarget();
                    yield break;
                }

                continue;
            }

            Vector3 targetPos = gridManager.GridToWorldPosition(nextStep);

            // Move towards the next tile
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                if (hasNewCommand)
                {
                    Debug.LogWarning("Movement interrupted by new command.");
                    gridManager.SetTile(currentGridPos, gameObject);
                    isMoving = false;
                    yield break;
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPos, stats.movementSpeed * Time.deltaTime);
                yield return null;
            }

            // Occupy the new tile after completing the step
            if (gridManager.GetTile(currentGridPos).OccupyingObject == gameObject)
            {
                gridManager.SetTile(currentGridPos, null);
            }
            gridManager.SetTile(nextStep, gameObject);
            currentGridPos = nextStep;

            // Now check if the target is within range after completing the step
            if (target != null && Vector3.Distance(transform.position, target.transform.position) <= stats.range)
            {
                Debug.Log("Reached target tile, ready to attack.");
                isMoving = false;
                yield break;
            }
        }

        isMoving = false;

        if (currentTarget != null && Vector3.Distance(transform.position, (currentTarget as MonoBehaviour).transform.position) > stats.range)
        {
            Debug.Log("Target too far to walk to or attack.");
            ResetTarget();
        }
    }


    private void ResetTarget()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        Debug.Log("Resetting target.");
        gridManager.SetTile(currentGridPos, gameObject);
        currentTarget = null;
        isMoving = false;
        hasNewCommand = false;
    }

    public void MoveTo(Vector2Int newPosition)
    {
        if (currentGridPos == newPosition) return;
        hasNewCommand = true;
        ResetTarget();
        StartMovingTowardsTarget(newPosition);
        hasNewCommand = false;
    }

    public UnitStats ReturnInfoPanelInfo()
    {
        return stats;
    }
}
