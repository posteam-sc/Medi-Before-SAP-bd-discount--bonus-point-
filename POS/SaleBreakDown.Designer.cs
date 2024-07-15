namespace POS
{
    partial class SaleBreakDown
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
            this.gbPeriod = new System.Windows.Forms.GroupBox();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbUnitPrice = new System.Windows.Forms.RadioButton();
            this.rdbSaleTrueValue = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbSegment = new System.Windows.Forms.RadioButton();
            this.rdbRange = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdbVIP = new System.Windows.Forms.RadioButton();
            this.rdbAll = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cboCity = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lblCounterName = new System.Windows.Forms.Label();
            this.cboCounter = new System.Windows.Forms.ComboBox();
            this.gbPeriod.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPeriod
            // 
            this.gbPeriod.Controls.Add(this.dtTo);
            this.gbPeriod.Controls.Add(this.dtFrom);
            this.gbPeriod.Controls.Add(this.label3);
            this.gbPeriod.Controls.Add(this.label2);
            this.gbPeriod.Location = new System.Drawing.Point(12, 12);
            this.gbPeriod.Name = "gbPeriod";
            this.gbPeriod.Size = new System.Drawing.Size(439, 51);
            this.gbPeriod.TabIndex = 0;
            this.gbPeriod.TabStop = false;
            this.gbPeriod.Text = "By Period";
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(301, 22);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(111, 20);
            this.dtTo.TabIndex = 5;
            this.dtTo.ValueChanged += new System.EventHandler(this.dtTo_ValueChanged);
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(85, 23);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(110, 20);
            this.dtFrom.TabIndex = 4;
            this.dtFrom.ValueChanged += new System.EventHandler(this.dtFrom_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(247, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "To";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "From";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdbUnitPrice);
            this.groupBox2.Controls.Add(this.rdbSaleTrueValue);
            this.groupBox2.Location = new System.Drawing.Point(12, 190);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(439, 55);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "By Require Sale Value";
            // 
            // rdbUnitPrice
            // 
            this.rdbUnitPrice.AutoSize = true;
            this.rdbUnitPrice.Location = new System.Drawing.Point(250, 24);
            this.rdbUnitPrice.Name = "rdbUnitPrice";
            this.rdbUnitPrice.Size = new System.Drawing.Size(71, 17);
            this.rdbUnitPrice.TabIndex = 3;
            this.rdbUnitPrice.TabStop = true;
            this.rdbUnitPrice.Text = "Unit Price";
            this.rdbUnitPrice.UseVisualStyleBackColor = true;
            this.rdbUnitPrice.CheckedChanged += new System.EventHandler(this.rdbUnitPrice_CheckedChanged);
            // 
            // rdbSaleTrueValue
            // 
            this.rdbSaleTrueValue.AutoSize = true;
            this.rdbSaleTrueValue.Location = new System.Drawing.Point(29, 24);
            this.rdbSaleTrueValue.Name = "rdbSaleTrueValue";
            this.rdbSaleTrueValue.Size = new System.Drawing.Size(98, 17);
            this.rdbSaleTrueValue.TabIndex = 2;
            this.rdbSaleTrueValue.TabStop = true;
            this.rdbSaleTrueValue.Text = "Sale True Price";
            this.rdbSaleTrueValue.UseVisualStyleBackColor = true;
            this.rdbSaleTrueValue.CheckedChanged += new System.EventHandler(this.rdbSaleTrueValue_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbSegment);
            this.groupBox1.Controls.Add(this.rdbRange);
            this.groupBox1.Location = new System.Drawing.Point(12, 130);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(439, 54);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select One";
            // 
            // rdbSegment
            // 
            this.rdbSegment.AutoSize = true;
            this.rdbSegment.Location = new System.Drawing.Point(250, 28);
            this.rdbSegment.Name = "rdbSegment";
            this.rdbSegment.Size = new System.Drawing.Size(82, 17);
            this.rdbSegment.TabIndex = 1;
            this.rdbSegment.TabStop = true;
            this.rdbSegment.Text = "By Segment";
            this.rdbSegment.UseVisualStyleBackColor = true;
            this.rdbSegment.CheckedChanged += new System.EventHandler(this.rdbSegment_CheckedChanged);
            // 
            // rdbRange
            // 
            this.rdbRange.AutoSize = true;
            this.rdbRange.Location = new System.Drawing.Point(29, 28);
            this.rdbRange.Name = "rdbRange";
            this.rdbRange.Size = new System.Drawing.Size(72, 17);
            this.rdbRange.TabIndex = 0;
            this.rdbRange.TabStop = true;
            this.rdbRange.Text = "By Range";
            this.rdbRange.UseVisualStyleBackColor = true;
            this.rdbRange.CheckedChanged += new System.EventHandler(this.rdbRange_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.reportViewer1);
            this.groupBox3.Location = new System.Drawing.Point(12, 251);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(791, 423);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sale List";
            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(6, 29);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ShowPrintButton = false;
            this.reportViewer1.ShowRefreshButton = false;
            this.reportViewer1.ShowStopButton = false;
            this.reportViewer1.ShowZoomControl = false;
            this.reportViewer1.Size = new System.Drawing.Size(779, 388);
            this.reportViewer1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdbVIP);
            this.groupBox4.Controls.Add(this.rdbAll);
            this.groupBox4.Location = new System.Drawing.Point(12, 70);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(439, 54);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Select One";
            // 
            // rdbVIP
            // 
            this.rdbVIP.AutoSize = true;
            this.rdbVIP.Location = new System.Drawing.Point(250, 28);
            this.rdbVIP.Name = "rdbVIP";
            this.rdbVIP.Size = new System.Drawing.Size(83, 17);
            this.rdbVIP.TabIndex = 1;
            this.rdbVIP.TabStop = true;
            this.rdbVIP.Text = "VIP Member";
            this.rdbVIP.UseVisualStyleBackColor = true;
            this.rdbVIP.CheckedChanged += new System.EventHandler(this.rdbVIP_CheckedChanged);
            // 
            // rdbAll
            // 
            this.rdbAll.AutoSize = true;
            this.rdbAll.Location = new System.Drawing.Point(29, 28);
            this.rdbAll.Name = "rdbAll";
            this.rdbAll.Size = new System.Drawing.Size(36, 17);
            this.rdbAll.TabIndex = 0;
            this.rdbAll.TabStop = true;
            this.rdbAll.Text = "All";
            this.rdbAll.UseVisualStyleBackColor = true;
            this.rdbAll.CheckedChanged += new System.EventHandler(this.rdbAll_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cboCity);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.lblCounterName);
            this.groupBox5.Controls.Add(this.cboCounter);
            this.groupBox5.Location = new System.Drawing.Point(464, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(339, 112);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Select City And Cuounter";
            // 
            // cboCity
            // 
            this.cboCity.FormattingEnabled = true;
            this.cboCity.Location = new System.Drawing.Point(106, 27);
            this.cboCity.Name = "cboCity";
            this.cboCity.Size = new System.Drawing.Size(227, 21);
            this.cboCity.TabIndex = 1;
            this.cboCity.SelectedIndexChanged += new System.EventHandler(this.cboCity_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "City";
            // 
            // lblCounterName
            // 
            this.lblCounterName.AutoSize = true;
            this.lblCounterName.Location = new System.Drawing.Point(9, 71);
            this.lblCounterName.Name = "lblCounterName";
            this.lblCounterName.Size = new System.Drawing.Size(75, 13);
            this.lblCounterName.TabIndex = 2;
            this.lblCounterName.Text = "Counter Name";
            // 
            // cboCounter
            // 
            this.cboCounter.FormattingEnabled = true;
            this.cboCounter.Location = new System.Drawing.Point(106, 68);
            this.cboCounter.Name = "cboCounter";
            this.cboCounter.Size = new System.Drawing.Size(227, 21);
            this.cboCounter.TabIndex = 3;
            this.cboCounter.SelectedIndexChanged += new System.EventHandler(this.cboCounter_SelectedIndexChanged);
            // 
            // SaleBreakDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.ClientSize = new System.Drawing.Size(807, 676);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbPeriod);
            this.Name = "SaleBreakDown";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sale Breakdown";
            this.Load += new System.EventHandler(this.SaleBreakDown_Load);
            this.gbPeriod.ResumeLayout(false);
            this.gbPeriod.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbPeriod;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdbUnitPrice;
        private System.Windows.Forms.RadioButton rdbSaleTrueValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbSegment;
        private System.Windows.Forms.RadioButton rdbRange;
        private System.Windows.Forms.GroupBox groupBox3;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdbVIP;
        private System.Windows.Forms.RadioButton rdbAll;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lblCounterName;
        private System.Windows.Forms.ComboBox cboCounter;
        private System.Windows.Forms.ComboBox cboCity;
        private System.Windows.Forms.Label label8;
    }
}