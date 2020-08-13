using FluorineEditor.winapi.types;
using System;
using System.Runtime.InteropServices;

namespace FluorineEditor
{
    public enum Colors : ushort
    {
        FG_BLACK = 0x0000,
        FG_DARK_BLUE = 0x0001,
        FG_DARK_GREEN = 0x0002,
        FG_DARK_CYAN = 0x0003,
        FG_DARK_RED = 0x0004,
        FG_DARK_MAGENTA = 0x0005,
        FG_DARK_YELLOW = 0x0006,
        FG_GREY = 0x0007,
        FG_DARK_GREY = 0x0008,
        FG_BLUE = 0x0009,
        FG_GREEN = 0x000A,
        FG_CYAN = 0x000B,
        FG_RED = 0x000C,
        FG_MAGENTA = 0x000D,
        FG_YELLOW = 0x000E,
        FG_WHITE = 0x000F,
        BG_BLACK = 0x0000,
        BG_DARK_BLUE = 0x0010,
        BG_DARK_GREEN = 0x0020,
        BG_DARK_CYAN = 0x0030,
        BG_DARK_RED = 0x0040,
        BG_DARK_MAGENTA = 0x0050,
        BG_DARK_YELLOW = 0x0060,
        BG_GREY = 0x0070,
        BG_DARK_GREY = 0x0080,
        BG_BLUE = 0x0090,
        BG_GREEN = 0x00A0,
        BG_CYAN = 0x00B0,
        BG_RED = 0x00C0,
        BG_MAGENTA = 0x00D0,
        BG_YELLOW = 0x00E0,
        BG_WHITE = 0x00F0,
    }

    public static class AdvancedConsole
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool WriteConsoleOutputW(IntPtr hConsoleOut, CHAR_INFO[] lpBuffer, COORD bufferSize, COORD buferCoord, ref SMALL_RECT writeRegion);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int handle);

        private static CHAR_INFO[] screen_buffer;
        public static int Width, Height;

        private static IntPtr handle;

        static AdvancedConsole()
        {
            handle = GetStdHandle((int)CONSTANTS.STD_OUTPUT_HANDLE);
            ReSize();
        }

        public static void SetPixel(int x, int y, char c, Colors color = Colors.FG_WHITE)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                int i = y * Width + x;
                screen_buffer[i].modify(c, color);
            }
        }

        public static void SetColor(int x, int y, Colors color)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                int i = y * Width + x;
                screen_buffer[i].modify_attr(color);
            }
        }

        public static void Draw()
        {
            COORD bufferSize = new COORD();
            bufferSize.x = (short)Width;
            bufferSize.y = (short)Height;

            COORD bufferCoord = new COORD();
            bufferCoord.x = 0;
            bufferCoord.y = 0;

            SMALL_RECT lpWriteRegion = new SMALL_RECT();
            lpWriteRegion.Left = 0;
            lpWriteRegion.Top = 0;
            lpWriteRegion.Bottom = (short)(Height);
            lpWriteRegion.Right = (short)(Width);

            WriteConsoleOutputW(handle, screen_buffer, bufferSize, bufferCoord, ref lpWriteRegion);
        }

        public static void Draw(CHAR_INFO[] buffer, int width, int height, int x = 0, int y = 0)
        {
            COORD bufferSize = new COORD();
            bufferSize.x = (short)width;
            bufferSize.y = (short)height;

            COORD bufferCoord = new COORD();
            bufferCoord.x = 0;
            bufferCoord.y = 0;

            SMALL_RECT lpWriteRegion = new SMALL_RECT();
            lpWriteRegion.Left = (short)x;
            lpWriteRegion.Top = (short)y;
            lpWriteRegion.Bottom = (short)(y + height);
            lpWriteRegion.Right = (short)(x + width);

            WriteConsoleOutputW(handle, buffer, bufferSize, bufferCoord, ref lpWriteRegion);
        }

        public static void ReSize()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;

            screen_buffer = new CHAR_INFO[Width * Height];

            Clear();
        }

        public static void Clear()
        {
            for (int i = 0; i < screen_buffer.Length; i++)
                screen_buffer[i].modify(' ', Colors.FG_WHITE);
        }
    }
}
