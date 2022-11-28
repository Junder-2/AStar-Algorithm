using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestPath : MonoBehaviour
{
    public Transform npc, goal;

    public Tilemap collisionTileMap;
    public Tile collisionTile;
    
    private Vector2Int[] _currentPath;
    public void GeneratePath()
    {
        _currentPath = FindObjectOfType<AStarAlgorithm>().GetPath(Vector2Int.RoundToInt(npc.position),
            Vector2Int.RoundToInt(goal.position));
    }
    
    [ContextMenu("FollowPath")]
    public void MovePath()
    {
        StartCoroutine(FollowPath());
    }
    
    IEnumerator FollowPath()
    {
        foreach (var node in _currentPath)
        {
            npc.position = (Vector2)node;
            yield return new WaitForSeconds(.1f);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if(_currentPath == null)
            return;

        for (int i = 1; i < _currentPath.Length; i++)
        {
            Vector2 lastNode = _currentPath[i - 1];
            Vector2 currentNode = _currentPath[i];
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lastNode, currentNode);
        }
    }
}
