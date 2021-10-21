using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels/New Level")]
public class LevelModel : ScriptableObject
{
    public string SceneName;
    public Sprite MenuSprite;
}