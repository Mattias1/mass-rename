using System;
using System.Drawing;
using System.Windows.Forms;

namespace MassRename
{
    class MassRenameControl : MattyUserControl
    {
        Btn btnBrowse, btnRename;
        Tb tbBrowse;
        Tb tbOld, tbNew;

        public MassRenameControl() {
            // The controls
            this.tbBrowse = new Tb(this);
            this.btnBrowse = new Btn("Browse", this);
            this.btnBrowse.Click += this.browse;
            this.btnRename = new Btn("Rename", this);
            this.btnRename.Click += this.rename;
            this.tbOld = new Tb(this);
            this.tbOld.Multiline = true;
            this.tbOld.ReadOnly = true;
            this.tbNew = new Tb(this);
            this.tbNew.Multiline = true;
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

        }

        private void rename(object o, EventArgs e) {

        }
    }
}
