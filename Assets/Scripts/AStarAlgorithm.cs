using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Priority_Queue;
using UnityEngine.Tilemaps;

public class AStarAlgorithm : MonoBehaviour
{
    private const int NormalCost = 10;
    private const int DiagonalCost = 14;
    
    [SerializeField] private Tilemap debugTilemap;
    [SerializeField] private Tile correctTile, searchTile;
    
    private Dictionary<Vector2Int, Vector2Int> _passedNode = new Dictionary<Vector2Int, Vector2Int>();
    private Dictionary<Vector2Int, int> _costSoFar = new Dictionary<Vector2Int, int>();
    private SimplePriorityQueue<Vector2Int> _nodeQueue = new SimplePriorityQueue<Vector2Int>();
    private HashSet<Vector2Int> _closestPathDebug = new HashSet<Vector2Int>();

    private const int BoundrySize = 100;

    public Vector2Int[] GetPath(Vector2Int start, Vector2Int goal)
    {
        if(Physics2D.OverlapPoint(start) != null) return null;
        if(Physics2D.OverlapPoint(goal) != null) return null;
        
        _passedNode.Clear();
        _costSoFar.Clear();
        _nodeQueue.Clear();
        _passedNode.Add(start, start);
        _costSoFar.Add(start, 0);
        _nodeQueue.Enqueue(start, 0);

        Vector2Int currentNode;

        bool foundPath = false;

        while (_nodeQueue.Count > 0)
        {
            currentNode = _nodeQueue.Dequeue();

            if (currentNode == goal)
            {
                foundPath = true;
                break;
            }

            foreach (var next in GetNeighbors(currentNode))
            {
                var nextPos = (Vector2Int)next;
                
                if (Mathf.Abs(nextPos.x - start.x) > BoundrySize ||
                    Mathf.Abs(nextPos.y - start.y) > BoundrySize) continue;

                    var newCost = _costSoFar[currentNode] + next.z;
                
                if (_costSoFar.ContainsKey(nextPos) && newCost >= _costSoFar[nextPos]) continue;
                
                _costSoFar[nextPos] = newCost;
                var priority = newCost + DistanceCost(nextPos, goal);
                _nodeQueue.Enqueue(nextPos, priority);
                _passedNode[nextPos] = currentNode;
            }
        }

        if (!foundPath) return null;

        var closestPath = new List<Vector2Int>();

        currentNode = goal;

        while (currentNode != start)
        {
            closestPath.Add(currentNode);
            currentNode = _passedNode[currentNode];
        }
        
        closestPath.Add(start);
        
        closestPath.Reverse();
        
        if (debugTilemap == null) return closestPath.ToArray();
        
        _closestPathDebug = closestPath.ToHashSet();
        debugTilemap.ClearAllTiles();

        foreach (var node in _passedNode)
        {
            var tile = _closestPathDebug.Contains(node.Key) ? correctTile : searchTile;

            debugTilemap.SetTile((Vector3Int)node.Key, tile);
        }

        return closestPath.ToArray();
    }

    private Vector3Int[] GetNeighbors(Vector2Int position)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        for (int i = 0; i < 8; i++)
        {
            Vector3Int direction = i switch
            {
                0 => new Vector3Int(0, 1, NormalCost),
                1 => new Vector3Int(1, 0, NormalCost),
                2 => new Vector3Int(-1, 0, NormalCost),
                3 => new Vector3Int(0, -1, NormalCost),
                4 => new Vector3Int(1, -1, DiagonalCost),
                5 => new Vector3Int(-1, 1, DiagonalCost),
                6 => new Vector3Int(1, 1, DiagonalCost),
                7 => new Vector3Int(-1, -1, DiagonalCost),
                _ => Vector3Int.zero
            };

            Vector3Int potentialNeighbor = (Vector3Int)position + direction;

            if(Physics2D.OverlapPoint((Vector2Int)potentialNeighbor) != null) continue; 
            if(Physics2D.Raycast(position, (new Vector2(direction.x, direction.y)).normalized, 1).collider != null) continue;
            
            neighbors.Add(potentialNeighbor);
        }

        return neighbors.ToArray();
    }

    private int DistanceCost(Vector2Int a, Vector2Int b)
    {
        var dstX = Mathf.Abs(a.x - b.x);
        var dstY = Mathf.Abs(a.y - b.y);

        return dstX > dstY ? 
            DiagonalCost * dstY + NormalCost * (dstX - dstY) :
            DiagonalCost * dstX + NormalCost * (dstY - dstX);
    }
}
