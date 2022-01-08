using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BarCanvas : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private ShotUIView shotPrefab;
    [SerializeField] private GameObject grid;
    [SerializeField] private Image captureBar;

    public Image HealthBar => healthBar;
    public Image ManaBar => manaBar;
    public ShotUIView ShotUIView => shotPrefab;

    public Image CaptureBar => captureBar;

    public Stack<ShotUIView> SpawnShotUI(int count)
    {
        Stack<ShotUIView> stack = new Stack<ShotUIView>();
        List<GameObject> shots = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            shots.Add(Instantiate(shotPrefab.gameObject, grid.transform));
        }

        shots.Reverse();
        shots.ForEach(shot => stack.Push(shot.GetComponent<ShotUIView>()));
        return stack;
    }
}