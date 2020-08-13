using System;
using System.Collections.Generic;

namespace FluorineEditor.SplashScreen
{
    public class SplashWindow : ConsoleWindow
    {
        private Tuple<string, Action>[] actions;
        private int index = 0;

        public SplashWindow(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            Width = width;
            Height = height;

            name = "splash";

            screen_buffer = new winapi.types.CHAR_INFO[Width * Height];

            actions = new Tuple<string, Action>[]
            {
                new Tuple<string, Action>("Open File", OpenFile),
                new Tuple<string, Action>("New File", OpenFile),
            };

        }

        public override void Draw()
        {
            for (int i = 0; i < Width; i++)
                SetPixel(i, Height - 1, '▓', Colors.BG_GREEN);

            for (int i = 0; i < Width; i++)
                SetPixel(i, 1, '▓', Colors.BG_GREEN);

            for (int i = 1; i < Height; i++)
                SetPixel(0, i, '▓', Colors.BG_GREEN);

            for (int i = 1; i < Height; i++)
                SetPixel(Width - 1, i, '▓', Colors.BG_GREEN);

            int mid_y = Height / 2;
            int mid_x = Width / 2;

            for (int i = 0; i < actions.Length; i++)
            {
                string str = i == index ? $"==={actions[i].Item1}===" : actions[i].Item1;
                SetString((int)Math.Round(mid_x - (str.Length / 2.0)), mid_y - 4 + i, str, (ushort)(i == index ? Colors.FG_YELLOW : Colors.FG_WHITE));
            }
        }

        public override void OnFocus()
        {

        }

        public override void OnUpdate(List<ConsoleKeyInfo> keys)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                        index--;
                }
                else if (keys[i].Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < actions.Length)
                        index++;
                }
                else if(keys[i].Key == ConsoleKey.Enter)
                {
                    actions[index].Item2?.Invoke();
                }
            }
        }

        public override void ReSize()
        {
            x = 0;
            y = 0;
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;

            screen_buffer = new winapi.types.CHAR_INFO[Width * Height];
        }

        private void OpenFile()
        {
            manager.SetCurrentWindow("file_explorer");
        }
    }
}
