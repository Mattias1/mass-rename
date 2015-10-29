using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MassRename
{
    class MassRenameControl : MattyUserControl
    {
        Btn btnBrowse, btnRename;
        Tb tbBrowse;
        RichTb tbOld, tbNew;

        public MassRenameControl() {
            // The controls
            this.tbBrowse = new Tb(this);
            this.btnBrowse = new Btn("Browse", this);
            this.btnBrowse.Click += this.browse;
            this.btnRename = new Btn("Rename", this);
            this.btnRename.Click += this.rename;
            this.tbOld = new RichTb(this);
            this.tbOld.ReadOnly = true;
            int color = 250;
            this.tbOld.BackColor = Color.FromArgb(color, color, color);
            this.tbOld.ForeColor = Color.Black;
            this.tbNew = new RichTb(this);
            this.tbNew.BackColor = Color.FromArgb(color, color, color);
            this.tbNew.ForeColor = Color.Black;
        }

        public override void OnResize() {
            // All the locations and sizes
            this.tbBrowse.LocateInside(this);
            this.tbBrowse.Size = new Size(this.Width - this.btnBrowse.Width * 2 - 50, this.tbBrowse.Height);
            this.btnBrowse.LocateFrom(this.tbBrowse, Btn.Horizontal.Right, Btn.Vertical.CopyTop);

            this.btnRename.LocateInside(this, Btn.Horizontal.Right, Btn.Vertical.Top);

            this.tbOld.LocateFrom(this.tbBrowse, Btn.Horizontal.CopyLeft, Btn.Vertical.Bottom);
            this.tbOld.Size = new Size(this.Width / 2 - 15, this.Height - this.tbOld.Location.Y - 10);
            this.tbNew.LocateFrom(this.tbOld, Btn.Horizontal.Right, Btn.Vertical.CopyTop);
            this.tbNew.Size = this.tbOld.Size;
        }

        private void browse(object o, EventArgs e) {
            // Get all the file names with a shiny dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "The files to rename";
            dialog.InitialDirectory = Settings.Get.LastDir;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileNames.Length > 0) {
                this.tbBrowse.Text = Path.GetDirectoryName(dialog.FileNames[0]);
                Settings.Get.LastDir = this.tbBrowse.Text;
                StringBuilder sb = new StringBuilder();
                foreach (string path in dialog.FileNames)
                    sb.AppendLine(Path.GetFileName(path));
                this.tbOld.Text = sb.ToString();
                this.tbNew.Text = this.tbOld.Text;
                this.tbNew.Focus();
            }
        }

        private void rename(object o, EventArgs e) {
            // Rename all the files
            char[] delimiter = Environment.NewLine.ToCharArray();
            string[] original = this.tbOld.Text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] replacement = this.tbNew.Text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string prefix = this.tbBrowse.Text + Path.DirectorySeparatorChar;
            if (original.Length != replacement.Length) {
                MessageBox.Show("The number of files is not equal", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < original.Length; i++) {
                try {
                    File.Move(prefix + original[i], prefix + replacement[i]);
                }
                catch (Exception ex) {
                    string msg = "Error trying to rename the file (#" + i.ToString() + "): " + original[i] + Environment.NewLine + "Message: " + ex.Message;
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // We succeeded, so let's celebrate
            this.tbOld.Text = this.tbNew.Text;
            this.tbNew.Text = "The files are renamed successfully.";
        }
    }
}
