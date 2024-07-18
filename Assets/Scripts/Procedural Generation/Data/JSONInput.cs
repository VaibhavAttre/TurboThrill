using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using System;
using TMPro;
using UnityEngine.SceneManagement;

[Serializable]
public class Seed
{
    public string name;
    public float noiseScale;
    public int octaves;
    public float persistence;
    public float lacunarity;
    public int worldseed;
    public float meshHeightMult;
}

[Serializable]
public class WorldData
{
    public Dictionary<string, Seed> Seeds = new Dictionary<string, Seed>();
}

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}


public class JSONInput : MonoBehaviour
{
    private int seedInput;
    [SerializeField] private TMP_Text reactionTextBox;
    [SerializeField] private NoiseData noiseData;
    [SerializeField] private TerrainData terrainData;

    private WorldData world;
    private void Start()
    {
        world = new WorldData();
    }
    public void ReadInput(string s)
    {
        seedInput = int.Parse(s);
        LoadData();
    }

    public void LoadData()
    {

        string filePath = Application.dataPath + "/myObject.json";
        WorldData world = LoadWorldData(filePath);
        

        Seed seed = new Seed();
        seed.worldseed = seedInput;
        seed.noiseScale = 27.26f;
        seed.octaves = 11;
        seed.persistence = 0.463f;
        seed.meshHeightMult = 24.43f;
        seed.lacunarity = -0.6f;

        int worldNum = world.Seeds.Count + 1;
        string name = "World" + worldNum;
        world.Seeds.Add(name, seed);

        List<KeyValuePair<string, Seed>> seedList = new List<KeyValuePair<string, Seed>>(world.Seeds);

        SerializableDictionary<string, Seed> serializableDictionary = new SerializableDictionary<string, Seed>();
        foreach (var kvp in seedList)
        {
            serializableDictionary.Add(kvp.Key, kvp.Value);
        }
        string json = JsonUtility.ToJson(serializableDictionary);

        Debug.Log("Wrote to file");
        File.WriteAllText(filePath, json);

        noiseData.noiseScale = seed.noiseScale;
        noiseData.octaves = seed.octaves;
        noiseData.persistence = seed.persistence;
        noiseData.lacunarity = seed.lacunarity;
        noiseData.seed = seed.worldseed;
        terrainData.meshHeightMultiplier = seed.meshHeightMult;
    }

    public void RunGame()
    {
        SceneManager.LoadScene(2);
    }

    private WorldData LoadWorldData(string filePath)
    {
        WorldData world = new WorldData();

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            SerializableDictionary<string, Seed> dict = new SerializableDictionary<string, Seed>();
            dict = JsonUtility.FromJson<SerializableDictionary<string, Seed>>(jsonData);

            List<string> worldNames = new List<string>();
            foreach (var kvp in dict)
            {
                string name = kvp.Key;
                Seed currSeed = kvp.Value;
                world.Seeds.Add(name, currSeed);
            }
        }

        return world;
    }
}

