using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JSONReader : MonoBehaviour
{
    [SerializeField] private NoiseData noiseData; 
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private TMPro.TMP_Dropdown dropdown;
    private SerializableDictionary<string, Seed> serializableDictionary;

    void Start()
    {
        PopulateDropdown();
    }
    void PopulateDropdown()
    {
        string filePath = Application.dataPath + "/myObject.json"; 
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            serializableDictionary = JsonUtility.FromJson<SerializableDictionary<string, Seed>>(jsonData);

            List<string> worldNames = new List<string>();
            foreach (var kvp in serializableDictionary)
            {
                worldNames.Add(kvp.Key);
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(worldNames);
        }
    }
    public void OnDropdownValueChanged(int index)
    {
        string selectedWorld = dropdown.options[index].text;
        Seed seed = serializableDictionary[selectedWorld];

        noiseData.noiseScale = seed.noiseScale;
        noiseData.octaves = seed.octaves;
        noiseData.persistence = seed.persistence;
        noiseData.lacunarity = seed.lacunarity;
        noiseData.seed = seed.worldseed;
        terrainData.meshHeightMultiplier = seed.meshHeightMult;
        Debug.Log("Selected world: " + selectedWorld);
    }
    public void RunGame()
    {
        SceneManager.LoadScene(2);
    }
}
