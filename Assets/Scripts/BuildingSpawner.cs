using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public Terrain terrain;                 // Assign your terrain
    public GameObject[] buildingPrefabs;    // Drag your buildings here
    public int numberOfBuildings = 100;     // How many to spawn
    public float minSpacing = 10f;          // Distance between buildings

    private Vector3[] placedPositions;

    void Start()
    {
        placedPositions = new Vector3[numberOfBuildings];

        int count = 0;
        int attempts = 0;

        while (count < numberOfBuildings && attempts < numberOfBuildings * 10)
        {
            attempts++;

            float x = Random.Range(0, terrain.terrainData.size.x);
            float z = Random.Range(0, terrain.terrainData.size.z);

            float y = terrain.SampleHeight(new Vector3(x, 0, z));

            Vector3 position = new Vector3(x, y, z);

            // Check spacing
            bool tooClose = false;
            for (int i = 0; i < count; i++)
            {
                if (Vector3.Distance(position, placedPositions[i]) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            // Spawn building
            GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            Instantiate(prefab, position, rotation);

            placedPositions[count] = position;
            count++;
        }

        Debug.Log("Placed Buildings: " + count);
    }
}