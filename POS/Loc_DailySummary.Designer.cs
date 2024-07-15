namespace POS
{
    partial class Loc_DailySummary
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loc_DailySummary));
            this.chkTester = new System.Windows.Forms.CheckBox();
            this.chkFOC = new System.Windows.Forms.CheckBox();
            this.chkMPU = new System.Windows.Forms.CheckBox();
            this.chkCredit = new System.Windows.Forms.CheckBox();
            this.chkCash = new System.Windows.Forms.CheckBox();
            this.gbPaymentType = new System.Windows.Forms.GroupBox();
            this.chkGiftCard = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rdbRefund = new System.Windows.Forms.RadioButton();
            this.rdbSale = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gbList = new System.Windows.Forms.GroupBox();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.btnPrint = new System.Windows.Forms.Button();
            this.gbPaymentType.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbList.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkTester
            // 
            this.chkTester.AutoSize = true;
            this.chkTester.Checked = true;
            this.chkTester.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTester.Location = new System.Drawing.Point(587, 20);
            this.chkTester.Name = "chkTester";
            this.chkTester.Size = new System.Drawing.Size(56, 17);
            this.chkTester.TabIndex = 6;
            this.chkTester.Text = "Tester";
            this.chkTester.UseVisualStyleBackColor = true;
            // 
            // chkFOC
            // 
            this.chkFOC.AutoSize = true;
            this.chkFOC.Checked = true;
            this.chkFOC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFOC.Location = new System.Drawing.Point(499, 20);
            this.chkFOC.Name = "chkFOC";
            this.chkFOC.Size = new System.Drawing.Size(47, 17);
            this.chkFOC.TabIndex = 5;
            this.chkFOC.Text = "FOC";
            this.chkFOC.UseVisualStyleBackColor = true;
            // 
            // chkMPU
            // 
            this.chkMPU.AutoSize = true;
            this.chkMPU.Checked = true;
            this.chkMPU.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMPU.Location = new System.Drawing.Point(285, 20);
            this.chkMPU.Name = "chkMPU";
            this.chkMPU.Size = new System.Drawing.Size(50, 17);
            this.chkMPU.TabIndex = 4;
            this.chkMPU.Text = "MPU";
            this.chkMPU.UseVisualStyleBackColor = true;
            // 
            // chkCredit
            // 
            this.chkCredit.AutoSize = true;
            this.chkCredit.Checked = true;
            this.chkCredit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCredit.Location = new System.Drawing.Point(397, 20);
            this.chkCredit.Name = "chkCredit";
            this.chkCredit.Size = new System.Drawing.Size(53, 17);
            this.chkCredit.TabIndex = 2;
            this.chkCredit.Text = "Credit";
            this.chkCredit.UseVisualStyleBackColor = true;
            // 
            // chkCash
            // 
            this.chkCash.AutoSize = true;
            this.chkCash.Checked = true;
            this.chkCash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCash.Location = new System.Drawing.Point(58, 20);
            this.chkCash.Name = "chkCash";
            this.chkCash.Size = new System.Drawing.Size(50, 17);
            this.chkCash.TabIndex = 0;
            this.chkCash.Text = "Cash";
            this.chkCash.UseVisualStyleBackColor = true;
            // 
            // gbPaymentType
            // 
            this.gbPaymentType.Controls.Add(this.chkTester);
            this.gbPaymentType.Controls.Add(this.chkFOC);
            this.gbPaymentType.Controls.Add(this.chkMPU);
            this.gbPaymentType.Controls.Add(this.chkCredit);
            this.gbPaymentType.Controls.Add(this.chkGiftCard);
            this.gbPaymentType.Controls.Add(this.chkCash);
            this.gbPaymentType.Location = new System.Drawing.Point(21, 539);
            this.gbPaymentType.Name = "gbPaymentType";
            this.gbPaymentType.Size = new System.Drawing.Size(673, 51);
            this.gbPaymentType.TabIndex = 19;
            this.gbPaymentType.TabStop = false;
            this.gbPaymentType.Text = "Report Payment Type";
            this.gbPaymentType.Visible = false;
            // 
            // chkGiftCard
            // 
            this.chkGiftCard.AutoSize = true;
            this.chkGiftCard.Checked = true;
            this.chkGiftCard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGiftCard.Location = new System.Drawing.Point(166, 20);
            this.chkGiftCard.Name = "chkGiftCard";
            this.chkGiftCard.Size = new System.Drawing.Size(67, 17);
            this.chkGiftCard.TabIndex = 1;
            this.chkGiftCard.Text = "Gift Card";
            this.chkGiftCard.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtTo);
            this.groupBox1.Controls.Add(this.dtFrom);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(21, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(673, 51);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "By Period";
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(407, 23);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(104, 20);
            this.dtTo.TabIndex = 5;
            this.dtTo.ValueChanged += new System.EventHandler(this.dtTo_ValueChanged);
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(123, 23);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(103, 20);
            this.dtFrom.TabIndex = 4;
            this.dtFrom.ValueChanged += new System.EventHandler(this.dtFrom_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(353, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "To";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "From";
            // 
            // rdbRefund
            // 
            this.rdbRefund.AutoSize = true;
            this.rdbRefund.Location = new System.Drawing.Point(373, 19);
            this.rdbRefund.Name = "rdbRefund";
            this.rdbRefund.Size = new System.Drawing.Size(60, 17);
            this.rdbRefund.TabIndex = 1;
            this.rdbRefund.Text = "Refund";
            this.rdbRefund.UseVisualStyleBackColor = true;
            this.rdbRefund.CheckedChanged += new System.EventHandler(this.rdbRefund_CheckedChanged);
            // 
            // rdbSale
            // 
            this.rdbSale.AutoSize = true;
            this.rdbSale.Checked = true;
            this.rdbSale.Location = new System.Drawing.Point(25, 19);
            this.rdbSale.Name = "rdbSale";
            this.rdbSale.Size = new System.Drawing.Size(46, 17);
            this.rdbSale.TabIndex = 0;
            this.rdbSale.TabStop = true;
            this.rdbSale.Text = "Sale";
            this.rdbSale.UseVisualStyleBackColor = true;
            this.rdbSale.CheckedChanged += new System.EventHandler(this.rdbSale_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdbRefund);
            this.groupBox2.Controls.Add(this.rdbSale);
            this.groupBox2.Location = new System.Drawing.Point(21, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(673, 38);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "By Category";
            // 
            // gbList
            // 
            this.gbList.Controls.Add(this.reportViewer1);
            this.gbList.Location = new System.Drawing.Point(21, 126);
            this.gbList.Name = "gbList";
            this.gbList.Size = new System.Drawing.Size(673, 399);
            this.gbList.TabIndex = 16;
            this.gbList.TabStop = false;
            this.gbList.Text = "Item Sale Report";
            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(25, 32);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ShowPrintButton = false;
            this.reportViewer1.ShowRefreshButton = false;
            this.reportViewer1.ShowStopButton = false;
            this.reportViewer1.ShowZoomControl = false;
            this.reportViewer1.Size = new System.Drawing.Size(635, 352);
            this.reportViewer1.TabIndex = 4;
            // 
            // btnPrint
            // 
            this.btnPrint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Location = new System.Drawing.Point(582, 626);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(112, 24);
            this.btnPrint.TabIndex = 20;
            this.btnPrint.UseVisualStyleBackColor = true;
            // 
            // Loc_DailySummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.ClientSize = new System.Drawing.Size(722, 680);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.gbPaymentType);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Loc_DailySummary";
            this.Text = "Loc Daily Summary";
            this.Load += new System.EventHandler(this.Loc_DailySummary_Load);
            this.gbPaymentType.ResumeLayout(false);
            this.gbPaymentType.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkTester;
        private System.Windows.Forms.CheckBox chkFOC;
        private System.Windows.Forms.CheckBox chkMPU;
        private System.Windows.Forms.CheckBox chkCredit;
        private System.Windows.Forms.CheckBox chkCash;
        private System.Windows.Forms.GroupBox gbPaymentType;
        private System.Windows.Forms.CheckBox chkGiftCard;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rdbRefund;
        private System.Windows.Forms.RadioButton rdbSale;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox gbList;
        private System.Windows.Forms.Button btnPrint;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}