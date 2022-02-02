using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Items;
using UnityEngine;

namespace HexFiled
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates coordinates;
        public event Action<HexCell> OnHexPainted;
        
        

        [SerializeField] private HexCell[] neighbors;
        [SerializeField] private Item _item;
        private UnitColor _color;
        private MeshRenderer _renderer;
        
        public UnitColor Color => _color;

        public Item Item
        {
            get => _item;
            set => _item = _item == null ? value : null;
        }
        private GameObject _building;

        public GameObject Building
        {
            get => _building;
            set
            {
                if (_building == null)
                {
                    _building = value;
                }
            }
        }

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _color = UnitColor.Grey;
            if (!HexManager.CellByColor.ContainsKey(_color))
            {
                HexManager.CellByColor.Add(_color, new List<HexCell>(){this});
            }
            else
            {
                HexManager.CellByColor[_color].Add(this);
            }
        }
        

        public List<HexCell> GetListNeighbours()
        {
            return neighbors.ToList();
        }
        

        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Back()] = this;
        }

        public void PaintHex(UnitColor color)
        {
            if (color == _color) return;
            
            if (!HexManager.CellByColor.ContainsKey(color))
            {
                HexManager.CellByColor.Add(color, new List<HexCell>(){this});
            }

            _renderer.material.mainTexture = HexGrid.Colors[color].Texture;
            var previousColor = _color;
            HexManager.CellByColor[previousColor].Remove(this);
            _color = color;
            HexManager.CellByColor[_color].Add(this);
            OnHexPainted?.Invoke(this);
            if(color == UnitColor.Grey) return;
            
            var vfx = VFXController.Instance.PlayEffect(HexGrid.Colors[color].VFXCellCapturePrefab, transform.position + new Vector3(0,0.1f,0));
            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayRandomClip(MusicController.Instance.MusicData.SfxMusic.Captures, vfx);
            
            
        }
    }
}