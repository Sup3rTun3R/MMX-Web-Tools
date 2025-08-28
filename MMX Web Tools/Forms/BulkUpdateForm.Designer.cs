namespace MMX_Web_Tools
{
    partial class BulkUpdateForm
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
            this.numAmount = new System.Windows.Forms.NumericUpDown();
            this.groupBoxType = new System.Windows.Forms.GroupBox();
            this.radioPercent = new System.Windows.Forms.RadioButton();
            this.radioFixed = new System.Windows.Forms.RadioButton();
            this.groupBoxDirection = new System.Windows.Forms.GroupBox();
            this.radioIncrease = new System.Windows.Forms.RadioButton();
            this.radioDecrease = new System.Windows.Forms.RadioButton();
            this.chkRetail = new System.Windows.Forms.CheckBox();
            this.chkSale = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            this.groupBoxType.SuspendLayout();
            this.groupBoxDirection.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true; this.label1.Text = "Amount"; this.label1.Left=12; this.label1.Top=15;
            // 
            // numAmount
            // 
            this.numAmount.Left=80; this.numAmount.Top=12; this.numAmount.DecimalPlaces=2; this.numAmount.Maximum=1000000; this.numAmount.Minimum=0; this.numAmount.Value=5;
            this.numAmount.Width = 100;
            // 
            // groupBoxType
            // 
            this.groupBoxType.Text = "Amount Type"; this.groupBoxType.Left=12; this.groupBoxType.Top=45; this.groupBoxType.Width=260; this.groupBoxType.Height=55;
            this.groupBoxType.Controls.Add(this.radioPercent);
            this.groupBoxType.Controls.Add(this.radioFixed);
            // 
            // radioPercent
            // 
            this.radioPercent.Left=12; this.radioPercent.Top=22; this.radioPercent.Text="Percent"; this.radioPercent.Checked=true; this.radioPercent.AutoSize = true;
            // 
            // radioFixed
            // 
            this.radioFixed.Left=120; this.radioFixed.Top=22; this.radioFixed.Text="Fixed"; this.radioFixed.AutoSize = true;
            // 
            // groupBoxDirection
            // 
            this.groupBoxDirection.Text = "Direction"; this.groupBoxDirection.Left=12; this.groupBoxDirection.Top=105; this.groupBoxDirection.Width=260; this.groupBoxDirection.Height=55;
            this.groupBoxDirection.Controls.Add(this.radioIncrease);
            this.groupBoxDirection.Controls.Add(this.radioDecrease);
            // 
            // radioIncrease
            // 
            this.radioIncrease.Left=12; this.radioIncrease.Top=22; this.radioIncrease.Text="Increase"; this.radioIncrease.Checked=true; this.radioIncrease.AutoSize = true;
            // 
            // radioDecrease
            // 
            this.radioDecrease.Left=120; this.radioDecrease.Top=22; this.radioDecrease.Text="Decrease"; this.radioDecrease.AutoSize = true;
            // 
            // chkRetail
            // 
            this.chkRetail.Left=12; this.chkRetail.Top=170; this.chkRetail.Text="Apply to Retail"; this.chkRetail.Checked=true; this.chkRetail.AutoSize = true;
            // 
            // chkSale
            // 
            this.chkSale.Left=12; this.chkSale.Top=195; this.chkSale.Text="Apply to Sale"; this.chkSale.AutoSize = true;
            // 
            // btnOk
            // 
            this.btnOk.Text = "OK"; this.btnOk.Left=124; this.btnOk.Top=230; this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Text = "Cancel"; this.btnCancel.Left=205; this.btnCancel.Top=230; this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // BulkUpdateForm
            // 
            this.ClientSize = new System.Drawing.Size(300, 270);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numAmount);
            this.Controls.Add(this.groupBoxType);
            this.Controls.Add(this.groupBoxDirection);
            this.Controls.Add(this.chkRetail);
            this.Controls.Add(this.chkSale);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bulk Update Prices";
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
            this.groupBoxType.ResumeLayout(false);
            this.groupBoxType.PerformLayout();
            this.groupBoxDirection.ResumeLayout(false);
            this.groupBoxDirection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numAmount;
        private System.Windows.Forms.GroupBox groupBoxType;
        private System.Windows.Forms.RadioButton radioPercent;
        private System.Windows.Forms.RadioButton radioFixed;
        private System.Windows.Forms.GroupBox groupBoxDirection;
        private System.Windows.Forms.RadioButton radioIncrease;
        private System.Windows.Forms.RadioButton radioDecrease;
        private System.Windows.Forms.CheckBox chkRetail;
        private System.Windows.Forms.CheckBox chkSale;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
