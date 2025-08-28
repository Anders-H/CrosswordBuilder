#nullable enable
using System;
using System.Linq;

namespace CrosswordBuilder;

public class Crossword
{
    private CrosswordCell?[,]? _cells;
    public int GridSize { get; private set; }
    public WordList Words { get; }

    public Crossword()
    {
        GridSize = 5;
        Words = [];
    }

    public void SortWords(WordSortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case WordSortOrder.Alphabetical:
            {
                var sortedList = Words.OrderBy(w => w.Content).ToList();
                Words.Clear();
                Words.AddRange(sortedList);
                break;
            }
            case WordSortOrder.Length:
            {
                var sortedList = Words.OrderByDescending(w => w.Content.Length).ToList();
                Words.Clear();
                Words.AddRange(sortedList);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
        }
    }

    public bool Build(int gridSize, out string failedWord)
    {
        failedWord = "";
        GridSize = gridSize;

        if (Words.Count <= 1)
            return false;

        _cells = new CrosswordCell[GridSize, GridSize];
        var startRow = GridSize / 2;
        var startCol = (GridSize - Words[0].Content.Length) / 2;
        PlaceWord(Words[0], startRow, startCol);

        for (var i = 1; i < Words.Count; i++)
        {
            var wordToPlace = Words[i];

            if (!PlaceWordSomewhere(wordToPlace))
            {
                failedWord = wordToPlace.Content;
                return false;
            }
        }
        
        return true;
    }

    public CrosswordCell? GetCell(int row, int col)
    {
        if (_cells == null)
            return null;

        if (row < 0 || col < 0 || row >= GridSize || col >= GridSize)
            return null;

        return _cells[row, col];
    }

    private void PlaceWord(Word word, int row, int col)
    {
        for (var i = 0; i < word.Content.Length; i++)
            SetCell(row, col + i, word.Content[i], true, i == 0, i == word.Content.Length - 1);
    }

    private bool PlaceWordSomewhere(Word word)
    {
        if (_cells == null)
            return false;

        for (var row = 0; row < GridSize; row++)
        {
            for (var col = 0; col < GridSize; col++)
            {
                var cell = _cells[row, col];

                if (cell == null)
                    continue;

                if (CanPlaceWordAt(word, row, col, false))
                {
                    PlaceWordAt(word, row, col, false);
                    return true;
                }

                if (CanPlaceWordAt(word, row, col, true))
                {
                    PlaceWordAt(word, row, col, true);
                    return true;
                }
            }
        }

        return false;
    }

    private bool CanPlaceWordAt(Word word, int row, int col, bool horizontal)
    {
        if (_cells == null)
            return false;

        for (var i = 0; i < word.Content.Length; i++)
        {
            var rr = horizontal ? row : row + i;
            var cc = horizontal ? col + i : col;
            
            if (rr < 0 || cc < 0 || rr >= GridSize || cc >= GridSize)
                return false;
            
            var cell = _cells[rr, cc];

            if (cell != null && !word.MatchAtPos(i, cell.Letter))
                return false;
        }

        return true;
    }

    private void PlaceWordAt(Word word, int row, int col, bool horizontal)
    {
        for (var i = 0; i < word.Content.Length; i++)
        {
            var character = word.Content[i];
            var isFirst = i == 0;
            var isLast = i == word.Content.Length - 1;

            if (horizontal)
                SetCell(row, col + i, character, horizontal, isFirst, isLast);
            else
                SetCell(row + i, col, character, horizontal, isFirst, isLast);
        }
    }

    private void SetCell(int row, int col, char character, bool isHorizontal, bool isFirst, bool isLast)
    {
        if (_cells == null)
            return;

        var cell = new CrosswordCell(character, isFirst, isHorizontal, isLast);
        cell.CreateWalls();

        if (_cells[row, col] == null)
        {
            _cells[row, col] = cell;
        }
        else
        {
            _cells[row, col]!.Merge(cell);
        }
    }
}