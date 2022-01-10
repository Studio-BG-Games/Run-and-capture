using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexFiled
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates coordinates;
        public Action<HexCell> onHexPainted;
        
        public float gCost;
        public float hCost;
        public float fCost;
        public HexCell parent;

        [SerializeField] private HexCell[] neighbors;
        private Item _item;
        private UnitColor _color;
        private MeshRenderer _renderer;

        public HexCell[] Neighbors => neighbors;
        public UnitColor Color => _color;

        public Item Item => _item;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _color = UnitColor.GREY;
            if (!HexManager.CellByColor.ContainsKey(_color))
            {
                HexManager.CellByColor.Add(_color, new List<HexCell>(){this});
            }
            else
            {
                HexManager.CellByColor[_color].Add(this);
            }
        }

        public void SetItem(Item item)
        {
            _item = item == _item ? null : item;
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
            cell.neighbors[(int)direction.Opposite()] = this;
        }

        public void PaintHex(UnitColor color)
        {
            if (color == _color) return;
            if (color == UnitColor.GREY)
            {
                _renderer.material.mainTexture = HexGrid.Colors[color].Texture;
                _color = color;
                return;
            }

            _renderer.material.mainTexture = HexGrid.Colors[color].Texture;

            _color = color;
            
            if (!HexManager.CellByColor.ContainsKey(_color))
            {
                HexManager.CellByColor.Add(_color, new List<HexCell>(){this});
            }
            else
            {
                HexManager.CellByColor[_color].Add(this);
            }
            
            
            
            var vfx = VFXController.Instance.PlayEffect(HexGrid.Colors[color].VFXPrefab, transform.position + new Vector3(0,0.1f,0));
            MusicController.Instance.AddAudioSource(vfx);
            MusicController.Instance.PlayRandomClip(MusicController.Instance.MusicData.SfxMusic.Captures, vfx);
            
            onHexPainted?.Invoke(this);
        }
    }
}