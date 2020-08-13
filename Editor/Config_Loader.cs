using System;
using System.IO;

namespace FluorineEditor.Editor
{
    public partial class EditorWindow
    {

        public void LoadConfig(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    LoadRawConfig(sr.ReadToEnd());
                }
            }
            else
            {
                File.Create(path).Close();
            }
        }

        private void LoadRawConfig(string str_config)
        {

            string[] lines = str_config.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tag = "";
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().StartsWith("[") && lines[i].Trim().EndsWith("]"))
                {
                    tag = lines[i].Trim().Trim('[', ']');
                    continue;
                }

                if (tag == "keywords")
                {
                    if (!config.keywords.Contains(lines[i].Trim()))
                        config.keywords.Add(lines[i].Trim());
                }
                else if (tag == "single_line_comments")
                {
                    if (!config.single_line_comment_starts.Contains(lines[i].Trim()))
                        config.single_line_comment_starts.Add(lines[i].Trim());
                }
                else if (tag == "multi_line_comments")
                {
                    string start = lines[i].Trim().Split(',')[0].Trim();
                    string end = lines[i].Trim().Split(',')[1].Trim();

                    Tuple<string, string> tuple = new Tuple<string, string>(start, end);
                    if (!config.multi_line_comment_delimiters.Contains(tuple))
                        config.multi_line_comment_delimiters.Add(tuple);
                }
                else if (tag == "syntax_colors")
                {
                    string syntax_elem = lines[i].Trim().Split('=')[0].Trim();
                    Colors color = (Colors)Enum.Parse(typeof(Colors), lines[i].Trim().Split('=')[1].Trim(), true);

                    if (!config.syntax_colors.ContainsKey(syntax_elem))
                        config.syntax_colors.Add(syntax_elem, color);
                    else
                        config.syntax_colors[syntax_elem] = color;
                }
            }
        }
    }
}
