namespace GameCreator.Core
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Characters;
    using GameCreator.Camera;
    using GameCreator.Variables;

    public static class GameCreatorToolbar
    {
        private enum BTN_POS { L, M, R };

        public class Item
        {
            public string iconPath;
            public string tooltip;
            public UnityAction callback;
            public int priority;
            private GUIContent content;
            private Texture2D texture;

            public Item(string iconPath, string tooltip, UnityAction callback, int priority = 100)
            {
                this.iconPath = iconPath;
                this.tooltip  = tooltip;
                this.callback = callback;
                this.priority = priority;
            }

            public GUIContent GetContent()
            {
                if (this.content == null)
                {
                    this.content = new GUIContent("", this.tooltip);
                }

                return this.content;
            }

            public Texture2D GetTexture()
            {
                if (this.texture == null)
                {
                    this.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(this.iconPath);
                }

                return this.texture;
            }
        }

        public const string PATH_ICONS = "Assets/Plugins/GameCreator/Extra/Icons/Toolbar/{0}";

        private const string KEY_TOOLBAR_ENABLED = "gamecreator-toolbar-enabled";
        private const bool TOOLBAR_DEFAULT = true;

        private const float PADDING_LFT = 10f;
        private const float PADDING_TOP = 10f;
        private const float TOOLBAR_HEIGHT = 20f;
        private const float BUTTONS_WIDTH  = 20f;

        private static bool DRAGGING = false;
        private static Vector2 MOUSE_POSITION = Vector2.zero;

        private static float MOVE_OFFSET_X = 0.0f;
        private static float MOVE_OFFSET_Y = 0.0f;

        private static bool STYLES_INITIALIZED = false;
        private static GUIStyle BTN_LFT;
        private static GUIStyle BTN_MID;
        private static GUIStyle BTN_RHT;

        private static List<Item> ITEMS = new List<Item>();
        public static Stack<Item> REGISTER_ITEMS = new Stack<Item>();

        // PUBLIC METHDOS: ------------------------------------------------------------------------

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (IsEnabled()) Enable();
            else Disable();

            RegisterDefaultItems();
        }

        [MenuItem("Game Creator/Show Toolbar %&t", true)]
        public static bool ShowToolbarValidate()
        {
            return !IsEnabled();
        }

        [MenuItem("Game Creator/Show Toolbar %&t", false)]
        public static void ShowToolbar()
        {
            Enable();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void Enable()
        {
            EditorPrefs.SetBool(KEY_TOOLBAR_ENABLED, true);
            SceneView.onSceneGUIDelegate += OnPaintToolbar;
            SceneView.RepaintAll();
        }

        private static void Disable()
        {
            EditorPrefs.SetBool(KEY_TOOLBAR_ENABLED, false);
            SceneView.onSceneGUIDelegate -= OnPaintToolbar;
            SceneView.RepaintAll();
        }

        private static bool IsEnabled()
        {
            return EditorPrefs.GetBool(KEY_TOOLBAR_ENABLED, TOOLBAR_DEFAULT);
        }

        private static void RegisterDefaultItems()
        {
            RegisterItem("drag.png", "Move the Toolbar", null, 0);
            RegisterItem("close.png", "Close the Toolbar", GameCreatorToolbar.Disable, 1);
            RegisterItem("trigger.png", "Create Trigger", TriggerEditor.CreateTrigger, 2);
            RegisterItem("event.png", "Create Event", EventEditor.CreateEvent, 3);
            RegisterItem("actions.png", "Create Actions", ActionsEditor.CreateAction, 4);
            RegisterItem("character.png", "Create Character", CharacterEditor.CreateCharacter, 5);
            RegisterItem("player.png", "Create Player", PlayerCharacterEditor.CreatePlayer, 6);
            RegisterItem("marker.png", "Create Navigation Marker", NavigationMarkerEditor.CreateMarker, 7);
            RegisterItem("hotspot.png", "Create Hotspot", HotspotEditor.CreateHotspot, 8);
            RegisterItem("motor.png", "Create Camera Motor", CameraMotorEditor.CreateCameraMotor, 9);
            RegisterItem("localvariables.png", "Create Local Variables", LocalVariablesEditor.CreateLocalVariables, 10);
        }

        private static void RegisterItem(string icon, string hint, UnityAction callback, int priority)
        {
            REGISTER_ITEMS.Push(new Item(string.Format(PATH_ICONS, icon), hint, callback, priority));
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private static void OnPaintToolbar(SceneView sceneview)
        {
            GUISkin prevSkin = GUI.skin;
            GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

            bool registeredItem = false;
            while (REGISTER_ITEMS.Count > 0)
            {
                registeredItem = true;
                Item item = REGISTER_ITEMS.Pop();
                ITEMS.Add(item);
            }

            if (registeredItem) ITEMS.Sort((Item x, Item y) => x.priority.CompareTo(y.priority));
            if (ITEMS.Count == 0) return;

            Rect rect = new Rect(
                PADDING_LFT + MOVE_OFFSET_X, 
                PADDING_TOP + MOVE_OFFSET_Y, 
                BUTTONS_WIDTH * ITEMS.Count,
                TOOLBAR_HEIGHT
            );

            bool mouseInRect = rect.Contains(UnityEngine.Event.current.mousePosition);
            if (UnityEngine.Event.current.type == EventType.MouseUp) DRAGGING = false;
            if (UnityEngine.Event.current.type == EventType.MouseDown && mouseInRect)
            {
                MOUSE_POSITION = UnityEngine.Event.current.mousePosition;
                DRAGGING = true;
            }

            if (DRAGGING)
            {
                Vector2 delta = UnityEngine.Event.current.mousePosition - MOUSE_POSITION;

                MOVE_OFFSET_X += delta.x;
                MOVE_OFFSET_Y += delta.y;
                SceneView.currentDrawingSceneView.Repaint();
            }

            MOUSE_POSITION = UnityEngine.Event.current.mousePosition;

            Handles.BeginGUI();
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginHorizontal();

            PaintButton(ITEMS[0], BTN_POS.L, MouseCursor.Pan);
            for (int i = 1; i < ITEMS.Count; ++i)
            {
                BTN_POS position = BTN_POS.M;
                if (i == ITEMS.Count - 1) position = BTN_POS.R;
                PaintButton(ITEMS[i], position);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            Handles.EndGUI();

            GUI.skin = prevSkin;
        }

        private static void PaintButton(Item item, BTN_POS position, MouseCursor cursor = MouseCursor.Link)
        {
            if (!STYLES_INITIALIZED)
            {
                BTN_LFT = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
                BTN_LFT.normal = BTN_LFT.active;
                BTN_LFT.onNormal = BTN_LFT.onActive;

                BTN_MID = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
                BTN_RHT = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));

                STYLES_INITIALIZED = true;
            }

            GUIStyle style;
            switch (position)
            {
                case BTN_POS.L: style = BTN_LFT; break;
                case BTN_POS.M: style = BTN_MID; break;
                case BTN_POS.R: style = BTN_RHT; break;
                default: style = null; break;
            }

            if (GUILayout.Button(item.GetContent(), style, GUILayout.Width(BUTTONS_WIDTH)))
            {
                if (item.callback != null) item.callback.Invoke();
            }

            Rect rect = GUILayoutUtility.GetLastRect();
            Rect textureRect = new Rect(
                rect.x + (rect.width/2.0f - rect.height/2.0f),
                rect.y,
                rect.height,
                rect.height
            );

            GUI.DrawTexture(textureRect, item.GetTexture());
            EditorGUIUtility.AddCursorRect(rect, cursor);
        }
    }
}