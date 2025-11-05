using System;
using System.IO;
using UnityEngine;

namespace VTFLogger.Runtime
{
    public interface ILogSink
    {
        void Initialize();
        void Release();
    }
    
    public class FileLogHandler : ILogSink
    {
        private StreamWriter writer;
        private string logFilePath;
        

      
        public void Initialize()
        {
            string logDir = Path.Combine(Application.persistentDataPath, "logs");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            logFilePath = Path.Combine(logDir, $"{timestamp}.log");

            Application.logMessageReceived += Log;
            writer = new StreamWriter(logFilePath, append: false);
            writer.AutoFlush = true;

            writer.WriteLine($"=== Log session started: {DateTime.Now} ===");
        }

        

        public void Release()
        {
            Application.logMessageReceived -= Log;
            writer?.Close();
        }
        
        private void Log(string message, string stackTrace, LogType type)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] [{type}] {message}";
            writer.WriteLine(line);
        }
    }
}