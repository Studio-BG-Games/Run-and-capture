using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class AH_SerializableAssetInfo
{
    public string ID;

    /// <summary>
    /// In 2.1.5 and older this property is a list of paths
    /// </summary>
    public List<string> Refs;
    /// <summary>
    /// but in 2.1.6 and newer its a list of indexes that points to a guid (Scene assets)
    /// </summary>
    //[UnityEngine.SerializeField] private List<string> sceneIDs;

    public AH_SerializableAssetInfo()
    { }

    public AH_SerializableAssetInfo(string assetPath, List<string> scenes)
    {
        this.ID = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
        this.Refs = scenes;// scenes.Select(x => UnityEditor.AssetDatabase.AssetPathToGUID(x)).ToList();
    }

    /*public List<string> SceneIDs
    {
        get
        {
            if (sceneIDs.Count>0 || Refs==null)
                return sceneIDs;
            else
                return sceneIDs = Refs.Select(x => UnityEditor.AssetDatabase.AssetPathToGUID(x)).ToList();
        }
        set
        {
            sceneIDs = value;
        }
    }*/

    internal void ChangePathToGUID()
    {
        Refs = Refs.Select(x => UnityEditor.AssetDatabase.AssetPathToGUID(x)).ToList();
    }
}