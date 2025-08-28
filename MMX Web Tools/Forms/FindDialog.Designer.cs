namespace MMX_Web_Tools.Forms
{
    partial class FindDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // label
            this.label1.AutoSize = true; this.label1.Text = "Find:"; this.label1.Left=12; this.label1.Top=15;
            // txt
            this.txtSearch.Left=60; this.txtSearch.Top=12; this.txtSearch.Width=260;
            // ok/cancel
            this.btnOK.Text = "OK"; this.btnOK.Left=164; this.btnOK.Top=45; this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            this.btnCancel.Text = "Cancel"; this.btnCancel.Left=245; this.btnCancel.Top=45; this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // dialog
            this.ClientSize = new System.Drawing.Size(340, 80);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
