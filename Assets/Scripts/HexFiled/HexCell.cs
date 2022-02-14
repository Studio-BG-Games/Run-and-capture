using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using DefaultNamespace;
using Items;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HexFiled
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates coordinates;
        public (int x, int z, int i) index;
        public event Action<HexCell> OnHexPainted;
        public bool isSpawnPos;
        [HideInInspector] public GameObject BuildingInstance;

        [SerializeField] private HexCell[] neighbors;
        [SerializeField] private Item _item;
        [SerializeField, AssetsOnly] public GameObject Building;

        

        private UnitColor _color;
        private MeshRenderer _renderer;

        public UnitColor Color => _color;

        private void OnDrawGizmos()
        {
            if (isSpawnPos)
            {
                Gizmos.DrawIcon(transform.position + new Vector3(0,1,0),"Spawner.png", true);
            }
        }

        public Item Item
        {
            get => _item;
            set => _item = value;
        }


        [Button("Set Building", ButtonSizes.Gigantic)]
        public void SetBuilding()
        {
            if (BuildingInstance != null)
            {
                DestroyImmediate(BuildingInstance);
            }

            if (Building != null)
            {
                BuildingInstance = Instantiate(Building, transform);
            }
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

            if (BuildingInstance != null)
            {
                Destroy(BuildingInstance);
            }

            HexManager.UnitCurrentCell
                .Where(cells
                    => HexManager.CellByColor[cells.Key].Count < 3 && !cells.Value.unit.IsStaned)
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