using MMX_Web_Tools.Models;
using System;
using System.Globalization;
using System.Windows.Forms;
using MMX_Web_Tools.Utils;

namespace MMX_Web_Tools
{
    public partial class ProductEditForm : Form
    {
        private readonly Product _product;
        public decimal OriginalRetail { get; private set; }
        public decimal OriginalSale { get; private set; }

        public ProductEditForm(Product product)
        {
            _product = product ?? throw new ArgumentNullException(nameof(product));
            InitializeComponent();
            txtName.Text = _product.Name;
            txtSku.Text = _product.Sku;
            txtCode.Text = _product.Code;
            numCost1.Value = _product.Cost1;
            numCost2.Value = _product.Cost2;
            numRetail.Value = _product.Retail;
            numSale.Value = _product.Sale;
            numStock.Value = _product.Stock;
            OriginalRetail = _product.Retail;
            OriginalSale = _product.Sale;

            // Apply theme and keep in sync
            ThemeManager.ApplyTheme(this, ThemeManager.CurrentTheme);
            System.Windows.Forms.FormClosedEventHandler onClosed = null;
            Action<AppTheme> handler = t => ThemeManager.ApplyTheme(this, t);
            ThemeManager.ThemeChanged += handler;
            onClosed = (s, e) => { ThemeManager.ThemeChanged -= handler; this.FormClosed -= onClosed; };
            this.FormClosed += onClosed;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _product.Name = txtName.Text;
            _product.Sku = txtSku.Text;
            _product.Code = txtCode.Text;
            _product.Cost1 = numCost1.Value;
            _product.Cost2 = numCost2.Value;
            _product.Retail = numRetail.Value;
            _product.Sale = numSale.Value;
            _product.Stock = (int)numStock.Value;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
