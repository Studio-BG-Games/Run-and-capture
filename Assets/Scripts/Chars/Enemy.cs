using Data;
using DG.Tweening;
using HexFiled;
using UnityEngine;

namespace Chars
{
    public class Enemy : IUnit
    {

        private EnemyInfo _data;
        private HexCell _cell;
        private HexGrid _grid;
        private GameObject _instance;
        private bool _isAlive;
        private UnitView _unitView;
        private bool _isBusy;
        private Animator _animator;
        private AnimLength _animLength;
        private BarCanvas _barCanvas;
        private float _mana;
        private float _hp;

        public UnitView EnemyView => _unitView;
        public bool IsBusy => _isBusy;
        
        public Enemy(EnemyInfo enemyInfo, HexGrid grid)
        {
            _data = enemyInfo;
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
            if (_hp > _data.maxHP)
                _hp = _data.maxHP;
            if (_mana > _data.maxMana)
                _mana = _data.maxMana;

            float hp = _hp;
            float mana = _mana;
            float maxHp = _data.maxHP;
            float maxMana = _data.maxMana;
            _barCanvas.ManaBar.DOFillAmount(mana / maxMana, 0.5f).SetEase(Ease.InQuad);
            _barCanvas.HealthBar.DOFillAmount(hp / maxHp, 0.5f).SetEase(Ease.InQuad);
        }
        
        public void Spawn()
        {
            if(!_isAlive)
            {
                _cell = _grid.GetCellFromCoord(_data.spawnPos);
                _instance = Object.Instantiate(_data.playerPrefab, _cell.transform.parent);
                _instance.transform.localPosition = _cell.transform.localPosition;
                _cell.PaintHex(_data.color);
                for (int i = 0; i < 6; i++)
                {
                    _cell.GetNeighbor((HexDirection)i).PaintHex(_data.color);
                }

                _isAlive = true;
                _unitView = _instance.GetComponent<UnitView>();
                _animator = _instance.GetComponent<Animator>();
                _barCanvas = _unitView.BarCanvas.GetComponent<BarCanvas>();
                _hp = _data.maxHP;
                _mana = _data.maxMana;
                SetAnimLength();
                SetUpActions();
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

        public void StartAttack(Vector2 direction)
        {
            throw new System.NotImplementedException();
        }


        public void Damage(int dmg)
        {
            _hp -= dmg;
            UpdateCanvas();
        }
    }
}