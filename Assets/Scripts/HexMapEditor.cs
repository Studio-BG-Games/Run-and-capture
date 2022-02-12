using System.Collections.Generic;
using System.IO;
using System.Linq;
using HexFiled;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


#if UNITY_EDITOR

namespace Editor
{
    public class HexMapEditor : SerializedMonoBehaviour
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
        [SerializeField] private HexCell[] _cells;
        [SerializeField, HideInInspector] private int _width;
        [SerializeField, HideInInspector] private int _height;

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


        #region Load Map

        public void LoadMap(string fileName)
        {
            
            GridToSave data = JsonUtility.FromJson<GridToSave>(File.ReadAllText($"{fileName}/map.dat"));
            
            
            DestroyImmediate(_fieldBaseGameObject != null ? _fieldBaseGameObject : GameObject.Find("HexGrid"));

            DestroyImmediate(_gridCanvasInstance != null ? _gridCanvasInstance : GameObject.Find("CoordCanvas(Clone)"));

            _gridCanvasInstance = Instantiate(gridCanvas);

            _fieldBaseGameObject = new GameObject("HexField");
            _height = data.height;
            _width = data.width;
            _cells = new HexCell[_height * _width];


            var buildings = new List<GameObject>();
            Directory.GetFiles($"{fileName}/Buildings", "*.prefab", SearchOption.AllDirectories)
                .ToList().ForEach(building =>
                {
                    var go = PrefabUtility.LoadPrefabContents(building);
                    go.name = go.name.Replace("(Clone)", "");
                    go.name = go.name.Replace("Buildings\\", "");
                    buildings.Add(go);
                });
            foreach (var cell in data.cells)
            {
                var building = buildings.Find(x => x.name == $"({cell.x}, {cell.z}, {cell.i})");
                CreateCell(cell, building);
            }

            GameObject.FindGameObjectsWithTag("Save").Where(x =>
                    x.name != "HexField" && x.name != "CoordCanvas(Clone)" && !x.GetComponent<Camera>() &&
                    !x.GetComponent<HexMapEditor>() && !x.GetComponent<HexCell>() && x.name != "Hex Cell Label(Clone)")
                .ToList().ForEach(DestroyImmediate);


            Directory.GetFiles($"{fileName}/Enviroment", "*.prefab", SearchOption.AllDirectories).ToList().ForEach(x =>
            {
                var prefab = x.Replace("\\", "/");
                var go = PrefabUtility.LoadPrefabContents(prefab);
                var instance = Instantiate(go);
                instance.name = go.name.Replace("(Clone)", "");
                instance.tag = "Save";
            });

            AssetDatabase.Refresh();
            Debug.Log("Game data loaded!");
        }

        #endregion

        [Button("Save", ButtonSizes.Gigantic)]

        #region Save Map

        private void SaveMap()
        {
            Directory.CreateDirectory($"{pathToMap}/{levelName}");
            Directory.CreateDirectory($"{pathToMap}/{levelName}/Enviroment");
            Directory.CreateDirectory($"{pathToMap}/{levelName}/Buildings");
            
            GridToSave data = new GridToSave();

            DirectoryInfo dir = new DirectoryInfo($"{pathToMap}/{levelName}/Enviroment/");

            foreach (FileInfo f in dir.GetFiles())
            {
                f.Delete();
            }

            dir = new DirectoryInfo($"{pathToMap}/{levelName}/Buildings/");

            foreach (FileInfo f in dir.GetFiles())
            {
                f.Delete();
            }

            var tmp = new List<SerializableHexCell>();
            _cells.ToList().Where(x => x != null).ToList().ForEach(cell =>
            {
                var scell = ToSerializibleHexCell(cell);
                tmp.Add(scell);

                if (cell.BuildingInstance != null)
                {
                    PrefabUtility.SaveAsPrefabAsset(cell.BuildingInstance,
                        $"{pathToMap}/{levelName}/Buildings/({scell.x}, {scell.z}, {scell.i}).prefab");
                }
            });


            GameObject.FindGameObjectsWithTag("Save").Where(x =>
                    x.name != "HexField" && x.name != "CoordCanvas(Clone)" && !x.GetComponent<Camera>() &&
                    !x.GetComponent<HexMapEditor>() && !x.GetComponent<HexCell>() && x.name != "Hex Cell Label(Clone)")
                .ToList().ForEach(x =>
                {
                    if (File.Exists($"{pathToMap}/{levelName}/Enviroment/{x.name}.prefab"))
                        File.Delete($"{pathToMap}/{levelName}/Enviroment/{x.name}.prefab");
                    PrefabUtility.SaveAsPrefabAsset(x, $"{pathToMap}/{levelName}/Enviroment/{x.name}.prefab");
                });

            data.cells = tmp.ToArray();
            data.width = _width;
            data.height = _height;
            if (File.Exists($"{pathToMap}/{levelName}/map.dat"))
            {
                File.Delete($"{pathToMap}/{levelName}/map.dat");
            }

            File.Create($"{pathToMap}/{levelName}/map.dat").Close();
            File.WriteAllText($"{pathToMap}/{levelName}/map.dat", JsonUtility.ToJson(data));
            
            AssetDatabase.Refresh();
            Debug.Log("Game data saved!");
        }

        #endregion

        private SerializableHexCell ToSerializibleHexCell(HexCell cell)
        {
            var scell = new SerializableHexCell
            {
                x = cell.index.x,
                z = cell.index.z,
                i = cell.index.i,
                IsSpawnPos = cell.isSpawnPos
            };

            return scell;
        }

        private void NewLevel()
        {
            SaveMap();
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

        private void CreateCell(SerializableHexCell scell, GameObject building)
        {
            Vector3 position;
            var x = scell.x;
            var z = scell.z;
            var i = scell.i;

            var cellGO = Instantiate(hexPrefab);
            HexCell cell = _cells[i] = cellGO.AddComponent<HexCell>();

            cell.Building = building;
            cell.isSpawnPos = scell.IsSpawnPos;
            

            cell.SetBuilding();
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);


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
                    var scell = new SerializableHexCell();
                    scell.x = x;
                    scell.z = z;
                    scell.i = i++;
                    CreateCell(scell, null);
                }
            }
        }
    }
}
#endif