namespace POS
{
    partial class Loc_RedeemPoint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loc_RedeemPoint));
            this.cboRedeemPointAmount = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotalPoint = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblGiftAmount = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTotalGiftCard = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.giftcertificate = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btnRedeem = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboRedeemPointAmount
            // 
            this.cboRedeemPointAmount.FormattingEnabled = true;
            this.cboRedeemPointAmount.Items.AddRange(new object[] {
            "12 points",
            "20 points",
            "30 points",
            "50 points",
            "80 points"});
            this.cboRedeemPointAmount.Location = new System.Drawing.Point(167, 36);
            this.cboRedeemPointAmount.Name = "cboRedeemPointAmount";
            this.cboRedeemPointAmount.Size = new System.Drawing.Size(106, 21);
            this.cboRedeemPointAmount.TabIndex = 0;
            this.cboRedeemPointAmount.SelectedIndexChanged += new System.EventHandler(this.cboRedeemPointAmount_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Redeem Point";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Point Left";
            // 
            // lblTotalPoint
            // 
            this.lblTotalPoint.AutoSize = true;
            this.lblTotalPoint.Location = new System.Drawing.Point(167, 0);
            this.lblTotalPoint.Name = "lblTotalPoint";
            this.lblTotalPoint.Size = new System.Drawing.Size(34, 13);
            this.lblTotalPoint.TabIndex = 3;
            this.lblTotalPoint.Text = "Point ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(22, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Redeem point for ";
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerName.Location = new System.Drawing.Point(131, 22);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(95, 13);
            this.lblCustomerName.TabIndex = 5;
            this.lblCustomerName.Text = "Customer Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Gift Amount";
            // 
            // lblGiftAmount
            // 
            this.lblGiftAmount.AutoSize = true;
            this.lblGiftAmount.Location = new System.Drawing.Point(167, 66);
            this.lblGiftAmount.Name = "lblGiftAmount";
            this.lblGiftAmount.Size = new System.Drawing.Size(43, 13);
            this.lblGiftAmount.TabIndex = 7;
            this.lblGiftAmount.Text = "12,000 ";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lblTotalGiftCard, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label8, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.giftcertificate, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label13, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblGiftAmount, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblTotalPoint, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.cboRedeemPointAmount, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(25, 63);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 133);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // lblTotalGiftCard
            // 
            this.lblTotalGiftCard.AutoSize = true;
            this.lblTotalGiftCard.Location = new System.Drawing.Point(167, 99);
            this.lblTotalGiftCard.Name = "lblTotalGiftCard";
            this.lblTotalGiftCard.Size = new System.Drawing.Size(13, 13);
            this.lblTotalGiftCard.TabIndex = 9;
            this.lblTotalGiftCard.Text = "1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(141, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(10, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = ":";
            // 
            // giftcertificate
            // 
            this.giftcertificate.AutoSize = true;
            this.giftcertificate.Location = new System.Drawing.Point(3, 99);
            this.giftcertificate.Name = "giftcertificate";
            this.giftcertificate.Size = new System.Drawing.Size(109, 13);
            this.giftcertificate.TabIndex = 20;
            this.giftcertificate.Text = "Gift Certificate(12000)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(141, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = ":";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(141, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = ":";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(141, 66);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(10, 13);
            this.label13.TabIndex = 17;
            this.label13.Text = ":";
            // 
            // btnRedeem
            // 
            this.btnRedeem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.btnRedeem.FlatAppearance.BorderSize = 0;
            this.btnRedeem.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnRedeem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnRedeem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRedeem.Image = global::POS.Properties.Resources.redeem;
            this.btnRedeem.Location = new System.Drawing.Point(267, 202);
            this.btnRedeem.Name = "btnRedeem";
            this.btnRedeem.Size = new System.Drawing.Size(88, 32);
            this.btnRedeem.TabIndex = 9;
            this.btnRedeem.UseVisualStyleBackColor = true;
            this.btnRedeem.Click += new System.EventHandler(this.btnRedeem_Click);
            // 
            // Loc_RedeemPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(202)))), ((int)(((byte)(125)))));
            this.ClientSize = new System.Drawing.Size(389, 258);
            this.Controls.Add(this.btnRedeem);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblCustomerName);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Loc_RedeemPoint";
            this.Text = "Loc_RedeemPoint";
            this.Load += new System.EventHandler(this.Loc_RedeemPoint_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboRedeemPointAmount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalPoint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblGiftAmount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblTotalGiftCard;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label giftcertificate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnRedeem;
    }
}