using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LoadMapWindows : OdinEditorWindow
    {
        [MenuItem("Tools/LevelEditor")]
        private static void OpenWindow()
        {
            var loadMapWindow = GetWindow<LoadMapWindows>();
            loadMapWindow.Show();
            loadMapWindow.MapsList = new List<Maps>();
            List<string> pathes = Directory.GetDirectories("Assets/Resources/Maps").ToList();

            var editor = Transform.FindObjectOfType<HexMapEditor>();
            if (editor == null)
            {
                Debug.LogError("Не найден на сцене объект HexMapEditor");
                return;
            }
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
                Directory.Delete(path, true);
                File.Delete($"{path}.meta");
                AssetDatabase.Refresh();
                OnMapDeleted.Invoke(this);
            }

            [InlineProperty()] public string path;
        }
    }
}