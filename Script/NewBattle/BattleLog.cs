using System.Collections;
using System.Collections.Generic;

namespace TestBattle
{
    public interface IBattleLog {
        void Log(string message);
        void LogError(string message);
    }
    public static class BattleLog
    {
        public static IBattleLog LogHandler = null;
        public static void Log(string message)
        {
            if (LogHandler != null) {
                LogHandler.Log(message);
            }
        }
        public static void LogError(string message)
        {
            if (LogHandler != null)
            {
                LogHandler.LogError(message);
            }
        }
    }
}
