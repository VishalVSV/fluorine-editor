using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FluorineEditor.Editor
{
    public partial class EditorWindow
    {
        private bool opened_file = false;

        public void Save()
        {
            edited = false;
            bool stop = false;
            StringBuilder filename = new StringBuilder();
            if (!opened_file)
            {
                while (true)
                {
                    List<ConsoleKeyInfo> keys = input.GetKeys();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (keys[i].Key == ConsoleKey.Enter)
                        {
                            stop = true;
                            break;
                        }
                        else if (!char.IsControl(keys[i].KeyChar))
                        {
                            filename.Append(keys[i].KeyChar);
                        }
                        else if (keys[i].Key == ConsoleKey.Backspace)
                        {
                            if (filename.Length > 0)
                                filename.Remove(filename.Length - 1, 1);
                        }
                    }

                    Clear();
                    SetString(Width - 1 - filename.Length, 0, filename.ToString());
                    Draw();
                    DrawBuffer();
                    if (stop)
                        break;
                }

                current_file_name = filename.ToString();
            }

            using (StreamWriter sw = new StreamWriter(current_file_name))
            {
                sw.Write(GetContent());
            }
            Console.Title = "Saved!";
        }

        public void Open(string path)
        {
            current_file_name = path;
            opened_file = true;

            string text;
            using (StreamReader sr = new StreamReader(path))
            {
                text = sr.ReadToEnd();
            }
            LoadString(text);
        }

        public void LoadString(string str)
        {
            List<KeyEntry> new_text = new List<KeyEntry>();

            int i = 0;
            while (i < str.Length)
            {
                if (i + Environment.NewLine.Length < str.Length && str.Substring(i, Environment.NewLine.Length) == Environment.NewLine)
                {
                    new_text.Add(newline);
                    i += Environment.NewLine.Length;
                }
                else if (i + config.tab.Length < str.Length && str.Substring(i, config.tab.Length) == config.tab)
                {
                    new_text.Add(tab);
                    i += config.tab.Length;
                }
                else if (!char.IsControl(str[i]))
                {
                    new_text.Add(new KeyEntry(str[i], EntryType.Char));
                    i++;
                }
                else
                {
                    i++;
                }
            }

            _text = new_text;
            _original_text.Clear();
            _original_text.AddRange(_text);
            index = 0;

            UpdateColumnAndLine();
            if (current_line > max_lines_per_screen)
                start_line = current_line - max_lines_per_screen;
            else
                start_line = 0;
            DoSyntaxHighlighting();
            RePositionCursor();
        }
    }
}
