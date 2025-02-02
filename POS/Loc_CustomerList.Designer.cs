﻿namespace POS
{
    partial class Loc_CustomerList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loc_CustomerList));
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.rdoVIP = new System.Windows.Forms.RadioButton();
            this.rdoNonVIP = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoMemberId = new System.Windows.Forms.RadioButton();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.lblSearchTitle = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.rdoCustomerName = new System.Windows.Forms.RadioButton();
            this.rdoPhoneNumber = new System.Windows.Forms.RadioButton();
            this.rdoEmail = new System.Windows.Forms.RadioButton();
            this.rdoNIRC = new System.Windows.Forms.RadioButton();
            this.btnAddNewCustomer = new System.Windows.Forms.Button();
            this.dgvCustomerList = new System.Windows.Forms.DataGridView();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewLinkColumn();
            this.createdshop = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_fix = new System.Windows.Forms.Button();
            this.groupvipstarted = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerList)).BeginInit();
            this.groupvipstarted.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdoAll
            // 
            this.rdoAll.AutoSize = true;
            this.rdoAll.Checked = true;
            this.rdoAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.rdoAll.Location = new System.Drawing.Point(21, 37);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(88, 17);
            this.rdoAll.TabIndex = 4;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "All Customers";
            this.rdoAll.UseVisualStyleBackColor = true;
            this.rdoAll.CheckedChanged += new System.EventHandler(this.rdoAll_CheckedChanged);
            // 
            // rdoVIP
            // 
            this.rdoVIP.AutoSize = true;
            this.rdoVIP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.rdoVIP.Location = new System.Drawing.Point(131, 37);
            this.rdoVIP.Name = "rdoVIP";
            this.rdoVIP.Size = new System.Drawing.Size(94, 17);
            this.rdoVIP.TabIndex = 5;
            this.rdoVIP.Text = "VIP Customers";
            this.rdoVIP.UseVisualStyleBackColor = true;
            this.rdoVIP.CheckedChanged += new System.EventHandler(this.rdoVIP_CheckedChanged);
            // 
            // rdoNonVIP
            // 
            this.rdoNonVIP.AutoSize = true;
            this.rdoNonVIP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.rdoNonVIP.Location = new System.Drawing.Point(244, 37);
            this.rdoNonVIP.Name = "rdoNonVIP";
            this.rdoNonVIP.Size = new System.Drawing.Size(117, 17);
            this.rdoNonVIP.TabIndex = 6;
            this.rdoNonVIP.Text = "Non-VIP Customers";
            this.rdoNonVIP.UseVisualStyleBackColor = true;
            this.rdoNonVIP.CheckedChanged += new System.EventHandler(this.rdoNonVIP_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoMemberId);
            this.groupBox1.Controls.Add(this.btnClearSearch);
            this.groupBox1.Controls.Add(this.lblSearchTitle);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.rdoCustomerName);
            this.groupBox1.Controls.Add(this.rdoPhoneNumber);
            this.groupBox1.Controls.Add(this.rdoEmail);
            this.groupBox1.Controls.Add(this.rdoNIRC);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(55)))), ((int)(((byte)(46)))));
            this.groupBox1.Location = new System.Drawing.Point(12, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(903, 100);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search By";
            // 
            // rdoMemberId
            // 
            this.rdoMemberId.AutoSize = true;
            this.rdoMemberId.Checked = true;
            this.rdoMemberId.Location = new System.Drawing.Point(34, 31);
            this.rdoMemberId.Name = "rdoMemberId";
            this.rdoMemberId.Size = new System.Drawing.Size(77, 17);
            this.rdoMemberId.TabIndex = 17;
            this.rdoMemberId.TabStop = true;
            this.rdoMemberId.Text = "Member ID";
            this.rdoMemberId.UseVisualStyleBackColor = true;
            this.rdoMemberId.CheckedChanged += new System.EventHandler(this.rdoMemberId_CheckedChanged);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnClearSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnClearSearch.FlatAppearance.BorderSize = 0;
            this.btnClearSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnClearSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnClearSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSearch.Image = global::POS.Properties.Resources.refresh_small;
            this.btnClearSearch.Location = new System.Drawing.Point(494, 60);
            this.btnClearSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(75, 33);
            this.btnClearSearch.TabIndex = 16;
            this.btnClearSearch.UseVisualStyleBackColor = false;
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // lblSearchTitle
            // 
            this.lblSearchTitle.AutoSize = true;
            this.lblSearchTitle.Location = new System.Drawing.Point(38, 67);
            this.lblSearchTitle.Name = "lblSearchTitle";
            this.lblSearchTitle.Size = new System.Drawing.Size(59, 13);
            this.lblSearchTitle.TabIndex = 15;
            this.lblSearchTitle.Text = "Member ID";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Image = global::POS.Properties.Resources.search_small;
            this.btnSearch.Location = new System.Drawing.Point(400, 60);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 33);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(127, 67);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(262, 20);
            this.txtSearch.TabIndex = 5;
            // 
            // rdoCustomerName
            // 
            this.rdoCustomerName.AutoSize = true;
            this.rdoCustomerName.Location = new System.Drawing.Point(144, 31);
            this.rdoCustomerName.Name = "rdoCustomerName";
            this.rdoCustomerName.Size = new System.Drawing.Size(100, 17);
            this.rdoCustomerName.TabIndex = 0;
            this.rdoCustomerName.Text = "Customer Name";
            this.rdoCustomerName.UseVisualStyleBackColor = true;
            this.rdoCustomerName.CheckedChanged += new System.EventHandler(this.rdoCustomerName_CheckedChanged);
            // 
            // rdoPhoneNumber
            // 
            this.rdoPhoneNumber.AutoSize = true;
            this.rdoPhoneNumber.Location = new System.Drawing.Point(284, 31);
            this.rdoPhoneNumber.Name = "rdoPhoneNumber";
            this.rdoPhoneNumber.Size = new System.Drawing.Size(96, 17);
            this.rdoPhoneNumber.TabIndex = 1;
            this.rdoPhoneNumber.Text = "Phone Number";
            this.rdoPhoneNumber.UseVisualStyleBackColor = true;
            this.rdoPhoneNumber.CheckedChanged += new System.EventHandler(this.rdoPhoneNumber_CheckedChanged);
            // 
            // rdoEmail
            // 
            this.rdoEmail.AutoSize = true;
            this.rdoEmail.Location = new System.Drawing.Point(519, 31);
            this.rdoEmail.Name = "rdoEmail";
            this.rdoEmail.Size = new System.Drawing.Size(50, 17);
            this.rdoEmail.TabIndex = 3;
            this.rdoEmail.Text = "Email";
            this.rdoEmail.UseVisualStyleBackColor = true;
            this.rdoEmail.CheckedChanged += new System.EventHandler(this.rdoEmail_CheckedChanged);
            // 
            // rdoNIRC
            // 
            this.rdoNIRC.AutoSize = true;
            this.rdoNIRC.Location = new System.Drawing.Point(424, 31);
            this.rdoNIRC.Name = "rdoNIRC";
            this.rdoNIRC.Size = new System.Drawing.Size(51, 17);
            this.rdoNIRC.TabIndex = 2;
            this.rdoNIRC.Text = "NRIC";
            this.rdoNIRC.UseVisualStyleBackColor = true;
            this.rdoNIRC.CheckedChanged += new System.EventHandler(this.rdoNIRC_CheckedChanged);
            // 
            // btnAddNewCustomer
            // 
            this.btnAddNewCustomer.BackColor = System.Drawing.Color.Transparent;
            this.btnAddNewCustomer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnAddNewCustomer.FlatAppearance.BorderSize = 0;
            this.btnAddNewCustomer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnAddNewCustomer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnAddNewCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNewCustomer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAddNewCustomer.Image = global::POS.Properties.Resources.newcustomer_130x36_;
            this.btnAddNewCustomer.Location = new System.Drawing.Point(774, 22);
            this.btnAddNewCustomer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddNewCustomer.Name = "btnAddNewCustomer";
            this.btnAddNewCustomer.Size = new System.Drawing.Size(141, 46);
            this.btnAddNewCustomer.TabIndex = 3;
            this.btnAddNewCustomer.UseVisualStyleBackColor = false;
            this.btnAddNewCustomer.Click += new System.EventHandler(this.btnAddNewCustomer_Click);
            // 
            // dgvCustomerList
            // 
            this.dgvCustomerList.AllowUserToAddRows = false;
            this.dgvCustomerList.AllowUserToResizeColumns = false;
            this.dgvCustomerList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvCustomerList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomerList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column6,
            this.Column10,
            this.Column1,
            this.Column2,
            this.Column5,
            this.Column8,
            this.Column3,
            this.Column9,
            this.Column4});
            this.dgvCustomerList.Location = new System.Drawing.Point(12, 183);
            this.dgvCustomerList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvCustomerList.Name = "dgvCustomerList";
            this.dgvCustomerList.Size = new System.Drawing.Size(975, 410);
            this.dgvCustomerList.TabIndex = 8;
            this.dgvCustomerList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomerList_CellClick);
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "Id";
            this.Column6.HeaderText = "ID";
            this.Column6.Name = "Column6";
            this.Column6.Visible = false;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "VIPMemberId";
            this.Column10.HeaderText = "MemberId";
            this.Column10.Name = "Column10";
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Name";
            this.Column1.HeaderText = "Customer Name";
            this.Column1.Name = "Column1";
            this.Column1.Width = 180;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "PhoneNumber";
            this.Column2.HeaderText = "Phone Number";
            this.Column2.Name = "Column2";
            this.Column2.Width = 120;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "Email";
            this.Column5.HeaderText = "Email";
            this.Column5.Name = "Column5";
            this.Column5.Width = 150;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "NRC";
            this.Column8.HeaderText = "NRIC";
            this.Column8.Name = "Column8";
            this.Column8.Width = 120;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "";
            this.Column3.Name = "Column3";
            this.Column3.Text = "Detail";
            this.Column3.UseColumnTextForLinkValue = true;
            this.Column3.VisitedLinkColor = System.Drawing.Color.Blue;
            this.Column3.Width = 80;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "";
            this.Column9.Name = "Column9";
            this.Column9.Text = "Edit";
            this.Column9.UseColumnTextForLinkValue = true;
            this.Column9.Width = 80;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "";
            this.Column4.Name = "Column4";
            this.Column4.Text = "Delete";
            this.Column4.UseColumnTextForLinkValue = true;
            this.Column4.Width = 80;
            // 
            // createdshop
            // 
            this.createdshop.Font = new System.Drawing.Font("Zawgyi-One", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createdshop.FormattingEnabled = true;
            this.createdshop.Location = new System.Drawing.Point(95, 19);
            this.createdshop.Name = "createdshop";
            this.createdshop.Size = new System.Drawing.Size(204, 26);
            this.createdshop.TabIndex = 9;
            this.createdshop.SelectedIndexChanged += new System.EventHandler(this.createdshop_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "VipStartedIn";
            // 
            // btn_fix
            // 
            this.btn_fix.Location = new System.Drawing.Point(933, 32);
            this.btn_fix.Name = "btn_fix";
            this.btn_fix.Size = new System.Drawing.Size(66, 31);
            this.btn_fix.TabIndex = 18;
            this.btn_fix.Text = "Fix Data";
            this.btn_fix.UseVisualStyleBackColor = true;
            this.btn_fix.Click += new System.EventHandler(this.btn_fix_Click);
            // 
            // groupvipstarted
            // 
            this.groupvipstarted.Controls.Add(this.label1);
            this.groupvipstarted.Controls.Add(this.createdshop);
            this.groupvipstarted.Location = new System.Drawing.Point(386, 8);
            this.groupvipstarted.Name = "groupvipstarted";
            this.groupvipstarted.Size = new System.Drawing.Size(325, 56);
            this.groupvipstarted.TabIndex = 18;
            this.groupvipstarted.TabStop = false;
            this.groupvipstarted.Text = "select shop";
            this.groupvipstarted.Visible = false;
            // 
            // Loc_CustomerList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.ClientSize = new System.Drawing.Size(1021, 606);
            this.Controls.Add(this.groupvipstarted);
            this.Controls.Add(this.btn_fix);
            this.Controls.Add(this.dgvCustomerList);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rdoNonVIP);
            this.Controls.Add(this.rdoVIP);
            this.Controls.Add(this.rdoAll);
            this.Controls.Add(this.btnAddNewCustomer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Loc_CustomerList";
            this.Text = "Customer List";
            this.Load += new System.EventHandler(this.CustomerList_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerList)).EndInit();
            this.groupvipstarted.ResumeLayout(false);
            this.groupvipstarted.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddNewCustomer;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.RadioButton rdoVIP;
        private System.Windows.Forms.RadioButton rdoNonVIP;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblSearchTitle;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.RadioButton rdoCustomerName;
        private System.Windows.Forms.RadioButton rdoPhoneNumber;
        private System.Windows.Forms.RadioButton rdoEmail;
        private System.Windows.Forms.RadioButton rdoNIRC;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.DataGridView dgvCustomerList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewLinkColumn Column3;
        private System.Windows.Forms.DataGridViewLinkColumn Column9;
        private System.Windows.Forms.DataGridViewLinkColumn Column4;
        private System.Windows.Forms.RadioButton rdoMemberId;
        private System.Windows.Forms.ComboBox createdshop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_fix;
        private System.Windows.Forms.GroupBox groupvipstarted;
    }
}