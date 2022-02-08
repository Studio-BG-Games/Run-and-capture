using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using HexFiled;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
    [Serializable]
    public class GridToSave
    {
        public SerializableHexCell[] cells;

        public int height;

        public int width;
        
    }

    [Serializable]
    public class SerializableHexCell
    {
        public HexCoordinates HexCoordinates;

        public (int x, int z, int i) index;
    }


    public class HexMapEditor : MonoBehaviour
    {
        [SerializeField] private GameObject hexPrefab;
        [SerializeField] private TMP_Text labelPrefab;
        [SerializeField] private GameObject gridCanvas;
        [SerializeField] private string levelName;

        [SerializeField, ListDrawerSettings(
             CustomAddFunction = "NewLevel",
             CustomRemoveIndexFunction = "RemoveLevel"
         )]
        private List<string> levels;

        [SerializeField] private string pathToMap;

        private GameObject _gridCanvasInstance;
        private HexCell[] _cells;
        private int _width;
        private int _height;

        private Color activeColor;
        private GameObject _fieldBaseGameObject;


        [Button("Draw Map")]
        private void DrawMap(int x, int y)
        {
            DestroyImmediate(_fieldBaseGameObject != null ? _fieldBaseGameObject : GameObject.Find("HexGrid"));

            DestroyImmediate(_gridCanvasInstance != null ? _gridCanvasInstance : GameObject.Find("CoordCanvas(Clone)"));

            _gridCanvasInstance = Instantiate(gridCanvas);

            _fieldBaseGameObject = new GameObject("HexField");
            _cells = new HexCell[x * y];
            _width = x;
            _height = y;

            SpawnField();
        }

        [Button("Load Map")]
        private void OpenLoadWindow()
        {
            LoadMapWindows.OpenWindow(this);
        }


        private void LoadMap(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(fileName,
                FileMode.Open);

            GridToSave data = (GridToSave)bf.Deserialize(fs);


            fs.Close();
            DestroyImmediate(_fieldBaseGameObject != null ? _fieldBaseGameObject : GameObject.Find("HexGrid"));

            DestroyImmediate(_gridCanvasInstance != null ? _gridCanvasInstance : GameObject.Find("CoordCanvas(Clone)"));

            _gridCanvasInstance = Instantiate(gridCanvas);

            _fieldBaseGameObject = new GameObject("HexField");
            _height = data.height;
            _width = data.width;
            _cells = new HexCell[_width * _height];


            foreach (var cell in data.cells)
            {
                CreateCell(cell.index.x, cell.index.z, cell.index.i);
            }

            GameObject.FindGameObjectsWithTag("Save").Where(x =>
                    x.name != "HexField" && x.name != "CoordCanvas(Clone)" && !x.GetComponent<Camera>() &&
                    !x.GetComponent<HexMapEditor>() && !x.GetComponent<HexCell>() && x.name != "Hex Cell Label(Clone)")
                .ToList().ForEach(DestroyImmediate);

            Regex rx = new Regex(@"\b[\\]\w+.dat\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matchedAuthors = rx.Matches(fileName);  
            var mapName = fileName.Replace(matchedAuthors[0].Value, "");
            Directory.GetFiles($"{mapName}/Enviroment", "*.prefab", SearchOption.AllDirectories).ToList().ForEach(x =>
            {
                var prefab = x.Replace("\\", "/");
                var go = PrefabUtility.LoadPrefabContents(prefab);
                var instance = Instantiate(go);
                instance.name = go.name.Replace("(Clone)", "");
                instance.tag = "Save";
            });
            
            Debug.Log("Game data loaded!");
        }


        [Button("Save", ButtonSizes.Gigantic)]
        void SaveGrid()
        {
            Directory.CreateDirectory($"{pathToMap}/{levelName}");
            Directory.CreateDirectory($"{pathToMap}/{levelName}/Enviroment");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create($"{pathToMap}/{levelName}/{levelName}.dat");
            GridToSave data = new GridToSave();
            var tmp = new List<SerializableHexCell>();
            _cells.ToList().Where(x => x != null).ToList().ForEach(cell =>
            {
                tmp.Add(cell == null ? null : cell.ToSerializibleHexCell());
            });


            GameObject.FindGameObjectsWithTag("Save").Where(x =>
                    x.name != "HexField" && x.name != "CoordCanvas(Clone)" && !x.GetComponent<Camera>() &&
                    !x.GetComponent<HexMapEditor>() && !x.GetComponent<HexCell>() && x.name != "Hex Cell Label(Clone)")
                .ToList().ForEach(x =>
                {
                    if(File.Exists($"{pathToMap}/{levelName}/Enviroment/{x.name}.prefab"))
                        File.Delete($"{pathToMap}/{levelName}/Enviroment/{x.name}.prefab");
                    PrefabUtility.SaveAsPrefabAsset(x, $"{pathToMap}/{levelName}/Enviroment/{x.name}.prefab");
                });

            data.cells = tmp.ToArray();
            data.width = _width;
            data.height = _height;


            bf.Serialize(file, data);
            file.Close();

            Debug.Log("Game data saved!");
        }


        private void NewLevel()
        {
            SaveGrid();
            levels.Add(levelName);
            levelName = "";

            DestroyImmediate(_gridCanvasInstance);
            DestroyImmediate(_fieldBaseGameObject);
        }

        private void RemoveLevel(int i)
        {
            File.Delete($"{pathToMap}/{levels[i]}.dat");
            levels.RemoveAt(i);
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
            }
        }

        void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(inputRay, out var hit))
            {
                var coord = HexCoordinates.FromPosition(hit.transform.position);
                _cells.First(x => x.coordinates.Equals(coord)).gameObject.GetComponent<MeshRenderer>().material.color =
                    Color.green;
            }
        }

        private void CreateCell(int x, int z, int i, bool isHexCoord = false)
        {
            Vector3 position;
            var cellGO = Object.Instantiate(hexPrefab);
            HexCell cell = _cells[i] = cellGO.AddComponent<HexCell>();
            if (isHexCoord)
            {
                HexCoordinates coordinates = new HexCoordinates(x, z);
                position = HexCoordinates.ToPosition(coordinates);
                (x, z) = HexCoordinates.ToOffsetCoordinates(coordinates);
            }
            else
            {
                position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
                position.y = 0f;
                position.z = z * (HexMetrics.outerRadius * 1.5f);
                cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            }


            cell.transform.SetParent(_fieldBaseGameObject.transform, false);
            cell.transform.localPosition = position;

            cell.index.i = i;
            cell.index.x = x;
            cell.index.z = z;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
            }

            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - _width]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, _cells[i - _width - 1]);
                    }
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - _width]);


                    if (x < _width - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, _cells[i - _width + 1]);
                    }
                }
            }
#if UNITY_EDITOR
            TMP_Text label = Object.Instantiate(labelPrefab, _gridCanvasInstance.transform, false);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
#endif
        }


        private void SpawnField()
        {
            for (int z = 0, i = 0; z < _height; z++)
            {
                for (int x = 0; x < _width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        private class LoadMapWindows : OdinEditorWindow
        {
            public static void OpenWindow(HexMapEditor editor)
            {
                var loadMapWindow = GetWindow<LoadMapWindows>();
                loadMapWindow.Show();
                loadMapWindow.MapsList = new List<Maps>();
                List<string> pathes = new List<string>();

                pathes = Directory.GetFiles("Assets/Resources/Maps", "*.dat", SearchOption.AllDirectories).ToList();
                pathes.ForEach(x => { loadMapWindow.MapsList.Add(new Maps(x, editor, DeleteMap)); });
            }

            [TableList(IsReadOnly = true, DrawScrollView = false, AlwaysExpanded = true, HideToolbar = true)]
            public List<Maps> MapsList;

            private static void DeleteMap(Maps maps)
            {
                GetWindow<LoadMapWindows>().MapsList.Remove(maps);
            }

            public class Maps
            {
                private HexMapEditor _editor;
                private Action<Maps> OnMapDeleted;

                public Maps(string path, HexMapEditor editor, Action<Maps> onMapDeleted)
                {
                    this.path = path;
                    _editor = editor;
                    OnMapDeleted += onMapDeleted;
                }

                [Button("Load")]
                public void LoadMap()
                {
                    _editor.LoadMap(path);
                }

                [Button("Remove")]
                public void RemoveMap()
                {
                    File.Delete(path);
                    OnMapDeleted.Invoke(this);
                }

                [InlineProperty()] public string path;
            }
        }
    }
}