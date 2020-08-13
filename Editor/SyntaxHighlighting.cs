namespace FluorineEditor.Editor
{
    public partial class EditorWindow
    {
        private void DoSyntaxHighlighting()
        {
            for (int i = 0; i < _text.Count; i++)
            {
                _text[i] = _text[i].modify_color(Colors.FG_WHITE);
                //================================Strings===============================
                if (i < _text.Count && _text[i].type == EntryType.Char && _text[i].c == '"')
                {
                    _text[i] = _text[i].modify_color(config.syntax_colors["string"]);
                    i++;
                    while (i < _text.Count)
                    {
                        _text[i] = _text[i].modify_color(config.syntax_colors["string"]);
                        if (_text[i].type == EntryType.Char && _text[i].c == '"')
                        {
                            i++;
                            break;
                        }
                        i++;
                    }
                }

                //================================Comments==============================
                //SINGLE LINE
                for (int csi = 0; csi < config.single_line_comment_starts.Count; csi++)
                {
                    if (SubstrEqual(i, config.single_line_comment_starts[csi]))
                    {
                        while (i < _text.Count && _text[i].type != EntryType.NewLine)
                        {
                            _text[i] = _text[i].modify_color(config.syntax_colors["comments"]);
                            i++;
                        }
                    }
                }
                //MULTI LINE
                for (int csi = 0; csi < config.multi_line_comment_delimiters.Count; csi++)
                {
                    if (SubstrEqual(i, config.multi_line_comment_delimiters[csi].Item1))
                    {
                        while (i < _text.Count && !SubstrEqual(i, config.multi_line_comment_delimiters[csi].Item2))
                        {
                            _text[i] = _text[i].modify_color(config.syntax_colors["comments"]);
                            i++;
                        }
                        for (int _ = 0; _ < config.multi_line_comment_delimiters[csi].Item2.Length; _++)
                        {
                            if (i < _text.Count && i >= 0)
                            {
                                _text[i] = _text[i].modify_color(config.syntax_colors["comments"]);
                                i++;
                            }
                            else
                                break;
                        }
                    }
                }
                //================================Comments==============================

                //================================KEYWORDS==============================
                for (int ki = 0; ki < config.keywords.Count; ki++)
                {
                    if (SubstrEqual(i, config.keywords[ki]))
                    {
                        bool isKeyword = true;
                        if (i + config.keywords[ki].Length < _text.Count && _text[i + config.keywords[ki].Length].type == EntryType.Char && (char.IsLetterOrDigit(_text[i + config.keywords[ki].Length].c) || _text[i + config.keywords[ki].Length].c == '_'))
                        {
                            char cc = _text[i + config.keywords[ki].Length].c;
                            bool isletter = char.IsLetterOrDigit(cc);
                            isKeyword = false;
                        }
                        if (isKeyword)
                        {
                            for (int _ = 0; _ < config.keywords[ki].Length; _++)
                            {
                                _text[i] = _text[i].modify_color(config.syntax_colors["keywords"]);
                                i++;
                            }
                        }
                    }
                }
                //================================KEYWORDS==============================

                //================================IDENTIFIERS===========================
                while (i < _text.Count && _text[i].type == EntryType.Char && char.IsLetterOrDigit(_text[i].c))
                {
                    _text[i] = _text[i].modify_color(config.syntax_colors["identifiers"]);
                    i++;
                }
                //================================IDENTIFIERS===========================

                if (i < _text.Count)
                    _text[i] = _text[i].modify_color(config.syntax_colors["default"]);
            }
        }

        private bool SubstrEqual(int i, string c)
        {
            for (int i0 = i; i0 < i + c.Length; i0++)
            {
                if (i0 >= 0 && i0 < _text.Count)
                {
                    if (_text[i0].type == EntryType.Char)
                    {
                        if (_text[i0].c != c[i0 - i])
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }

            return true;
        }
    }
}
