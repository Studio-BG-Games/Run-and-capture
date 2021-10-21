using UnityEngine;

public class BotsCountSetter : MonoBehaviour
{
    [Range(0, 3)]
    [SerializeField] private int _botsCount = 1;

    private int _currentBotsCount;

    private void Start()
    {
        SetBotsCount(_botsCount);
    }

    private void Update()
    {
        if (_botsCount != _currentBotsCount)
        {
            SetBotsCount(_botsCount);
        }
    }

    private void SetBotsCount(int count)
    {
        _currentBotsCount = count;
        GameData.gameMaxPlayers = count + 1;
    }
}