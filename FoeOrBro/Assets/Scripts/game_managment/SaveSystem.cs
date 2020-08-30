using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem{


    public static void SaveMap(int _width, int _height, Tile[,] _tile)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/map.sig";
        FileStream stream = new FileStream(path, FileMode.Create);
        Map data = new Map(_width, _height, _tile);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static Map LoadMap()
    {
        string path = Application.persistentDataPath + "/map.sig";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Map data = formatter.Deserialize(stream) as Map;
            stream.Close();
            return data;
        }
        else{
            Debug.LogError("Can't find map" + path);
            return null;
        }
    }

}