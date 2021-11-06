using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainHeightRandomizer))]
public class TerrainRandomEditor : Editor {
    public override void OnInspectorGUI() {
        TerrainHeightRandomizer thr = (TerrainHeightRandomizer) target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Terrain")) {
            thr.GenerateTerrain();
            // string output = "";
            // for(int i = 0; i < 5; i++) {
            //     output += (thr.treeInstances[i].position.x * 2000) + ", " + (thr.treeInstances[i].position.z * 2000) + "\n";
            // }
            // Debug.Log("Terrain generated. Trees added: \n" + output);
        }
        if (GUILayout.Button("Filter Trees")) {
            thr.FilterTrees();
        }
    }
}