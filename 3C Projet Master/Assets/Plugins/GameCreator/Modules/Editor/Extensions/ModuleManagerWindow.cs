namespace GameCreator.ModuleManager
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    public class ModuleManagerWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "Module Manager";
        private static readonly string[] OPTIONS = new string[] { "Project", "Store" };

        private const float WINDOW_W = 700f;
        private const float WINDOW_H = 500f;
        public const float WINDOW_SIDE_WIDTH = 200f;
        public const float WINDOW_SIDE_HEIGHT = 500f;

        public static ModuleManagerWindow WINDOW { get; private set; }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Vector2 contentScroll = Vector2.zero;
        private Vector2 sidebarScroll = Vector2.zero;
        private GUIStyle sidebarStyle = GUIStyle.none;
        private bool stylesInitialized = false;

        public int toolbarOptionsIndex = 0;
        public int sidebarIndex = 0;

        // INITIALIZERS: --------------------------------------------------------------------------

        [MenuItem("Game Creator/Module Manager %&m")]
        public static void OpenModuleManager()
        {
            Rect windowRect = new Rect(0f, 0f, WINDOW_W, WINDOW_H);
            ModuleManagerWindow.WINDOW = (ModuleManagerWindow)EditorWindow.GetWindowWithRect<ModuleManagerWindow>(
                windowRect, true, WINDOW_TITLE, true
            );

            ModuleManager.SetDirty();
            ModuleManagerWindow.WINDOW.Show();

            #if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += ModuleManager.SetDirty;
            #else
            EditorApplication.hierarchyWindowChanged += ModuleManager.SetDirty;
            #endif
            
        }

        private static void CloseModuleManager()
        {
            if (ModuleManagerWindow.WINDOW != null)
            {
                ModuleManagerWindow.WINDOW.Close();
            }
        }

		private void OnDestroy()
		{
            #if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged -= ModuleManager.SetDirty;
            #else
            EditorApplication.hierarchyWindowChanged -= ModuleManager.SetDirty;
            #endif
		}

		// PAINT METHODS: -------------------------------------------------------------------------

		void OnGUI()
        {
            this.InitializeStyles();
            if (ModuleManagerWindow.WINDOW == null)
            {
                ModuleManagerWindow.OpenModuleManager();
            }

            EditorGUILayout.BeginHorizontal();
            this.sidebarScroll = EditorGUILayout.BeginScrollView(
                this.sidebarScroll,
                false, false,
                GUIStyle.none,
                GUIStyle.none,
                this.sidebarStyle,
                GUILayout.MinWidth(WINDOW_SIDE_WIDTH),
                GUILayout.MaxWidth(WINDOW_SIDE_WIDTH),
                GUILayout.ExpandHeight(true)
            );

            this.PaintSidebar();

            EditorGUILayout.EndScrollView();
            Rect borderRect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandHeight(true), GUILayout.Width(1f));
            EditorGUI.DrawTextureAlpha(borderRect, Texture2D.blackTexture);

            EditorGUILayout.BeginVertical();

            this.PaintContent();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private void PaintSidebar()
        {
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(WINDOW_SIDE_WIDTH));
            ModuleManagerSidebar.PaintHeader();
            this.toolbarOptionsIndex = GUILayout.Toolbar(this.toolbarOptionsIndex, OPTIONS);

            EditorGUILayout.Space();
            switch (this.toolbarOptionsIndex)
            {
                case 0: ModuleManagerSidebar.PaintSidebarProjects(); break;
                case 1: ModuleManagerSidebar.PaintSidebarStore(); break;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void PaintContent()
        {
            this.PaintToolbar();
            this.contentScroll = EditorGUILayout.BeginScrollView(
                this.contentScroll,
                GUIStyle.none,
                GUI.skin.verticalScrollbar
            );

            EditorGUILayout.Space();
            switch (this.toolbarOptionsIndex)
            {
                case 0:
                    ModuleManifest[] manifests = ModuleManager.GetProjectManifests();
                    if (this.sidebarIndex < 0 || this.sidebarIndex >= manifests.Length)
                    {
                        ModuleManagerContent.PaintContentMessage();
                    }
                    else
                    {
                        ModuleManifest manifest = manifests[this.sidebarIndex];
                        ModuleManagerContent.PaintProjectModule(manifest);
                    }
                    break;

                case 1:
                    if (ModuleManagerStore.REQUEST_STATUS == ModuleManagerStore.StoreRequestStatus.Error)
                    {
                        ModuleManagerContent.PaintContentMessage(ModuleManagerStore.REQUEST_DATA.error);
                        break;
                    }
                    else if (ModuleManagerStore.REQUEST_STATUS == ModuleManagerStore.StoreRequestStatus.Requesting)
                    {
                        ModuleManagerContent.PaintContentMessage("Loading...");
                        break;
                    }

                    Module[] modules = ModuleManager.GetStoreModules();
                    if (this.sidebarIndex < 0 || this.sidebarIndex >= modules.Length)
                    {
                        ModuleManagerContent.PaintContentMessage();
                    }
                    else
                    {
                        Module module = modules[this.sidebarIndex];
                        ModuleManagerContent.PaintStoreModule(module);
                    }
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }

        private void PaintToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                ModuleManager.Refresh();
            }

            EditorGUILayout.EndHorizontal();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void InitializeStyles()
        {
            if (this.stylesInitialized) return;

            Texture2D texture = new Texture2D(1, 1);
            if (EditorGUIUtility.isProSkin) texture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.35f));
            else texture.SetPixel(0, 0, new Color(256f, 256f, 256f, 0.5f));

            texture.alphaIsTransparency = true;
            texture.Apply();

            this.sidebarStyle = new GUIStyle();
            this.sidebarStyle.normal.background = texture;
            this.sidebarStyle.margin = new RectOffset(0, 0, 0, 0);
        }
    }
}