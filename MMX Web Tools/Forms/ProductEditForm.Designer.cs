namespace MMX_Web_Tools
{
    partial class ProductEditForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtSku = new System.Windows.Forms.TextBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.numCost1 = new System.Windows.Forms.NumericUpDown();
            this.numCost2 = new System.Windows.Forms.NumericUpDown();
            this.numRetail = new System.Windows.Forms.NumericUpDown();
            this.numSale = new System.Windows.Forms.NumericUpDown();
            this.numStock = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numCost1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCost2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStock)).BeginInit();
            this.SuspendLayout();
            // 
            // labels
            // 
            this.label1.AutoSize = true; this.label1.Text = "Name"; this.label1.Left=12; this.label1.Top=15;
            this.label2.AutoSize = true; this.label2.Text = "SKU"; this.label2.Left=12; this.label2.Top=45;
            this.label3.AutoSize = true; this.label3.Text = "Code"; this.label3.Left=12; this.label3.Top=75;
            this.label4.AutoSize = true; this.label4.Text = "Cost1"; this.label4.Left=12; this.label4.Top=105;
            this.label5.AutoSize = true; this.label5.Text = "Cost2"; this.label5.Left=12; this.label5.Top=135;
            this.label6.AutoSize = true; this.label6.Text = "Retail"; this.label6.Left=12; this.label6.Top=165;
            this.label7.AutoSize = true; this.label7.Text = "Sale"; this.label7.Left=12; this.label7.Top=195;
            // 
            // textboxes
            // 
            this.txtName.Left=80; this.txtName.Top=12; this.txtName.Width=300;
            this.txtSku.Left=80; this.txtSku.Top=42; this.txtSku.Width=300;
            this.txtCode.Left=80; this.txtCode.Top=72; this.txtCode.Width=300;
            // 
            // numbers
            // 
            var decPlaces = 2;
            this.numCost1.Left=80; this.numCost1.Top=102; this.numCost1.DecimalPlaces=decPlaces; this.numCost1.Maximum=1000000; this.numCost1.ThousandsSeparator=true;
            this.numCost2.Left=80; this.numCost2.Top=132; this.numCost2.DecimalPlaces=decPlaces; this.numCost2.Maximum=1000000; this.numCost2.ThousandsSeparator=true;
            this.numRetail.Left=80; this.numRetail.Top=162; this.numRetail.DecimalPlaces=decPlaces; this.numRetail.Maximum=1000000; this.numRetail.ThousandsSeparator=true;
            this.numSale.Left=80; this.numSale.Top=192; this.numSale.DecimalPlaces=decPlaces; this.numSale.Maximum=1000000; this.numSale.ThousandsSeparator=true;
            this.numStock.Left=80; this.numStock.Top=222; this.numStock.Maximum=100000000; this.numStock.ThousandsSeparator=true;
            // 
            // Buttons
            // 
            this.btnOk.Text = "OK"; this.btnOk.Left=224; this.btnOk.Top=260; this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            this.btnCancel.Text = "Cancel"; this.btnCancel.Left=305; this.btnCancel.Top=260; this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ProductEditForm
            // 
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtSku);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.numCost1);
            this.Controls.Add(this.numCost2);
            this.Controls.Add(this.numRetail);
            this.Controls.Add(this.numSale);
            this.Controls.Add(this.numStock);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Text = "Edit Product";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.numCost1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCost2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtSku;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.NumericUpDown numCost1;
        private System.Windows.Forms.NumericUpDown numCost2;
        private System.Windows.Forms.NumericUpDown numRetail;
        private System.Windows.Forms.NumericUpDown numSale;
        private System.Windows.Forms.NumericUpDown numStock;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
