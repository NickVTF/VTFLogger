using UnityEngine;

namespace VTFLogger.Runtime
{
    using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VTFLogger.Runtime
{
    public class GUILogHandler : MonoBehaviour, ILogSink
    {
        private struct LogMessage
        {
            public string Message;
            public string StackTrace;
            public LogType Type;
        }

        [Header("GUI Settings")]
        [SerializeField] private KeyCode toggleKey = KeyCode.F1;
        [SerializeField] private int maxMessages = 100;

        private readonly List<LogMessage> _logMessages = new List<LogMessage>();
        private readonly Dictionary<LogType, bool> _logTypeFilters = new Dictionary<LogType, bool>
        {
            { LogType.Log, true },
            { LogType.Warning, true },
            { LogType.Error, true },
            { LogType.Exception, true },
            { LogType.Assert, true }
        };

        private Vector2 _scrollPosition;
        private bool _isVisible;
        private bool _autoScroll = true;

        public void Initialize()
        {
            Application.logMessageReceived += HandleLog;
        }

        public void Release()
        {
            Application.logMessageReceived -= HandleLog;
        }

        public void ToggleVisibility()
        {
            _isVisible = !_isVisible;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleVisibility();
            }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;

            DrawLogWindow();
        }
        
        private void HandleLog(string message, string stackTrace, LogType type)
        {
            _logMessages.Add(new LogMessage
            {
                Message = message,
                StackTrace = stackTrace,
                Type = type
            });

            if (_logMessages.Count > maxMessages)
            {
                _logMessages.RemoveAt(0);
            }

            if (_autoScroll)
            {
                _scrollPosition.y = float.MaxValue;
            }
        }
        
        private void DrawLogWindow()
        {
            var width = Screen.width > 1200 ? 1200 : Screen.width;
            var height = Screen.height > 760 ? 760 : Screen.height;
            
            // Main Window
            Rect windowRect = new Rect(Screen.width - width, Screen.height - height, width - 20, height - 20);
            GUI.Window(0, windowRect, id =>
            {
                // Toolbar for filtering
                GUILayout.BeginHorizontal();
                foreach (var logType in _logTypeFilters.Keys.ToList())
                {
                    _logTypeFilters[logType] = GUILayout.Toggle(_logTypeFilters[logType], logType.ToString());
                }
                _autoScroll = GUILayout.Toggle(_autoScroll, "Auto-scroll");
                if (GUILayout.Button("Clear")) _logMessages.Clear();
                if (GUILayout.Button("Close")) _isVisible = false;
                GUILayout.EndHorizontal();

                // Log messages area
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
                foreach (var logMessage in _logMessages)
                {
                    if (_logTypeFilters[logMessage.Type])
                    {
                        GUI.contentColor = GetColorForLogType(logMessage.Type);
                        GUILayout.Label($"[{logMessage.Type}] {logMessage.Message}");
                    }
                }
                GUI.contentColor = Color.white;
                GUILayout.EndScrollView();

                // Allow window dragging
                GUI.DragWindow(new Rect(0, 0, 10000, 20));

            }, "In-Game Log");
        }
        
        private static Color GetColorForLogType(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    return Color.red;
                case LogType.Warning:
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }
    }
}
}