#nullable enable
using System;

namespace CrosswordBuilder;

public class CrosswordCell
{
    public bool IsStartHorizontal { get; set; }
    public bool IsStartVertical { get; set; }
    public bool IsEnd { get; set; }
    public int CrosswordStartNumber { get; set; }
    public bool ExpandsHorizontal { get; set; }
    public bool ExpandsVertical { get; set; }
    public char Letter { get; set; }
    public bool HasWallUp { get; set; }
    public bool HasWallRight { get; set; }
    public bool HasWallDown { get; set; }
    public bool HasWallLeft { get; set; }
    
    public CrosswordCell(char letter, bool isStart, bool isHorizontal, bool isEnd) : this(0, false, false, letter, false, false, false, false)
    {
        IsEnd = isEnd;

        if (isHorizontal)
        {
            IsStartHorizontal = true;
            HasWallUp = true;
            HasWallRight = !isEnd;
            HasWallDown = true;
            HasWallLeft = isStart;
        }
        else
        {
            IsStartVertical = true;
            HasWallUp = isStart;
            HasWallRight = true;
            HasWallDown = !isEnd;
            HasWallLeft = true;
        }

        ExpandsHorizontal = isHorizontal;
        ExpandsVertical = !isHorizontal;
    }

    public CrosswordCell(int crosswordStartNumber, bool expandsHorizontal, bool expandsVertical, char letter, bool hasWallUp, bool hasWallRight, bool hasWallDown, bool hasWallLeft)
    {
        IsStartHorizontal = false;
        IsStartVertical = false;
        IsEnd = false;
        CrosswordStartNumber = crosswordStartNumber;
        ExpandsHorizontal = expandsHorizontal;
        ExpandsVertical = expandsVertical;
        Letter = letter;
        HasWallUp = hasWallUp;
        HasWallRight = hasWallRight;
        HasWallDown = hasWallDown;
        HasWallLeft = hasWallLeft;
    }

    public bool IsStart =>
        IsStartHorizontal || IsStartVertical;

    public bool IsHorizontal =>
        IsStartHorizontal || ExpandsHorizontal;

    public void CreateWalls()
    {
        if (IsHorizontal)
        {
            HasWallUp = true;
            HasWallDown = true;
            HasWallLeft = IsStart;
            HasWallRight = IsEnd;
        }
        else
        {
            HasWallUp = IsStart;
            HasWallDown = IsEnd;
            HasWallLeft = true;
            HasWallRight = true;
        }
    }

    public void Merge(CrosswordCell other)
    {
        if (Letter != other.Letter)
            throw new InvalidOperationException("Cannot merge cells with different letters.");

        IsStartHorizontal |= other.IsStartHorizontal;
        IsStartVertical |= other.IsStartVertical;
        IsEnd |= other.IsEnd;
        ExpandsHorizontal |= other.ExpandsHorizontal;
        ExpandsVertical |= other.ExpandsVertical;
        HasWallUp &= other.HasWallUp;
        HasWallRight &= other.HasWallRight;
        HasWallDown &= other.HasWallDown;
        HasWallLeft &= other.HasWallLeft;
    }

    public override string ToString() =>
        Letter.ToString();
}