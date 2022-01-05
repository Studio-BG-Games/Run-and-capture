using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text shotsCount;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private TMP_Text weaponTitle;
    [SerializeField] private GameObject icon;
    
    public TMP_Text DamageText => damageText;

    public TMP_Text ReloadText => reloadText;

    public Button Button => button;
    public TMP_Text ShotsCount => shotsCount;
    public TMP_Text WeaponTitle => weaponTitle;
    public GameObject Icon => icon;
}
