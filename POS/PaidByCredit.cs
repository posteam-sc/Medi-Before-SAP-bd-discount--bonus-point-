using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using POS.APP_Data;

namespace POS
{
    public partial class PaidByCredit : Form
    {
        #region Variables

        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        public List<GiftSystem> GiftList = new List<GiftSystem>();

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int ExtraTax { get; set; }

        public int? CustomerId { get; set; }

        public Boolean isDraft { get; set; }

        public string DraftId { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        //private long outstandingBalance = 0;
        long OldOutstandingAmount = 0;
        int PrepaidDebt = 0;

        #endregion
        
        #region Events

        public PaidByCredit()
        {
            InitializeComponent();
        }

        private void PaidByCredit_Load(object sender, EventArgs e)
        {
            List<Customer> CustomerList = new List<Customer>();
            Customer none = new Customer();
            none.Name = "Select Customer";
            none.Id = 0;
            CustomerList.Add(none);
            CustomerList.AddRange(entity.Customers.ToList());
            cboCustomerList.DataSource = CustomerList;
            cboCustomerList.DisplayMember = "Name";
            cboCustomerList.ValueMember = "Id";
            cboCustomerList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCustomerList.AutoCompleteSource = AutoCompleteSource.ListItems;
            if (CustomerId != null)
            {
                cboCustomerList.Text = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault().Name;
            }


            lblTotalCost.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax).ToString();
            lblAccuralCost.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax).ToString();
            lblNetPayable.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax).ToString();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;
            
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            if (cboCustomerList.SelectedIndex == 0)
            {
                tp.SetToolTip(cboCustomerList, "Error");
                tp.Show("Please select customer name!", cboCustomerList);
                hasError = true;
            }

            else if (txtReceiveAmount.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtReceiveAmount, "Error");
                tp.Show("Please fill up receive amount!", txtReceiveAmount);
                hasError = true;
            }

            if (!hasError)
            {
                long receiveAmount = 0;
                Int64.TryParse(txtReceiveAmount.Text, out receiveAmount);
                if (chkIsPrePaid.Checked)
                {
                    receiveAmount += Convert.ToInt64(lblPrePaid.Text);
                }
                int customerId = 0;
                Int32.TryParse(cboCustomerList.SelectedValue.ToString(), out customerId);
                
                //set old credit transaction record to paid coz this transaction store old outstanding amount
                Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
                List<Transaction> transList = cust.Transactions.Where(unpaid => unpaid.IsPaid == false).ToList();
                List<Transaction> prePaidTranList = cust.Transactions.Where(type => type.Type == TransactionType.Prepaid).Where(active => active.IsActive == false).ToList();
                //foreach (Transaction ts in transList)
                //{
                //    //ts.IsPaid = true;
                //}               

                System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount, receiveAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                
                string resultId = Id.FirstOrDefault().ToString();
                Transaction insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();


                foreach (TransactionDetail detail in DetailList)
                {
                    detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                    detail.Product.Qty = detail.Product.Qty - detail.Qty;
                    insertedTransaction.TransactionDetails.Add(detail);
                }

                //Add promotion gift records for this transaction
                if (GiftList.Count > 0)
                {
                    foreach (GiftSystem gs in GiftList)
                    {
                        AttachGiftSystemForTransaction agft = new AttachGiftSystemForTransaction();
                        agft.AttachGiftSystemId = gs.Id;
                        agft.TransactionId = insertedTransaction.Id;
                        entity.AttachGiftSystemForTransactions.Add(agft);
                    }
                }

                bool prepaidCredit = false;
                #region current transation paid and remain amount is paid other transaction
                if (insertedTransaction.TotalAmount <= receiveAmount)
                {
                    insertedTransaction.IsPaid = true;
                    receiveAmount = receiveAmount - (long)insertedTransaction.TotalAmount;
                    prepaidCredit = true;
                    long tamount = 0;
                    long differentAmount = receiveAmount;
                    List<Transaction> tList = new List<Transaction>();
                    List<Transaction> RefundList = new List<Transaction>();
                    if (differentAmount != 0)
                    {
                        foreach (Transaction t in transList)
                        {
                            tamount = (long)t.TotalAmount - (long)t.RecieveAmount;
                            RefundList = (from tr in entity.Transactions where tr.ParentId == t.Id && tr.Type == TransactionType.CreditRefund select tr).ToList();
                            if (RefundList.Count > 0)
                            {
                                foreach (Transaction TRefund in RefundList)
                                {
                                    tamount -= (long)TRefund.RecieveAmount;
                                }
                            }
                            if (tamount <= differentAmount)
                            {
                                tList.Add(t);
                                differentAmount -= tamount;
                            }

                        }
                        int index = tList.Count;
                        for (int outer = index - 1; outer >= 1; outer--)
                        {
                            for (int inner = 0; inner < outer; inner++)
                            {
                                if (tList[inner].TotalAmount - tList[inner].RecieveAmount < tList[inner + 1].TotalAmount - tList[inner + 1].RecieveAmount)
                                {
                                    Transaction t = tList[inner];
                                    tList[inner] = tList[inner + 1];
                                    tList[inner + 1] = t;
                                }
                            }
                        }
                        foreach (Transaction tranObj in tList)
                        {
                            tamount = (long)tranObj.TotalAmount - (long)tranObj.RecieveAmount;
                            RefundList = (from tr in entity.Transactions where tr.ParentId == tranObj.Id && tr.Type == TransactionType.CreditRefund select tr).ToList();
                            if (RefundList.Count > 0)
                            {
                                foreach (Transaction TRefund in RefundList)
                                {
                                    tamount -= (long)TRefund.RecieveAmount;
                                }
                            }
                            if (tamount <= receiveAmount)
                            {
                                tranObj.IsPaid = true;
                                receiveAmount -= tamount;
                            }
                        }
                    }
                }
                 #endregion
                else
                {
                    receiveAmount = 0;                  
                }
                //set isactive true when prepaid transactions is paided
                foreach (Transaction preTrans in prePaidTranList)
                {
                    preTrans.IsActive = true;
                }
                if (isDraft)
                {
                    Transaction draft = (from trans in entity.Transactions where trans.Id == DraftId select trans).FirstOrDefault<Transaction>();
                    draft.TransactionDetails.Clear();
                    var Detail = entity.TransactionDetails.Where(d => d.TransactionId == draft.Id);
                    foreach (var d in Detail)
                    {
                        entity.TransactionDetails.Remove(d);
                    }
                    entity.Transactions.Remove(draft);
                }               
                entity.SaveChanges();
                if (receiveAmount > 0 && prepaidCredit == true)
                {
                    System.Data.Objects.ObjectResult<String> PrePaidId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, null, null, receiveAmount, receiveAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                    entity.SaveChanges();
                }
               
                //Print Invoice
                #region [ Print ]

                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                int _tAmt = 0;
                foreach (TransactionDetail transaction in DetailList)
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    newRow.DiscountPercent = transaction.DiscountRate.ToString();
                    //newRow.TotalAmount = (int)transaction.TotalAmount;
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; 
                    newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                    _tAmt += newRow.TotalAmount;
                    dtReport.AddItemListRow(newRow);
                }

                string reportPath = "";
                ReportViewer rv = new ReportViewer();
                ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                reportPath = Application.StartupPath + "\\Reports\\InvoiceCredit.rdlc";
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);

                ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
                rv.LocalReport.SetParameters(TAmt);

                ReportParameter ShopName = new ReportParameter("ShopName", SettingController.DefaultShop.ShopName);
                rv.LocalReport.SetParameters(ShopName);

                ReportParameter BranchName = new ReportParameter("BranchName", SettingController.DefaultShop.Address);
                rv.LocalReport.SetParameters(BranchName);

                ReportParameter Phone = new ReportParameter("Phone", SettingController.DefaultShop.PhoneNumber);
                rv.LocalReport.SetParameters(Phone);

                ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.DefaultShop.OpeningHours);
                rv.LocalReport.SetParameters(OpeningHours);

                ReportParameter TransactionId = new ReportParameter("TransactionId", resultId.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter counter = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                ReportParameter CounterName = new ReportParameter("CounterName", counter.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                rv.LocalReport.SetParameters(PrintDateTime);

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);

                ReportParameter TotalAmount = new ReportParameter("TotalAmount", insertedTransaction.TotalAmount.ToString());
                rv.LocalReport.SetParameters(TotalAmount);

                ReportParameter TaxAmount = new ReportParameter("TaxAmount", insertedTransaction.TaxAmount.ToString());
                rv.LocalReport.SetParameters(TaxAmount);

                ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", insertedTransaction.DiscountAmount.ToString());
                rv.LocalReport.SetParameters(DiscountAmount);

                ReportParameter PaidAmount = new ReportParameter("PaidAmount", txtReceiveAmount.Text);
                rv.LocalReport.SetParameters(PaidAmount);

                ReportParameter PrevOutstanding = new ReportParameter("PrevOutstanding", lblPreviousBalance.Text);
                rv.LocalReport.SetParameters(PrevOutstanding);

                ReportParameter PrePaidDebt = new ReportParameter("PrePaidDebt", lblPrePaid.Text);
                rv.LocalReport.SetParameters(PrePaidDebt);

                ReportParameter NetPayable = new ReportParameter("NetPayable", (OldOutstandingAmount + insertedTransaction.TotalAmount).ToString());
                rv.LocalReport.SetParameters(NetPayable);

                ReportParameter Balance = new ReportParameter("Balance", ((OldOutstandingAmount + insertedTransaction.TotalAmount) - insertedTransaction.RecieveAmount).ToString());
                rv.LocalReport.SetParameters(Balance);

                ReportParameter CustomerName = new ReportParameter("CustomerName", cboCustomerList.Text );
                rv.LocalReport.SetParameters(CustomerName);

                PrintDoc.PrintReport(rv, "Slip");
                #endregion

                MessageBox.Show("Payment Completed");
                if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                {
                    Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                    newForm.Clear();
                }
                this.Dispose();
            }
        }      
       
        private void cboCustomerList_SelectedIndexChanged(object sender, EventArgs e)        
        {
            if (cboCustomerList.SelectedIndex != 0)
            {
                //get remaining outstanding amount
                    int customerId = Convert.ToInt32(cboCustomerList.SelectedValue.ToString());
                List<Transaction> rtList = new List<Transaction>();
                Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();
                List<Transaction> OldOutStandingList = entity.Transactions.Where(x => x.CustomerId == cust.Id).ToList().Where(x => x.IsDeleted != true).ToList();
                OldOutstandingAmount = 0; 
                foreach (Transaction ts in OldOutStandingList)
                {
                    if (ts.IsPaid == false)
                    {
                        OldOutstandingAmount += (long)ts.TotalAmount - (long)ts.RecieveAmount;
                        rtList = (from t in entity.Transactions where t.Type == TransactionType.CreditRefund && t.ParentId == ts.Id select t).ToList();
                        if (rtList.Count > 0)
                        {
                            foreach (Transaction rt in rtList)
                            {
                                OldOutstandingAmount -= (int)rt.RecieveAmount;
                            }
                        }
                    }
                    if (ts.Type == TransactionType.Prepaid && ts.IsActive == false)
                    {
                        PrepaidDebt += (int)ts.RecieveAmount;
                    }
                }


                if (OldOutstandingAmount < 0)
                {
                    lblPreviousBalance.Text = "0";
                    lblPrePaid.Text = PrepaidDebt.ToString();
                    lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) ).ToString();
                    lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) ).ToString();
                }
                else
                {
                    lblPreviousBalance.Text = OldOutstandingAmount.ToString();
                    lblPrePaid.Text = PrepaidDebt.ToString();
                    lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) + OldOutstandingAmount).ToString();
                    lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) + OldOutstandingAmount).ToString();
                }
                //to 
                int amount = 0;
                Int32.TryParse(txtReceiveAmount.Text, out amount);
                int totalCost = 0;
                Int32.TryParse(lblTotalCost.Text, out totalCost);

                if (amount >= totalCost)
                {
                    lblNetPayableTitle.Text = "Change";
                    lblNetPayable.Text = (amount - totalCost).ToString();
                }
                else
                {
                    lblNetPayable.Text = (totalCost - amount).ToString();
                }
            }
            else
            {
                lblPreviousBalance.Text = "0";
                lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) + 0).ToString();
                lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) + 0).ToString();

                //to 
                int amount = 0;
                Int32.TryParse(txtReceiveAmount.Text, out amount);

                if (amount >= (Int32.Parse(lblTotalCost.Text)))
                {
                    lblNetPayableTitle.Text = "Change";
                    lblNetPayable.Text = (amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax)).ToString();
                }
                else
                {
                    lblNetPayableTitle.Text = "NetPayable";
                    lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) - amount).ToString();
                }
            }
        }

        private void PaidByCredit_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(cboCustomerList);
            tp.Hide(txtReceiveAmount);
        }

        private void txtReceiveAmount_KeyUp(object sender, KeyEventArgs e)
        {
            int amount = 0;
            Int32.TryParse(txtReceiveAmount.Text, out amount);
            int totalCost = 0;
            Int32.TryParse(lblTotalCost.Text, out totalCost);

            if (amount >= totalCost)
            {
                lblNetPayableTitle.Text = "Change";
                lblNetPayable.Text = (amount - totalCost).ToString();
            }
            else
            {
                lblNetPayableTitle.Text = "NetPayable";
                lblNetPayable.Text = (totalCost - amount).ToString();
            }
          
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            NewCustomer newform = new NewCustomer();
            newform.Show();
        }

        //private void PaidByCredit_Activated(object sender, EventArgs e)
        //{
            //List<Customer> CustomerList = new List<Customer>();
            //Customer none = new Customer();
            //none.Name = "Select Customer";
            //none.Id = 0;
            //CustomerList.Add(none);
            //CustomerList.AddRange(entity.Customers.ToList());
            //cboCustomerList.DataSource = CustomerList;
            //cboCustomerList.DisplayMember = "Name";
            //cboCustomerList.ValueMember = "Id";
            //cboCustomerList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cboCustomerList.AutoCompleteSource = AutoCompleteSource.ListItems;
        //}
        #endregion

        #region Function

        public void LoadForm()
        {
            List<Customer> CustomerList = new List<Customer>();
            Customer none = new Customer();
            none.Name = "Select Customer";
            none.Id = 0;
            CustomerList.Add(none);
            CustomerList.AddRange(entity.Customers.ToList());
            cboCustomerList.DataSource = CustomerList;
            cboCustomerList.DisplayMember = "Name";
            cboCustomerList.ValueMember = "Id";
            cboCustomerList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboCustomerList.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        #endregion

        private void txtReceiveAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void chkIsPrePaid_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
