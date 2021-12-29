﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexFiled
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates coordinates;
        public Action<HexCell> onHexPainted;

        [SerializeField] private HexCell[] neighbors;
        private UnitColor _color;
        private MeshRenderer _renderer;
        private Dictionary<UnitColor, CellColor> _cellColor;

        public UnitColor Color => _color;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            MusicController.Instance.AddAudioSource(gameObject);
            _color = UnitColor.GREY;
        }

        public void SetDictionary(Dictionary<UnitColor, CellColor> colors)
        {
            _cellColor = colors;
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
                _renderer.material.mainTexture = _cellColor[color].Texture;
                _color = color;
                return;
            }

            _renderer.material.mainTexture = _cellColor[color].Texture;
            
            _color = color;
            Instantiate(_cellColor[color].VFXPrefab, transform);
            onHexPainted?.Invoke(this);

        }
    }
}