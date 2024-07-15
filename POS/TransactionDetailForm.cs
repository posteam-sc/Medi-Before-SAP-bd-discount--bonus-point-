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
    public partial class TransactionDetailForm : Form
    {
        #region Variable

        private POSEntities entity = new POSEntities();
        public string transactionId;
        int ExtraDiscount, ExtraTax;
        private int CustomerId = 0;
        long AmountFromGiftCards = 0;
        public bool IsDelelog = false;//zmh
        #endregion

        #region Event

        public TransactionDetailForm()
        {
            InitializeComponent();
        }

        private void TransactionDetailForm_Load(object sender, EventArgs e)
        {
            List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
            APP_Data.Customer customer = new APP_Data.Customer();
            customer.Id = 0;
            customer.Name = "None";
            customerList.Add(customer);
            customerList.AddRange(entity.Customers.OrderBy(x => x.Name).ToList());
            cboCustomer.DataSource = customerList;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";
            //cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
            dgvPaymentList.AutoGenerateColumns = false;
            LoadData();
        }

        private void dgvTransactionDetail_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvTransactionDetail.Rows)
            {
                TransactionDetail transactionDetailObj = (TransactionDetail)row.DataBoundItem;
                row.Cells[0].Value = transactionDetailObj.Product.ProductCode;
                row.Cells[1].Value = transactionDetailObj.Product.Name;
                row.Cells[2].Value = transactionDetailObj.Qty;
                row.Cells[3].Value = transactionDetailObj.UnitPrice;
                row.Cells[4].Value = transactionDetailObj.DiscountRate + "%";
                row.Cells[5].Value = transactionDetailObj.TaxRate + "%";
                row.Cells[6].Value = transactionDetailObj.TotalAmount;
                row.Cells[9].Value = transactionDetailObj.IsDeductedBy == (decimal?)null ? "0.00%" : transactionDetailObj.IsDeductedBy + "%";
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Transaction transactionObj = (from t in entity.Transactions where t.Id == transactionId select t).FirstOrDefault();
            if (transactionObj.PaymentTypeId == 2)
            {
                #region [ Print ] for Credit

                int outStandingAmount = 0;
                Int32.TryParse(lblOutstandingAmount.Text, out outStandingAmount);
                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                int _tAmt = 0;


                string tranId = transactionObj.Id;

                foreach (TransactionDetail transaction in transactionObj.TransactionDetails)
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    newRow.DiscountPercent = transaction.DiscountRate.ToString();
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty;
                    newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                    _tAmt += newRow.TotalAmount;
                    dtReport.AddItemListRow(newRow);
                }

                int CusId = Convert.ToInt32(cboCustomer.SelectedValue);
                // int PrepaidDebt = 0;
                List<Transaction> rtList = new List<Transaction>();
                List<Transaction> OldOutStandingList = entity.Transactions.Where(x => x.CustomerId == CusId && !x.Id.Contains(tranId)).ToList().Where(x => x.IsDeleted != true).ToList();
                long OldOutstandingAmount = 0;
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

                }


                string reportPath = "";
                ReportViewer rv = new ReportViewer();
                ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                //reportPath = Application.StartupPath + "\\Reports\\InvoiceCredit.rdlc";
                //if (!SettingController.DefaultShop.ShopName.Contains("Mandalay"))
                //{
                //    reportPath = Application.StartupPath + "\\HagalReports\\InvoiceCredit.rdlc";
                //}
                //else
                //{
                //    reportPath = Application.StartupPath + "\\MDY_Reports\\InvoiceCredit.rdlc";
                //}


                if (DefaultPrinter.SlipPrinter.Contains("EPSON TM-T88IV Receipt"))
                {
                    reportPath = Application.StartupPath + "\\Epson\\InvoiceCredit.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("XP-80C"))
                {
                    reportPath = Application.StartupPath + "\\XP\\InvoiceCredit.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("Birch BP-003"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\InvoiceCredit.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("JM Thermal Series Printer"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\InvoiceCredit.rdlc";
                }
                else
                {
                    reportPath = Application.StartupPath + "\\Epson\\InvoiceCredit.rdlc";
                }
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);


                ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
                rv.LocalReport.SetParameters(TAmt);


                string _Point = Loc_CustomerPointSystem.GetPointFromCustomerId(Convert.ToInt32(transactionObj.Customer.Id)).ToString();
                ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", _Point);
                rv.LocalReport.SetParameters(AvailablePoint);


                ReportParameter ShopName = new ReportParameter("ShopName", SettingController.DefaultShop.ShopName);
                rv.LocalReport.SetParameters(ShopName);

                ReportParameter BranchName = new ReportParameter("BranchName", SettingController.DefaultShop.Address);
                rv.LocalReport.SetParameters(BranchName);

                ReportParameter Phone = new ReportParameter("Phone", SettingController.DefaultShop.PhoneNumber);
                rv.LocalReport.SetParameters(Phone);

                ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.DefaultShop.OpeningHours);
                rv.LocalReport.SetParameters(OpeningHours);

                ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter counter = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                ReportParameter CounterName = new ReportParameter("CounterName", counter.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                rv.LocalReport.SetParameters(PrintDateTime);

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);


                ReportParameter TotalAmount = new ReportParameter("TotalAmount", Convert.ToInt32(transactionObj.TotalAmount).ToString());
                rv.LocalReport.SetParameters(TotalAmount);

                ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
                rv.LocalReport.SetParameters(TaxAmount);

                ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", transactionObj.DiscountAmount.ToString());
                rv.LocalReport.SetParameters(DiscountAmount);

                ReportParameter PaidAmount = new ReportParameter("PaidAmount", Convert.ToInt32(transactionObj.RecieveAmount).ToString());
                rv.LocalReport.SetParameters(PaidAmount);



                ReportParameter PrevOutstanding = new ReportParameter("PrevOutstanding", OldOutstandingAmount.ToString());
                rv.LocalReport.SetParameters(PrevOutstanding);


                ReportParameter PrePaidDebt = new ReportParameter("PrePaidDebt", lblOutstandingAmount.Text);
                rv.LocalReport.SetParameters(PrePaidDebt);


                ReportParameter NetPayable = new ReportParameter("NetPayable", Convert.ToInt32(OldOutstandingAmount + transactionObj.TotalAmount - Convert.ToInt32(lblOutstandingAmount.Text)).ToString());
                rv.LocalReport.SetParameters(NetPayable);


                ReportParameter Balance = new ReportParameter("Balance", Convert.ToInt32(OldOutstandingAmount + transactionObj.TotalAmount - (Convert.ToInt32(lblOutstandingAmount.Text)) - transactionObj.RecieveAmount).ToString());
                rv.LocalReport.SetParameters(Balance);

                ReportParameter CustomerName = new ReportParameter("CustomerName", transactionObj.Customer.Title + " " + transactionObj.Customer.Name);
                rv.LocalReport.SetParameters(CustomerName);

                PrintDoc.PrintReport(rv, "Slip");
                #endregion
            }
            else if (transactionObj.PaymentTypeId == 3)
            {
                #region [ Print ] for GiftCard


                //int GiftCardAmount =Convert.ToInt32(transactionObj.TotalAmount - transactionObj.RecieveAmount);


                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                int _tAmt = 0;
                foreach (TransactionDetail transaction in transactionObj.TransactionDetails)
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    newRow.DiscountPercent = transaction.DiscountRate.ToString();
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH
                    newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                    _tAmt += newRow.TotalAmount;
                    dtReport.AddItemListRow(newRow);
                }


                string reportPath = "";
                ReportViewer rv = new ReportViewer();
                ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                //  reportPath = Application.StartupPath + "\\Reports\\InvoiceGiftcardLoc.rdlc";
                //if (!SettingController.DefaultShop.ShopName.Contains("Mandalay"))
                //{
                //    reportPath = Application.StartupPath + "\\HagalReports\\InvoiceGiftcard.rdlc";
                //}
                //else
                //{
                //    reportPath = Application.StartupPath + "\\MDY_Reports\\InvoiceGiftcard.rdlc";
                //}

                if (DefaultPrinter.SlipPrinter.Contains("EPSON TM-T88IV Receipt"))
                {
                    reportPath = Application.StartupPath + "\\Epson\\InvoiceGiftcard.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("XP-80C"))
                {
                    reportPath = Application.StartupPath + "\\XP\\InvoiceGiftcard.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("Birch BP-003"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\InvoiceGiftcard.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("JM Thermal Series Printer"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\InvoiceGiftcard.rdlc";
                }
                else
                {
                    reportPath = Application.StartupPath + "\\Epson\\InvoiceGiftcard.rdlc";
                }
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);

                APP_Data.Customer cus = entity.Customers.Where(x => x.Id == transactionObj.Customer.Id).FirstOrDefault();

                ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Title + " " + cus.Name);
                rv.LocalReport.SetParameters(CustomerName);

                ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
                rv.LocalReport.SetParameters(TAmt);


                string _Point = Loc_CustomerPointSystem.GetPointFromCustomerId(Convert.ToInt32(cus.Id)).ToString();
                ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", _Point);
                rv.LocalReport.SetParameters(AvailablePoint);


                ReportParameter ShopName = new ReportParameter("ShopName", SettingController.DefaultShop.ShopName);
                rv.LocalReport.SetParameters(ShopName);

                ReportParameter BranchName = new ReportParameter("BranchName", SettingController.DefaultShop.Address);
                rv.LocalReport.SetParameters(BranchName);

                ReportParameter Phone = new ReportParameter("Phone", SettingController.DefaultShop.PhoneNumber);
                rv.LocalReport.SetParameters(Phone);

                ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.DefaultShop.OpeningHours);
                rv.LocalReport.SetParameters(OpeningHours);

                ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                rv.LocalReport.SetParameters(PrintDateTime);

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);



                ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
                rv.LocalReport.SetParameters(TaxAmount);

                ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", transactionObj.DiscountAmount.ToString());
                rv.LocalReport.SetParameters(DiscountAmount);

                Int64 _paidAmt = transactionObj.RecieveAmount == null ? 0 : Convert.ToInt64(transactionObj.RecieveAmount);
                ReportParameter PaidAmount = new ReportParameter("PaidAmount", _paidAmt.ToString());
                rv.LocalReport.SetParameters(PaidAmount);

                ReportParameter Change = new ReportParameter("Change", "0");
                rv.LocalReport.SetParameters(Change);

                //ReportParameter GiftCardNo = new ReportParameter("GiftCardNo", transactionObj.GiftCardId.ToString());
                //rv.LocalReport.SetParameters(GiftCardNo);

                ReportParameter AmountFromGiftCard = new ReportParameter("AmountFromGiftCard", AmountFromGiftCards.ToString());
                rv.LocalReport.SetParameters(AmountFromGiftCard);


                //  ReportParameter TotalAmount = new ReportParameter("TotalAmount", transactionObj.TotalAmount.ToString());
                ReportParameter TotalAmount = new ReportParameter("TotalAmount", (Convert.ToInt32(transactionObj.TotalAmount) - Convert.ToInt32(AmountFromGiftCards)).ToString());
                rv.LocalReport.SetParameters(TotalAmount);



                PrintDoc.PrintReport(rv, "Slip");
                #endregion
            }
            else if (false)
            {
                #region [ Print ] for Cash

                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                int _tAmt = 0;
                foreach (TransactionDetail transaction in transactionObj.TransactionDetails)
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    newRow.DiscountPercent = transaction.DiscountRate.ToString();
                    // newRow.TotalAmount = (int)transaction.TotalAmount;
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty;
                    newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                    _tAmt += newRow.TotalAmount;
                    dtReport.AddItemListRow(newRow);
                }

                string reportPath = "";
                ReportViewer rv = new ReportViewer();
                ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                // reportPath = Application.StartupPath + "\\Reports\\Loc_InvoiceCash.rdlc";
                //if (!SettingController.DefaultShop.ShopName.Contains("Mandalay"))
                //{
                //    reportPath = Application.StartupPath + "\\HagalReports\\Loc_InvoiceCash.rdlc";
                //}
                //else
                //{
                //    reportPath = Application.StartupPath + "\\MDY_Reports\\Loc_InvoiceCash.rdlc";
                //}

                if (DefaultPrinter.SlipPrinter.Contains("EPSON TM-T88IV Receipt"))
                {
                    reportPath = Application.StartupPath + "\\Epson\\Loc_InvoiceCash.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("XP-80C"))
                {
                    reportPath = Application.StartupPath + "\\XP\\Loc_InvoiceCash.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("Birch BP-003"))
                {

                    reportPath = Application.StartupPath + "\\Birch\\Loc_InvoiceCash.rdlc";

                }
                else if (DefaultPrinter.SlipPrinter.Contains("JM Thermal Series Printer"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\Loc_InvoiceCash.rdlc";
                }
                else
                {

                    reportPath = Application.StartupPath + "\\Epson\\Loc_InvoiceCash.rdlc";

                }
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);



                APP_Data.Customer cus = entity.Customers.Where(x => x.Id == transactionObj.Customer.Id).FirstOrDefault();

                ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Title + " " + cus.Name);
                rv.LocalReport.SetParameters(CustomerName);

                ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
                rv.LocalReport.SetParameters(TAmt);

                string _Point = Loc_CustomerPointSystem.GetPointFromCustomerId(cus.Id).ToString();
                ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", _Point);
                rv.LocalReport.SetParameters(AvailablePoint);



                ReportParameter ShopName = new ReportParameter("ShopName", SettingController.DefaultShop.ShopName);
                rv.LocalReport.SetParameters(ShopName);

                ReportParameter BranchName = new ReportParameter("BranchName", SettingController.DefaultShop.Address);
                rv.LocalReport.SetParameters(BranchName);

                ReportParameter Phone = new ReportParameter("Phone", SettingController.DefaultShop.PhoneNumber);
                rv.LocalReport.SetParameters(Phone);

                ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.DefaultShop.OpeningHours);
                rv.LocalReport.SetParameters(OpeningHours);

                ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                rv.LocalReport.SetParameters(PrintDateTime);

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);

                //ReportParameter TotalAmount = new ReportParameter("TotalAmount", transactionObj.TotalAmount.ToString());
                //rv.LocalReport.SetParameters(TotalAmount);

                Int64 totalAmountRep = transactionObj.TotalAmount == null ? 0 : Convert.ToInt64(transactionObj.TotalAmount);
                ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                rv.LocalReport.SetParameters(TotalAmount);

                ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
                rv.LocalReport.SetParameters(TaxAmount);

                ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", transactionObj.DiscountAmount.ToString());
                rv.LocalReport.SetParameters(DiscountAmount);

                Int64 RecieveAmount = transactionObj.RecieveAmount == null ? 0 : Convert.ToInt64(transactionObj.RecieveAmount);
                ReportParameter PaidAmount = new ReportParameter("PaidAmount", RecieveAmount.ToString());
                rv.LocalReport.SetParameters(PaidAmount);

                // ReportParameter Change = new ReportParameter("Change", (transactionObj.RecieveAmount - (transactionObj.TotalAmount - ExtraDiscount + ExtraTax)).ToString());//(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
                Int64 _change = transactionObj.RecieveAmount == null ? 0 : Convert.ToInt64(transactionObj.RecieveAmount - transactionObj.TotalAmount);
                ReportParameter Change = new ReportParameter("Change", _change.ToString());
                //(amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax))
                rv.LocalReport.SetParameters(Change);


                PrintDoc.PrintReport(rv, "Slip");
                #endregion
            }
            else if (transactionObj.PaymentTypeId == 4 || transactionObj.PaymentTypeId == 6)
            {
                #region [ Print ] for FOC and Tester

                string HeaderTitle = "";
                switch (transactionObj.PaymentTypeId)
                {
                    case 4:
                        HeaderTitle = "FOC";
                        break;
                    case 6:
                        HeaderTitle = "Tester";
                        break;
                }


                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                int _tAmt = 0;
                foreach (TransactionDetail transaction in transactionObj.TransactionDetails)
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    newRow.DiscountPercent = transaction.DiscountRate.ToString();
                    // newRow.TotalAmount = (int)transaction.TotalAmount;
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty;
                    newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                    _tAmt += newRow.TotalAmount;
                    dtReport.AddItemListRow(newRow);
                }

                string reportPath = "";
                ReportViewer rv = new ReportViewer();
                ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                // reportPath = Application.StartupPath + "\\Reports\\Loc_InvoiceCash.rdlc";

                //if (!SettingController.DefaultShop.ShopName.Contains("Mandalay"))
                //{
                //    reportPath = Application.StartupPath + "\\HagalReports\\InvoiceFOC.rdlc";
                //}
                //else
                //{
                //    reportPath = Application.StartupPath + "\\MDY_Reports\\InvoiceFOC.rdlc";
                //}

                if (DefaultPrinter.SlipPrinter.Contains("EPSON TM-T88IV Receipt"))
                {
                    reportPath = Application.StartupPath + "\\Epson\\InvoiceFOC.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("XP-80C"))
                {
                    reportPath = Application.StartupPath + "\\XP\\InvoiceFOC.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("Birch BP-003"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\InvoiceFOC.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("JM Thermal Series Printer"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\InvoiceFOC.rdlc";
                }
                else
                {
                    reportPath = Application.StartupPath + "\\Epson\\InvoiceFOC.rdlc";
                }
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);



                APP_Data.Customer cus = entity.Customers.Where(x => x.Id == transactionObj.Customer.Id).FirstOrDefault();

                ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Title + " " + cus.Name);
                rv.LocalReport.SetParameters(CustomerName);

                ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
                rv.LocalReport.SetParameters(TAmt);

                string _Point = Loc_CustomerPointSystem.GetPointFromCustomerId(cus.Id).ToString();
                ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", _Point);
                rv.LocalReport.SetParameters(AvailablePoint);



                ReportParameter ShopName = new ReportParameter("ShopName", SettingController.DefaultShop.ShopName);
                rv.LocalReport.SetParameters(ShopName);


                ReportParameter BranchName = new ReportParameter("BranchName", SettingController.DefaultShop.Address);
                rv.LocalReport.SetParameters(BranchName);

                ReportParameter Title = new ReportParameter("Title", HeaderTitle);
                rv.LocalReport.SetParameters(Title);

                ReportParameter Phone = new ReportParameter("Phone", SettingController.DefaultShop.PhoneNumber);
                rv.LocalReport.SetParameters(Phone);

                ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.DefaultShop.OpeningHours);
                rv.LocalReport.SetParameters(OpeningHours);

                ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                rv.LocalReport.SetParameters(PrintDateTime);

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);

                // ReportParameter TotalAmount = new ReportParameter("TotalAmount", Convert.ToInt32(insertedTransaction.TotalAmount + insertedTransaction.DiscountAmount).ToString());
                ReportParameter TotalAmount = new ReportParameter("TotalAmount", Convert.ToInt32(transactionObj.TotalAmount).ToString());
                rv.LocalReport.SetParameters(TotalAmount);

                ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
                rv.LocalReport.SetParameters(TaxAmount);

                ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", transactionObj.DiscountAmount.ToString());
                rv.LocalReport.SetParameters(DiscountAmount);

                ReportParameter PaidAmount = new ReportParameter("PaidAmount", "0");
                rv.LocalReport.SetParameters(PaidAmount);

                ReportParameter Change = new ReportParameter("Change", "0");
                rv.LocalReport.SetParameters(Change);


                PrintDoc.PrintReport(rv, "Slip");
                #endregion
            }

            else if (transactionObj.PaymentTypeId == 5)
            {
                #region [ Print ] for MPU



                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                int _tAmt = 0;
                foreach (TransactionDetail transaction in transactionObj.TransactionDetails)
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.Name = transaction.Product.Name;
                    newRow.Qty = transaction.Qty.ToString();
                    newRow.DiscountPercent = transaction.DiscountRate.ToString();
                    // newRow.TotalAmount = (int)transaction.TotalAmount;
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty;
                    newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                    _tAmt += newRow.TotalAmount;
                    dtReport.AddItemListRow(newRow);
                }

                string reportPath = "";
                ReportViewer rv = new ReportViewer();
                ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
                // reportPath = Application.StartupPath + "\\Reports\\Loc_InvoiceCash.rdlc";

                //if (!SettingController.DefaultShop.ShopName.Contains("Mandalay"))
                //{
                //    reportPath = Application.StartupPath + "\\HagalReports\\InvoiceMPU.rdlc";
                //}
                //else
                //{
                //    reportPath = Application.StartupPath + "\\MDY_Reports\\InvoiceMPU.rdlc";
                //}

                if (DefaultPrinter.SlipPrinter.Contains("EPSON TM-T88IV Receipt"))
                {
                    reportPath = Application.StartupPath + "\\Epson\\InvoiceMPU.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("XP-80C"))
                {
                    reportPath = Application.StartupPath + "\\XP\\InvoiceMPU.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("Birch BP-003"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\InvoiceMPU.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("JM Thermal Series Printer"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\InvoiceMPU.rdlc";
                }
                else
                {
                    reportPath = Application.StartupPath + "\\Epson\\InvoiceMPU.rdlc";
                }
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);



                APP_Data.Customer cus = entity.Customers.Where(x => x.Id == transactionObj.Customer.Id).FirstOrDefault();

                ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Title + " " + cus.Name);
                rv.LocalReport.SetParameters(CustomerName);

                ReportParameter TAmt = new ReportParameter("TAmt", _tAmt.ToString());
                rv.LocalReport.SetParameters(TAmt);

                string _Point = Loc_CustomerPointSystem.GetPointFromCustomerId(cus.Id).ToString();
                ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", _Point);
                rv.LocalReport.SetParameters(AvailablePoint);

                ReportParameter ShopName = new ReportParameter("ShopName", SettingController.DefaultShop.ShopName);
                rv.LocalReport.SetParameters(ShopName);

                ReportParameter BranchName = new ReportParameter("BranchName", SettingController.DefaultShop.Address);
                rv.LocalReport.SetParameters(BranchName);

                ReportParameter Phone = new ReportParameter("Phone", SettingController.DefaultShop.PhoneNumber);
                rv.LocalReport.SetParameters(Phone);

                ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.DefaultShop.OpeningHours);
                rv.LocalReport.SetParameters(OpeningHours);

                ReportParameter TransactionId = new ReportParameter("TransactionId", transactionId.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                rv.LocalReport.SetParameters(PrintDateTime);

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);

                // ReportParameter TotalAmount = new ReportParameter("TotalAmount", Convert.ToInt32(insertedTransaction.TotalAmount + insertedTransaction.DiscountAmount).ToString());
                ReportParameter TotalAmount = new ReportParameter("TotalAmount", Convert.ToInt32(transactionObj.TotalAmount).ToString());
                rv.LocalReport.SetParameters(TotalAmount);

                ReportParameter TaxAmount = new ReportParameter("TaxAmount", transactionObj.TaxAmount.ToString());
                rv.LocalReport.SetParameters(TaxAmount);

                ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", transactionObj.DiscountAmount.ToString());
                rv.LocalReport.SetParameters(DiscountAmount);



                ReportParameter PaidAmount = new ReportParameter("PaidAmount", Convert.ToInt32(transactionObj.RecieveAmount).ToString());
                rv.LocalReport.SetParameters(PaidAmount);

                ReportParameter Change = new ReportParameter("Change", "0");
                rv.LocalReport.SetParameters(Change);
                PrintDoc.PrintReport(rv, "Slip");
                #endregion
            }
            else
            {
                #region [ Print ]


                dsReportTemp dsReport = new dsReportTemp();
                dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                dsReportTemp.MultiPaymentDataTable multiReport = (dsReportTemp.MultiPaymentDataTable)dsReport.Tables["MultiPayment"];
                int _tAmt = 0;

                foreach (TransactionDetail transaction in transactionObj.TransactionDetails)
                {
                    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    newRow.ItemId = transaction.Product.ProductCode;
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
                ReportDataSource rds2 = new ReportDataSource("MultiPayment", dsReport.Tables["MultiPayment"]);

                //if (!SettingController.DefaultShop.ShopName.Contains("Mandalay"))
                if (DefaultPrinter.SlipPrinter.Contains("EPSON TM-T88IV Receipt"))
                {
                    reportPath = Application.StartupPath + "\\Epson\\Loc_InvoiceCash.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("XP-80C"))
                {
                    reportPath = Application.StartupPath + "\\XP\\Loc_InvoiceCash.rdlc";
                }
                else if (DefaultPrinter.SlipPrinter.Contains("Birch BP-003"))
                {

                    reportPath = Application.StartupPath + "\\Birch\\Loc_InvoiceCash.rdlc";

                }
                else if (DefaultPrinter.SlipPrinter.Contains("JM Thermal Series Printer"))
                {
                    reportPath = Application.StartupPath + "\\Birch\\Loc_InvoiceCash.rdlc";
                }
                else
                {

                    reportPath = Application.StartupPath + "\\Epson\\Loc_InvoiceCash.rdlc";

                }

                var data = entity.TransactionPaymentDetails.Where(x => x.TransactionId.Trim() == transactionObj.Id.Trim()).ToList();
                foreach (var item in data)
                {
                    dsReportTemp.MultiPaymentRow newRow = multiReport.NewMultiPaymentRow();
                    newRow.PaymentMethod = entity.PaymentMethods.Where(x => x.Id == item.PaymentMethodId).Select(x => x.Name).FirstOrDefault();
                    newRow.Amount = Convert.ToString(item.Amount);
                    multiReport.AddMultiPaymentRow(newRow);
                }

                //    reportPath = Application.StartupPath + "\\HagalReports\\Loc_InvoiceCash.rdlc";
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);
                rv.LocalReport.DataSources.Add(rds2);

                var cID = Convert.ToInt32(cboCustomer.SelectedValue);
                Customer cus = entity.Customers.Where(x => x.Id == cID).FirstOrDefault();

                string _Point = Loc_CustomerPointSystem.GetPointFromCustomerId(cus.Id).ToString();

                ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Title + " " + cus.Name);
                rv.LocalReport.SetParameters(CustomerName);


                ReportParameter AvailablePoint = new ReportParameter("AvailablePoint", _Point);
                rv.LocalReport.SetParameters(AvailablePoint);

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

                ReportParameter TransactionId = new ReportParameter("TransactionId", transactionObj.Id.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter c = entity.Counters.Where(x => x.Id == MemberShip.CounterId).FirstOrDefault();

                ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                rv.LocalReport.SetParameters(PrintDateTime);

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);

                // Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount + insertedTransaction.DiscountAmount);
                Int64 totalAmountRep = transactionObj.TotalAmount == null ? 0 : Convert.ToInt64(transactionObj.TotalAmount);
                ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                rv.LocalReport.SetParameters(TotalAmount);

                Int64 taxAmountRep = transactionObj.TaxAmount == null ? 0 : Convert.ToInt64(transactionObj.TaxAmount);
                ReportParameter TaxAmount = new ReportParameter("TaxAmount", taxAmountRep.ToString());
                rv.LocalReport.SetParameters(TaxAmount);

                Int64 disAmountRep = transactionObj.DiscountAmount == null ? 0 : Convert.ToInt64(transactionObj.DiscountAmount);
                ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", disAmountRep.ToString());
                rv.LocalReport.SetParameters(DiscountAmount);

                ReportParameter PaidAmount = new ReportParameter("PaidAmount", (Convert.ToInt32(transactionObj.RecieveAmount)).ToString());
                rv.LocalReport.SetParameters(PaidAmount);

                var CurrencySymbol = "Ks";
                ReportParameter CurrencyCode = new ReportParameter("CurrencyCode", CurrencySymbol);
                rv.LocalReport.SetParameters(CurrencyCode);

                ReportParameter Change = new ReportParameter("Change", "0");
                rv.LocalReport.SetParameters(Change);
                PrintDoc.PrintReport(rv, "Slip");

                #endregion
            }
        }

        private void dgvTransactionDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                int currentTransactionId = Convert.ToInt32(dgvTransactionDetail.Rows[e.RowIndex].Cells[8].Value.ToString());
                //Delete the record and add delete log
                if (e.ColumnIndex == 7)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {

                        int count = (from AttGift in entity.AttachGiftSystemForTransactions where AttGift.TransactionId == transactionId select AttGift).ToList().Count;

                        //ZMH
                        APP_Data.TransactionDetail tdOBj = new TransactionDetail();
                        APP_Data.Transaction tObj = new Transaction();

                        tdOBj = entity.TransactionDetails.Where(x => x.Id == currentTransactionId).FirstOrDefault();

                        if (tdOBj != null)
                        {
                            tObj = entity.Transactions.Where(x => x.ParentId == tdOBj.TransactionId && x.IsDeleted == false).FirstOrDefault();
                        }
                        bool IsSame = false;

                        if (tObj != null)
                        {
                            TransactionDetail td = entity.TransactionDetails.Where(x => x.TransactionId == tObj.Id).FirstOrDefault();
                            if (td != null)
                            {
                                if (td.ProductId == tdOBj.ProductId)
                                {
                                    IsSame = true;
                                }
                            }
                        }
                        //ZMH
                        if (count > 1)
                        {
                            MessageBox.Show("This Product name is include in GWP List!", "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (IsSame)
                        {
                            MessageBox.Show("This transaction detail already make refund. So it can't be delete!");
                            return;
                        }
                        else
                        {
                            if (dgvTransactionDetail.Rows.Count <= 1)
                            {
                                DialogResult result2 = MessageBox.Show("You have only one record!.If you delete this,system will automatically delete Transaction of this record", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                if (result2.Equals(DialogResult.OK))
                                {
                                    //string CuId = currentTransactionId.ToString();
                                    TransactionDetail ts = entity.TransactionDetails.Where(x => x.Id == currentTransactionId).FirstOrDefault();

                                    ts.IsDeleted = true;
                                    ts.Product.Qty = ts.Product.Qty + ts.Qty;

                                    if (ts.Product.IsWrapper == true)
                                    {
                                        List<WrapperItem> wplist = ts.Product.WrapperItems.ToList();

                                        foreach (WrapperItem wp in wplist)
                                        {
                                            wp.Product1.Qty = wp.Product1.Qty + ts.Qty;
                                        }
                                    }

                                    TransactionDetail tds = entity.TransactionDetails.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                                    Transaction ts1 = entity.Transactions.Where(x => x.Id == tds.TransactionId).FirstOrDefault();
                                    if (ts1 != null)
                                    {
                                        ts1.IsDeleted = true;
                                        foreach (TransactionDetail td in ts1.TransactionDetails)
                                        {
                                            td.IsDeleted = true;
                                        }

                                        DeleteLog dl = new DeleteLog();
                                        dl.DeletedDate = DateTime.Now;
                                        dl.CounterId = MemberShip.CounterId;
                                        dl.UserId = MemberShip.UserId;
                                        dl.IsParent = true;
                                        dl.TransactionId = ts1.Id;
                                        entity.DeleteLogs.Add(dl);
                                        entity.SaveChanges();

                                        Transaction ParentTransaction = entity.Transactions.Where(x => x.Id == ts.TransactionId).FirstOrDefault();
                                        ParentTransaction.TotalAmount = ParentTransaction.TotalAmount - ts.TotalAmount;
                                        //zp
                                        List<GiftCardInTransaction> gt = (from p in entity.GiftCardInTransactions where p.TransactionId == tds.TransactionId select p).ToList();
                                        if (gt != null)
                                        {
                                            foreach (GiftCardInTransaction g in gt)
                                            {
                                                GiftCard gift = (from p in entity.GiftCards where p.Id == g.GiftCardId select p).FirstOrDefault();
                                                gift.IsUsed = false;
                                            }
                                        }
                                        entity.SaveChanges();

                                        List<DeleteLog> delist = entity.DeleteLogs.Where(x => x.TransactionId == ts1.Id && x.TransactionDetailId != null && x.IsParent == false).ToList();

                                        foreach (DeleteLog d in delist)
                                        {
                                            entity.DeleteLogs.Remove(d);
                                        }
                                        entity.SaveChanges();
                                        LoadData();
                                        this.Close();
                                    }
                                }
                            }
                            else
                            {
                                TransactionDetail ts = entity.TransactionDetails.Where(x => x.Id == currentTransactionId).FirstOrDefault();
                                ts.IsDeleted = true;
                                ts.Product.Qty = ts.Product.Qty + ts.Qty;

                                if (ts.Product.IsWrapper == true)
                                {
                                    List<WrapperItem> wplist = ts.Product.WrapperItems.ToList();

                                    foreach (WrapperItem wp in wplist)
                                    {
                                        wp.Product1.Qty = wp.Product1.Qty + ts.Qty;
                                    }
                                }
                                DeleteLog dl = new DeleteLog();
                                dl.DeletedDate = DateTime.Now;
                                dl.CounterId = MemberShip.CounterId;
                                dl.UserId = MemberShip.UserId;
                                dl.IsParent = false;
                                dl.TransactionId = ts.TransactionId;
                                dl.TransactionDetailId = ts.Id;

                                Transaction ParentTransaction = entity.Transactions.Where(x => x.Id == ts.TransactionId).FirstOrDefault();
                                ParentTransaction.TotalAmount = ParentTransaction.TotalAmount - ts.TotalAmount;

                                entity.DeleteLogs.Add(dl);

                                entity.SaveChanges();

                                LoadData();
                            }

                            if (System.Windows.Forms.Application.OpenForms["TransactionList"] != null)
                            {
                                TransactionList newForm = (TransactionList)System.Windows.Forms.Application.OpenForms["TransactionList"];
                                newForm.LoadData();
                            }
                            if (System.Windows.Forms.Application.OpenForms["CreditTransactionList"] != null)
                            {
                                CreditTransactionList newForm = (CreditTransactionList)System.Windows.Forms.Application.OpenForms["CreditTransactionList"];
                                newForm.LoadData();
                            }

                        }

                    }
                }
            }
        }

        #endregion

        #region Function

        private void LoadData()
        {
            dgvTransactionDetail.AutoGenerateColumns = false;
            tlpCredit.Visible = false;
            Transaction transactionObject = (from t in entity.Transactions where t.Id == transactionId select t).FirstOrDefault();
            lblSalePerson.Text = (transactionObject.User == null) ? "-" : transactionObject.User.Name;
            lblCounter.Text = transactionObject.Counter.Name;
            lblDate.Text = transactionObject.DateTime.ToString("dd-MM-yyyy");
            lblTime.Text = transactionObject.DateTime.ToString("hh:mm");
            lblCustomerName.Text = (transactionObject.Customer == null) ? "-" : transactionObject.Customer.Name;
            dgvPaymentList.DataSource = entity.TransactionPaymentDetails.Where(x => x.TransactionId.Trim() == transactionId.Trim()).ToList();
            if (transactionObject.Customer.Name == null)
            {
                cboCustomer.SelectedIndex = 0;
            }
            else
            {
                cboCustomer.Text = transactionObject.Customer.Name;
            }

            if (transactionObject.Type == TransactionType.Settlement)
            {
                dgvTransactionDetail.DataSource = "";
                lblRecieveAmunt.Text = transactionObject.RecieveAmount.ToString();
                lblDiscount.Text = "0";
                lblTotal.Text = transactionObject.TotalAmount.ToString();
                lblPaymentMethod1.Text = (transactionObject.PaymentType == null) ? "-" : transactionObject.PaymentType.Name;

                tlpCash.Visible = true;
            }
            else if (transactionObject.Type == TransactionType.Sale || transactionObject.Type == TransactionType.Credit)
            {
                if (IsDelelog) //ZMH
                {
                    dgvTransactionDetail.DataSource = transactionObject.TransactionDetails.ToList();
                }
                else
                {
                    dgvTransactionDetail.DataSource = transactionObject.TransactionDetails.Where(x => x.IsDeleted == false).ToList();       //Edit From IsDelete is Null to Zer0             
                }


                if (transactionObject.ReceivedCurrencyId == 1 || transactionObject.ReceivedCurrencyId == null)
                {

                    lblRecieveAmunt.Text = transactionObject.RecieveAmount.ToString();
                }
                else
                {
                    decimal dollartotal;
                    ExchangeRateForTransaction e = entity.ExchangeRateForTransactions.Where(x => x.TransactionId == transactionObject.Id).FirstOrDefault();
                    dollartotal = Convert.ToDecimal(transactionObject.RecieveAmount / e.ExchangeRate);

                    lblRecieveAmunt.Text = dollartotal.ToString() + "$ [ 1$=" + e.ExchangeRate.ToString() + "Ks ]";
                }

                int discount = 0;
                int tax = 0;
                foreach (TransactionDetail td in transactionObject.TransactionDetails)
                {
                    discount += Convert.ToInt32(((td.UnitPrice) * (td.DiscountRate / 100)) * td.Qty);
                    tax += Convert.ToInt32((td.UnitPrice * (td.TaxRate / 100)) * td.Qty);
                }
                lblDiscount.Text = (transactionObject.DiscountAmount - discount).ToString();
                lblTotalTax.Text = (transactionObject.TaxAmount).ToString();
                lblTotal.Text = transactionObject.TotalAmount.ToString();
                ExtraDiscount = Convert.ToInt32(transactionObject.DiscountAmount - discount);
                ExtraTax = Convert.ToInt32(transactionObject.TaxAmount - tax);

                lblPaymentMethod1.Text = (transactionObject.PaymentType == null) ? "-" : transactionObject.PaymentType.Name;
                //Credit
                if (transactionObject.PaymentTypeId == 2)
                {
                    List<Transaction> OldOutStandingList = entity.Transactions.Where(x => x.CustomerId == transactionObject.CustomerId).Where(x => x.IsPaid == false).Where(x => x.DateTime < transactionObject.DateTime).ToList().Where(x => x.IsDeleted != true).ToList();

                    long OldOutstandingAmount = 0;

                    foreach (Transaction t in OldOutStandingList)
                    {
                        OldOutstandingAmount += (long)t.TotalAmount - (long)t.RecieveAmount;
                    }
                    long PrepaidDebt = 0;
                    List<Transaction> PrePaidList = entity.Transactions.Where(x => x.CustomerId == transactionObject.CustomerId).Where(x => x.IsActive == false).Where(x => x.Type == TransactionType.Prepaid).ToList().Where(x => x.IsDeleted != true).ToList();
                    foreach (Transaction t in PrePaidList)
                    {
                        long useAmount = 0;
                        if (t.UsePrePaidDebts != null)
                        {
                            useAmount = (long)t.UsePrePaidDebts.Sum(x => x.UseAmount);
                        }
                        PrepaidDebt += Convert.ToInt32(t.RecieveAmount - useAmount);
                    }
                    if (transactionObject.UsePrePaidDebts != null)
                    {
                        PrepaidDebt += (long)transactionObject.UsePrePaidDebts.Sum(x => x.UseAmount);
                    }
                    if (OldOutstandingAmount > 0)
                    {
                        OldOutstandingAmount -= PrepaidDebt;
                    }
                    tlpCredit.Visible = true;
                    lblOutstandingAmount.Text = PrepaidDebt.ToString();
                    lblPrevTitle.Text = "Used Prepaid Amount";
                    //lblPayableCredit.Text = ((transactionObject.TotalAmount + OldOutstandingAmount) - transactionObject.RecieveAmount).ToString();
                    //lblOutstandingAmount.Text = OldOutstandingAmount.ToString();
                }
                //GiftCard
                else if (transactionObject.PaymentTypeId == 3)
                {
                    lblRecieveAmunt.Text = transactionObject.RecieveAmount.ToString();

                    AmountFromGiftCards = 0;
                    foreach (GiftCardInTransaction git in transactionObject.GiftCardInTransactions)
                    {
                        //if ((git.GiftCard.IsDeleted == false && git.GiftCard.IsUsed == true) || (git.GiftCard.IsDeleted == true && git.GiftCard.IsUsed == false))
                        //{
                        AmountFromGiftCards += (long)git.GiftCard.Amount;
                        //}

                    }

                    lblAmountFromGiftCard.Visible = true;
                    lblAmountFromGiftcardTitle.Visible = true;
                    lblAmountFromGiftCard.Text = AmountFromGiftCards.ToString() + ".00";
                }

                tlpCash.Visible = true;
            }
        }

        #endregion

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to update?", "Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {

                Transaction transactionObject = (from t in entity.Transactions where t.Id == transactionId select t).FirstOrDefault();
                transactionObject.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue);
                transactionObject.UpdatedDate = DateTime.Now;

                List<GiftCardInTransaction> gt = (from p in entity.GiftCardInTransactions where p.TransactionId == transactionId select p).ToList();
                if (gt != null)
                {
                    foreach (GiftCardInTransaction g in gt)
                    {
                        GiftCard gift = (from p in entity.GiftCards where p.Id == g.GiftCardId select p).FirstOrDefault();
                        gift.IsUsedDate = transactionObject.UpdatedDate;
                        gift.CustomerId = transactionObject.CustomerId;
                    }
                }
                entity.Entry(transactionObject).State = EntityState.Modified;
                entity.SaveChanges();
                MessageBox.Show("Successfully Updated!", "Update");
            }
        }

        private void lbAdvanceSearch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CustomerSearch form = new CustomerSearch();
            form.ShowDialog();
        }

        public void SetCurrentCustomer(Int32 CId)
        {
            CustomerId = CId;
            Customer currentCustomer = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
            lblCustomerName.Text = currentCustomer.Name;
            cboCustomer.SelectedItem = currentCustomer;
        }

        private void cboCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            cboCustomer.DroppedDown = true;
        }

        private void dgvPaymentList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvPaymentList.Rows)
            {
                POSEntities entities = new POSEntities();
                TransactionPaymentDetail transactionDetailObj = (TransactionPaymentDetail)row.DataBoundItem;
                var paymentName = entities.PaymentMethods.Where(x => x.Id == transactionDetailObj.PaymentMethodId).Select(x => x.Name).FirstOrDefault();
                row.Cells[0].Value = paymentName;
                row.Cells[1].Value = transactionDetailObj.Amount;
            }
        }

        private void cboCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            cboCustomer.DroppedDown = true;
        }

    }
}
