using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VTFLogger.Runtime.VTFLogger.Runtime;

namespace VTFLogger.Runtime
{
    public static class LogManager
    {
        private static FileLogHandler _fileLogHandler;
        
        private static readonly List<ILogSink> _logSinks = new List<ILogSink>();
        private static GUILogHandler _guiLogHandler;
        private static GameObject _guiLogHandlerGameObject;
        
        /// <summary>
        /// Initializes the core logging system (e.g., file logging).
        /// This is called automatically before the first scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            // By default, only the file logger is created.
            var fileLogHandler = new FileLogHandler();
            _logSinks.Add(fileLogHandler);
            
            foreach (var sink in _logSinks)
            {
                sink.Initialize();
            }
        }

        /// <summary>
        /// Enables or disables the in-game GUI log viewer at runtime.
        /// </summary>
        /// <param name="isEnabled">True to create and show the viewer, false to destroy it.</param>
        public static void SetInGameLogViewerEnabled(bool isEnabled)
        {
            bool isCurrentlyEnabled = _guiLogHandler != null;

            if (isEnabled == isCurrentlyEnabled)
            {
                return; // No change needed.
            }

            if (isEnabled)
            {
                // Create a new GameObject to host the GuiLogHandler component.
                _guiLogHandlerGameObject = new GameObject("GuiLogHandler");
                Object.DontDestroyOnLoad(_guiLogHandlerGameObject);
                
                _guiLogHandler = _guiLogHandlerGameObject.AddComponent<GUILogHandler>();
                _logSinks.Add(_guiLogHandler);
                _guiLogHandler.Initialize();
            }
            else
            {
                // Clean up and destroy the GUI viewer.
                if (_guiLogHandler != null)
                {
                    _guiLogHandler.Release();
                    _logSinks.Remove(_guiLogHandler);
                    _guiLogHandler = null;
                }
                if (_guiLogHandlerGameObject != null)
                {
                    Object.Destroy(_guiLogHandlerGameObject);
                    _guiLogHandlerGameObject = null;
                }
            }
        }

        /// <summary>
        /// Toggles the visibility of the GUI log viewer if it exists.
        /// </summary>
        public static void ToggleGuiLogViewer()
        {
            _guiLogHandler?.ToggleVisibility();
        }

        /// <summary>
        /// Releases all logging resources.
        /// </summary>
        public static void Release()
        {
            foreach (var sink in _logSinks)
            {
                sink.Release();
            }
            _logSinks.Clear();
            
            // Ensure the GUI GameObject is destroyed if it still exists.
            if (_guiLogHandlerGameObject != null)
            {
                Object.Destroy(_guiLogHandlerGameObject);
            }
        }
    
        
    }
}