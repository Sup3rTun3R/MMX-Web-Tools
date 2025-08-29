using System;
using System.Windows.Forms;
using MMX_Web_Tools.Utils;

namespace MMX_Web_Tools.Forms
{
    public partial class FindDialog : Form
    {
        public string SearchText => txtSearch.Text;

        public FindDialog(string initial)
        {
            InitializeComponent();
            txtSearch.Text = initial ?? string.Empty;
            txtSearch.SelectAll();

            // Apply current theme and keep in sync
            ThemeManager.ApplyTheme(this, ThemeManager.CurrentTheme);
            System.Windows.Forms.FormClosedEventHandler onClosed = null;
            Action<AppTheme> handler = t => ThemeManager.ApplyTheme(this, t);
            ThemeManager.ThemeChanged += handler;
            onClosed = (s, e) => { ThemeManager.ThemeChanged -= handler; this.FormClosed -= onClosed; };
            this.FormClosed += onClosed;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
