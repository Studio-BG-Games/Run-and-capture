using Cinemachine;
using UnityEngine;

public class CameraSizeController : MonoBehaviour
{
    [SerializeField] private float _maxSize = 7.5f;
    [SerializeField] private float _camExpandSmoothness = 0.01f;
    [SerializeField] private TileOwner _trackingPlayer = TileOwner.Ariost;
    [SerializeField] private CinemachineVirtualCamera _cam;

    private float _startSize;

    private void Awake()
    {
        _startSize = _cam.m_Lens.OrthographicSize;
    }

    private void FixedUpdate()
    {
        UpdateCamSize();
    }

    private void UpdateCamSize()
    {
        int maxTilesNumber = TileManagment.levelTiles.Count;
        int playerTilesNumber = TileManagment.charTiles[(int) _trackingPlayer].Count;

        float camSizeDelta = _maxSize - _startSize;

        float targetSize = _startSize + (float) playerTilesNumber / maxTilesNumber * camSizeDelta;

        _cam.m_Lens.OrthographicSize = Mathf.Lerp(_cam.m_Lens.OrthographicSize, targetSize, _camExpandSmoothness);
    }
}