using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder
{
    List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal);
}
