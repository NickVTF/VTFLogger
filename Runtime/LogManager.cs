using System.IO;
using UnityEngine;

namespace VTFLogger.Runtime
{
    public static class LogManager
    {
        private static FileLogHandler _fileLogHandler;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            _fileLogHandler = new FileLogHandler();
            _fileLogHandler.Initialize();
        }

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Release()
        {
            _fileLogHandler.Release();
        }
        
    }
}