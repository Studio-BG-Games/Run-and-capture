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
    [SerializeField] private ChosenWeapon chosenWeapon;
    private Action<Weapon> changeStats;
    private List<Button> _buttons;

    private void Awake()
    {
        var dataPah = Application.persistentDataPath + "/" + dataFilePath;
        if (!File.Exists(dataPah))
        {
            FileStream stream = new FileStream(dataPah, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(JsonUtility.ToJson(data.WeaponsList[0]));
        }

        changeStats = chosenWeapon.ChangeChosenWeapon;
        _buttons = new List<Button>();
        data.WeaponsList.ForEach(x =>
        {
            var go = Instantiate(weaponIcon, grid);
            var icon = Instantiate(x.icon, go.Icon.transform);
            icon.transform.localPosition = Vector3.zero;
            go.DamageText.text = x.damage.ToString();
            go.ReloadText.text = x.reloadTime.ToString();
            go.ShotsCount.text = x.shots.ToString();
            go.WeaponTitle.text = x.name;
            go.Button.onClick.AddListener(() => ChoseWeapon(x));
            _buttons.Add(go.Button);
        });
    }

    private void ChoseWeapon(Weapon weapon)
    {
        FileStream stream = new FileStream(Application.persistentDataPath + "/" + dataFilePath, FileMode.Create);
        using StreamWriter writer = new StreamWriter(stream);
        writer.Write(JsonUtility.ToJson(weapon));
        changeStats?.Invoke(weapon);
    }
}