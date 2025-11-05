using System;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = UnityEngine.ILogger;

namespace VTFLogger
{
    public class Test : MonoBehaviour
    {
        private void Awake()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(logging => { logging.SetMinimumLevel(LogLevel.Trace); });
        }
    }
}