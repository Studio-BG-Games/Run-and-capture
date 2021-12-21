using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharBar : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _manaBar;

    public Image HealthBar => _healthBar;
    public Image ManaBar => _manaBar;
}