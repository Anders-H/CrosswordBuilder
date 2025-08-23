#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace CrosswordBuilder;

public class WordList : List<Word>
{
    public int GetLongestWordLength()
    {
        if (Count <= 0)
            return 0;

        return this.OrderByDescending(x => x.Content.Length).First().Content.Length;
    }
}