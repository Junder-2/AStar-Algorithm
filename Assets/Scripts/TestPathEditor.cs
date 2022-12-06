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
        
        _testPath.GeneratePath();
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
        if(_testPath.Npc == null || _testPath.Goal == null) return;

        Vector3 npcPos = _testPath.Npc.position, goalPos = _testPath.Goal.position;

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
                _testPath.SetCollisionTile(mousePosition);
                _refreshTilemap = true;
            }
            if (mouseDown && guiEvent.button == 1)
            {
                Event.current.Use();
                _testPath.RemoveCollisionTile(mousePosition);
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

        _testPath.Npc.position = newNpcPos;
        _testPath.Goal.position = newGoalPos;
        
        Handles.SnapToGrid(new[] {_testPath.Npc, _testPath.Goal });

        _testPath.GeneratePath();
    }
}
