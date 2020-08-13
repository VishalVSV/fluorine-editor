using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FluorineEditor.Editor
{
    public class EditorConfig
    {
        public string tab = "    ";

        public Dictionary<string, Colors> syntax_colors = new Dictionary<string, Colors>()
        {
            { "comments" , Colors.FG_BLUE },
            { "keywords" , Colors.FG_MAGENTA },
            { "identifiers" , Colors.FG_CYAN },
            { "default" , Colors.FG_DARK_GREY },
            { "string" , Colors.FG_YELLOW }
        };

        public List<string> single_line_comment_starts = new List<string>()
        {
            "//",
            "#"
        };

        public List<Tuple<string, string>> multi_line_comment_delimiters = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("/*","*/")
        };

        public List<string> keywords = new List<string>() {
            "import",
            "using",
            "int",
            "return",
        };

        private static bool isnum(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s[i]))
                    return false;
            }
            return true;
        }
    }

    public enum EntryType
    {
        Char,
        NewLine,
        Tab
    }

    public partial class EditorWindow : ConsoleWindow
    {
        public EditorWindow(int x, int y, int width, int height)
        {
            name = "editor";

            this.x = x;
            this.y = y;
            Width = width;
            Height = height;

            max_lines_per_screen = Height - 4;

            screen_buffer = new winapi.types.CHAR_INFO[Width * Height];
        }


        struct KeyEntry
        {
            public char c;
            public EntryType type;
            public Colors color;

            public KeyEntry(char _char, EntryType type)
            {
                color = Colors.FG_WHITE;
                c = _char;
                this.type = type;
            }
            public KeyEntry(EntryType type)
            {
                color = Colors.FG_WHITE;
                c = ' ';
                this.type = type;
            }
            public KeyEntry modify_color(Colors color)
            {
                this.color = color;
                return this;
            }
        }

        int index = 0;
        List<KeyEntry> _text = new List<KeyEntry>();
        List<KeyEntry> _original_text = new List<KeyEntry>();

        string current_file_name = "";

        private readonly KeyEntry newline = new KeyEntry(EntryType.NewLine);
        private readonly KeyEntry tab = new KeyEntry(EntryType.Tab);

        private EditorConfig config = new EditorConfig();
        private StringBuilder current_word = new StringBuilder();

        private int start_line = 0;
        private int current_line = 0;
        private int max_lines_per_screen;

        private int current_column = 0;

        private bool edited = false;

        public override void OnFocus()
        {
            Console.CursorVisible = true;
            RePositionCursor();
        }

        public override void Draw()
        {
            int x = 1, y = 2;
            int cline = 0;
            for (int i = 0; i < _text.Count; i++)
            {
                if (cline >= start_line && cline <= start_line + max_lines_per_screen + 1)
                {

                    if (x >= Width)
                    {
                        x = 1;
                        y++;
                        cline++;
                    }

                    if (_text[i].type == EntryType.Char)
                    {
                        SetPixel(x, y, _text[i].c, _text[i].color);
                        x++;
                    }
                    if (_text[i].type == EntryType.Tab)
                    {
                        for (int j = 0; j < config.tab.Length; j++)
                        {
                            SetPixel(x, y, config.tab[j]);
                            x++;
                        }
                    }
                    else if (_text[i].type == EntryType.NewLine)
                    {
                        x = 1;
                        y++;
                        cline++;
                    }

                }
                else
                {
                    if (_text[i].type == EntryType.NewLine)
                    {
                        cline++;
                    }
                }
            }

            for (int i = 0; i < Width; i++)
                SetPixel(i, Height - 1, '▓', Colors.BG_GREEN);

            for (int i = 0; i < Width; i++)
                SetPixel(i, 1, '▓', Colors.BG_GREEN);

            for (int i = 1; i < Height; i++)
                SetPixel(0, i, '▓', Colors.BG_GREEN);

            for (int i = 1; i < Height; i++)
                SetPixel(Width - 1, i, '▓', Colors.BG_GREEN);

            SetString(0, 0, $"File: {Path.GetFileName(current_file_name)}{(edited ? "*" : "")}");

            string line_col = $"{current_line}:{current_column}";

            SetString(Width - 1 - line_col.Length, Height - 1, line_col, (ushort)Colors.BG_GREEN);
        }

        public string GetContent()
        {
            StringBuilder res = new StringBuilder(_text.Count);
            for (int i = 0; i < _text.Count; i++)
            {
                if (_text[i].type == EntryType.Char)
                {
                    res.Append(_text[i].c);
                }
                else if (_text[i].type == EntryType.Tab)
                {
                    res.Append(config.tab);
                }
                else if (_text[i].type == EntryType.NewLine)
                {
                    res.Append(Environment.NewLine);
                }
            }
            return res.ToString();
        }

        public override void ReSize()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;

            max_lines_per_screen = Height - 4;

            screen_buffer = new winapi.types.CHAR_INFO[Width * Height];
        }

        public void ReSize(int width, int height)
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;

            max_lines_per_screen = Height - 4;

            screen_buffer = new winapi.types.CHAR_INFO[Width * Height];
        }

        DateTime tp = DateTime.Now;
        public override void OnUpdate(List<ConsoleKeyInfo> keys)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Modifiers.HasFlag(ConsoleModifiers.Control))
                {
                    if (keys[i].Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        if (keys[i].Key == ConsoleKey.T)
                        {
                            LoadConfig("./config.txt");
                            Console.Title = "Config reloaded!";
                            continue;
                        }
                    }

                    if (keys[i].Key == ConsoleKey.S)
                    {
                        Save();
                    }
                    else if (keys[i].Key == ConsoleKey.O)
                    {
                        manager.SetCurrentWindow("file_explorer");
                    }
                }
                else
                {
                    if (!char.IsControl(keys[i].KeyChar))
                    {
                        if (char.IsLetterOrDigit(keys[i].KeyChar))
                        {
                            current_word.Append(keys[i].KeyChar);
                        }
                        else
                        {
                            current_word.Clear();
                        }

                        _text.Insert(index, new KeyEntry(keys[i].KeyChar, EntryType.Char));
                        edited = true;
                        index++;
                    }
                    else if (keys[i].Key == ConsoleKey.Enter)
                    {
                        current_word.Clear();
                        _text.Insert(index, newline);
                        edited = true;
                        index++;
                    }
                    else if (keys[i].Key == ConsoleKey.Tab)
                    {
                        current_word.Clear();
                        _text.Insert(index, tab);
                        edited = true;
                        index++;
                    }
                    else if (keys[i].Key == ConsoleKey.Backspace)
                    {
                        if (_text.Count > 0)
                        {
                            if (index - 1 >= 0 && _text[index - 1].type == EntryType.Char && char.IsLetterOrDigit(_text[index - 1].c) && current_word.Length > 0)
                            {
                                current_word.Remove(current_word.Length - 1, 1);
                            }

                            _text.RemoveAt(index - 1);
                            edited = true;
                            index--;
                        }
                    }
                    else if (keys[i].Key == ConsoleKey.Delete)
                    {
                        if (_text.Count > 0)
                        {
                            if (index <= _text.Count)
                            {
                                edited = true;
                                _text.RemoveAt(index);
                            }
                        }
                    }
                    else if (keys[i].Key == ConsoleKey.RightArrow)
                    {
                        if (index + 1 <= _text.Count)
                            index++;
                    }
                    else if (keys[i].Key == ConsoleKey.LeftArrow)
                    {
                        if (index - 1 >= 0)
                            index--;
                    }
                    else if (keys[i].Key == ConsoleKey.DownArrow)
                    {
                        if (index + 1 <= _text.Count)
                            index++;
                        while (index + 1 < _text.Count)
                        {
                            if (_text[index].type == EntryType.NewLine)
                                break;
                            index++;
                        }
                    }
                    else if (keys[i].Key == ConsoleKey.UpArrow)
                    {
                        if (index - 1 >= 0)
                            index--;
                        while (index - 1 >= 0)
                        {
                            if (_text[index].type == EntryType.NewLine)
                                break;
                            index--;
                        }
                    }

                }
                

                UpdateColumnAndLine();
                if (current_line - start_line > max_lines_per_screen)
                    start_line++;
                if (current_line - start_line < 0)
                    start_line--;

                DoSyntaxHighlighting();
                RePositionCursor();
            }

            if ((DateTime.Now - tp).TotalSeconds > 5)
            {
                tp = DateTime.Now;
                if (_text.Count == _original_text.Count)
                {
                    for (int ii = 0; ii < _text.Count; ii++)
                    {
                        if (_text[ii].type != _original_text[ii].type || _text[ii].c != _original_text[ii].c)
                        {
                            edited = true;
                            break;
                        }
                    }
                    edited = false;
                }
                else
                {
                    edited = true;
                }
            }
        }

        private void UpdateColumnAndLine()
        {
            current_line = 0;
            current_column = 0;
            for (int i = 0; i < index; i++)
            {
                if (i >= _text.Count)
                    break;
                if (current_column >= Width)
                {
                    current_line++;
                    current_column = 0;
                }

                current_column += EditorHelperUtils.GetNumberOfChars(_text[i].type, config);
                if (_text[i].type == EntryType.NewLine)
                {
                    current_line++;
                    current_column = 0;
                }
            }
        }

        private void RePositionCursor()
        {
            int x = this.x + 1, y = this.y + 2;
            for (int i = 0; i < index; i++)
            {
                if (i >= _text.Count)
                    break;
                if (x - this.x >= Width)
                {
                    y++;
                    x = this.x + 1;
                }
                if (_text[i].type == EntryType.NewLine)
                {
                    y++;
                    x = this.x + 1;
                }
                else if (_text[i].type == EntryType.Char)
                {
                    x++;
                }
                else if (_text[i].type == EntryType.Tab)
                {
                    for (int j = 0; j < config.tab.Length; j++)
                        x++;
                }
            }

            y -= start_line;

            if (x >= this.x + 1 && x <= this.x + Width - 1 && y >= this.y + 2 && y <= this.y + Height - 2)
                Console.SetCursorPosition(x, y);
        }
    }
}
