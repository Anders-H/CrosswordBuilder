#nullable enable
using System;
using System.Windows.Forms;

namespace CrosswordBuilder;

public partial class AddWordDialog : Form
{
    public string NewWord { get; private set; }

    public AddWordDialog()
    {
        InitializeComponent();
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