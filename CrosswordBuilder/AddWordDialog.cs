#nullable enable
using System;
using System.Windows.Forms;

namespace CrosswordBuilder;

public partial class AddWordDialog : Form
{
    public string? NewWord { get; set; }
    public bool? AddMode { get; set; }

    public AddWordDialog()
    {
        InitializeComponent();
    }

    private void AddWordDialog_Load(object sender, EventArgs e)
    {
        if (!AddMode.HasValue)
            throw new SystemException("AddMode is null.");

        if (!AddMode.Value)
        {
            Text = $@"Edit Word: {NewWord}";
            txtWord.Text = NewWord;
        }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        var t = txtWord.Text.Trim().ToUpperInvariant();
        txtWord.Text = t;

        if (string.IsNullOrWhiteSpace(t))
        {
            txtWord.Focus();
            MessageBox.Show(@"Please enter a valid word.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        NewWord = t;
        DialogResult = DialogResult.OK;
    }
}