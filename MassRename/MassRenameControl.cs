using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace MassRename
{
    class MassRenameControl : MattyUserControl
    {
        private Btn btnBrowseFiles, btnBrowseDir, btnRename;
        private Tb tbBrowse;
        private RichTb tbOld, tbNew;

        public MassRenameControl() {
            // The controls
            this.tbBrowse = new Tb(this);

            this.btnBrowseFiles = new Btn("Browse files", this);
            this.btnBrowseFiles.Click += this.browseFiles;

            this.btnBrowseDir = new Btn("Browse dir", this);
            this.btnBrowseDir.Click += this.browseDir;

            this.btnRename = new Btn("Rename", this);
            this.btnRename.Click += this.rename;

            int color = 250;
            this.tbOld = new RichTb(this);
            this.tbOld.ReadOnly = true;
            this.tbOld.BackColor = Color.FromArgb(color, color, color);
            this.tbOld.ForeColor = Color.Black;

            this.tbNew = new RichTb(this);
            this.tbNew.BackColor = Color.FromArgb(color, color, color);
            this.tbNew.ForeColor = Color.Black;
        }

        public override void OnResize() {
            // All the locations and sizes
            this.tbBrowse.LocateInside(this);
            this.tbBrowse.Size = new Size(this.Width - this.btnBrowseFiles.Width * 3 - 50, this.tbBrowse.Height);
            this.btnBrowseFiles.LocateFrom(this.tbBrowse, Btn.Horizontal.Right, Btn.Vertical.CopyTop);
            this.btnBrowseDir.LocateFrom(this.btnBrowseFiles, Btn.Horizontal.Right, Btn.Vertical.CopyTop);

            this.btnRename.LocateInside(this, Btn.Horizontal.Right, Btn.Vertical.Top);

            this.tbOld.LocateFrom(this.tbBrowse, Btn.Horizontal.CopyLeft, Btn.Vertical.Bottom);
            this.tbOld.Size = new Size(this.Width / 2 - 15, this.Height - this.tbOld.Location.Y - 10);
            this.tbNew.LocateFrom(this.tbOld, Btn.Horizontal.Right, Btn.Vertical.CopyTop);
            this.tbNew.Size = this.tbOld.Size;
        }

        private void browseFiles(object o, EventArgs e) {
            // Get all the file names with a shiny dialog
            var dialog = new OpenFileDialog();
            dialog.Title = "The files to rename";
            dialog.InitialDirectory = Settings.Get.LastDir;
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileNames.Length > 0) {
                setFiles(Path.GetDirectoryName(dialog.FileNames[0]), dialog.FileNames);
            }
        }

        private void browseDir(object o, EventArgs e) {
            // Get all the file names with a shiny dialog
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Rename all folders and files in the selected folder.";
            dialog.SelectedPath = Settings.Get.LastDir;

            if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath)) {
                SetFilesFromDir(dialog.SelectedPath);
            }
        }

        public void SetFilesFromDir(string directoryPath) {
            if (!Directory.Exists(directoryPath)) {
                return;
            }

            var filePaths = Directory.GetFiles(directoryPath);
            setFiles(directoryPath, filePaths);
        }

        private void setFiles(string pathPrefix, IEnumerable<string> fileNames) {
            // Fill the filename text boxes
            this.tbBrowse.Text = pathPrefix;
            Settings.Get.LastDir = pathPrefix;

            StringBuilder sb = new StringBuilder();
            foreach (string path in fileNames) {
                sb.AppendLine(Path.GetFileName(path));
            }

            this.tbOld.Text = sb.ToString();
            this.tbNew.Text = this.tbOld.Text;
            this.tbNew.Focus();
        }

        private void rename(object o, EventArgs e) {
            // Rename all the files
            char[] delimiter = Environment.NewLine.ToCharArray();
            string[] original = this.tbOld.Text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] replacement = this.tbNew.Text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string prefix = this.tbBrowse.Text + Path.DirectorySeparatorChar;

            if (original.Length == 0) {
                MessageBox.Show("No files to rename", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (original.Length != replacement.Length) {
                MessageBox.Show("The number of files is not equal", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int i = 0; i < original.Length; i++) {
                try {
                    string originalPath = prefix + original[i];
                    string newPath = prefix + replacement[i];

                    if (originalPath == newPath)
                        continue;

                    if (IsDirectory(originalPath))
                        Directory.Move(originalPath, newPath);
                    else
                        File.Move(originalPath, newPath);
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

        // Returns true if the path is a dir, false if it's a file and null if it's neither or doesn't exist.
        public static bool IsDirectory(string path) {
            if (Directory.Exists(path) || File.Exists(path)) {
                var fileAttr = File.GetAttributes(path);
                return (fileAttr & FileAttributes.Directory) == FileAttributes.Directory;
            }
            throw new FileNotFoundException("The path doesn't exist.");
        }
    }
}
