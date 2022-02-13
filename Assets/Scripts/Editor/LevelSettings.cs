using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Items;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelSettings : OdinMenuEditorWindow
    {
        private CreateNewLevel _createNewLevel;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_createNewLevel != null)
            {
                DestroyImmediate(_createNewLevel.data);
                _createNewLevel.datas.ForEach(DestroyImmediate);
                
            }
        }
        

        [MenuItem("Tools/Level Settings")]
        public static void OpenWindow()
        {
            GetWindow<LevelSettings>().Show();
        }

        protected override void OnBeginDrawEditors()
        {
            var selection = this.MenuTree.Selection;
            if (selection.SelectedValue != null) return;
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton("Delete"))
                {

                    string levelName = selection[0].Name;
                    var data = new List<string>();
                    Resources.LoadAll<ScriptableObject>($"Data/{levelName}").ForEach(x =>
                    {
                        data.Add(AssetDatabase.GetAssetPath(x));
                    });
                    data.ToArray().ForEach(x => AssetDatabase.DeleteAsset(x));

                    Directory.Delete($"Assets/Resources/Data/{levelName}");
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            _createNewLevel = new CreateNewLevel();
            var tree = new OdinMenuTree
            {
                { "New Level", _createNewLevel },
                { "Items", new ItemList() }
            };
            var pathes = Resources.LoadAll<Data.Data>("Data/");
            pathes.ForEach(x =>
            {
                tree.AddAllAssetsAtPath($"{x.levelName}",$"Resources/Data/{x.levelName}");
            });
           
            
            return tree;
        }

        
        internal class ItemList
        {
            public ItemList()
            {
                items = Resources.LoadAll<Item>("Data/Items").ToList();
            }
            [OdinSerialize] public NewItem Type;
            
            [InlineEditor(Expanded = true), ListDrawerSettings(HideAddButton = true, CustomRemoveElementFunction = "RemoveItem")] public List<Item> items;

            
            private void RemoveItem(Item item)
            {
                var path = AssetDatabase.GetAssetPath(item);
                
                File.Delete(path);
                File.Delete($"{path}.meta");
                AssetDatabase.Refresh();
            }

            internal enum ItemType
            {
                Bonus,
                Building,
                CaptureAbility,
                SpecialWeapon
            }
            [Serializable]
            internal class NewItem
            {
                public ItemType Type;
                public string ItemName;
                
                [Button("Add Item")]
                private void AddItem()
                {
                    Item item = Type switch
                    {
                        ItemType.Bonus => CreateInstance<Bonus>(),
                        ItemType.Building => CreateInstance<Building>(),
                        ItemType.CaptureAbility => CreateInstance<CaptureAbility>(),
                        ItemType.SpecialWeapon => CreateInstance<SpecialWeapon>(),
                        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
                    };
                    AssetDatabase.CreateAsset(item,$"Assets/Resources/Data/Items/{ItemName}.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
        internal class CreateNewLevel
        {
            public CreateNewLevel()
            {
                data = CreateInstance<Data.Data>();
                datas = new List<ScriptableObject>
                {
                    CreateInstance<AIData>(),
                    CreateInstance<CameraData>(),
                    CreateInstance<FieldData>(),
                    CreateInstance<ItemsData>(),
                    CreateInstance<MusicData>(),
                    CreateInstance<UIData>(),
                    CreateInstance<UnitData>(),
                    CreateInstance<WeaponsData>()
                };
            }

           
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Boxed)]
            public Data.Data data;

            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Boxed), ListDrawerSettings(IsReadOnly = true)]
            public List<ScriptableObject> datas;
            

            [Button("Add new level", ButtonSizes.Gigantic)]
            private void CreateLevel()
            {
                if (data.levelName == "" || Directory.Exists($"Assets/Resources/Data/{data.levelName}"))
                {
                    return;
                }
                Directory.CreateDirectory($"Assets/Resources/Data/{data.levelName}");
                
                AssetDatabase.CreateAsset(data, $"Assets/Resources/Data/{data.levelName}/Data.asset");

                datas.ForEach(x =>
                {
                    AssetDatabase.CreateAsset(x, $"Assets/Resources/Data/{data.levelName}/{x.GetType().ToString().Replace("Data.", "")}.asset");
                });
                AssetDatabase.SaveAssets();
            }

           
        }
    }
}