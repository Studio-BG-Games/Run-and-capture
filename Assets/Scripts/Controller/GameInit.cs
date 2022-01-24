﻿using System.Collections.Generic;
using AI;
using CamControl;
using Chars;
using DefaultNamespace;
using DefaultNamespace.AI;
using GameUI;
using HexFiled;
using Items;
using Units;
using UnityEngine;
using Weapons;
using Random = UnityEngine.Random;

namespace Controller
{
    internal sealed class GameInit
    {
        public GameInit(Controllers controllers, Data.Data data)
        {
            new AIManager(data.AIData);
            var hexGrid = new HexGrid(data.FieldData);
            new MusicController();
            new VFXController();
            MusicController.Instance.SetMusicData(data.MusicData);
            
            var paintedController = new PaintedController();

            data.WeaponsData.WeaponsList.ForEach(x => x.SetModifiedDamage(0));

            ItemFabric itemFabric = new ItemFabric(data.ItemsData);
            hexGrid.OnGridLoaded += () => controllers.Add(itemFabric);

            UIController uiController = new UIController(data.UIData);
            uiController.Spawn(); //TODO при паузе Dotween ругается
            

            var unitFactory = new UnitFactory(hexGrid, data, uiController, paintedController, controllers);

            hexGrid.OnGridLoaded += () => unitFactory.SpawnList(data.UnitData.Units);
            
            hexGrid.OnHexPainted += paintedController.SetHexColors;
            hexGrid.OnHexPainted += itemFabric.UpdateCellToOpenList;
            hexGrid.OnHexPainted += paintedController.CheckDeathOrDestroy;
            hexGrid.SpawnField();
        }
        
    }
}