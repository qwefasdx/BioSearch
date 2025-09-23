using UnityEngine;

[System.Serializable]
public class File
{
    public string name;
    public string extension; // "txt" ¶Ç´Â "png"
    public Folder parent;

    public File(string name, string extension, Folder parent = null)
    {
        this.name = name;
        this.extension = extension;
        this.parent = parent;
    }
}
