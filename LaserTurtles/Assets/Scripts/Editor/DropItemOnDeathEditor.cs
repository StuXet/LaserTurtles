using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DropItemOnDeath))]
class DropItemOnDeathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var dropItemOnDeath = (DropItemOnDeath)target;
        if (dropItemOnDeath == null) return;

        DrawDefaultInspector();

        CheckIfHundred(dropItemOnDeath);
    }

    private void CheckIfHundred(DropItemOnDeath dropItemOnDeath)
    {
        int total = 0;

        foreach (var item in dropItemOnDeath.ItemsToDrop)
        {
            total += item.dropChance;
        }

        if (total != 100)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.HelpBox("The Sum Of All 'Drop Chances' Should Equal '100'!!!", MessageType.Warning, true);
            GUI.color = Color.white;
        }
    }
}
