using System;
using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Vector2Int[] _currentPath;

    [SerializeField] private Transform goal;

    [ContextMenu("GeneratePath")]
    public void GeneratePath()
    {
        _currentPath = FindObjectOfType<AStarAlgorithm>().GetPath(Vector2Int.RoundToInt(transform.position),
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
            transform.position = (Vector2)node;
            yield return new WaitForSeconds(.1f);
        }
    }
}
