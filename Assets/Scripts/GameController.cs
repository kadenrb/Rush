using UnityEngine;

public class GameController : MonoBehaviour
{
	// Grid prefab list
	public GameObject[] levelPrefabs;
	public int currentLevelIndex = 0;

	private GameObject activeLevelInstance;

	void Start()
	{
		// Spawn the current level
		SpawnLevel(currentLevelIndex);
	}

	public void SpawnLevel(int index)
	{
		// If there is already a Grid in the scene, get rid of it
		if (activeLevelInstance != null)
		{
			Destroy(activeLevelInstance);
		}

		// Make sure the index is valid
		if (index < levelPrefabs.Length)
		{
			// Spawn the new Grid prefab
			activeLevelInstance = Instantiate(levelPrefabs[index], Vector3.zero, Quaternion.identity);
			currentLevelIndex = index;
		}
		else
		{
			Debug.Log("No more levels left!");
		}
	}

	// This is the function the Ship calls when it hits the Green Beam
	public void CompleteLevel()
	{
		// Wait a second so the player can see the "Win", then spawn next level
		Invoke("LoadNext", 1.0f);
	}

	private void LoadNext()
	{
		SpawnLevel(currentLevelIndex + 1);
	}
}