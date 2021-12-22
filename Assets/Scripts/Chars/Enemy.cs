using Data;
using DG.Tweening;
using HexFiled;
using UnityEngine;

namespace Chars
{
    public class Enemy : IUnit
    {
        private GameObject _enemyPrefab;
        private HexCoordinates _spawnPos;
        private UnitColor _color;
        private HexCell _cell;
        private HexGrid _grid;
        private GameObject _instance;
        private bool _isAlive;
        private UnitView _unitView;
        private bool _isBusy;
        private Animator _animator;
        private AnimLength _animLength;
        private CharBar _charBar;
        private float _mana;
        private float _hp;

        public UnitView EnemyView => _unitView;
        public bool IsBusy => _isBusy;
        
        public Enemy(EnemyInfo enemyInfo, HexGrid grid)
        {
            _enemyPrefab = enemyInfo.playerPrefab;
            _spawnPos = enemyInfo.spawnPos;
            _color = enemyInfo.color;
            _grid = grid;
            _isAlive = false;
        }
        public void Move(HexDirection direction)
        {
            if (_cell.GetNeighbor(direction))
            {
                _isBusy = true;
                _cell = _cell.GetNeighbor(direction);
                _instance.transform.DOLookAt(_cell.transform.position, 0.1f);
                _animator.SetTrigger("Move");
                _animator.SetBool("isMoving", _isBusy);
                _instance.transform.DOMove(_cell.transform.position, _animLength.Move);
            }
        }
        
        private void SetAnimLength()
        {
            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                switch (clip.name)
                {
                    case "MoveJump":
                        _animLength.Move = clip.length;
                        break;
                    case "Attack":
                        _animLength.Attack = clip.length;
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateCanvas()
        {
            _charBar.ManaBar.fillAmount = _mana / 100;
            _charBar.HealthBar.fillAmount = _hp / 100;
        }
        
        public void Spawn()
        {
            if(!_isAlive)
            {
                _cell = _grid.GetCellFromCoord(_spawnPos);
                _instance = Object.Instantiate(_enemyPrefab, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;
                _cell.PaintHex(_color);
                for (int i = 0; i < 6; i++)
                {
                    _cell.GetNeighbor((HexDirection)i).PaintHex(_color);
                }

                _isAlive = true;
                _unitView = _instance.GetComponent<UnitView>();
                _animator = _instance.GetComponent<Animator>();
                _charBar = _instance.GetComponent<CharBar>();
                SetAnimLength();
            }
        }

        private void SetUpActions()
        {
            _unitView.OnHit += Damage;
        }
        public void Death()
        {
            throw new System.NotImplementedException();
        }

        public void Attack(Vector2 direction)
        {
            throw new System.NotImplementedException();
        }


        public void Damage(float dmg)
        {
            _hp -= dmg;
            UpdateCanvas();
        }
    }
}