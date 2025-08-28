#nullable enable
using System.Linq;
using System.Windows.Forms;

namespace CrosswordBuilder;

public partial class PickWordDialog : Form
{
    public WordList? Words { private get; set; }
    public Word? Word { get; private set; }

    public PickWordDialog()
    {
        InitializeComponent();
    }

    private void PickWordDialog_Load(object sender, System.EventArgs e)
    {
        if (Words == null)
            throw new System.InvalidOperationException("Words property must be set before showing the dialog.");
    }

    private void PickWordDialog_Shown(object sender, System.EventArgs e)
    {
        Refresh();
        Cursor = Cursors.WaitCursor;
        listView1.BeginUpdate();
        
        foreach (var i in Words!.Select(word => new ListViewItem(word.Content) { Tag = word }))
            listView1.Items.Add(i);
        
        listView1.EndUpdate();
        Cursor = Cursors.Default;
    }

    private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        var item = listView1.GetItemAt(e.X, e.Y);

        if (item == null) 
            return;

        item.Selected = true;
        btnOk_Click(sender, e);
    }

    private void btnOk_Click(object sender, System.EventArgs e)
    {
        if (listView1.SelectedItems.Count <= 0)
        {
            MessageBox.Show(this, Text, @"No word is selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            listView1.Focus();
            return;
        }

        Word = (Word)listView1.SelectedItems[0].Tag!;
    }
}