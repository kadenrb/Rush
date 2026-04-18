using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ColourPool
{
	public Material[] colours;
}

public sealed class ShipColourizer : MonoBehaviour
{
	public List<ColourPool> slotPalettes;

	void Start()
	{
		Renderer rend = GetComponent<Renderer>();
		if (rend == null) return;

		// Get the current materials from the list
		Material[] currentMats = rend.materials;

		for (int i = 0; i < slotPalettes.Count; i++)
		{
			// Stop if we run out of material slots on the mesh
			if (i >= currentMats.Length) break;

			// If the list is empty don't change the material
			if (slotPalettes[i].colours == null || slotPalettes[i].colours.Length == 0)
				continue;

			// Random material from this slot's specific pool
			int randomIndex = Random.Range(0, slotPalettes[i].colours.Length);
			currentMats[i] = slotPalettes[i].colours[randomIndex];
		}

		rend.materials = currentMats;
	}
}
