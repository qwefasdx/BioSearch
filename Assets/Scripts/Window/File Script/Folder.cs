using System.Collections.Generic;

[System.Serializable]
public class Folder
{
    public string name;
    public List<Folder> children = new List<Folder>();
    public Folder parent;
    public bool isAbnormal = false;

    public Folder(string name, Folder parent = null)
    {
        this.name = name;
        this.parent = parent;
    }
}
