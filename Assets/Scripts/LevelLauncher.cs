using UnityEngine;

[RequireComponent(typeof(LevelChooser))]
public class LevelLauncher : MonoBehaviour
{
    private LevelChooser _levelChooser;

    private void Start()
    {
        _levelChooser = GetComponent<LevelChooser>();
    }

    public void LaunchLevel()
    {
        SceneLoader.LoadScene(_levelChooser.CurrentLevel.SceneName);
    }
}