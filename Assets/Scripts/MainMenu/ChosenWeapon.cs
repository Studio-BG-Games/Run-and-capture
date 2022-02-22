using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Data;
using TMPro;
using UnityEngine;
using Weapons;

public class ChosenWeapon : MonoBehaviour
{
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private string chosenWeaponDataPath;
    [SerializeField] private WeaponsData _data;

    private Weapon Weapon =>
       _data.WeaponsList[int.Parse(File.ReadAllText(Application.persistentDataPath + "/" + chosenWeaponDataPath))];

    private void Start()
    {
        attackText.text = Weapon.damage.ToString();
        reloadText.text = Weapon.reloadTime.ToString(CultureInfo.CurrentCulture);
    }

    public void ChangeChosenWeapon(Weapon weapon)
    {
        attackText.text = weapon.damage.ToString();
        reloadText.text = weapon.reloadTime.ToString(CultureInfo.CurrentCulture);
    }
}