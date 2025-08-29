using System;
using System.Windows.Forms;
using MMX_Web_Tools.Utils;

namespace MMX_Web_Tools
{
    public partial class BulkUpdateForm : Form
    {
        public bool UsePercentage => radioPercent.Checked;
        public bool Increase => radioIncrease.Checked;
        public decimal Amount => numAmount.Value;
        public bool ApplyToRetail => chkRetail.Checked;
        public bool ApplyToSale => chkSale.Checked;

        public BulkUpdateForm()
        {
            InitializeComponent();
            // Apply current theme and keep in sync
            ThemeManager.ApplyTheme(this, ThemeManager.CurrentTheme);
            System.Windows.Forms.FormClosedEventHandler onClosed = null;
            Action<AppTheme> handler = t => ThemeManager.ApplyTheme(this, t);
            ThemeManager.ThemeChanged += handler;
            onClosed = (s, e) => { ThemeManager.ThemeChanged -= handler; this.FormClosed -= onClosed; };
            this.FormClosed += onClosed;
        }

        public decimal Apply(decimal value)
        {
            if (UsePercentage)
            {
                var delta = value * (Amount / 100m);
                return Increase ? value + delta : value - delta;
            }
            else
            {
                return Increase ? value + Amount : value - Amount;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ApplyToRetail && !ApplyToSale)
            {
                MessageBox.Show(this, "Select at least one target (Retail or Sale).", "Bulk Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
