using System;
using System.Collections.Generic;

namespace FluorineEditor
{
    public class WindowManager
    {
        public List<ConsoleWindow> windows = new List<ConsoleWindow>();
        public InputHandler input;

        public int CurrentWindow = 0;

        public WindowManager(ConsoleWindow window)
        {
            input = new InputHandler();
            AddWindow(window);
        }

        public void AddWindow(ConsoleWindow window)
        {
            window.manager = this;
            window.input = input;
            windows.Add(window);
        }

        public void SetCurrentWindow(string name)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].name == name)
                {
                    CurrentWindow = i;
                    windows[i].OnFocus();
                    break;
                }
            }
        }

        public void Start()
        {
            while (true)
            {
                List<ConsoleKeyInfo> keys = input.GetKeys();

                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i].Modifiers.HasFlag(ConsoleModifiers.Control) && keys[i].Key == ConsoleKey.R)
                    {
                        for (int ii = 0; ii < windows.Count; ii++)
                        {
                            windows[ii].ReSize();
                        }
                    }
                }



                windows[CurrentWindow].Clear();
                windows[CurrentWindow].OnUpdate(keys);
                windows[CurrentWindow].Draw();
                windows[CurrentWindow].DrawBuffer();

            }
        }
    }
}
