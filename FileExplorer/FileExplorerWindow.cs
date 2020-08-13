using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FluorineEditor.FileExplorer
{
    public partial class FileExplorerWindow : ConsoleWindow
    {
        private string directory;
        private int SelectedIndex = 0;

        public Action<string> open_file_action;

        private int start_index = 0;

        private List<ExplorerEntry> entries = new List<ExplorerEntry>();

        enum ExplorerEntryType
        {
            File,
            UpDir,
            Folder
        }

        struct ExplorerEntry
        {
            public ExplorerEntryType type;
            public string path;
            public string DisplayString;
            public Colors color;

            public ExplorerEntry(ExplorerEntryType type, string display, string path = "", Colors color = Colors.FG_WHITE)
            {
                this.color = color;
                DisplayString = display;
                this.type = type;
                this.path = path;
            }
        }

        public FileExplorerWindow(string dir, int width, int height, int x, int y)
        {
            name = "file_explorer";

            this.x = x;
            this.y = y;
            Width = width;
            Height = height;

            screen_buffer = new winapi.types.CHAR_INFO[Width * Height];

            directory = dir;

            Load();
        }

        public override void OnFocus()
        {
            Console.CursorVisible = false;
            Console.Title = Path.GetDirectoryName(directory);
            Console.SetCursorPosition(1, 2);
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

            int x = 1, y = 2;
            for (int i = start_index; i < entries.Count; i++)
            {
                SetString(x, y, i != SelectedIndex ? entries[i].DisplayString : " >" + entries[i].DisplayString, (ushort)(i == SelectedIndex ? Colors.FG_YELLOW : entries[i].color));
                x = 1;
                y++;
                if (y >= Height - 1)
                    break;
            }

            if (entering_new_filename)
            {
                SetString(1, start_index == 0 ? entries.Count + 2 : Height - 2, "> "+filename.ToString());
            }
        }

        public override void ReSize()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;

            screen_buffer = new winapi.types.CHAR_INFO[Width * Height];
        }
        StringBuilder filename = new StringBuilder();
        bool entering_new_filename = false;

        public override void OnUpdate(List<ConsoleKeyInfo> keys)
        {

            for (int k = 0; k < keys.Count; k++)
            {
                if (entering_new_filename)
                {
                    if (!char.IsControl(keys[k].KeyChar))
                    {
                        filename.Append(keys[k].KeyChar);
                    }
                    else if (keys[k].Key == ConsoleKey.Backspace)
                    {
                        if (filename.Length > 0)
                            filename.Remove(filename.Length - 1, 1);
                    }
                    else if (keys[k].Key == ConsoleKey.Enter)
                    {
                        if (!File.Exists(directory + "/" + filename.ToString()))
                        {
                            entering_new_filename = false;
                            File.Create(directory + "/" + filename.ToString()).Close();
                            entries.Add(new ExplorerEntry(ExplorerEntryType.File, filename.ToString(), Path.GetFullPath(directory + "/" + filename.ToString()), Colors.FG_MAGENTA));
                            filename.Clear();
                            SelectedIndex = entries.Count - 1;
                        }
                        else
                        {
                            Console.Title = "File already exists";
                        }
                    }

                    continue;
                }

                if (keys[k].Modifiers.HasFlag(ConsoleModifiers.Control) && keys[k].Key == ConsoleKey.N)
                {
                    if (entries.Count > Height - 3)
                        start_index = entries.Count - Height + 3 + 1;
                    Draw();
                    DrawBuffer();

                    entering_new_filename = true;
                    continue;
                }

                if (keys[k].Key == ConsoleKey.UpArrow)
                {
                    if (SelectedIndex > 0)
                        SelectedIndex--;
                }
                else if (keys[k].Key == ConsoleKey.DownArrow)
                {
                    if (SelectedIndex + 1 < entries.Count)
                        SelectedIndex++;
                }
                else if (keys[k].Key == ConsoleKey.Enter)
                {
                    if (entries[SelectedIndex].type == ExplorerEntryType.File)
                    {
                        open_file_action?.Invoke(entries[SelectedIndex].path);
                        manager.SetCurrentWindow("editor");
                    }
                    else if (entries[SelectedIndex].type == ExplorerEntryType.Folder)
                    {
                        directory = entries[SelectedIndex].path;
                        Load();
                    }
                    else if (entries[SelectedIndex].type == ExplorerEntryType.UpDir)
                    {
                        directory = (new DirectoryInfo(directory)).Parent.FullName;
                        Load();
                    }
                }

                if (SelectedIndex - start_index >= Height - 3)
                {
                    start_index++;
                }
                if (SelectedIndex - start_index < 0)
                    start_index--;
            }
        }
    }
}
