using System;
using System.Collections.Generic;

namespace FluorineEditor
{
    public enum KeyState
    {
        KeyDown,
        KeyUp
    }

    public class InputHandler
    {
        public static int MAX_KEYS_PER_POLL = 5;

        public List<ConsoleKeyInfo> GetKeys()
        {
            List<ConsoleKeyInfo> keys = new List<ConsoleKeyInfo>();

            int k = 0;
            while (Console.KeyAvailable && k < MAX_KEYS_PER_POLL)
            {
                keys.Add(Console.ReadKey(true));
                k++;
            }

            return keys;
        }
    }
}
