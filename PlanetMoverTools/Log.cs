using VRage.Utils;

namespace PlanetMover
{
    public static class Log
    {
        const string Prefix = "PlanetMover";

        public static bool DebugLog;
        public static void Msg(string msg)
        {
            MyLog.Default.WriteLine($"{Prefix}: {msg}");
        }

        public static void Debug(string msg)
        {
            if (DebugLog)
                Msg($"[DEBUG] {msg}");
        }
    }
}
