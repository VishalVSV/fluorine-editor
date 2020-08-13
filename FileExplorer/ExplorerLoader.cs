using System;
using System.IO;

namespace FluorineEditor.FileExplorer
{
    public partial class FileExplorerWindow
    {

        private void Load()
        {
            entries.Clear();

            string[] entry_paths;

            entries.Add(new ExplorerEntry(ExplorerEntryType.UpDir,"..",Directory.GetParent(directory).FullName));

            entry_paths = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < entry_paths.Length; i++)
            {
                if (Directory.Exists(entry_paths[i]))
                {
                    entries.Add(new ExplorerEntry(ExplorerEntryType.Folder, "# "+ Path.GetFileName(entry_paths[i]), entry_paths[i], Colors.FG_DARK_CYAN));
                }
            }

            entry_paths = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < entry_paths.Length; i++)
            {
                if (File.Exists(entry_paths[i]))
                {
                    entries.Add(new ExplorerEntry(ExplorerEntryType.File, Path.GetFileName(entry_paths[i]), entry_paths[i],Colors.FG_MAGENTA));
                }
            }

            Console.Title = Path.GetDirectoryName(directory + "/");
            SelectedIndex = 0;
            start_index = 0;
        }
    }
}
