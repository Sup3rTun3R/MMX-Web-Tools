using System;
using System.Windows.Forms;

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
