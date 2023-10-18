using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


// link: https://gamedev.stackexchange.com/questions/188771/creating-a-custom-editor-window-using-a-multi-column-header
// Thanks to Candid Moon _Max_ for multi column solution
namespace MapEdit{
    public class MapEditor : EditorWindow
    {
        //Editor editor;
        MapDataScriptableObject scriptableObject;
        AnimBool customizeValues;
        Level currentLevel;

        // fields to edit
        int playTime;
        [SerializeField] List<Tile> tilePool;

        // multi columns
        private MultiColumnHeaderState multiColumnHeaderState;
        private MultiColumnHeader multiColumnHeader;

        private MultiColumnHeaderState.Column[] _columns;

        private readonly Color lighterColor = Color.white * 0.3f;
        private readonly Color darkerColor = Color.white * 0.1f;

        private Vector2 scrollPosition;
        float columnHeight = EditorGUIUtility.singleLineHeight;

        [MenuItem("Tools/Map Editor")]
        public static void ShowExample()
        {
            MapEditor wnd = GetWindow<MapEditor>();
            wnd.titleContent = new GUIContent("Map Editor");
        }

        private void Initialize()
        {
            // We can move these columns into some ScriptableObject or some other data saving object/file to save their properties there, otherwise because of some events these settings will be recreated and state of the window won't be saved as expected.
            this._columns = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = false, // At least one column must be there.
                    autoResize = true,
                    minWidth = 50.0f,
                    width = 75f,
                    maxWidth = 90.0f,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    headerContent = new GUIContent("Idx", "An index of element."),
                    headerTextAlignment = TextAlignment.Center,
                },
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = true,
                    autoResize = true,
                    minWidth = 125.0f,
                    maxWidth = 175.0f,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    headerContent = new GUIContent("Sprite", "A sprite of an tile."),
                    headerTextAlignment = TextAlignment.Center,
                },
                new MultiColumnHeaderState.Column(){
                    allowToggleVisibility = true,
                    autoResize = true,
                    minWidth = 100.0f,
                    maxWidth = 125.0f,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    headerContent = new GUIContent("Chance", "A chance of an tile."),
                    headerTextAlignment = TextAlignment.Center,
                },
                new MultiColumnHeaderState.Column(){
                    allowToggleVisibility = true,
                    autoResize = true,
                    minWidth = 10.0f,
                    maxWidth = 10.0f,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Center,
                    headerContent = new GUIContent("Delete", "Delete a tile."),
                    headerTextAlignment = TextAlignment.Center,
                }
            };

            this.multiColumnHeaderState = new MultiColumnHeaderState(columns: this._columns);

            this.multiColumnHeader = new MultiColumnHeader(state: this.multiColumnHeaderState);
            // When we change visibility of the column we resize columns to fit in the window.
            this.multiColumnHeader.visibleColumnsChanged += (multiColumnHeader) => multiColumnHeader.ResizeToFit();

            // Initial resizing of the content.
            this.multiColumnHeader.ResizeToFit();
        }



        private void OnEnable() {
            customizeValues = new AnimBool(false);
            customizeValues.valueChanged.AddListener(Repaint);
        }

        private void OnGUI() {
            //GUILayout.FlexibleSpace();
            //if (!editor) { editor = Editor.CreateEditor(this); }
            
            GUILayout.Label("Map Editor",EditorStyles.boldLabel);

            EditorGUILayout.Space();

            scriptableObject = EditorGUILayout.ObjectField("Map Data",scriptableObject,typeof(MapDataScriptableObject),false) as MapDataScriptableObject;
            if (scriptableObject == null){
                return;
            }
            
            customizeValues.target = EditorGUILayout.ToggleLeft("Customize Values",customizeValues.target);
            if (EditorGUILayout.BeginFadeGroup(customizeValues.faded)){
                EditorGUI.indentLevel++;
                currentLevel = (Level)EditorGUILayout.EnumPopup("Level to edit: ",currentLevel);
                ShowCurrentMapDataField(currentLevel);
                EditorGUI.indentLevel--;
            }
            else{
                // unload data
                
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUI.BeginDisabledGroup(scriptableObject == null);

            if (GUILayout.Button("Save")){
                // TODO: perform save
                playTime = 0;
                tilePool = null;
            }

            EditorGUI.EndDisabledGroup();
            
        }

        private void OnDisable() {
            customizeValues.valueChanged.RemoveAllListeners();
        }

        private void ShowCurrentMapDataField(Level currentLevel){
            var settings = scriptableObject.mapData.maps[(int)currentLevel];
            playTime = settings.playTime;
            tilePool = settings.tilePool;

            //if (editor) { editor.OnInspectorGUI(); }
            DrawMapDataFields();
        }

        private void DrawMapDataFields(){
            // After compilation and some other events data of the window is lost if it's not saved in some kind of container. Usually those containers are ScriptableObject(s).
            playTime = EditorGUILayout.IntField("Play time (s)",playTime);

            // Basically we just draw something. Empty space. Which is `FlexibleSpace` here on top of the window.
            // We need this for - `GUILayoutUtility.GetLastRect()` because it needs at least 1 thing to be drawn before it.
            GUILayout.FlexibleSpace();

            // Get automatically aligned rect for our multi column header component.
            Rect windowRect = GUILayoutUtility.GetLastRect();

            if (this.multiColumnHeader == null)
            {
                this.Initialize();
            }

            // Here we are basically assigning the size of window to our newly positioned `windowRect`.
            windowRect.width = this.position.width;
            windowRect.height = this.position.height;

            // This is a rect for our multi column table.
            Rect columnRectPrototype = new Rect(source: windowRect)
            {
                height = columnHeight, // This is basically a height of each column including header.
            };

            // Just enormously large view if you want it to span for the whole window. This is how it works [shrugs in confusion].
            Rect positionalRectAreaOfScrollView = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);

            // Create a `viewRect` since it should be separate from `rect` to avoid circular dependency.
            Rect viewRect = new Rect(source: windowRect)
            {
                xMax = this._columns.Sum((column) => column.width), // Scroll max on X is basically a sum of width of columns.
                yMax = columnHeight * this.tilePool.Count + EditorGUIUtility.singleLineHeight * 7 //-> magic number i dont know man :(
            };

            this.scrollPosition = GUI.BeginScrollView(
                position: positionalRectAreaOfScrollView,
                scrollPosition: this.scrollPosition,
                viewRect: viewRect,
                alwaysShowHorizontal: false,
                alwaysShowVertical: false
            );

            // Draw header for columns here.
            this.multiColumnHeader.OnGUI(rect: columnRectPrototype, xScroll: 0.0f);

            for (int i = 0; i < this.tilePool.Count; i++)
            {
                //nmkha: cant serialize custom class
                //SerializedObject serializedObject = new SerializedObject(this.tilePool[i]);
                
                Rect rowRect = new Rect(source: columnRectPrototype);
                rowRect.y += columnHeight * (i + 1);

                // Draw a texture before drawing each of the fields for the whole row.
                if (i % 2 == 0){
                    EditorGUI.DrawRect(rect: rowRect, color: this.darkerColor);
                }
                else{
                    EditorGUI.DrawRect(rect: rowRect, color: this.lighterColor);
                }

                // Idx field
                int columnIndex = 0;

                if (this.multiColumnHeader.IsColumnVisible(columnIndex: columnIndex))
                {
                    int visibleColumnIndex = this.multiColumnHeader.GetVisibleColumnIndex(columnIndex: columnIndex);

                    Rect columnRect = this.multiColumnHeader.GetColumnRect(visibleColumnIndex: visibleColumnIndex);

                    // This here basically is a row height, you can make it any value you like. Or you could calculate the max field height here that your object has and store it somewhere then use it here instead of `EditorGUIUtility.singleLineHeight`.
                    // We move position of field on `y` by this height to get correct position.
                    columnRect.y = rowRect.y;

                    GUIStyle nameFieldGUIStyle = new GUIStyle(GUI.skin.label)
                    {
                        padding = new RectOffset(left: 10, right: 10, top: 2, bottom: 2)
                    };

                    EditorGUI.LabelField(
                        position: this.multiColumnHeader.GetCellRect(visibleColumnIndex: visibleColumnIndex, columnRect),
                        label: new GUIContent((i+1).ToString()),
                        style: nameFieldGUIStyle
                    );
                }

                // Sprite field
                columnIndex = 1;

                if (this.multiColumnHeader.IsColumnVisible(columnIndex: columnIndex))
                {
                    InitializeRectColumn(out int visibleColumnIndex, out Rect columnRect);
                    
                    EditorGUI.ObjectField(
                        this.multiColumnHeader.GetCellRect(visibleColumnIndex: visibleColumnIndex, columnRect),
                        GUIContent.none,
                        this.tilePool[i].sprite,
                        typeof(Sprite),
                        false
                    );
                }

                // chance field
                columnIndex = 2;
                if (this.multiColumnHeader.IsColumnVisible(columnIndex: columnIndex))
                {
                    InitializeRectColumn(out int visibleColumnIndex, out Rect columnRect);
                    
                    tilePool[i].chance = EditorGUI.Slider(
                        this.multiColumnHeader.GetCellRect(visibleColumnIndex: visibleColumnIndex, columnRect),
                        tilePool[i].chance,
                        0f,
                        1f
                    );
                }

                // delete field
                columnIndex = 3;
                if (this.multiColumnHeader.IsColumnVisible(columnIndex: columnIndex))
                {
                    InitializeRectColumn(out int visibleColumnIndex, out Rect columnRect);
                    Rect cellRect = this.multiColumnHeader.GetCellRect(visibleColumnIndex,columnRect);
                    var style = new GUIStyle(EditorStyles.toolbarButton);
                    style.normal.textColor = Color.red;
                    style.active.textColor = Color.magenta;
                    if (GUI.Button(cellRect,new GUIContent("X","Remove"),style)){
                        // TODO: get index of selected row
                        Debug.Log("delete at " + i);
                        tilePool.RemoveAt(i);
                    }
                }


                void InitializeRectColumn(out int visibleColumnIndex, out Rect columnRect){
                    visibleColumnIndex = this.multiColumnHeader.GetVisibleColumnIndex(columnIndex: columnIndex);
                    columnRect = this.multiColumnHeader.GetColumnRect(visibleColumnIndex: visibleColumnIndex);
                    columnRect.y = rowRect.y;
                }
            }

            GUI.EndScrollView();
        }
    }

    // [CustomEditor(typeof(MapEditor), true)]
    // public class SpritePoolEditorDrawer : Editor {
    //     public override void OnInspectorGUI() {
    //         var list = serializedObject.FindProperty("tilePool");
    //         EditorGUILayout.PropertyField(list, new GUIContent("Sprite Pool"), true);
    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }
}