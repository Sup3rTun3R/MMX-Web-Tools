using System;
using System.Windows.Forms;

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
