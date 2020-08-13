using System.Runtime.InteropServices;

namespace FluorineEditor.winapi.types
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CHAR_INFO
    {
        [FieldOffset(0)]
        public char unicodechar;
        [FieldOffset(0)]
        public char asciichar;
        [FieldOffset(2)]
        public ushort attributes;

        public CHAR_INFO(char c, ushort attr)
        {
            unicodechar = ' ';
            asciichar = ' ';
            if (c < 128)
                asciichar = c;
            else
                unicodechar = c;

            attributes = attr;
        }

        public void modify_char(char c)
        {
            if (c < 128)
                asciichar = c;
            else
                unicodechar = c;
        }

        public void modify_attr(ushort attr)
        {
            attributes = attr;
        }

        public void modify_attr(Colors attr)
        {
            attributes = (ushort)attr;
        }

        public void modify(char c, ushort attr)
        {
            modify_char(c);
            modify_attr(attr);
        }

        public void modify(char c, Colors attr)
        {
            modify_char(c);
            modify_attr(attr);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct COORD
    {
        public short x;
        public short y;
    }
    
[StructLayout(LayoutKind.Sequential)]
    public struct SMALL_RECT
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    public enum CONSTANTS : int
    {
        STD_INPUT_HANDLE = -10,
        STD_OUTPUT_HANDLE = -11
    }
}
