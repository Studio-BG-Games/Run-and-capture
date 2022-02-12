using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using HexFiled;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif


namespace Data
{
    [CreateAssetMenu(fileName = "FieldData", menuName = "Data/Field Data")]
    public class FieldData : SerializedScriptableObject
    {
        [SerializeField] private List<GameObject> enviroment;
        [SerializeField] private List<GameObject> buildings;

        [Sirenix.OdinInspector.ReadOnly, ExecuteAlways]
        public GridToSave Field;

        public bool isSimpleField;
        [EnableIf("isSimpleField")] public int width = 6;
        [EnableIf("isSimpleField")] public int height = 6;
        [EnableIf("isSimpleField")] public List<HexCoordinates> spawnFields;
#if UNITY_EDITOR
        [InlineProperty, DisableIf("isSimpleField")] [InlineButton("OpenLevelList")]
#endif
        public string levelPath;

        public int hexCaptureManaCost;
        public int hexHardCaptureManaCost;
        public float hexHardCaptureTime;
        public GameObject cellPrefab;
        public TMP_Text cellLabelPrefab;
        public GameObject CoordinatesCanvas;
        public Dictionary<UnitColor, CellColor> colors;


        public List<GameObject> Enviroment => enviroment;

        public List<GameObject> Buildings => buildings;

#if UNITY_EDITOR
        private void OpenLevelList()
        {
            SelectLevelWindow.OpenWindow(SetLevelName);
        }

        private void SetLevelName(string name, List<GameObject> enviroment, List<GameObject> buildings,
            GridToSave serializeField)
        {
            this.enviroment = enviroment;
            this.buildings = buildings;

            levelPath = name;
            Field = serializeField;
        }

        public class SelectLevelWindow : OdinEditorWindow
        {
            public static void OpenWindow(Action<string, List<GameObject>, List<GameObject>, GridToSave> SetName)
            {
                var loadMapWindow = GetWindow<SelectLevelWindow>();
                loadMapWindow.Show();
                loadMapWindow.MapsList = new List<MapPath>();
                Directory.GetDirectories("Assets/Resources/Maps").ToList().ForEach(x =>
                {
                    loadMapWindow.MapsList.Add(new MapPath(x, SetName));
                });
            }

            [TableList(IsReadOnly = true, DrawScrollView = false, AlwaysExpanded = true, HideToolbar = true)]
            public List<MapPath> MapsList;

            public class MapPath
            {
                private Action<string, List<GameObject>, List<GameObject>, GridToSave> _setName;

                public MapPath(string path, Action<string, List<GameObject>, List<GameObject>, GridToSave> SetName)
                {
                    this.path = path;
                    _setName += SetName;
                }

                public string path;

                [Button("Select")]
                private void Select()
                {
                    List<GameObject> enviroment = new List<GameObject>();
                    Directory.GetFiles($"{path}/Enviroment", "*.prefab", SearchOption.AllDirectories).ToList().ForEach(
                        x =>
                        {
                            var prefab = x.Replace("\\", "/");
                            var go = (GameObject) AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject));
                            enviroment.Add(go);
                        });
                    List<GameObject> buildings = new List<GameObject>();
                    Directory.GetFiles($"{path}/Buildings", "*.prefab", SearchOption.AllDirectories).ToList().ForEach(
                        x =>
                        {
                            var prefab = x.Replace("\\", "/");
                           
                            var go =(GameObject) AssetDatabase.LoadAssetAtPath(prefab, typeof(GameObject));
                            buildings.Add(go);
                        });


                    GridToSave data = JsonUtility.FromJson<GridToSave>(File.ReadAllText($"{path}/map.dat"));


                    _setName.Invoke(path, enviroment, buildings, data);
                    AssetDatabase.SaveAssets();
                }
            }
        }
#endif
    }
}