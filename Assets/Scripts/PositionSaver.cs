using System.Collections.Generic;
using UnityEngine;

public class PositionSaver : MonoBehaviour
{
    private Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();

    private void OnApplicationQuit()
    {
        // Find all objects with the tag "Player" or "Enemy"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Save their positions to the dictionary
        foreach (var player in players)
        {
            positions[player.name] = player.transform.position;
        }
        foreach (var enemy in enemies)
        {
            positions[enemy.name] = enemy.transform.position;
        }

        // Convert the dictionary to a JSON string and save it to a file
        string json = JsonUtility.ToJson(new Serialization<Vector3>(positions));
        System.IO.File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    private void Start()
    {
        // Load the JSON string from the file and convert it back to a dictionary
        if (System.IO.File.Exists(Application.persistentDataPath + "/savefile.json"))
        {
            string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/savefile.json");
            positions = JsonUtility.FromJson<Serialization<Vector3>>(json).ToDictionary();

            // Set the positions of the objects
            foreach (var kvp in positions)
            {
                GameObject obj = GameObject.Find(kvp.Key);
                if (obj != null)
                {
                    obj.transform.position = kvp.Value;
                }
            }
        }
    }
}

[System.Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<string> keys;
    [SerializeField]
    List<T> values;

    public Serialization(Dictionary<string, T> dict)
    {
        keys = new List<string>(dict.Keys);
        values = new List<T>(dict.Values);
    }

    // Convert the serialization back into a dictionary
    public Dictionary<string, T> ToDictionary()
    {
        var dict = new Dictionary<string, T>();
        for (int i = 0; i != Mathf.Min(keys.Count, values.Count); i++)
            dict.Add(keys[i], values[i]);
        return dict;
    }
}
