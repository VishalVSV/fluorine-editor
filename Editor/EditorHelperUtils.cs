namespace FluorineEditor.Editor
{
    public class EditorHelperUtils
    {
        public static int GetNumberOfChars(EntryType type,EditorConfig config)
        {
            switch (type)
            {
                case EntryType.Char:
                    return 1;
                case EntryType.NewLine:
                    return 0;
                case EntryType.Tab:
                    return config.tab.Length;
                default:
                    return -1;
            }
        }
    }
}
