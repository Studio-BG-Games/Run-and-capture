using System;
using System.Collections.Generic;
using System.IO;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

public class WeaponSelection : MonoBehaviour
{
    [SerializeField] private WeaponsData data;
    [SerializeField] private WeaponIcon weaponIcon;
    [SerializeField] private Transform grid;
    [SerializeField] private string dataFilePath;

    private List<Button> _buttons;

    private void Awake()
    {
        var dataPah = Application.persistentDataPath + "/" + dataFilePath;
        if (!File.Exists(dataPah))
        {
            FileStream stream = new FileStream(dataPah, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write("0");
        }

       
        _buttons = new List<Button>();
        
        for (var i = 0; i < data.WeaponsList.Count - 1; i++)
        {
            var go = Instantiate(weaponIcon, grid);
            var icon = Instantiate(data.WeaponsList[i].icon, go.Icon.transform);
            icon.transform.localPosition = Vector3.zero;
            go.DamageText.text = data.WeaponsList[i].damage.ToString();
            go.ReloadText.text = data.WeaponsList[i].reloadTime.ToString();
            go.ShotsCount.text = data.WeaponsList[i].shots.ToString();
            go.WeaponTitle.text = data.WeaponsList[i].name;
            go.Button.onClick.AddListener(() => ChoseWeapon(i));
            _buttons.Add(go.Button);
        }
        
       
    }

    private void ChoseWeapon(int i)
    {
        FileStream stream = new FileStream(Application.persistentDataPath + "/" + dataFilePath, FileMode.Create);
        using StreamWriter writer = new StreamWriter(stream);
        writer.Write($"{i}");
        
    }
}