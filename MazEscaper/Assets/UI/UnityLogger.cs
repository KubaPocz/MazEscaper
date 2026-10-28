using UnityEngine;
using ILogger = Core.ILogger;

namespace UI
{
    public class UnityLogger:ILogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
