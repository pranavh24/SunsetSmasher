using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform player;
    public Terrain terrain;
    public float spawnMargin = 100f;
    private void Start()
    {
        TerrainData td = terrain.terrainData;
        float normalizedMargin = spawnMargin / td.size.x;
        float normX = Random.Range(normalizedMargin, 1 - normalizedMargin);
        float normZ = Random.Range(normalizedMargin, 1 - normalizedMargin);
        float normY = td.GetHeight((int)(normX * td.heightmapResolution), (int)(normZ * td.heightmapResolution));
        Vector3 newPosition = new Vector3(
            td.size.x * normX + terrain.transform.position.x,
            normY + 2f + terrain.transform.position.y,
            td.size.z * normZ + terrain.transform.position.z
        );
        print("Setting player to random position: " + newPosition);
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller == null)
        {
            player.transform.position = newPosition;

        }
        else
        {
            controller.enabled = false;
            player.transform.position = newPosition;
            controller.enabled = true;
        }
    }
}
