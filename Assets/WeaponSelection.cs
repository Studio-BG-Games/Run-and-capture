using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace.Weapons;
using MainMenu;
using UnityEngine;
using Weapons;

public class WeaponSelection : MonoBehaviour
{
   [SerializeField] private WeaponsData data;
   [SerializeField] private WeaponIcon weaponIcon;
   [SerializeField] private Transform grid;
   [SerializeField] private string dataFilePath;
   
   private void Awake()
   {
      dataFilePath = Application.dataPath + dataFilePath;
      data.WeaponsList.ForEach(x =>
      {
         var go = Instantiate(weaponIcon, grid);
         go.Button.image.sprite = x.icon;
         go.DamageText.text = x.damage.ToString();
         go.ReloadText.text = x.reloadTime.ToString();
         go.Button.onClick.AddListener(() => ChoseWeapon(x));
      });
   }

   private void ChoseWeapon(Weapon weapon)
   {
      File.WriteAllText(dataFilePath, JsonUtility.ToJson(weapon));
   }
}
