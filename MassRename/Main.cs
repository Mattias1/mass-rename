using System;
using System.Drawing;
using System.Windows.Forms;

namespace MassRename
{
    class Main : Form
    {
        private static Size MinSize = new Size(700, 400);

        private MassRenameControl massRenameControl;
        private MattyUserControl[] userControls;
        public int GoToControl;

        public Main() {
            // Load and apply the settings
            Settings s = Settings.Get;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = s.Position;
            this.ClientSize = new Size(s.Size);
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Set some standard values
            this.Text = "Mass rename tool";
            this.MaximizeBox = false;

            // Add the controls
            this.massRenameControl = new MassRenameControl();
            this.userControls = new MattyUserControl[] { massRenameControl };
            foreach (MattyUserControl u in this.userControls) {
                u.Size = this.ClientSize;
                this.Controls.Add(u);
                u.OnResize();
            }

            // Locate the controls
            this.onResizeEnd(this, new EventArgs());

            // Register events
            this.LocationChanged += (o, e) => { Settings.Get.Position = this.Location; };
            this.ResizeEnd += onResizeEnd;
        }

        private void onResizeEnd(object o, EventArgs e) {
            // Make sure its not too small
            this.ClientSize = new Size(Math.Max(this.ClientSize.Width, MinSize.Width), Math.Max(this.ClientSize.Height, MinSize.Height));

            // Save the size to the settings
            Settings.Get.Size = new Point(this.ClientSize);

            // Resize the user controls
            foreach (MattyUserControl u in this.userControls) {
                u.Size = this.ClientSize;
                u.OnResize();
            }
        }

        /// <summary>
        /// Show usercontrol at index i and hide all others
        /// </summary>
        /// <param name="i">The index</param>
        public void ShowUserControl(int i) {
            foreach (MattyUserControl u in this.userControls) {
                u.Hide();
            }
            this.userControls[i].Show();
            this.userControls[i].Size = this.ClientSize;
            this.userControls[i].OnResize();
        }

        public void LoadArgumentsFileOrDir(string path) {
            this.massRenameControl.SetFilesFromDir(path);
        }
    }

    class MattyUserControl : UserControl
    {
        public int GoToControl {
            get => this.Main.GoToControl;
            set => this.Main.GoToControl = value;
        }
        public Main Main => (Main)this.Parent;

        /// <summary>
        /// Show usercontrol at index i and hide all others
        /// </summary>
        /// <param name="i">The index</param>
        public void ShowUserControl(int i) {
            this.GoToControl = -1;
            this.Main.ShowUserControl(i);
        }

        /// <summary>
        /// This method gets called after the usercontrol is resized
        /// </summary>
        public virtual void OnResize() { }
    }
}
