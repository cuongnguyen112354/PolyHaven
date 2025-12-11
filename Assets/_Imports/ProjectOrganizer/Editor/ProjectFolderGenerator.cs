// Assets/ProjectOrganizer/Editor/ProjectFolderGenerator.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ProjectOrganizerTool
{
    [Serializable]
    public class FileMoveOperation
    {
        public string Source;
        public string Destination;
        public FileMoveOperation(string source, string destination) { Source = source; Destination = destination; }
    }

    [Serializable]
    public class UndoMoveBatch { public List<FileMoveOperation> Moves = new(); }

    public class ProjectOrganizerWindow : EditorWindow
    {
        // -------- Paths / Assets --------
        private const string ConfigAssetPath = "Assets/Editor/ProjectOrganizerConfig.asset";
        private const string UndoLogPath     = "Library/ProjectOrganizerUndo.json";

        // -------- Theme (même DA que Diagram Generator / CodeHarvest) --------
        private const float BG_OVERLAY_ALPHA = 0.26f;
        private static readonly Color COL_BG        = new(0.12f, 0.12f, 0.14f, 1f);
        private static readonly Color COL_PANEL     = new(0.16f, 0.17f, 0.20f, 1f);
        private static readonly Color COL_PANEL_2   = new(0.20f, 0.21f, 0.24f, 1f);
        private static readonly Color COL_BORDER    = new(0.30f, 0.32f, 0.36f, 1f);
        private static readonly Color COL_TEXT      = new(0.94f, 0.94f, 0.97f, 1f);
        private static readonly Color COL_TEXT_SUB  = new(0.78f, 0.80f, 0.84f, 1f);
        private static readonly Color COL_ROW_EVEN  = new(1f,   1f,   1f,   0.03f);

        private static readonly Color ACCENT_VIOLET = new(0.43f, 0.34f, 0.68f, 1f);
        private static readonly Color ACCENT_BLUE   = new(0.36f, 0.62f, 0.97f, 1f);
        private static readonly Color ACCENT_V_HOVER= new(0.54f, 0.45f, 0.80f, 1f);
        private static readonly Color ACCENT_B_HOVER= new(0.52f, 0.74f, 1.00f, 1f);

        private static readonly Color COL_DANGER    = new(0.80f, 0.08f, 0.15f, 1f);
        private static readonly Color COL_WARN      = new(1.00f, 0.86f, 0.45f, 1f);
        private static readonly Color DARK_RED = new Color(0.40f, 0.08f, 0.12f, 1f); 

        // -------- State --------
        private ProjectOrganizerConfig _config;
        private bool _dryRunMode = true;
        private bool _filesScanned;
        private string _fileSearch = "";
        private string _statusMessage = "";

        // Files
        private readonly Dictionary<string, bool>   _fileToggles        = new();
        private readonly Dictionary<string, string> _fileToTargetFolder  = new();

        // Undo
        private UndoMoveBatch _lastUndoBatch;

        // UI
        private Texture2D _bgTex, _texPanel, _texPanel2, _logoTexture;
        private GUIStyle _titleStyle, _subtitleStyle, _cardHeaderStyle, _cardBodyStyle, _dirMini, _footerMini, _centerMini;
        private Vector2 _outerScroll, _mappingScroll, _filesScroll, _previewScroll;

        [MenuItem("Tools/Project Organizer")]
        public static void ShowWindow()
        {
            var w = GetWindow<ProjectOrganizerWindow>("Project Organizer");
            w.minSize = new Vector2(720, 800);
        }

        private void OnEnable()
        {
            _bgTex = Resources.Load<Texture2D>("settings_bg");
            if (_bgTex == null)
                _bgTex = Resources.Load<Texture2D>("inspector_bg");

            _logoTexture = Resources.Load<Texture2D>("Icon-160x160 - Project Organizer");
            LoadOrCreateConfig();
        }

        // -------- Ensure theme (crée textures/styles uniquement en OnGUI) --------
        void EnsureTheme()
        {
            if (Event.current == null) return;

            if (_texPanel  == null) _texPanel  = MakeTex(2, 2, COL_PANEL);
            if (_texPanel2 == null) _texPanel2 = MakeTex(2, 2, COL_PANEL_2);

            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, alignment = TextAnchor.MiddleLeft, richText = true };
                _titleStyle.normal.textColor = COL_TEXT;
            }
            if (_subtitleStyle == null)
            {
                _subtitleStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleLeft };
                _subtitleStyle.normal.textColor = COL_TEXT_SUB;
            }
            if (_cardHeaderStyle == null)
            {
                _cardHeaderStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
                _cardHeaderStyle.normal.textColor = COL_TEXT;
            }
            if (_cardBodyStyle == null)
            {
                _cardBodyStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(12,12,10,12),
                    margin  = new RectOffset(0,0,0,0)
                };
                _cardBodyStyle.normal.background = _texPanel2;
            }
            if (_dirMini == null)
            {
                _dirMini = new GUIStyle(EditorStyles.miniLabel) { fontSize = 10 };
                _dirMini.normal.textColor = new Color(0.76f, 0.88f, 1f, 1f);
                _dirMini.wordWrap = false;
                _dirMini.clipping = TextClipping.Clip;
            }
            if (_footerMini == null)
            {
                _footerMini = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.LowerCenter, fontStyle = FontStyle.Italic };
                _footerMini.normal.textColor = COL_TEXT_SUB;
            }
            if (_centerMini == null)
            {
                _centerMini = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
                _centerMini.normal.textColor = COL_TEXT;
            }
        }

        // -------- Layout helpers --------
        void DrawBackground()
        {
            var r = new Rect(0, 0, position.width, position.height);
            if (_bgTex != null)
            {
                GUI.DrawTexture(r, _bgTex, ScaleMode.ScaleAndCrop, true);
                EditorGUI.DrawRect(r, new Color(0,0,0, BG_OVERLAY_ALPHA));
            }
            else EditorGUI.DrawRect(r, COL_BG);
        }

        void Header()
        {
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (_logoTexture != null) GUILayout.Label(_logoTexture, GUILayout.Width(40), GUILayout.Height(40));
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("Project Organizer", _titleStyle);
                    EditorGUILayout.LabelField("Organize & migrate files with rules • dry-run • undo", _subtitleStyle);
                }
            }
            var line = GUILayoutUtility.GetRect(1,1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(line, COL_BORDER);
            GUILayout.Space(2);
        }

        void Footer()
        {
            var line = GUILayoutUtility.GetRect(1,1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(line, COL_BORDER);

            var r = GUILayoutUtility.GetRect(1, 32, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint) GUI.DrawTexture(r, _texPanel);
            GUI.Label(r, string.IsNullOrEmpty(_statusMessage) ? "Ready." : _statusMessage, _centerMini);

            GUILayout.Label("© 2025 ProjectOrganizerTool • v2.0.1", _footerMini);
            GUILayout.Space(4);
        }

        void Card(string title, Action body)
        {
            var header = GUILayoutUtility.GetRect(1, 24, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint) GUI.DrawTexture(header, _texPanel);
            GUI.Label(new Rect(header.x + 10, header.y, header.width - 20, header.height), title, _cardHeaderStyle);
            EditorGUI.DrawRect(new Rect(header.x, header.yMax - 1, header.width, 1), COL_BORDER);

            EditorGUILayout.BeginVertical(_cardBodyStyle);
            try { body?.Invoke(); } finally { EditorGUILayout.EndVertical(); }
            GUILayout.Space(10);
        }

        bool PrimaryButton(string label, float height = 42f, float minWidth = 240f, bool expand = false)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.miniButton,
                GUILayout.Height(height),
                GUILayout.MinWidth(minWidth),
                expand ? GUILayout.ExpandWidth(true) : GUILayout.ExpandWidth(false));

            bool hover = rect.Contains(Event.current.mousePosition);
            var top = new Rect(rect.x, rect.y, rect.width, Mathf.Round(rect.height * 0.5f));
            EditorGUI.DrawRect(top, hover ? ACCENT_B_HOVER : ACCENT_BLUE);
            var bot = new Rect(rect.x, rect.y + top.height, rect.width, rect.height - top.height);
            EditorGUI.DrawRect(bot, hover ? ACCENT_V_HOVER : ACCENT_VIOLET);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), COL_BORDER);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), COL_BORDER);

            var style = new GUIStyle(GUI.skin.button) { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = Color.white;
            return GUI.Button(rect, label, style);
        }

        bool SecondaryButton(string label, float height = 28f, float maxWidth = 180f)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.miniButton,
                GUILayout.Height(height), GUILayout.MaxWidth(maxWidth));
            EditorGUI.DrawRect(rect, COL_PANEL_2);
            var style = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = COL_TEXT;
            return GUI.Button(rect, label, style);
        }

        bool MiniButton(string label, float width)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.miniButton, GUILayout.Width(width));
            EditorGUI.DrawRect(rect, COL_PANEL_2);
            var s = new GUIStyle(EditorStyles.miniButton); s.normal.textColor = COL_TEXT;
            return GUI.Button(rect, label, s);
        }

        // -------- OnGUI --------
        private void OnGUI()
        {
            EnsureTheme();
            DrawBackground();
            Header();

            // Toolbar (options rapides)
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // libellé
                var tStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleLeft };
                tStyle.normal.textColor = COL_TEXT_SUB;
                GUILayout.Label("Mode:", tStyle, GUILayout.Width(40));

                // le toggle "Dry Run" existant
                _dryRunMode = GUILayout.Toggle(
                    _dryRunMode,
                    _dryRunMode ? "Dry Run" : "Apply Moves",
                    EditorStyles.toolbarButton,
                    GUILayout.Width(92)
                );

                GUILayout.FlexibleSpace();

                // ✅ Scan files au style toolbar
                if (GUILayout.Button("Scan files", EditorStyles.toolbarButton, GUILayout.Width(110)))
                {
                    ScanFiles();
                    _filesScanned = true;
                    _fileSearch = "";
                }

                // ✅ Simulate / Organize au style toolbar
                if (GUILayout.Button(_dryRunMode ? "Simulate" : "Organize Now",
                        EditorStyles.toolbarButton, GUILayout.Width(150)))
                {
                    Organize(!_dryRunMode);
                }
            }

            _outerScroll = EditorGUILayout.BeginScrollView(_outerScroll);

            // --- Cards ---
            Card("Rules & Scope", DrawConfigUI);
            Card("Scan & Select Files", DrawScanSelect);
            Card("Preview", DrawPreview);
            Card("Actions", DrawActions);

            EditorGUILayout.EndScrollView();
            Footer();
        }

        // -------- Sections --------
        void DrawConfigUI()
        {
            // Mapping
            EditorGUILayout.LabelField("Extension → Target Folder (editable)", EditorStyles.boldLabel);
            _mappingScroll = EditorGUILayout.BeginScrollView(_mappingScroll, GUILayout.Height(190));
            int mappingToRemove = -1;
            for (int i = 0; i < _config.ExtensionMappings.Count; i++)
            {
                var m = _config.ExtensionMappings[i];
                using (new EditorGUILayout.HorizontalScope())
                {
                    m.Extension    = EditorGUILayout.TextField(m.Extension, GUILayout.Width(60));
                    GUILayout.Label("→", GUILayout.Width(14));
                    m.TargetFolder = EditorGUILayout.TextField(m.TargetFolder);
                    if (DangerMiniButton("X")) mappingToRemove = i;
                }
            }
            if (mappingToRemove >= 0) _config.ExtensionMappings.RemoveAt(mappingToRemove);
            EditorGUILayout.EndScrollView();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (SecondaryButton("➕ Add mapping")) _config.ExtensionMappings.Add(new ExtensionMapping(".ext", "Assets/YourFolder"));
                GUILayout.FlexibleSpace();
            }
            EditorUtility.SetDirty(_config);

            GUILayout.Space(8);
            EditorGUILayout.LabelField("Included Folders", EditorStyles.boldLabel);
            DrawStringList(_config.IncludeFolders, addLabel: "➕ Add folder", defaultValue: "Assets");

            GUILayout.Space(6);
            EditorGUILayout.LabelField("Excluded Folders", EditorStyles.boldLabel);
            DrawStringList(_config.ExcludeFolders, addLabel: "➕ Add exclusion", defaultValue: "");

            GUILayout.Space(6);
            EditorGUILayout.LabelField("Extensions to scan (comma/semicolon, e.g. .cs,.png,.asset)", EditorStyles.boldLabel);
            _config.ExtensionsToScan = EditorGUILayout.TextField(_config.ExtensionsToScan);
            EditorUtility.SetDirty(_config);
        }
        
        bool DangerMiniButton(string label = "X", float width = 26f, float height = 22f)
        {
            // petites textures unies pour les états du bouton
            if (_texDanger == null) _texDanger = MakeTex(2, 2, DARK_RED);
            if (_texDangerHover == null) _texDangerHover =
                MakeTex(2, 2, new Color(Mathf.Min(DARK_RED.r + 0.08f,1f),
                    Mathf.Min(DARK_RED.g + 0.08f,1f),
                    Mathf.Min(DARK_RED.b + 0.08f,1f), 1f));
            if (_texDangerActive == null) _texDangerActive =
                MakeTex(2, 2, new Color(Mathf.Min(DARK_RED.r + 0.16f,1f),
                    Mathf.Min(DARK_RED.g + 0.16f,1f),
                    Mathf.Min(DARK_RED.b + 0.16f,1f), 1f));

            var s = new GUIStyle(EditorStyles.miniButton)
            {
                margin  = new RectOffset(0,0,0,0),
                padding = new RectOffset(0,0,0,0),
                alignment = TextAnchor.MiddleCenter
            };

            // on remplace le fond du bouton, plus besoin de DrawRect
            s.normal.background    = _texDanger;
            s.hover.background     = _texDangerHover;
            s.active.background    = _texDangerActive;
            s.onNormal.background  = _texDanger;
            s.onHover.background   = _texDangerHover;
            s.onActive.background  = _texDangerActive;

            s.normal.textColor  = Color.white;
            s.hover.textColor   = Color.white;
            s.active.textColor  = Color.white;

            return GUILayout.Button(label, s, GUILayout.Width(width), GUILayout.Height(height));
        }

        private Texture2D _texDanger, _texDangerHover, _texDangerActive;

        void DrawScanSelect()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _fileSearch = EditorGUILayout.TextField(new GUIContent("🔎 Search"), _fileSearch);
                if (MiniButton("✔ All", 56))  foreach (var k in _fileToggles.Keys.ToList()) _fileToggles[k] = true;
                if (MiniButton("✖ None", 64)) foreach (var k in _fileToggles.Keys.ToList()) _fileToggles[k] = false;
            }

            if (!_filesScanned)
            {
                GUILayout.Space(6);
                EditorGUILayout.HelpBox("Click “Scan files” in the toolbar to populate the list.", MessageType.Info);
                return;
            }

            var filtered = string.IsNullOrWhiteSpace(_fileSearch)
                ? _fileToggles.Keys.ToList()
                : _fileToggles.Keys.Where(k => Path.GetFileName(k).IndexOf(_fileSearch, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            _filesScroll = EditorGUILayout.BeginScrollView(_filesScroll, GUILayout.Height(320));
            for (int i = 0; i < filtered.Count; i++)
            {
                string file = filtered[i];
                var row = GUILayoutUtility.GetRect(1, 22, GUILayout.ExpandWidth(true));
                if (i % 2 == 0) EditorGUI.DrawRect(row, COL_ROW_EVEN);

                var tRect = new Rect(row.x + 6, row.y + 2, 18, 18);
                _fileToggles[file] = EditorGUI.Toggle(tRect, _fileToggles[file]);

                var nameRect = new Rect(tRect.xMax + 6, row.y + 1, 280, 18);
                var nameStyle = new GUIStyle(EditorStyles.label); nameStyle.normal.textColor = COL_TEXT;
                GUI.Label(nameRect, Path.GetFileName(file), nameStyle);

                var dirRect = new Rect(nameRect.xMax + 8, row.y + 1, row.width - nameRect.width - 250, 18);
                GUI.Label(dirRect, $"[{Path.GetDirectoryName(file)}]", _dirMini);

                // target folder (read-only inline)
                var target = _fileToTargetFolder.TryGetValue(file, out var tf) ? tf : "Assets/Misc";
                var targetRect = new Rect(row.xMax - 240, row.y + 1, 230, 18);
                var mini = new GUIStyle(EditorStyles.miniLabel); mini.normal.textColor = COL_TEXT_SUB;
                GUI.Label(targetRect, $"→ {target}", mini);
            }
            EditorGUILayout.EndScrollView();
        }

        void DrawPreview()
        {
            _dryRunMode = EditorGUILayout.ToggleLeft(new GUIContent("Dry run (preview only, no file moves)"), _dryRunMode);
            GUILayout.Space(6);

            if (_fileToggles.Count == 0 || !_fileToggles.Any(kv => kv.Value))
            {
                EditorGUILayout.HelpBox("Nothing to preview. Scan files and select some entries.", MessageType.None);
                return;
            }

            _previewScroll = EditorGUILayout.BeginScrollView(_previewScroll, GUILayout.Height(220));
            foreach (var kv in _fileToggles.Where(kv => kv.Value))
            {
                var file = kv.Key;
                var fileName = Path.GetFileName(file);
                var assetPath = file.Substring(Math.Max(0, file.IndexOf("Assets/")));
                var targetFolder = _fileToTargetFolder.TryGetValue(file, out var tf) ? tf : "Assets/Misc";

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("→", GUILayout.Width(16));
                    GUILayout.Label(fileName, GUILayout.Width(220));
                    GUILayout.Label("From:", GUILayout.Width(36));
                    GUILayout.Label(Path.GetDirectoryName(assetPath), EditorStyles.miniLabel, GUILayout.MinWidth(120));
                    GUILayout.Label("To:", GUILayout.Width(20));
                    GUILayout.Label(targetFolder, EditorStyles.boldLabel, GUILayout.MinWidth(120));
                }
            }
            EditorGUILayout.EndScrollView();
        }

        void DrawActions()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (PrimaryButton(_dryRunMode ? "SIMULATE" : "ORGANIZE NOW", 42f, 240f, expand:false))
                    Organize(!_dryRunMode);
                GUILayout.FlexibleSpace();
            }

            // Undo (chargé à la volée)
            var undoBatch = _lastUndoBatch ?? LoadUndoBatchFromFile();
            if (undoBatch != null && undoBatch.Moves != null && undoBatch.Moves.Count > 0)
            {
                GUILayout.Space(6);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (SecondaryButton("⏪ UNDO last organize", 30f, 220f))
                    {
                        _lastUndoBatch = undoBatch;
                        UndoLastMoveBatch();
                        Repaint();
                    }
                    GUILayout.FlexibleSpace();
                }
            }

            if (!string.IsNullOrEmpty(_statusMessage))
            {
                var col = _statusMessage.Contains("UNDO") ? COL_WARN :
                          (_dryRunMode ? new Color(0.6f, 0.8f, 1f, 1f) :
                           (_statusMessage.Contains("Moved") ? ACCENT_BLUE : COL_DANGER));

                var style = new GUIStyle(EditorStyles.label) { wordWrap = true, fontSize = 13 };
                style.normal.textColor = col;
                GUILayout.Space(6);
                EditorGUILayout.LabelField(_statusMessage, style);
            }
        }

        void DrawStringList(List<string> list, string addLabel, string defaultValue)
        {
            int toRemove = -1;
            for (int i = 0; i < list.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    list[i] = EditorGUILayout.TextField(list[i]);
                    if (DangerMiniButton("X")) toRemove = i;
                }
            }
            if (toRemove >= 0) list.RemoveAt(toRemove);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (SecondaryButton(addLabel)) list.Add(defaultValue);
                GUILayout.FlexibleSpace();
            }
            EditorUtility.SetDirty(_config);
        }

        // -------- Logic --------
        private void LoadOrCreateConfig()
        {
            _config = AssetDatabase.LoadAssetAtPath<ProjectOrganizerConfig>(ConfigAssetPath);
            if (_config == null)
            {
                _config = CreateInstance<ProjectOrganizerConfig>();
                var dir = Path.GetDirectoryName(ConfigAssetPath).Replace("\\","/");
                if (!AssetDatabase.IsValidFolder(dir))
                {
                    // crée Assets/Editor si besoin
                    if (dir.StartsWith("Assets/"))
                    {
                        var parts = dir.Substring(7).Split('/');
                        var parent = "Assets";
                        foreach (var p in parts)
                        {
                            var test = $"{parent}/{p}";
                            if (!AssetDatabase.IsValidFolder(test)) AssetDatabase.CreateFolder(parent, p);
                            parent = test;
                        }
                    }
                }
                AssetDatabase.CreateAsset(_config, ConfigAssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("ProjectOrganizerConfig asset created at: " + ConfigAssetPath);
            }
        }

        private void ScanFiles()
        {
            _fileToggles.Clear();
            _fileToTargetFolder.Clear();

            var exts = _config.ExtensionsToScan.Split(',', ';')
                .Select(e => e.Trim().ToLower())
                .Where(e => !string.IsNullOrEmpty(e) && e.StartsWith("."))
                .ToArray();

            foreach (var folder in _config.IncludeFolders)
            {
                if (!Directory.Exists(folder)) continue;

                foreach (var ext in exts)
                {
                    var files = Directory.GetFiles(folder, "*" + ext, SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var norm = file.Replace("\\", "/");
                        if (_config.ExcludeFolders.Exists(e => !string.IsNullOrEmpty(e) && norm.StartsWith(e))) continue;
                        if (_fileToggles.ContainsKey(norm)) continue;

                        _fileToggles.Add(norm, true);
                        _fileToTargetFolder.Add(norm, GetTargetFolderForFile(norm));
                    }
                }
            }
        }

        private string GetTargetFolderForFile(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            var mapping = _config.ExtensionMappings.FirstOrDefault(m => m.Extension.ToLower() == ext);
            return mapping != null ? mapping.TargetFolder : "Assets/Misc";
        }

        private void Organize(bool applyMoves)
        {
            // Crée l'arbo de template si besoin
            foreach (var folder in _config.TemplateFolders)
            {
                var fullPath = Path.Combine("Assets", folder).Replace("\\", "/");
                if (!AssetDatabase.IsValidFolder(fullPath))
                {
                    var parts = fullPath.Substring(7).Split('/');
                    var parent = "Assets";
                    foreach (var part in parts)
                    {
                        var testPath = $"{parent}/{part}";
                        if (!AssetDatabase.IsValidFolder(testPath))
                            AssetDatabase.CreateFolder(parent, part);
                        parent = testPath;
                    }
                }
            }

            if (_fileToggles.Count == 0 || !_fileToggles.Any(kv => kv.Value))
            {
                _statusMessage = "Nothing to organize! Please scan and check files first.";
                return;
            }

            int moved = 0, skipped = 0, conflict = 0;
            var log = new StringBuilder();
            var undoBatch = new UndoMoveBatch();

            foreach (var kv in _fileToggles.Where(k => k.Value))
            {
                var file        = kv.Key;
                var fileName    = Path.GetFileName(file);
                var targetFolder= GetTargetFolderForFile(file);
                var assetPath   = file.Substring(file.IndexOf("Assets/", StringComparison.Ordinal));
                var targetPath  = $"{targetFolder}/{fileName}";

                if (assetPath == targetPath)
                {
                    log.AppendLine($"[SKIP] {fileName} is already in the correct folder.");
                    skipped++; continue;
                }

                if (File.Exists(targetPath))
                {
                    log.AppendLine($"[CONFLICT] {fileName} already exists at destination (ignored).");
                    conflict++; continue;
                }

                if (applyMoves)
                {
                    var moveResult = AssetDatabase.MoveAsset(assetPath, targetPath);
                    if (string.IsNullOrEmpty(moveResult))
                    {
                        moved++;
                        log.AppendLine($"[MOVE] {fileName} → {targetFolder}");
                        undoBatch.Moves.Add(new FileMoveOperation(targetPath, assetPath));
                    }
                    else log.AppendLine($"[ERROR] {fileName}: {moveResult}");
                }
                else
                {
                    log.AppendLine($"[SIMULATE] {fileName} → {targetFolder}");
                }
            }

            AssetDatabase.Refresh();

            if (applyMoves)
            {
                _lastUndoBatch = undoBatch;
                SaveUndoBatchToFile(undoBatch);
                _statusMessage = $"Moved {moved} files! ({skipped} in place, {conflict} conflicts)\nYou can UNDO this operation.";
            }
            else
            {
                _statusMessage = $"[DRY RUN] {moved + skipped + conflict} simulated operations.\n{log}";
            }
        }

        // -------- UNDO --------
        private void SaveUndoBatchToFile(UndoMoveBatch batch)
        {
            try { File.WriteAllText(UndoLogPath, JsonUtility.ToJson(batch, true)); }
            catch (Exception e) { Debug.LogWarning("Could not save undo log: " + e); }
        }

        private UndoMoveBatch LoadUndoBatchFromFile()
        {
            if (!File.Exists(UndoLogPath)) return null;
            try { return JsonUtility.FromJson<UndoMoveBatch>(File.ReadAllText(UndoLogPath)); }
            catch { return null; }
        }

        private void UndoLastMoveBatch()
        {
            int undone = 0, failed = 0, conflicts = 0;
            var log = new StringBuilder();

            foreach (var move in _lastUndoBatch.Moves)
            {
                if (!File.Exists(move.Source)) { log.AppendLine($"[SKIP] {move.Source} missing."); continue; }
                if (File.Exists(move.Destination)) { log.AppendLine($"[CONFLICT] {move.Destination} exists."); conflicts++; continue; }

                var result = AssetDatabase.MoveAsset(move.Source, move.Destination);
                if (string.IsNullOrEmpty(result)) { log.AppendLine($"[UNDO] {Path.GetFileName(move.Source)}"); undone++; }
                else { log.AppendLine($"[ERROR] {move.Source}: {result}"); failed++; }
            }

            AssetDatabase.Refresh();
            try { File.Delete(UndoLogPath); } catch { /* ignore */ }
            _lastUndoBatch = null;

            _statusMessage = $"UNDO: {undone} restored • {conflicts} conflicts • {failed} errors.\n{log}";
            Repaint();
        }

        // -------- Utils --------
        private static Texture2D MakeTex(int w, int h, Color c)
        {
            var tex = new Texture2D(w, h);
            var pix = new Color[w*h];
            for (int i=0;i<pix.Length;i++) pix[i] = c;
            tex.SetPixels(pix); tex.Apply(); return tex;
        }
    }
}
