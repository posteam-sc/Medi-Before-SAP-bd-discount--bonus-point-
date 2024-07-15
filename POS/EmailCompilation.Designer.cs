namespace POS
{
    partial class EmailCompilation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmailCompilation));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbNonVIP = new System.Windows.Forms.RadioButton();
            this.rdbVIP = new System.Windows.Forms.RadioButton();
            this.rdbAll = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbNonVIP);
            this.groupBox1.Controls.Add(this.rdbVIP);
            this.groupBox1.Controls.Add(this.rdbAll);
            this.groupBox1.Location = new System.Drawing.Point(4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(483, 63);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "By Type";
            // 
            // rdbNonVIP
            // 
            this.rdbNonVIP.AutoSize = true;
            this.rdbNonVIP.Location = new System.Drawing.Point(339, 32);
            this.rdbNonVIP.Name = "rdbNonVIP";
            this.rdbNonVIP.Size = new System.Drawing.Size(118, 17);
            this.rdbNonVIP.TabIndex = 2;
            this.rdbNonVIP.TabStop = true;
            this.rdbNonVIP.Text = "Non_VIP_Customer";
            this.rdbNonVIP.UseVisualStyleBackColor = true;
            // 
            // rdbVIP
            // 
            this.rdbVIP.AutoSize = true;
            this.rdbVIP.Location = new System.Drawing.Point(163, 32);
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
            this.groupBox2.Location = new System.Drawing.Point(4, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(483, 602);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Email List";
            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(8, 29);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ShowPrintButton = false;
            this.reportViewer1.ShowRefreshButton = false;
            this.reportViewer1.ShowStopButton = false;
            this.reportViewer1.ShowZoomControl = false;
            this.reportViewer1.Size = new System.Drawing.Size(469, 567);
            this.reportViewer1.TabIndex = 4;
            // 
            // EmailCompilation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.ClientSize = new System.Drawing.Size(491, 676);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EmailCompilation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Email Compilation";
            this.Load += new System.EventHandler(this.EmailCompilation_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbNonVIP;
        private System.Windows.Forms.RadioButton rdbVIP;
        private System.Windows.Forms.RadioButton rdbAll;
        private System.Windows.Forms.GroupBox groupBox2;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}