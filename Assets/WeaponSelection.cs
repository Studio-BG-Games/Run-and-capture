using System.Collections.Generic;
using System.IO;
using DefaultNamespace.Weapons;
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
        _buttons = new List<Button>();
        data.WeaponsList.ForEach(x =>
        {
            var go = Instantiate(weaponIcon, grid);
            go.Button.image.sprite = x.icon;
            go.DamageText.text = x.damage.ToString();
            go.ReloadText.text = x.reloadTime.ToString();
            go.Button.onClick.AddListener(() =>
            {
                ChoseWeapon(x);
                go.Button.image.color = Color.cyan;
            });
            _buttons.Add(go.Button);
        });
    }

    private void ChoseWeapon(Weapon weapon)
    {
        _buttons.ForEach(x => x.image.color = Color.white);
        FileStream stream = new FileStream(Application.persistentDataPath + "/" + dataFilePath, FileMode.Create);
        using StreamWriter writer = new StreamWriter(stream);
        writer.Write(JsonUtility.ToJson(weapon));
    }
}