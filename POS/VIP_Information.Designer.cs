namespace POS
{
    partial class VIP_Information
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VIP_Information));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbNonVIP = new System.Windows.Forms.RadioButton();
            this.rdbVIP = new System.Windows.Forms.RadioButton();
            this.rdbAll = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.gbPeriod = new System.Windows.Forms.GroupBox();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbYesNo = new System.Windows.Forms.GroupBox();
            this.rdbNo = new System.Windows.Forms.RadioButton();
            this.rdbYes = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cboCity = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cboPreferContact = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gpselectshop = new System.Windows.Forms.GroupBox();
            this.vipstartedin = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbPeriod.SuspendLayout();
            this.gbYesNo.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gpselectshop.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbNonVIP);
            this.groupBox1.Controls.Add(this.rdbVIP);
            this.groupBox1.Controls.Add(this.rdbAll);
            this.groupBox1.Location = new System.Drawing.Point(9, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(702, 63);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "By Type";
            // 
            // rdbNonVIP
            // 
            this.rdbNonVIP.AutoSize = true;
            this.rdbNonVIP.Location = new System.Drawing.Point(464, 32);
            this.rdbNonVIP.Name = "rdbNonVIP";
            this.rdbNonVIP.Size = new System.Drawing.Size(118, 17);
            this.rdbNonVIP.TabIndex = 2;
            this.rdbNonVIP.TabStop = true;
            this.rdbNonVIP.Text = "Non_VIP_Customer";
            this.rdbNonVIP.UseVisualStyleBackColor = true;
            this.rdbNonVIP.CheckedChanged += new System.EventHandler(this.rdbNonVIP_CheckedChanged);
            // 
            // rdbVIP
            // 
            this.rdbVIP.AutoSize = true;
            this.rdbVIP.Location = new System.Drawing.Point(228, 32);
            this.rdbVIP.Name = "rdbVIP";
            this.rdbVIP.Size = new System.Drawing.Size(92, 17);
            this.rdbVIP.TabIndex = 1;
            this.rdbVIP.TabStop = true;
            this.rdbVIP.Text = "VIP_Customer";
            this.rdbVIP.UseVisualStyleBackColor = true;
            this.rdbVIP.CheckedChanged += new System.EventHandler(this.rdbVIP_CheckedChanged);
            // 
            // rdbAll
            // 
            this.rdbAll.AutoSize = true;
            this.rdbAll.Checked = true;
            this.rdbAll.Location = new System.Drawing.Point(29, 32);
            this.rdbAll.Name = "rdbAll";
            this.rdbAll.Size = new System.Drawing.Size(83, 17);
            this.rdbAll.TabIndex = 0;
            this.rdbAll.TabStop = true;
            this.rdbAll.Text = "All Customer";
            this.rdbAll.UseVisualStyleBackColor = true;
            this.rdbAll.CheckedChanged += new System.EventHandler(this.rdbAll_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.reportViewer1);
            this.groupBox2.Location = new System.Drawing.Point(4, 205);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1024, 469);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Customer List";
            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(12, 28);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ShowPrintButton = false;
            this.reportViewer1.ShowRefreshButton = false;
            this.reportViewer1.ShowStopButton = false;
            this.reportViewer1.ShowZoomControl = false;
            this.reportViewer1.Size = new System.Drawing.Size(1006, 431);
            this.reportViewer1.TabIndex = 0;
            // 
            // gbPeriod
            // 
            this.gbPeriod.Controls.Add(this.dtTo);
            this.gbPeriod.Controls.Add(this.dtFrom);
            this.gbPeriod.Controls.Add(this.label3);
            this.gbPeriod.Controls.Add(this.label2);
            this.gbPeriod.Enabled = false;
            this.gbPeriod.Location = new System.Drawing.Point(283, 137);
            this.gbPeriod.Name = "gbPeriod";
            this.gbPeriod.Size = new System.Drawing.Size(428, 51);
            this.gbPeriod.TabIndex = 4;
            this.gbPeriod.TabStop = false;
            this.gbPeriod.Text = "By Period";
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(300, 23);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(111, 20);
            this.dtTo.TabIndex = 3;
            this.dtTo.ValueChanged += new System.EventHandler(this.dtTo_ValueChanged);
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(84, 23);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(110, 20);
            this.dtFrom.TabIndex = 1;
            this.dtFrom.ValueChanged += new System.EventHandler(this.dtFrom_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(246, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "To";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "From";
            // 
            // gbYesNo
            // 
            this.gbYesNo.Controls.Add(this.rdbNo);
            this.gbYesNo.Controls.Add(this.rdbYes);
            this.gbYesNo.Controls.Add(this.label1);
            this.gbYesNo.Enabled = false;
            this.gbYesNo.Location = new System.Drawing.Point(9, 135);
            this.gbYesNo.Name = "gbYesNo";
            this.gbYesNo.Size = new System.Drawing.Size(263, 53);
            this.gbYesNo.TabIndex = 3;
            this.gbYesNo.TabStop = false;
            this.gbYesNo.Text = "Do you want to search VIP list by promoted date?";
            // 
            // rdbNo
            // 
            this.rdbNo.AutoSize = true;
            this.rdbNo.Location = new System.Drawing.Point(152, 24);
            this.rdbNo.Name = "rdbNo";
            this.rdbNo.Size = new System.Drawing.Size(39, 17);
            this.rdbNo.TabIndex = 2;
            this.rdbNo.TabStop = true;
            this.rdbNo.Text = "No";
            this.rdbNo.UseVisualStyleBackColor = true;
            this.rdbNo.CheckedChanged += new System.EventHandler(this.rdbNo_CheckedChanged);
            // 
            // rdbYes
            // 
            this.rdbYes.AutoSize = true;
            this.rdbYes.Checked = true;
            this.rdbYes.Location = new System.Drawing.Point(29, 24);
            this.rdbYes.Name = "rdbYes";
            this.rdbYes.Size = new System.Drawing.Size(43, 17);
            this.rdbYes.TabIndex = 1;
            this.rdbYes.TabStop = true;
            this.rdbYes.Text = "Yes";
            this.rdbYes.UseVisualStyleBackColor = true;
            this.rdbYes.CheckedChanged += new System.EventHandler(this.rdbYes_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cboCity);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Location = new System.Drawing.Point(9, 77);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(320, 51);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Select  City";
            // 
            // cboCity
            // 
            this.cboCity.FormattingEnabled = true;
            this.cboCity.Location = new System.Drawing.Point(59, 19);
            this.cboCity.Name = "cboCity";
            this.cboCity.Size = new System.Drawing.Size(194, 21);
            this.cboCity.TabIndex = 1;
            this.cboCity.SelectedIndexChanged += new System.EventHandler(this.cboCity_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(29, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "City";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cboPreferContact);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(340, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(371, 51);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select Prefer Contact";
            // 
            // cboPreferContact
            // 
            this.cboPreferContact.FormattingEnabled = true;
            this.cboPreferContact.Items.AddRange(new object[] {
            "ALL",
            "Email",
            "Phone",
            "SMS"});
            this.cboPreferContact.Location = new System.Drawing.Point(133, 19);
            this.cboPreferContact.Name = "cboPreferContact";
            this.cboPreferContact.Size = new System.Drawing.Size(194, 21);
            this.cboPreferContact.TabIndex = 1;
            this.cboPreferContact.SelectedIndexChanged += new System.EventHandler(this.cboCity_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Prefer Contact";
            // 
            // gpselectshop
            // 
            this.gpselectshop.Controls.Add(this.vipstartedin);
            this.gpselectshop.Controls.Add(this.label5);
            this.gpselectshop.Location = new System.Drawing.Point(718, 4);
            this.gpselectshop.Name = "gpselectshop";
            this.gpselectshop.Size = new System.Drawing.Size(304, 74);
            this.gpselectshop.TabIndex = 7;
            this.gpselectshop.TabStop = false;
            this.gpselectshop.Text = "Select  Shop";
            this.gpselectshop.Visible = false;
            // 
            // vipstartedin
            // 
            this.vipstartedin.Font = new System.Drawing.Font("Zawgyi-One", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vipstartedin.FormattingEnabled = true;
            this.vipstartedin.Location = new System.Drawing.Point(92, 26);
            this.vipstartedin.Name = "vipstartedin";
            this.vipstartedin.Size = new System.Drawing.Size(194, 28);
            this.vipstartedin.TabIndex = 1;
            this.vipstartedin.SelectedIndexChanged += new System.EventHandler(this.vipstartedin_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Vip Stated In";
            // 
            // VIP_Information
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.ClientSize = new System.Drawing.Size(1033, 676);
            this.Controls.Add(this.gpselectshop);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.gbYesNo);
            this.Controls.Add(this.gbPeriod);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VIP_Information";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VIP Information";
            this.Load += new System.EventHandler(this.VIP_Information_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.gbPeriod.ResumeLayout(false);
            this.gbPeriod.PerformLayout();
            this.gbYesNo.ResumeLayout(false);
            this.gbYesNo.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gpselectshop.ResumeLayout(false);
            this.gpselectshop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbNonVIP;
        private System.Windows.Forms.RadioButton rdbVIP;
        private System.Windows.Forms.RadioButton rdbAll;
        private System.Windows.Forms.GroupBox groupBox2;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.GroupBox gbPeriod;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbYesNo;
        private System.Windows.Forms.RadioButton rdbNo;
        private System.Windows.Forms.RadioButton rdbYes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cboCity;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cboPreferContact;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gpselectshop;
        private System.Windows.Forms.ComboBox vipstartedin;
        private System.Windows.Forms.Label label5;
    }
}