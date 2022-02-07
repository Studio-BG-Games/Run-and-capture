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
        public int index;
        public event Action<HexCell> OnHexPainted;


        [SerializeField] private HexCell[] neighbors;
        [SerializeField] private Item _item;
        private UnitColor _color;
        private MeshRenderer _renderer;

        public UnitColor Color => _color;

        public Item Item
        {
            get => _item;
            set => _item = value;
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

        public SerializibleHexCell ToSerializibleHexCell()
        {
            SerializibleHexCell cell = new SerializibleHexCell();
            cell.HexCoordinates = coordinates;
            cell.index = index;
            return cell;
        }

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _color = UnitColor.Grey;
            if (HexManager.CellByColor == null) return;
            if (!HexManager.CellByColor.ContainsKey(_color))
            {
                HexManager.CellByColor.Add(_color, new List<HexCell>() { this });
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
            neighbors ??= new HexCell[6];
            
            neighbors[(int)direction] = cell;
            
            if (cell == null) return;
            cell.neighbors ??= new HexCell[6];
            cell.neighbors[(int)direction.Back()] = this;
            cell.neighbors ??= new HexCell[6];
        }

        public void PaintHex(UnitColor color, bool isSetting = false)
        {
            if (color == _color) return;

            if (!HexManager.CellByColor.ContainsKey(color))
            {
                HexManager.CellByColor.Add(color, new List<HexCell>() { this });
            }

            _renderer.material.mainTexture = HexGrid.Colors[color].Texture;

            HexManager.CellByColor[_color].Remove(this);

            _color = color;
            HexManager.CellByColor[_color].Add(this);
            if (!isSetting)
                OnHexPainted?.Invoke(this);

            if (_building != null)
            {
                Destroy(_building);
            }

            HexManager.UnitCurrentCell
                .Where(cells
                    => HexManager.CellByColor[cells.Key].Count < 3
                       || (cells.Value.cell == this && cells.Value.unit.Color != Color))
                .Select(cells => cells.Value.unit)
                .ToList().ForEach(x => x.Death());

            if (color == UnitColor.Grey)
            {
                if (_item != null)
                {
                    _item.Despawn();
                }

                return;
            }

            var vfx = VFXController.Instance.PlayEffect(HexGrid.Colors[color].VFXCellCapturePrefab,
                transform.position + new Vector3(0, 0.1f, 0));
            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayRandomClip(MusicController.Instance.MusicData.SfxMusic.Captures, vfx);
        }
    }
}