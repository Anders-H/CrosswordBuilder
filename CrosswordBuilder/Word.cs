#nullable enable
namespace CrosswordBuilder;

public class Word
{
    private string _content;

    public Word(string content)
    {
        _content = "";
        Content = content;
    }

    public string Content
    {
        get => _content;
        set => _content = string.IsNullOrWhiteSpace(value) ? "X" : value.Trim().ToUpperInvariant();
    }

    public bool MatchAtPos(int pos, char letter)
    {
        if (letter == ' ')
            return true;

        return _content[pos] == letter;
    }

    public override string ToString() =>
        Content;
}