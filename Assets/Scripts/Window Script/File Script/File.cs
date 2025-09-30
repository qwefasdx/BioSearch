using UnityEngine;
public class File
{
    public string name;
    public string extension;
    public Folder parent;

    // 颇老 能刨明
    public string textContent;   // txt老 版快
    public Sprite imageContent;  // png老 版快

    public File(string name, string extension, Folder parent, string textContent = null, Sprite imageContent = null)
    {
        this.name = name;
        this.extension = extension;
        this.parent = parent;
        this.textContent = textContent;
        this.imageContent = imageContent;
    }

}
