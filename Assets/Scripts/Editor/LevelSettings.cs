using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
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
        private static void OpenWindow()
        {
            GetWindow<LevelSettings>().Show();
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selection = this.MenuTree.Selection;
            if(selection.SelectedValue == null)
            {
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
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            _createNewLevel = new CreateNewLevel();
            var tree = new OdinMenuTree();
            tree.Add("New Level", _createNewLevel);
            var pathes = Resources.LoadAll<Data.Data>("Data/");
            pathes.ForEach(x =>
            {
                tree.AddAllAssetsAtPath($"{x.levelName}",$"Resources/Data/{x.levelName}");
            });
           
            
            return tree;
        }

        private class CreateNewLevel
        {
            public CreateNewLevel()
            {
                data = ScriptableObject.CreateInstance<Data.Data>();
                datas = new List<ScriptableObject>();
                datas.Add(ScriptableObject.CreateInstance<AIData>());
                datas.Add(ScriptableObject.CreateInstance<CameraData>());
                datas.Add(ScriptableObject.CreateInstance<FieldData>());
                datas.Add(ScriptableObject.CreateInstance<ItemsData>());
                datas.Add(ScriptableObject.CreateInstance<MusicData>());
                datas.Add(ScriptableObject.CreateInstance<UIData>());
                datas.Add(ScriptableObject.CreateInstance<UnitData>());
                datas.Add(ScriptableObject.CreateInstance<WeaponsData>());
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