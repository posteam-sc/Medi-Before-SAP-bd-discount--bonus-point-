namespace POS
{
    partial class TransactionDetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransactionDetailForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvTransactionDetail = new System.Windows.Forms.DataGridView();
            this.tlpCash = new System.Windows.Forms.TableLayoutPanel();
            this.lblTotalTax = new System.Windows.Forms.Label();
            this.lblAmountFromGiftcardTitle = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblDis = new System.Windows.Forms.Label();
            this.lblDiscount = new System.Windows.Forms.Label();
            this.lblt = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblPaymentMethod1 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblR = new System.Windows.Forms.Label();
            this.lblRecieveAmunt = new System.Windows.Forms.Label();
            this.lblAmountFromGiftCard = new System.Windows.Forms.Label();
            this.lblSalePerson = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.tlpCredit = new System.Windows.Forms.TableLayoutPanel();
            this.lblOutstandingAmount = new System.Windows.Forms.Label();
            this.lblPrevTitle = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.lblCounter = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cboCustomer = new System.Windows.Forms.ComboBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lbAdvanceSearch = new System.Windows.Forms.LinkLabel();
            this.dgvPaymentList = new System.Windows.Forms.DataGridView();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColProductCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDiscountPercent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTaxPercent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDelete = new System.Windows.Forms.DataGridViewLinkColumn();
            this.ColId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDPR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactionDetail)).BeginInit();
            this.tlpCash.SuspendLayout();
            this.tlpCredit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaymentList)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(23, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sale Person :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(648, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Date :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(647, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Time :";
            // 
            // dgvTransactionDetail
            // 
            this.dgvTransactionDetail.AllowUserToAddRows = false;
            this.dgvTransactionDetail.AllowUserToResizeColumns = false;
            this.dgvTransactionDetail.AllowUserToResizeRows = false;
            this.dgvTransactionDetail.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvTransactionDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTransactionDetail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColProductCode,
            this.ColItemName,
            this.ColQty,
            this.ColUnitPrice,
            this.ColDiscountPercent,
            this.ColTaxPercent,
            this.ColCost,
            this.ColDelete,
            this.ColId,
            this.ColDPR});
            this.dgvTransactionDetail.Location = new System.Drawing.Point(22, 119);
            this.dgvTransactionDetail.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.dgvTransactionDetail.Name = "dgvTransactionDetail";
            this.dgvTransactionDetail.RowHeadersVisible = false;
            this.dgvTransactionDetail.Size = new System.Drawing.Size(1075, 273);
            this.dgvTransactionDetail.TabIndex = 3;
            this.dgvTransactionDetail.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTransactionDetail_CellClick);
            this.dgvTransactionDetail.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvTransactionDetail_DataBindingComplete);
            // 
            // tlpCash
            // 
            this.tlpCash.BackColor = System.Drawing.Color.Transparent;
            this.tlpCash.ColumnCount = 2;
            this.tlpCash.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.43283F));
            this.tlpCash.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.56717F));
            this.tlpCash.Controls.Add(this.lblTotalTax, 1, 3);
            this.tlpCash.Controls.Add(this.lblAmountFromGiftcardTitle, 0, 5);
            this.tlpCash.Controls.Add(this.label4, 0, 3);
            this.tlpCash.Controls.Add(this.lblDis, 0, 2);
            this.tlpCash.Controls.Add(this.lblDiscount, 1, 2);
            this.tlpCash.Controls.Add(this.lblt, 0, 1);
            this.tlpCash.Controls.Add(this.label10, 0, 0);
            this.tlpCash.Controls.Add(this.lblPaymentMethod1, 1, 0);
            this.tlpCash.Controls.Add(this.lblTotal, 1, 1);
            this.tlpCash.Controls.Add(this.lblR, 0, 4);
            this.tlpCash.Controls.Add(this.lblRecieveAmunt, 1, 4);
            this.tlpCash.Controls.Add(this.lblAmountFromGiftCard, 1, 5);
            this.tlpCash.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tlpCash.Location = new System.Drawing.Point(16, 414);
            this.tlpCash.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tlpCash.Name = "tlpCash";
            this.tlpCash.RowCount = 6;
            this.tlpCash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpCash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpCash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpCash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpCash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpCash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpCash.Size = new System.Drawing.Size(453, 154);
            this.tlpCash.TabIndex = 4;
            this.tlpCash.Visible = false;
            // 
            // lblTotalTax
            // 
            this.lblTotalTax.AutoSize = true;
            this.lblTotalTax.Location = new System.Drawing.Point(177, 75);
            this.lblTotalTax.Name = "lblTotalTax";
            this.lblTotalTax.Size = new System.Drawing.Size(11, 15);
            this.lblTotalTax.TabIndex = 13;
            this.lblTotalTax.Text = "-";
            // 
            // lblAmountFromGiftcardTitle
            // 
            this.lblAmountFromGiftcardTitle.AutoSize = true;
            this.lblAmountFromGiftcardTitle.Location = new System.Drawing.Point(3, 125);
            this.lblAmountFromGiftcardTitle.Name = "lblAmountFromGiftcardTitle";
            this.lblAmountFromGiftcardTitle.Size = new System.Drawing.Size(132, 15);
            this.lblAmountFromGiftcardTitle.TabIndex = 14;
            this.lblAmountFromGiftcardTitle.Text = "Amount From Giftcards";
            this.lblAmountFromGiftcardTitle.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Tax Amount";
            // 
            // lblDis
            // 
            this.lblDis.AutoSize = true;
            this.lblDis.Location = new System.Drawing.Point(3, 50);
            this.lblDis.Name = "lblDis";
            this.lblDis.Size = new System.Drawing.Size(100, 15);
            this.lblDis.TabIndex = 1;
            this.lblDis.Text = "Discount Amount";
            // 
            // lblDiscount
            // 
            this.lblDiscount.AutoSize = true;
            this.lblDiscount.Location = new System.Drawing.Point(177, 50);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(11, 15);
            this.lblDiscount.TabIndex = 4;
            this.lblDiscount.Text = "-";
            // 
            // lblt
            // 
            this.lblt.AutoSize = true;
            this.lblt.Location = new System.Drawing.Point(3, 25);
            this.lblt.Name = "lblt";
            this.lblt.Size = new System.Drawing.Size(34, 15);
            this.lblt.TabIndex = 0;
            this.lblt.Text = "Total";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 15);
            this.label10.TabIndex = 6;
            this.label10.Text = "Payment Method";
            // 
            // lblPaymentMethod1
            // 
            this.lblPaymentMethod1.AutoSize = true;
            this.lblPaymentMethod1.Location = new System.Drawing.Point(177, 0);
            this.lblPaymentMethod1.Name = "lblPaymentMethod1";
            this.lblPaymentMethod1.Size = new System.Drawing.Size(11, 15);
            this.lblPaymentMethod1.TabIndex = 7;
            this.lblPaymentMethod1.Text = "-";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(177, 25);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(11, 15);
            this.lblTotal.TabIndex = 5;
            this.lblTotal.Text = "-";
            // 
            // lblR
            // 
            this.lblR.AutoSize = true;
            this.lblR.Location = new System.Drawing.Point(3, 100);
            this.lblR.Name = "lblR";
            this.lblR.Size = new System.Drawing.Size(96, 15);
            this.lblR.TabIndex = 2;
            this.lblR.Text = "Recived Amount";
            // 
            // lblRecieveAmunt
            // 
            this.lblRecieveAmunt.AutoSize = true;
            this.lblRecieveAmunt.Location = new System.Drawing.Point(177, 100);
            this.lblRecieveAmunt.Name = "lblRecieveAmunt";
            this.lblRecieveAmunt.Size = new System.Drawing.Size(11, 15);
            this.lblRecieveAmunt.TabIndex = 3;
            this.lblRecieveAmunt.Text = "-";
            // 
            // lblAmountFromGiftCard
            // 
            this.lblAmountFromGiftCard.AutoSize = true;
            this.lblAmountFromGiftCard.Location = new System.Drawing.Point(177, 125);
            this.lblAmountFromGiftCard.Name = "lblAmountFromGiftCard";
            this.lblAmountFromGiftCard.Size = new System.Drawing.Size(11, 15);
            this.lblAmountFromGiftCard.TabIndex = 15;
            this.lblAmountFromGiftCard.Text = "-";
            this.lblAmountFromGiftCard.Visible = false;
            // 
            // lblSalePerson
            // 
            this.lblSalePerson.AutoSize = true;
            this.lblSalePerson.BackColor = System.Drawing.Color.Transparent;
            this.lblSalePerson.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSalePerson.Location = new System.Drawing.Point(150, 22);
            this.lblSalePerson.Name = "lblSalePerson";
            this.lblSalePerson.Size = new System.Drawing.Size(11, 15);
            this.lblSalePerson.TabIndex = 6;
            this.lblSalePerson.Text = "-";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.BackColor = System.Drawing.Color.Transparent;
            this.lblDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDate.Location = new System.Drawing.Point(697, 22);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(11, 15);
            this.lblDate.TabIndex = 7;
            this.lblDate.Text = "-";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.BackColor = System.Drawing.Color.Transparent;
            this.lblTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTime.Location = new System.Drawing.Point(696, 54);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(11, 15);
            this.lblTime.TabIndex = 8;
            this.lblTime.Text = "-";
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Image = global::POS.Properties.Resources.print_big;
            this.btnPrint.Location = new System.Drawing.Point(977, 404);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(111, 34);
            this.btnPrint.TabIndex = 9;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // tlpCredit
            // 
            this.tlpCredit.BackColor = System.Drawing.Color.Transparent;
            this.tlpCredit.ColumnCount = 2;
            this.tlpCredit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.40206F));
            this.tlpCredit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.59794F));
            this.tlpCredit.Controls.Add(this.lblOutstandingAmount, 1, 0);
            this.tlpCredit.Controls.Add(this.lblPrevTitle, 0, 0);
            this.tlpCredit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tlpCredit.Location = new System.Drawing.Point(16, 595);
            this.tlpCredit.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tlpCredit.Name = "tlpCredit";
            this.tlpCredit.RowCount = 2;
            this.tlpCredit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCredit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCredit.Size = new System.Drawing.Size(453, 54);
            this.tlpCredit.TabIndex = 10;
            this.tlpCredit.Visible = false;
            // 
            // lblOutstandingAmount
            // 
            this.lblOutstandingAmount.AutoSize = true;
            this.lblOutstandingAmount.Location = new System.Drawing.Point(176, 0);
            this.lblOutstandingAmount.Name = "lblOutstandingAmount";
            this.lblOutstandingAmount.Size = new System.Drawing.Size(11, 15);
            this.lblOutstandingAmount.TabIndex = 7;
            this.lblOutstandingAmount.Text = "-";
            // 
            // lblPrevTitle
            // 
            this.lblPrevTitle.AutoSize = true;
            this.lblPrevTitle.Location = new System.Drawing.Point(3, 0);
            this.lblPrevTitle.Name = "lblPrevTitle";
            this.lblPrevTitle.Size = new System.Drawing.Size(127, 15);
            this.lblPrevTitle.TabIndex = 6;
            this.lblPrevTitle.Text = "Used Prepaid Amount";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "Customer Name :";
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(429, 33);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(11, 15);
            this.lblCustomerName.TabIndex = 12;
            this.lblCustomerName.Text = "-";
            this.lblCustomerName.Visible = false;
            // 
            // lblCounter
            // 
            this.lblCounter.AutoSize = true;
            this.lblCounter.Location = new System.Drawing.Point(150, 50);
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.Size = new System.Drawing.Size(11, 15);
            this.lblCounter.TabIndex = 14;
            this.lblCounter.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Counter :";
            // 
            // cboCustomer
            // 
            this.cboCustomer.FormattingEnabled = true;
            this.cboCustomer.Location = new System.Drawing.Point(154, 75);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(181, 23);
            this.cboCustomer.TabIndex = 15;
            this.cboCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboCustomer_KeyDown);
            this.cboCustomer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboCustomer_KeyPress);
            // 
            // btnUpdate
            // 
            this.btnUpdate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnUpdate.FlatAppearance.BorderSize = 0;
            this.btnUpdate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnUpdate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Image = global::POS.Properties.Resources.update_small;
            this.btnUpdate.Location = new System.Drawing.Point(350, 77);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 16;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lbAdvanceSearch
            // 
            this.lbAdvanceSearch.AutoSize = true;
            this.lbAdvanceSearch.LinkColor = System.Drawing.SystemColors.ControlText;
            this.lbAdvanceSearch.Location = new System.Drawing.Point(440, 78);
            this.lbAdvanceSearch.Name = "lbAdvanceSearch";
            this.lbAdvanceSearch.Size = new System.Drawing.Size(95, 15);
            this.lbAdvanceSearch.TabIndex = 17;
            this.lbAdvanceSearch.TabStop = true;
            this.lbAdvanceSearch.Text = "Advance Search";
            this.lbAdvanceSearch.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbAdvanceSearch_LinkClicked);
            // 
            // dgvPaymentList
            // 
            this.dgvPaymentList.AllowUserToAddRows = false;
            this.dgvPaymentList.AllowUserToDeleteRows = false;
            this.dgvPaymentList.BackgroundColor = System.Drawing.Color.White;
            this.dgvPaymentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPaymentList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column10,
            this.Column11});
            this.dgvPaymentList.Location = new System.Drawing.Point(538, 414);
            this.dgvPaymentList.Name = "dgvPaymentList";
            this.dgvPaymentList.ReadOnly = true;
            this.dgvPaymentList.Size = new System.Drawing.Size(246, 150);
            this.dgvPaymentList.TabIndex = 18;
            this.dgvPaymentList.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvPaymentList_DataBindingComplete);
            // 
            // Column10
            // 
            this.Column10.HeaderText = "PaymentName";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            // 
            // Column11
            // 
            this.Column11.HeaderText = "Amount";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            // 
            // ColProductCode
            // 
            this.ColProductCode.HeaderText = "Product Code";
            this.ColProductCode.Name = "ColProductCode";
            this.ColProductCode.Width = 120;
            // 
            // ColItemName
            // 
            this.ColItemName.HeaderText = "Item Name";
            this.ColItemName.Name = "ColItemName";
            this.ColItemName.Width = 200;
            // 
            // ColQty
            // 
            this.ColQty.HeaderText = "Qty";
            this.ColQty.Name = "ColQty";
            // 
            // ColUnitPrice
            // 
            this.ColUnitPrice.HeaderText = "Unit Price";
            this.ColUnitPrice.Name = "ColUnitPrice";
            this.ColUnitPrice.Width = 120;
            // 
            // ColDiscountPercent
            // 
            this.ColDiscountPercent.HeaderText = "DiscountPercent";
            this.ColDiscountPercent.Name = "ColDiscountPercent";
            this.ColDiscountPercent.Width = 120;
            // 
            // ColTaxPercent
            // 
            this.ColTaxPercent.HeaderText = "Tax Percent";
            this.ColTaxPercent.Name = "ColTaxPercent";
            // 
            // ColCost
            // 
            this.ColCost.HeaderText = "Cost";
            this.ColCost.Name = "ColCost";
            this.ColCost.Width = 120;
            // 
            // ColDelete
            // 
            this.ColDelete.HeaderText = "";
            this.ColDelete.Name = "ColDelete";
            this.ColDelete.Text = "Delete";
            this.ColDelete.UseColumnTextForLinkValue = true;
            // 
            // ColId
            // 
            this.ColId.DataPropertyName = "Id";
            this.ColId.HeaderText = "Id";
            this.ColId.Name = "ColId";
            this.ColId.Visible = false;
            // 
            // ColDPR
            // 
            this.ColDPR.HeaderText = "DPR";
            this.ColDPR.Name = "ColDPR";
            this.ColDPR.Width = 90;
            // 
            // TransactionDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.ClientSize = new System.Drawing.Size(1100, 659);
            this.Controls.Add(this.dgvPaymentList);
            this.Controls.Add(this.lbAdvanceSearch);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.cboCustomer);
            this.Controls.Add(this.lblCounter);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblCustomerName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tlpCredit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblSalePerson);
            this.Controls.Add(this.tlpCash);
            this.Controls.Add(this.dgvTransactionDetail);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "TransactionDetailForm";
            this.Text = "Transaction Detail";
            this.Load += new System.EventHandler(this.TransactionDetailForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactionDetail)).EndInit();
            this.tlpCash.ResumeLayout(false);
            this.tlpCash.PerformLayout();
            this.tlpCredit.ResumeLayout(false);
            this.tlpCredit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaymentList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvTransactionDetail;
        private System.Windows.Forms.TableLayoutPanel tlpCash;
        private System.Windows.Forms.Label lblt;
        private System.Windows.Forms.Label lblDis;
        private System.Windows.Forms.Label lblR;
        private System.Windows.Forms.Label lblRecieveAmunt;
        private System.Windows.Forms.Label lblDiscount;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblSalePerson;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblPaymentMethod1;
        private System.Windows.Forms.TableLayoutPanel tlpCredit;
        private System.Windows.Forms.Label lblPrevTitle;
        private System.Windows.Forms.Label lblOutstandingAmount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.Label lblTotalTax;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCounter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblAmountFromGiftcardTitle;
        private System.Windows.Forms.Label lblAmountFromGiftCard;
        private System.Windows.Forms.ComboBox cboCustomer;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.LinkLabel lbAdvanceSearch;
        private System.Windows.Forms.DataGridView dgvPaymentList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColProductCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDiscountPercent;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTaxPercent;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCost;
        private System.Windows.Forms.DataGridViewLinkColumn ColDelete;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDPR;
    }
}