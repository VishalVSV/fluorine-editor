using System;

namespace FluorineEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            Editor.EditorWindow window = new Editor.EditorWindow(0, 0, Console.WindowWidth, Console.WindowHeight);
            window.LoadConfig("./config.txt");
      
            SplashScreen.SplashWindow splash = new SplashScreen.SplashWindow(0, 0, Console.WindowWidth, Console.WindowHeight);

            FileExplorer.FileExplorerWindow explorer = new FileExplorer.FileExplorerWindow("./", Console.WindowWidth, Console.WindowHeight, 0, 0);
            explorer.open_file_action = window.Open;

            WindowManager manager = new WindowManager(window);

            manager.AddWindow(explorer);
            manager.AddWindow(splash);

            manager.SetCurrentWindow("splash");

            manager.Start();
        }
    }
}
