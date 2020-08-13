using FluorineEditor.winapi.types;
using System;
using System.Collections.Generic;

namespace FluorineEditor
{
    public abstract class ConsoleWindow
    {
        public int x, y;
        public int Width, Height;
        public CHAR_INFO[] screen_buffer;

        public InputHandler input;
        public WindowManager manager;
        public string name = "";

        public void SetPixel(int x, int y, char c, Colors color = Colors.FG_WHITE)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                int i = y * Width + x;
                screen_buffer[i].modify(c, color);
            }
        }

        public void SetColor(int x, int y, Colors color)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                int i = y * Width + x;
                screen_buffer[i].modify_attr(color);
            }
        }

        public void SetString(int x, int y, string str, ushort attr = (ushort)Colors.FG_WHITE)
        {
            for (int x0 = x; x0 < x + str.Length; x0++)
                SetPixel(x0, y, str[x0 - x], attr);
        }

        private void SetPixel(int x, int y, char c, ushort attr)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                int i = y * Width + x;
                screen_buffer[i].modify(c, attr);
            }
        }

        public void DrawBuffer()
        {
            AdvancedConsole.Draw(screen_buffer, Width, Height, x, y);
        }

        public void Clear()
        {
            for (int i = 0; i < screen_buffer.Length; i++)
            {
                screen_buffer[i].modify(' ', Colors.FG_WHITE);
            }
        }


        public abstract void OnFocus();
        public abstract void Draw();
        public abstract void ReSize();
        public abstract void OnUpdate(List<ConsoleKeyInfo> keys);
    }
}
