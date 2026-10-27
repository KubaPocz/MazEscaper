using UnityEngine;

namespace UI
{
    public class UnityLogger:MazEscaper.Core.ILogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
