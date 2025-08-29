#nullable enable
using System.Drawing;
using System.Windows.Forms;

namespace CrosswordBuilder;

public partial class MainWindow : Form
{
    private bool _documentChanged;
    private string _filename;
    private readonly Crossword _document;

    public MainWindow()
    {
        _filename = "";
        _document = new Crossword();
        InitializeComponent();
    }

    private void MainWindow_Load(object sender, System.EventArgs e)
    {
        printPreviewControl1.AutoZoom = true;
    }

    private void MainWindow_Shown(object sender, System.EventArgs e)
    {
        Refresh();
    }

    public bool DocumentChanged
    {
        get => _documentChanged;
        set
        {
            if (_documentChanged == value)
                return;

            _documentChanged = value;
            UpdateWindowTitle();
        }
    }

    public string Filename
    {
        get => _filename;
        set
        {
            if (_filename == value)
                return;

            _filename = value;
            UpdateWindowTitle();
        }
    }

    private void UpdateWindowTitle()
    {
        var title = "Crossword Builder";
        
        if (!string.IsNullOrEmpty(Filename))
            title = $"{title} - [{Filename}]";
        
        Text = DocumentChanged ? $"{title}*" : title;
    }

    private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
    {
        using var font = new Font(FontFamily.GenericSerif, 11);
        var g = e.Graphics;

        if (_document.Words.Count <= 0)
        {
            g.DrawString("Crossword contain no words.", font, Brushes.Black, 40, 40);
            return;
        }

        if (_document.Words.Count <= 1)
        {
            g.DrawString("Crossword contain too few words.", font, Brushes.Black, 40, 40);
            return;
        }

        var longestWord = _document.Words.GetLongestWordLength();

        if (longestWord <= 0)
        {
            g.DrawString("Crossword contain no words.", font, Brushes.Black, 40, 40);
            return;
        }

        _document.SortWords(WordSortOrder.Length);
        var success = false;
        var failedWord = "";

        for (var size = longestWord; size < longestWord + 100; size++)
        {
            if (_document.Build(size, out failedWord))
            {
                success = true;
                break;
            }
        }

        if (!success)
        {
            g.DrawString($@"Failed to build crossword. The word ""{failedWord}"" does not fit.", font, Brushes.Black, 40, 40);
            return;
        }

        var documentWidth = printDocument1.DefaultPageSettings.Bounds.Width - 80;
        var cellWidth = documentWidth / _document.GridSize;

        if (cellWidth > 50)
            cellWidth = 50;

        var crosswordWidth = cellWidth * _document.GridSize;
        var xStart = ((documentWidth / 2) - (crosswordWidth / 2)) + 40;
        using var wallPen = new Pen(Color.Black, 4);

        // TODO: Trim crossword to actual used size

        for (var row = 0; row < _document.GridSize; row++)
        {
            for (var col = 0; col < _document.GridSize; col++)
            {
                var cell = _document.GetCell(row, col);

                if (cell == null)
                    continue;
                
                var x = xStart + (col * cellWidth);
                var y = 40 + (row * cellWidth);
                g.DrawRectangle(Pens.Black, x, y, cellWidth, cellWidth);
                
                if (cell.IsStart)
                {
                    var numberString = cell.CrosswordStartNumber.ToString();
                    var numberSize = g.MeasureString(numberString, font);
                    g.DrawString(numberString, font, Brushes.Black, x + 2, y + 2);
                }
                
                if (cell.Letter != ' ' && solutionVisibleToolStripMenuItem.Checked)
                {
                    var letterString = cell.Letter.ToString();
                    var letterSize = g.MeasureString(letterString, font);
                    // TODO: Deal with fractions
                    g.DrawString(letterString, font, Brushes.Black, x + (cellWidth / 2) - (letterSize.Width / 2), y + (cellWidth / 2) - (letterSize.Height / 2));
                }
                
                // TODO: Draw thicker walls in a nice way
                
                if (cell.HasWallUp)
                    g.DrawLine(wallPen, x, y, x + cellWidth, y);
                
                if (cell.HasWallRight)
                    g.DrawLine(wallPen, x + cellWidth, y, x + cellWidth, y + cellWidth);
                
                if (cell.HasWallDown)
                    g.DrawLine(wallPen, x, y + cellWidth, x + cellWidth, y + cellWidth);
                
                if (cell.HasWallLeft)
                    g.DrawLine(wallPen, x, y, x, y + cellWidth);
            }
        }
    }

    private void refreshToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
        printPreviewControl1.InvalidatePreview();
    }

    private void autoToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
        autoToolStripMenuItem.Checked = true;
        toolStripMenuItem2.Checked = false;
        toolStripMenuItem3.Checked = false;
        toolStripMenuItem4.Checked = false;
        toolStripMenuItem5.Checked = false;
        toolStripMenuItem6.Checked = false;
        printPreviewControl1.AutoZoom = true;
    }

    private void toolStripMenuItem2_Click(object sender, System.EventArgs e)
    {
        autoToolStripMenuItem.Checked = false;
        toolStripMenuItem2.Checked = true;
        toolStripMenuItem3.Checked = false;
        toolStripMenuItem4.Checked = false;
        toolStripMenuItem5.Checked = false;
        toolStripMenuItem6.Checked = false;
        printPreviewControl1.Zoom = 0.5;
    }

    private void toolStripMenuItem3_Click(object sender, System.EventArgs e)
    {
        autoToolStripMenuItem.Checked = false;
        toolStripMenuItem2.Checked = false;
        toolStripMenuItem3.Checked = true;
        toolStripMenuItem4.Checked = false;
        toolStripMenuItem5.Checked = false;
        toolStripMenuItem6.Checked = false;
        printPreviewControl1.Zoom = 0.75;
    }

    private void toolStripMenuItem4_Click(object sender, System.EventArgs e)
    {
        autoToolStripMenuItem.Checked = false;
        toolStripMenuItem2.Checked = false;
        toolStripMenuItem3.Checked = false;
        toolStripMenuItem4.Checked = true;
        toolStripMenuItem5.Checked = false;
        toolStripMenuItem6.Checked = false;
        printPreviewControl1.Zoom = 1.0;
    }

    private void toolStripMenuItem5_Click(object sender, System.EventArgs e)
    {
        autoToolStripMenuItem.Checked = false;
        toolStripMenuItem2.Checked = false;
        toolStripMenuItem3.Checked = false;
        toolStripMenuItem4.Checked = false;
        toolStripMenuItem5.Checked = true;
        toolStripMenuItem6.Checked = false;
        printPreviewControl1.Zoom = 1.5;
    }

    private void toolStripMenuItem6_Click(object sender, System.EventArgs e)
    {
        autoToolStripMenuItem.Checked = false;
        toolStripMenuItem2.Checked = false;
        toolStripMenuItem3.Checked = false;
        toolStripMenuItem4.Checked = false;
        toolStripMenuItem5.Checked = false;
        toolStripMenuItem6.Checked = true;
        printPreviewControl1.Zoom = 2.0;
    }

    private void addToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
        using var x = new AddWordDialog();
        x.AddMode = true;

        if (x.ShowDialog(this) != DialogResult.OK)
            return;

        _document.Words.Add(new Word(x.NewWord));
        DocumentChanged = true;
        lblStatus.Text = $@"Word added: {x.NewWord}";
        lblWordCount.Text = $@"Words: {_document.Words.Count}";
        printPreviewControl1.InvalidatePreview();
    }

    private void solutionVisibleToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
        solutionVisibleToolStripMenuItem.Checked = !solutionVisibleToolStripMenuItem.Checked;
        printPreviewControl1.InvalidatePreview();
    }

    private void editToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
        _document.SortWords(WordSortOrder.Alphabetical);
        using var x = new PickWordDialog();
        x.Words = _document.Words;
        var originalWord = "";

        if (x.ShowDialog(this) == DialogResult.OK)
        {
            var word = x.Word;
            originalWord = word?.Content ?? "";

            if (word == null)
                throw new System.InvalidOperationException("No word was selected.");

            using var y = new AddWordDialog();
            y.AddMode = false;
            y.NewWord = word.Content;

            if (y.ShowDialog(this) == DialogResult.OK)
            {
                DocumentChanged = true;
                word.Content = y.NewWord!;
                lblStatus.Text = $@"Word edited from {originalWord} to {word.Content}.";
                printPreviewControl1.InvalidatePreview();
            }
        }
    }

    private void removeToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
        _document.SortWords(WordSortOrder.Alphabetical);
        using var x = new PickWordDialog();
        x.Words = _document.Words;

        if (x.ShowDialog(this) == DialogResult.OK)
        {
            var word = x.Word;

            if (word == null)
                throw new System.InvalidOperationException("No word was selected.");

            if (MessageBox.Show(this, $@"Are you sure you want to remove the word ""{word.Content}""?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _document.Words.Remove(word);
            DocumentChanged = true;
            lblWordCount.Text = $@"Words: {_document.Words.Count}";
            lblStatus.Text = $@"Word removed: {word.Content}";
            printPreviewControl1.InvalidatePreview();
        }
    }
}