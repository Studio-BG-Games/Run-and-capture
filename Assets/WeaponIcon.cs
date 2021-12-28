using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text reloadText;
    
    public TMP_Text DamageText => damageText;

    public TMP_Text ReloadText => reloadText;

    public Button Button => button;
}
