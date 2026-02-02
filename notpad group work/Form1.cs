using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace notpad_group_work
{
    public partial class Form1 : Form
    {
        private PrintDocument printDoc = new PrintDocument();
        private PageSetupDialog pageSetupDialog = new PageSetupDialog();

        private string lastSearch = "";
        private int lastIndex = 0;

        public Form1()
        {
            InitializeComponent();
            pageSetupDialog.Document = printDoc;
            this.Text = "Untitled";
        }

        // ------------------- File Menu -------------------
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(richTextBox1.Text))
            {
                var result = MessageBox.Show("Do you want to save changes to the current document?",
                                             "New Document",
                                             MessageBoxButtons.YesNoCancel,
                                             MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    SaveFile();
                else if (result == DialogResult.Cancel)
                    return;
            }

            richTextBox1.Clear();
            this.Text = "Untitled";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Title = "Open";
                openDialog.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.LoadFile(openDialog.FileName, RichTextBoxStreamType.PlainText);
                    this.Text = openDialog.FileName;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void SaveFile()
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Save";
                saveDialog.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.SaveFile(saveDialog.FileName, RichTextBoxStreamType.PlainText);
                    this.Text = saveDialog.FileName;
                }
            }
        }

        private void SaveFileAs()
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Save As";
                saveDialog.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.SaveFile(saveDialog.FileName, RichTextBoxStreamType.PlainText);
                    this.Text = saveDialog.FileName;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void closeWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Undo();
        private void redoToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Redo();
        private void cutToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Cut();
        private void copyToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Copy();
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Paste();
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectedText = "";
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectAll();

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.Font = richTextBox1.Font;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                    richTextBox1.Font = fontDialog.Font;
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = richTextBox1.ForeColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                    richTextBox1.ForeColor = colorDialog.Color;
            }
        }

        private void clearFormattingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
            richTextBox1.SelectionFont = new Font(richTextBox1.Font.FontFamily, richTextBox1.Font.Size, FontStyle.Regular);
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.DeselectAll();
        }
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string searchText = Interaction.InputBox("Enter text to find:", "Find");
            if (!string.IsNullOrEmpty(searchText))
            {
                lastSearch = searchText;
                lastIndex = richTextBox1.Text.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase);
                if (lastIndex >= 0)
                {
                    richTextBox1.Select(lastIndex, searchText.Length);
                    richTextBox1.Focus();
                }
                else
                {
                    MessageBox.Show("Text not found!", "Find");
                }
            }
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lastSearch))
            {
                findToolStripMenuItem_Click(sender, e);
                return;
            }

            lastIndex = richTextBox1.Text.IndexOf(lastSearch, lastIndex + 1, StringComparison.CurrentCultureIgnoreCase);
            if (lastIndex >= 0)
            {
                richTextBox1.Select(lastIndex, lastSearch.Length);
                richTextBox1.Focus();
            }
            else
            {
                MessageBox.Show("No more occurrences found.", "Find Next");
                lastIndex = 0;
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string findText = Interaction.InputBox("Enter text to find:", "Replace");
            if (string.IsNullOrEmpty(findText)) return;

            string replaceText = Interaction.InputBox("Enter replacement text:", "Replace");
            if (replaceText == null) return;

            string text = richTextBox1.Text;
            int index = 0;
            while ((index = text.IndexOf(findText, index, StringComparison.CurrentCultureIgnoreCase)) != -1)
            {
                text = text.Remove(index, findText.Length).Insert(index, replaceText);
                index += replaceText.Length;
            }

            richTextBox1.Text = text;
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pageSetupDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Page setup saved:\n" +
                                "Margins: " + printDoc.DefaultPageSettings.Margins.Left + ", " +
                                printDoc.DefaultPageSettings.Margins.Top + "\n" +
                                "Orientation: " + printDoc.DefaultPageSettings.Landscape.ToString());
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDoc.PrintPage += (s, ev) =>
            {
                ev.Graphics.DrawString(richTextBox1.Text, richTextBox1.Font, Brushes.Black,
                    ev.MarginBounds.Left, ev.MarginBounds.Top);
            };

            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.Document = printDoc;
                if (printDialog.ShowDialog() == DialogResult.OK)
                    printDoc.Print();
            }
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText(Environment.NewLine + DateTime.Now.ToString());
        }

        private void recentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Recent files feature not implemented yet!");
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 newWindow = new Form1();
            newWindow.Show();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Title = "Open";
                openDialog.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        richTextBox1.Text = System.IO.File.ReadAllText(openDialog.FileName);
                        this.Text = openDialog.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error opening file: " + ex.Message);
                    }
                }
            }
        }

        private void timeDateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
