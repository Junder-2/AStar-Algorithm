using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestPath))]
public class TestPathEditor : Editor
{
    private TestPath _testPath;
    
    private void OnEnable()
    {
        _testPath = (TestPath)target;
        Tools.hidden = true;
    }

    private const string HelpText = "-Shift+Mouse0: Place Obstacle\n-Shift+Mouse1: Remove Obstacle\n-Shift+Mouse0+OnGoal: Move to Goal"; 

    private bool _refreshTilemap = false;

    public override void OnInspectorGUI()
    {
        GUILayout.Label(HelpText);
        
        GUILayout.Space(8);
        
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if(_testPath.collisionTileMap == null && _testPath.npc == null || _testPath.goal == null) return;

        Vector3 npcPos = _testPath.npc.position, goalPos = _testPath.goal.position;

        var guiEvent = Event.current;

        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float dstToDrawPlane = (0 - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3Int mousePosition = Vector3Int.RoundToInt((Vector2)mouseRay.GetPoint(dstToDrawPlane));

        if (guiEvent.modifiers == EventModifiers.Shift)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            
            bool mouseDown = guiEvent.type is EventType.MouseDrag or EventType.MouseDown;

            if (mouseDown && guiEvent.button == 0 && mousePosition != Vector3Int.RoundToInt(npcPos) && mousePosition != Vector3Int.RoundToInt(goalPos))
            {
                _testPath.collisionTileMap.SetTile(mousePosition, _testPath.collisionTile);
                _refreshTilemap = true;
            }
            if (mouseDown && guiEvent.button == 1)
            {
                Event.current.Use();
                _testPath.collisionTileMap.SetTile(mousePosition, null);
                _refreshTilemap = true;
            }
            
            Handles.color = Color.red;
            Handles.DrawWireCube(mousePosition, Vector3.one);
        }

        if (guiEvent.type == EventType.MouseUp)
        {
            if (_refreshTilemap)
            {
                _refreshTilemap = false;
                _testPath.GeneratePath();
            }
            
            if(guiEvent.modifiers == EventModifiers.Shift && guiEvent.button == 0 && mousePosition == Vector3Int.RoundToInt(goalPos))
                _testPath.MovePath();
        }

        EditorGUI.BeginChangeCheck();
        
        Vector3 newNpcPos = Handles.PositionHandle(npcPos, Quaternion.identity);
        Vector3 newGoalPos = Handles.PositionHandle(goalPos, Quaternion.identity);
        
        if(!EditorGUI.EndChangeCheck()) return;

        _testPath.npc.position = newNpcPos;
        _testPath.goal.position = newGoalPos;
        
        Handles.SnapToGrid(new[] {_testPath.npc, _testPath.goal });

        _testPath.GeneratePath();
    }
}
