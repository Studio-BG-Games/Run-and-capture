using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using HexFiled;
using Units;
using UnityEngine;
using Weapons;

public class TowerView : MonoBehaviour
{
    [Serializable]
    internal struct Crystal
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private UnitColor _unitColor;

        public GameObject GameObject => _gameObject;
        public UnitColor UnitColor => _unitColor;
    }
    private UnitColor _color;
    private GameObject _target;
    [SerializeField] private Weapon weapon;
    [SerializeField] private SkinnedMeshRenderer _meshRenderer;
    [SerializeField] private List<Crystal> crystals;
    


    public void SetUp(UnitColor unitColor)
    {
        _color = unitColor;
        _meshRenderer.material = HexGrid.Colors[unitColor].BuildingMaterial;
        crystals.First(x => x.UnitColor == unitColor).GameObject.SetActive(true);
        var capsule = gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = weapon.disnatce * HexGrid.HexDistance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var unit = collision.gameObject.GetComponent<UnitView>();
        if (unit == null || unit.Color == _color) return;
        
        weapon.SetModifiedDamage(0);
        _target = unit.gameObject;
        StartCoroutine(Shot());
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
            if (_target == null)
            {
                yield return null;
            }
            var direction = DirectionHelper.DirectionTo(transform.position, _target.transform.position);
            var ball = Instantiate(weapon.objectToThrow,
                transform.forward + transform.position + new Vector3(0, 1),
                Quaternion.LookRotation(direction));

            MusicController.Instance.AddAudioSource(ball);
            MusicController.Instance.PlayAudioClip(weapon.shotSound, ball);

            ball.AddComponent<WeaponView>().SetWeapon(weapon);
            ball.transform.DOMove(
                    new Vector3(direction.x,
                        0, direction.z) * weapon.disnatce * HexGrid.HexDistance +
                    transform.position + new Vector3(0, 1, 0),
                    weapon.speed)
                .SetEase(Ease.Linear)
                .OnComplete(() => Destroy(ball));
        }
    }
}