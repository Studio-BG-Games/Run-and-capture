using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Units;
using UnityEngine;
using Weapons;

public class TowerView : MonoBehaviour
{
    private UnitColor _color;
    private GameObject _target;
    [SerializeField] private Weapon weapon;


    public void SetUp(UnitColor unitColor)
    {
        _color = unitColor;
        GetComponent<MeshRenderer>().material.mainTexture = HexGrid.Colors[unitColor].BuildingTexture;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var unit = collision.gameObject.GetComponent<UnitView>();
        if (unit != null && unit.Color != _color) //TODO какие-то проблемы с задием цвета
        {
            weapon.SetModifiedDamage(0);
            _target = unit.gameObject;
            StartCoroutine(Shot());
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (_target == other.gameObject)
        {
            _target = null;
        }
    }

    private IEnumerator Shot()
    {
        while (_target != null)
        {
            yield return new WaitForSecondsRealtime(weapon.reloadTime);

            var direction = DirectionHelper.DirectionTo(transform.position, _target.transform.position);
            var ball = Instantiate(weapon.objectToThrow,
                transform.forward + transform.position + new Vector3(0, 2),
                Quaternion.LookRotation(direction));

            MusicController.Instance.AddAudioSource(ball);
            MusicController.Instance.PlayAudioClip(weapon.shotSound, ball);

            ball.AddComponent<WeaponView>().SetWeapon(weapon);
            ball.transform.DOMove(
                    new Vector3(direction.x,
                        0, direction.z) * weapon.disnatce * HexGrid.HexDistance +
                    transform.position + new Vector3(0, 2, 0),
                    weapon.speed)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(ball));
        }
    }
}