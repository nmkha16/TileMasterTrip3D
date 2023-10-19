using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


// link: https://gamedev.stackexchange.com/questions/188771/creating-a-custom-editor-window-using-a-multi-column-header
// Thanks to Candid Moon _Max_ for multi column solution
// updated: no need to use Multi Column solution anymore.
namespace MapEdit{
    public class MapEditor : EditorWindow, IDisposable
    {
        //Editor editor;
        [Header("Editor Properties")]
        MapDataScriptableObject scriptableObject;
        AnimBool customizeValues;
        Level currentLevel;

        [Header("Editable Fields")]
        int playTime;
        [SerializeField] List<Tile> tilePool;

        [Header("Checkerboard color to easier distinguish rows")]
        private readonly Color lighterColor = Color.white * 0.3f;
        private readonly Color darkerColor = Color.white * 0.1f;

        private Vector2 scrollPosition;
        float columnHeight = 100f;
        float columnWidth = 100f;
        [MenuItem("Tools/Map Editor")]
        public static void Open()
        {
            MapEditor wnd = GetWindow<MapEditor>();
            wnd.titleContent = new GUIContent("Map Editor");
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
            
            customizeValues.target = EditorGUILayout.ToggleLeft("Edit",customizeValues.target);
            if (EditorGUILayout.BeginFadeGroup(customizeValues.faded)){
                EditorGUI.indentLevel++;
                var level = (Level)EditorGUILayout.EnumPopup("Level to edit ",currentLevel);
                ShowCurrentMapDataField(level);
                EditorGUI.indentLevel--;
            }
            else{
                CleanUpNullTiles();
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUI.BeginDisabledGroup(scriptableObject == null);

            if (GUILayout.Button("Add Tile")){
                this.tilePool.Add(new Tile());
            }

            EditorGUI.EndDisabledGroup();
        }

        private void OnDisable() {
            Dispose();
        }

        private void ShowCurrentMapDataField(Level level){
            if (this.currentLevel != level){
                // perform null tiles check first
                CleanUpNullTiles();
            }
            this.currentLevel = level;
            var settings = scriptableObject.mapData.maps[(int)currentLevel];
            this.playTime = settings.playTime;
            tilePool = settings.tilePool;
            //if (editor) { editor.OnInspectorGUI(); }
            DrawMapDataFields();
        }

        private void DrawMapDataFields(){
            scriptableObject.mapData.maps[(int)currentLevel].playTime = EditorGUILayout.IntField("Play time (s)", playTime);
            
            // Basically we just draw something. Empty space. Which is `FlexibleSpace` here on top of the window.
            // We need this for - `GUILayoutUtility.GetLastRect()` because it needs at least 1 thing to be drawn before it.
            GUILayout.FlexibleSpace();

            Rect windowRect = GUILayoutUtility.GetLastRect();
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
                xMax = columnWidth * 5, // Scroll max on X is basically a sum of width of columns. 5 because there is 5 columns
                yMax = columnHeight * this.tilePool.Count + EditorGUIUtility.singleLineHeight
            };

            this.scrollPosition = GUI.BeginScrollView(
                position: positionalRectAreaOfScrollView,
                scrollPosition: this.scrollPosition,
                viewRect: viewRect,
                alwaysShowHorizontal: false,
                alwaysShowVertical: false
            );

            // draw column header
            GUILayout.BeginHorizontal();
            Rect headerRect = new Rect(source: columnRectPrototype);
            GUI.Label(headerRect,"Idx");
            headerRect.x = columnWidth * 0.85f;
            GUI.Label(headerRect,"Name");
            headerRect.x += columnWidth;
            GUI.Label(headerRect,"Sprite");
            headerRect.x += columnWidth;
            GUI.Label(headerRect,"Chance");
            headerRect.x += columnWidth * 0.90f;
            GUI.Label(headerRect,"Move");
            headerRect.x += columnWidth * 0.45f;
            GUI.Label(headerRect,"Remove");
            GUILayout.EndHorizontal();

            for (int i = 0; i < this.tilePool.Count; i++)
            {
                GUILayout.BeginHorizontal();
                Rect rowRect = new Rect(source: columnRectPrototype);
                if (i == 0){
                    rowRect.y += EditorGUIUtility.singleLineHeight *(i+1);
                }
                else{
                    rowRect.y -= columnHeight - EditorGUIUtility.singleLineHeight;
                    rowRect.y += columnHeight * (i+1);
                }

                Rect cellRect = new Rect(rowRect);
                cellRect.height -= EditorGUIUtility.singleLineHeight;
                // index column
                var leftAlignStyle = GUI.skin.GetStyle("Label");
                leftAlignStyle.alignment = TextAnchor.UpperLeft;
                GUI.Label(rowRect,(i+1).ToString(),leftAlignStyle); // index

                // tile name column
                cellRect.x = columnWidth * 0.5f;
                cellRect.height = columnHeight *.2f;
                cellRect.width = columnWidth;
                this.tilePool[i].name = (TileName)EditorGUI.EnumPopup(cellRect,this.tilePool[i].name);

                // sprite column
                cellRect.x += columnWidth;
                cellRect.height = columnHeight - EditorGUIUtility.singleLineHeight;
                cellRect.width = cellRect.height * 1.25f;
                this.tilePool[i].sprite = (Sprite)EditorGUI.ObjectField(cellRect, this.tilePool[i].sprite, typeof(Sprite),false);

                // chance column
                cellRect.x += columnWidth;
                cellRect.width = columnWidth*1.25f;
                cellRect.height = columnHeight * 0.20f;
                tilePool[i].chance = EditorGUI.Slider(cellRect, tilePool[i].chance,0f,1f);

                // move column
                cellRect.x += columnWidth * 1.25f;
                cellRect.width = columnWidth * 0.5f;
                if (i == 0){
                    if(GUI.Button(cellRect,"▼")){
                        this.tilePool.Swap(i,i+1);
                    }
                }
                //final cell, no more down button
                else if (i == tilePool.Count-1){
                    if(GUI.Button(cellRect,"▲")){
                        this.tilePool.Swap(i,i-1);
                    }
                }
                else{
                    // nmkha: cannot use HorizontalGroup for we can't use cell rect to add button
                    // split rect an achieve this although the code is messy 
                    var leftRect = cellRect;
                    leftRect.width /= 2;
                    var rightRect = cellRect;
                    rightRect.width /= 2;
                    rightRect.x += rightRect.width;

                    if(GUI.Button(leftRect,"▲")){
                        this.tilePool.Swap(i,i-1);
                    }
                    if(GUI.Button(rightRect,"▼")){
                        this.tilePool.Swap(i,i+1);
                    }
                }

                // remove column
                cellRect.x += cellRect.width;
                
                var style = new GUIStyle(EditorStyles.toolbarButton);
                style.normal.textColor = Color.red;
                style.active.textColor = Color.magenta;
                if (GUI.Button(cellRect,new GUIContent("X","Remove"),style)){
                    tilePool.RemoveAt(i);
                }


                GUILayout.EndHorizontal();
                
                // Draw a texture for row
                if (i % 2 == 0){
                    EditorGUI.DrawRect(rect: rowRect, color: this.darkerColor);
                }
                else{
                    EditorGUI.DrawRect(rect: rowRect, color: this.lighterColor);
                }
            }

            GUI.EndScrollView();
        }

        public void Dispose()
        {
            CleanUpNullTiles();

            // clean up event
            customizeValues.valueChanged.RemoveAllListeners();
        }

        private void CleanUpNullTiles(){
            if (this.tilePool == null) return;
            bool isFoundNullTile = false;
            // should dispose any null tile if user close editor
            for(int i = 0; i < this.tilePool.Count; ++i){
                if (this.tilePool[i].sprite == null){
                    this.tilePool.RemoveAt(i--);
                    isFoundNullTile = true;
                }
            }

            if (isFoundNullTile){
                EditorUtility.DisplayDialog("Alert","All null tiles have been removed on last edited level!","OK");
            }

            // unload data
            this.playTime = 0;
            this.tilePool = null;
        }
    }
}