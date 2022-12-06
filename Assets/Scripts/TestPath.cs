using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestPath : MonoBehaviour
{
    public Transform Npc, Goal;

    [SerializeField] private Tilemap _collisionTileMap;
    [SerializeField] private Tile _collisionTile;
    
    private Vector2Int[] _currentPath;

    public void GeneratePath()
    {
        _currentPath = FindObjectOfType<AStarAlgorithm>().GetPath(Vector2Int.RoundToInt(Npc.position),
            Vector2Int.RoundToInt(Goal.position));
    }
    
    [ContextMenu("FollowPath")]
    public void MovePath() => StartCoroutine(FollowPath());

    IEnumerator FollowPath()
    {
        foreach (var node in _currentPath)
        {
            Npc.position = (Vector2)node;
            yield return new WaitForSeconds(.1f);
        }
    }

    public void SetCollisionTile(Vector3Int pos) => _collisionTileMap.SetTile(pos, _collisionTile);

    public void RemoveCollisionTile(Vector3Int pos) => _collisionTileMap.SetTile(pos, null);
    
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
