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
    public partial class Loc_PaidByCash : Form
    {
        #region Variables




        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        private List<GiftCard> GiftCardList = new List<GiftCard>();

        public List<GiftSystem> GiftList = new List<GiftSystem>();

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int ExtraTax { get; set; }

        public Boolean isDraft { get; set; }

        public Boolean isDebt { get; set; }

        public string DraftId { get; set; }

        public string DebtId { get; set; }

        public long DebtAmount { get; set; }

        public int? CustomerId { get; set; }

        public long PrePaidAmount { get; set; }

        public List<Transaction> CreditTransaction { get; set; }

        public List<Transaction> PrePaidTransaction { get; set; }

        long total;

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        private long totalAmount = 0, prePaidAmount = 0; private decimal AmountWithExchange = 0;

        private Transaction printList = new Transaction();
        private string printId = string.Empty;
        string CurrencySymbol = string.Empty;
        #endregion
        public Loc_PaidByCash()
        {
            InitializeComponent();
        }

        private void Loc_PaidByCash_Load(object sender, EventArgs e)
        {
            #region currency
            Currency curreObj = new Currency();
            List<Currency> currencyList = new List<Currency>();
            currencyList.AddRange(entity.Currencies.ToList());
            cboCurrency.DataSource = currencyList.ToList();
            cboCurrency.DisplayMember = "CurrencyCode";
            cboCurrency.ValueMember = "Id";
            
            int id = 0;
            if (SettingController.DefaultCurrency != 0)
            {
                id = Convert.ToInt32(SettingController.DefaultCurrency);
                curreObj = entity.Currencies.FirstOrDefault(x => x.Id == id);
                cboCurrency.Text = curreObj.CurrencyCode;
            }
            //txtExchangeRate.Text = SettingController.DefaultExchangeRate.ToString();
            #endregion
            if (!isDebt)
            {
                
                total = (long)(DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax);
                //lblTotalCost.Text = (String.Format("{0:0.00}", Utility.CalculateExchangeRate(id, total)));
                lblTotalCost.Text = Utility.CalculateExchangeRate(id, total).ToString();
                AmountWithExchange = Convert.ToDecimal(lblTotalCost.Text);
            }
            else
            {
                foreach (Transaction tObj in CreditTransaction)
                {
                    if (tObj.Transaction1.Count <= 0)
                    {
                        totalAmount += (long)tObj.TotalAmount - (long)tObj.RecieveAmount;
                    }
                    //Has refund
                    else
                    {
                        totalAmount += (long)tObj.TotalAmount - (long)tObj.RecieveAmount;
                        foreach (Transaction Refund in tObj.Transaction1)
                        {
                            totalAmount -= (long)Refund.RecieveAmount;
                        }
                    }
                    if (tObj.UsePrePaidDebts != null)
                    {
                        long prepaid = (long)tObj.UsePrePaidDebts.Sum(x => x.UseAmount);
                        totalAmount -= prepaid;
                    }
                }
                foreach (Transaction tObj in PrePaidTransaction)
                {
                    prePaidAmount += (long)tObj.TotalAmount;
                    long useAmount = (tObj.UsePrePaidDebts1 == null) ? 0 : (int)tObj.UsePrePaidDebts1.Sum(x => x.UseAmount);
                    prePaidAmount -= useAmount;
                }
                DebtAmount = (totalAmount - prePaidAmount);
                lblTotalCost.Text = Utility.CalculateExchangeRate(id, DebtAmount).ToString();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            decimal receiveAmount = 0;
            long totalCost = (long)DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount;
            decimal.TryParse(txtReceiveAmount.Text, out receiveAmount);
            decimal totalCashSaleAmount = Convert.ToDecimal(lblTotalCost.Text);
            int currencyId = Convert.ToInt32(cboCurrency.SelectedValue);
            Currency cu = entity.Currencies.FirstOrDefault(x => x.Id == currencyId);
            
            //Validation
            if (receiveAmount == 0)
            {
                tp.SetToolTip(txtReceiveAmount, "Error");
                tp.Show("Please fill up receive amount!", txtReceiveAmount);
                hasError = true;
            }
            //else if (receiveAmount < totalCost)
            //{
            //    tp.SetToolTip(txtReceiveAmount, "Error");
            //    tp.Show("Receive amount must be greater than total cost!", txtReceiveAmount);
            //    hasError = true;
            //}
            else if (receiveAmount < AmountWithExchange)
            {
                tp.SetToolTip(txtReceiveAmount, "Error");
                tp.Show("Receive amount must be greater than total cost!", txtReceiveAmount);
                hasError = true;
            }
            //if (isDebt)
            //{
            //    if (receiveAmount < DebtAmount)
            //    {
            //        tp.SetToolTip(txtReceiveAmount, "Error");
            //        tp.Show("Receive amount must be greater than debt amount!", txtReceiveAmount);
            //        hasError = true;
            //    }
            //}
            if (!hasError)
            {
                System.Data.Objects.ObjectResult<String> Id;
                string resultId = "-";
                CurrencySymbol = string.Empty;
                Transaction insertedTransaction = new Transaction();
                List<Transaction> RefundList = new List<Transaction>();
                decimal change = 0;
                if (cu.CurrencyCode == "USD")
                {
                    totalCashSaleAmount = (decimal)totalCashSaleAmount * (decimal)cu.LatestExchangeRate;
                    receiveAmount = receiveAmount * (decimal)cu.LatestExchangeRate;
                    CurrencySymbol = "$";
                    change = Convert.ToDecimal(lblChanges.Text) * (decimal)cu.LatestExchangeRate;
                }
                else
                {
                    CurrencySymbol = "Ks";
                    change = Convert.ToDecimal(lblChanges.Text);
                }

                #region Paid
                //Paid by cash, Not Debt
                if (!isDebt)
                {
                    //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, receiveAmount, null, CustomerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);

                    Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, ExtraTax + Tax, ExtraDiscount + Discount, totalCashSaleAmount, receiveAmount, null, CustomerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                    
                    resultId = Id.FirstOrDefault().ToString();
                    printId = resultId;
                    insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                    printList = insertedTransaction;
                    string TId = insertedTransaction.Id;
                    insertedTransaction.ReceivedCurrencyId = currencyId;
                    insertedTransaction.IsDeleted = false;

                    foreach (TransactionDetail detail in DetailList)
                    {

                        detail.IsDeleted = false;//Update IsDelete (Null to 0)
                        var detailID = entity.InsertTransactionDetail(TId, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, Convert.ToDouble(detail.IsDeductedBy), false).SingleOrDefault();
                        detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                       

                        detail.Product.Qty = detail.Product.Qty - detail.Qty;
                        if (detail.Product.Brand != null)
                        {
                            if (detail.Product.Brand.Name == "Special Promotion")
                            {
                                List<WrapperItem> wList = detail.Product.WrapperItems.ToList();
                                if (wList.Count > 0)
                                {
                                    foreach (WrapperItem w in wList)
                                    {
                                        Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                                        wpObj.Qty = wpObj.Qty - detail.Qty;

                                        SPDetail spDetail = new SPDetail();
                                        spDetail.TransactionDetailID = Convert.ToInt32(detailID);
                                        spDetail.DiscountRate = detail.DiscountRate;
                                        spDetail.ParentProductID = w.ParentProductId;
                                        spDetail.ChildProductID = w.ChildProductId;
                                        spDetail.Price = wpObj.Price;
                                        entity.insertSPDetail(spDetail.TransactionDetailID, spDetail.ParentProductID, spDetail.ChildProductID, spDetail.Price, spDetail.DiscountRate, SettingController.DefaultShop.ShortCode);
                                        //entity.SPDetails.Add(spDetail);
                                    }
                                }
                            }
                        }
                        entity.SaveChanges();

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
                    entity.SaveChanges();
                    //Utility.AddExchangeRateForTransaction(currencyId, insertedTransaction.Id);

                    ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                    ex.TransactionId = TId;
                    ex.CurrencyId = cu.Id;
                    ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                    entity.ExchangeRateForTransactions.Add(ex);
                    entity.SaveChanges();
                    GiftList.Clear();
                    //if current transaction is draft transaction, draft transaction is deleted
                    if (isDraft)
                    {
                        Transaction draft = (from trans in entity.Transactions where trans.Id == DraftId select trans).FirstOrDefault<Transaction>();
                        if (draft != null)
                        {
                            draft.TransactionDetails.Clear();
                            var Detail = entity.TransactionDetails.Where(d => d.TransactionId == draft.Id);
                            foreach (var d in Detail)
                            {
                                entity.TransactionDetails.Remove(d);
                            }
                            entity.Transactions.Remove(draft);
                            entity.SaveChanges();
                        }
                    }

                    //Print Invoice
                    #region [ Print ]

                   
                        dsReportTemp dsReport = new dsReportTemp();
                        dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                        int _tAmt = 0;

                        foreach (TransactionDetail transaction in DetailList)
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

                    //    reportPath = Application.StartupPath + "\\HagalReports\\Loc_InvoiceCash.rdlc";
                        rv.Reset();
                        rv.LocalReport.ReportPath = reportPath;
                        rv.LocalReport.DataSources.Add(rds);

                  

                        APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

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

                        ReportParameter TransactionId = new ReportParameter("TransactionId", resultId.ToString());
                        rv.LocalReport.SetParameters(TransactionId);

                        APP_Data.Counter c = entity.Counters.Where(x => x.Id == MemberShip.CounterId).FirstOrDefault();

                        ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                        rv.LocalReport.SetParameters(CounterName);

                        ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                        rv.LocalReport.SetParameters(PrintDateTime);

                        ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                        rv.LocalReport.SetParameters(CasherName);

                       // Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount + insertedTransaction.DiscountAmount);
                        Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount );
                        ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                        rv.LocalReport.SetParameters(TotalAmount);

                        Int64 taxAmountRep = insertedTransaction.TaxAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TaxAmount);
                        ReportParameter TaxAmount = new ReportParameter("TaxAmount", taxAmountRep.ToString());
                        rv.LocalReport.SetParameters(TaxAmount);

                        Int64 disAmountRep = insertedTransaction.DiscountAmount == null ? 0 : Convert.ToInt64(insertedTransaction.DiscountAmount);
                        ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", disAmountRep.ToString());
                        rv.LocalReport.SetParameters(DiscountAmount);

                        ReportParameter PaidAmount = new ReportParameter("PaidAmount", txtReceiveAmount.Text);
                        rv.LocalReport.SetParameters(PaidAmount);

                        ReportParameter CurrencyCode = new ReportParameter("CurrencyCode", CurrencySymbol);
                        rv.LocalReport.SetParameters(CurrencyCode);

                        ReportParameter Change = new ReportParameter("Change", lblChanges.Text);
                        rv.LocalReport.SetParameters(Change);
                        for (int i = 0; i <= 1; i++)
                        {
                            PrintDoc.PrintReport(rv, "Slip");
                        }
                    #endregion

                    //print();
                    
                    MessageBox.Show("Payment Completed");
                }
                #endregion

                #region Debt
                //It is DEBT!!!
                else
                {
                    if (lblChangesText.Text == "Changes")
                    {
                        receiveAmount -= change;
                    }
                    decimal totalAmount = receiveAmount + prePaidAmount;
                    long totalCredit = 0;
                    Int64.TryParse(lblTotalCost.Text, out totalCredit);
                    decimal DebtAmount = 0;
                    if (totalAmount != 0)
                    {
                        if (CreditTransaction.Count > 0)
                        {
                            int index = CreditTransaction.Count;
                            for (int outer = index - 1; outer >= 1; outer--)
                            {
                                for (int inner = 0; inner < outer; inner++)
                                {
                                    if (CreditTransaction[inner].TotalAmount - CreditTransaction[inner].RecieveAmount < CreditTransaction[inner + 1].TotalAmount - CreditTransaction[inner + 1].RecieveAmount)
                                    {
                                        Transaction t = CreditTransaction[inner];
                                        CreditTransaction[inner] = CreditTransaction[inner + 1];
                                        CreditTransaction[inner + 1] = t;
                                    }
                                }
                            }
                            foreach (Transaction CT in CreditTransaction)
                            {
                                decimal CreditAmount = 0;
                                CreditAmount = (long)CT.TotalAmount - (long)CT.RecieveAmount;
                                RefundList = (from tr in entity.Transactions where tr.ParentId == CT.Id && tr.Type == TransactionType.CreditRefund select tr).ToList();
                                if (RefundList.Count > 0)
                                {
                                    foreach (Transaction TRefund in RefundList)
                                    {
                                        CreditAmount -= (long)TRefund.RecieveAmount;
                                    }
                                }
                                if (CT.UsePrePaidDebts != null)
                                {
                                    long prePaid = (long)CT.UsePrePaidDebts.Sum(x => x.UseAmount);
                                    CreditAmount -= prePaid;
                                }
                                if (CreditAmount <= totalAmount)
                                {
                                    //CT.IsPaid = true;
                                    //entity.SaveChanges();
                                    Transaction CreditT = (from t in entity.Transactions where t.Id == CT.Id select t).FirstOrDefault<Transaction>();
                                    CreditT.IsPaid = true;
                                    entity.Entry(CreditT).State = EntityState.Modified;
                                    entity.SaveChanges();
                                    totalAmount -= CreditAmount;
                                    if (CreditAmount <= receiveAmount)
                                    {
                                        DebtAmount += CreditAmount;
                                        receiveAmount -= CreditAmount;
                                    }
                                    else
                                    {
                                        CreditAmount -= receiveAmount;
                                        DebtAmount += receiveAmount;
                                        receiveAmount = 0;
                                        foreach (Transaction PrePaidDebtTrans in PrePaidTransaction)
                                        {
                                            long PrePaidamount = 0;
                                            //int useAmount = 0;
                                            int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                            PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                            //if (CreditAmount >= PrePaidamount)
                                            //{
                                            //    PrePaidDebtTrans.IsActive = true;
                                            //    //entity.Entry(PrePaidDebtTrans).State = System.Data.EntityState.Modified;
                                            //}

                                            if (CreditAmount >= PrePaidamount)
                                            {
                                                //PrePaidDebtTrans.IsActive = true;
                                                //entity.SaveChanges();
                                                Transaction PD = (from PT in entity.Transactions where PT.Id == PrePaidDebtTrans.Id select PT).FirstOrDefault<Transaction>();
                                                PD.IsActive = true;
                                                entity.Entry(PD).State = EntityState.Modified;
                                                UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                                usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                                usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                                usePrePaidDObj.CreditTransactionId = CT.Id;
                                                usePrePaidDObj.CashierId = MemberShip.UserId;
                                                usePrePaidDObj.CounterId = MemberShip.CounterId;
                                                entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                                entity.SaveChanges();
                                                CreditAmount -= PrePaidamount;
                                            }
                                            else
                                            {
                                                UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                                usePrePaidDObj.UseAmount = (int)CreditAmount;
                                                usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                                usePrePaidDObj.CreditTransactionId = CT.Id;
                                                usePrePaidDObj.CashierId = MemberShip.UserId;
                                                usePrePaidDObj.CounterId = MemberShip.CounterId;
                                                entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                                entity.SaveChanges();
                                                CreditAmount -= PrePaidamount;
                                            }
                                        }

                                        PrePaidTransaction = (from PDT in entity.Transactions where PDT.Type == TransactionType.Prepaid && PDT.IsActive == false select PDT).ToList();
                                    }
                                }
                            }
                            if (DebtAmount > 0)
                            {
                                System.Data.Objects.ObjectResult<string> DebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Settlement, true, true, 1, 0, 0, DebtAmount, DebtAmount, null, CustomerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                                
                                resultId = DebtId.FirstOrDefault().ToString();
                                insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                                insertedTransaction.ReceivedCurrencyId = cu.Id;
                                ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                                ex.TransactionId = resultId;
                                ex.CurrencyId = cu.Id;
                                ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                                entity.ExchangeRateForTransactions.Add(ex);
                                entity.SaveChanges();
                            }
                        }
                        else
                        {
                            totalAmount -= prePaidAmount;
                            receiveAmount -= prePaidAmount;
                            //System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.PrepaidDebt, true, false, 1, 0, 0, totalAmount, totalAmount, null, customerId);
                            //entity.SaveChanges();
                        }
                    }
                    if (receiveAmount > 0)
                    {
                        System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, 0, 0, receiveAmount, receiveAmount, null, CustomerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                        entity.SaveChanges();
                        if (DebtAmount == 0)
                        {
                          
                            resultId = PreDebtId.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                            insertedTransaction.ReceivedCurrencyId = cu.Id;
                            ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                            ex.TransactionId = resultId;
                            ex.CurrencyId = cu.Id;
                            ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                            entity.ExchangeRateForTransactions.Add(ex);
                            entity.SaveChanges();
                        }

                    }
                    if (isDraft)
                    {
                        Transaction draft = (from trans in entity.Transactions where trans.Id == DraftId select trans).FirstOrDefault<Transaction>();
                        if (draft != null)
                        {
                            draft.TransactionDetails.Clear();
                            var Detail = entity.TransactionDetails.Where(d => d.TransactionId == draft.Id);
                            foreach (var d in Detail)
                            {
                                entity.TransactionDetails.Remove(d);
                            }
                            entity.Transactions.Remove(draft);
                            entity.SaveChanges();
                        }
                    }


                    //Print Invoice
                    #region [ Print ]

                    //dsReportTemp dsReport = new dsReportTemp();
                    //dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                    //foreach (TransactionDetail transaction in DetailList)
                    //{
                    //    dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                    //    newRow.ItemId = transaction.Product.ProductCode;
                    //    newRow.Name = transaction.Product.Name;
                    //    newRow.Qty = transaction.Qty.ToString();
                    //    newRow.TotalAmount = (int)transaction.TotalAmount;
                    //    //newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty;
                    //    dtReport.AddItemListRow(newRow);
                    //}

                    string reportPath = "";
                    ReportViewer rv = new ReportViewer();
                    //ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);

                    //if (!SettingController.DefaultShop.ShopName.Contains("Mandalay"))
                    //{
                    //    reportPath = Application.StartupPath + "\\HagalReports\\InvoiceSettlement.rdlc";
                    //}
                    //else
                    //{
                    //    reportPath = Application.StartupPath + "\\MDY_Reports\\InvoiceSettlement.rdlc";
                    //}


                    if (DefaultPrinter.SlipPrinter.Contains("EPSON TM-T88IV Receipt"))
                    {
                        reportPath = Application.StartupPath + "\\Epson\\InvoiceSettlement.rdlc";
                    }
                    else if (DefaultPrinter.SlipPrinter.Contains("XP-80C"))
                    {
                        reportPath = Application.StartupPath + "\\XP\\InvoiceSettlement.rdlc";
                    }
                    else if (DefaultPrinter.SlipPrinter.Contains("Birch BP-003"))
                    {
                        reportPath = Application.StartupPath + "\\Birch\\InvoiceSettlement.rdlc";
                    }
                    else
                    {
                        reportPath = Application.StartupPath + "\\Epson\\InvoiceSettlement.rdlc";
                    }

                    //reportPath = Application.StartupPath + "\\Reports\\InvoiceSettlement.rdlc";
                    rv.Reset();
                    rv.LocalReport.ReportPath = reportPath;
                    //rv.LocalReport.DataSources.Add(rds);


                    APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

                    string _Point = Loc_CustomerPointSystem.GetPointFromCustomerId(cus.Id).ToString();

                    ReportParameter CustomerName = new ReportParameter("CustomerName", cus.Title + " " + cus.Name);
                    rv.LocalReport.SetParameters(CustomerName);


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

                    ReportParameter TransactionId = new ReportParameter("TransactionId", resultId.ToString());
                    rv.LocalReport.SetParameters(TransactionId);

                    APP_Data.Counter c = entity.Counters.Where(x => x.Id == MemberShip.CounterId).FirstOrDefault();

                    ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                    rv.LocalReport.SetParameters(CounterName);

                    ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                    rv.LocalReport.SetParameters(PrintDateTime);

                    ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                    rv.LocalReport.SetParameters(CasherName);

                   // Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount);

                    ReportParameter TotalAmount = new ReportParameter("TotalAmount", lblTotalCost.Text.ToString());
                    rv.LocalReport.SetParameters(TotalAmount);

              

                    ReportParameter PaidAmount = new ReportParameter("PaidAmount", txtReceiveAmount.Text);
                    rv.LocalReport.SetParameters(PaidAmount);


                    int balance = Convert.ToInt32(lblTotalCost.Text) - Convert.ToInt32(txtReceiveAmount.Text);
                  
                    balance = balance < 0 ? 0 : balance;
                    ReportParameter Balance = new ReportParameter("Balance", balance.ToString());
                    rv.LocalReport.SetParameters(Balance);


                    int _change = Convert.ToInt32(txtReceiveAmount.Text) - Convert.ToInt32(lblTotalCost.Text);

                    _change = _change < 0 ? 0 : _change;
                    ReportParameter Change = new ReportParameter("Change", _change.ToString());
                    rv.LocalReport.SetParameters(Change);

                    PrintDoc.PrintReport(rv, "Slip");
                    #endregion

                    MessageBox.Show("Payment Completed");
                }

                #endregion


                if (System.Windows.Forms.Application.OpenForms["OutstandingCustomerList"] != null)
                {
                    OutstandingCustomerList newForm = (OutstandingCustomerList)System.Windows.Forms.Application.OpenForms["OutstandingCustomerList"];
                    newForm.LoadData();
                }

              

                if (!isDebt)
                {
                    if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                    {
                        Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                        newForm.Clear();
                    }
                }
                else
                {
                    if (System.Windows.Forms.Application.OpenForms["OutstandingCustomerDetail"] != null)
                    {
                        OutstandingCustomerDetail newForm = (OutstandingCustomerDetail)System.Windows.Forms.Application.OpenForms["OutstandingCustomerDetail"];
                        newForm.Reload();
                    }
                }
                this.Dispose();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Loc_PaidByCash_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtReceiveAmount);
        }

        private void txtReceiveAmount_KeyUp(object sender, KeyEventArgs e)
        {
            decimal amount = 0;
            decimal.TryParse(txtReceiveAmount.Text, out amount);
            decimal Cost = 0;
            decimal.TryParse(lblTotalCost.Text, out Cost);
            if (!isDebt)
            {
                //lblChanges.Text = (amount - (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax)).ToString();
                lblChanges.Text = (amount - Cost).ToString();
            }
            else
            {
                decimal DAmount = Convert.ToDecimal(lblTotalCost.Text);
                int cId = Convert.ToInt32(cboCurrency.SelectedValue);
                Currency currencyObj = entity.Currencies.FirstOrDefault( x=> x.Id == cId);
                //if (currencyObj.Id == 2)
                //{
                //    DAmount = DebtAmount / (decimal)currencyObj.LatestExchangeRate;
                //}
                if (amount >= DAmount)
                {
                    lblChanges.Text = (amount - DAmount).ToString();
                    lblChangesText.Text = "Changes";
                }
                else
                {
                    lblChangesText.Text = "NetParable";
                    lblChanges.Text = (DAmount - amount).ToString();
                }
            }
        }

        private void txtReceiveAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && char.IsLetter('.'))
            {
                e.Handled = true;
            }
        }

        private void cboCurrency_SelectedValueChanged(object sender, EventArgs e)
        {    
            int currencyId = 0;                
            Int32.TryParse(cboCurrency.SelectedValue.ToString(),out currencyId);
            if (currencyId != 0)
            {
                Currency cu = entity.Currencies.FirstOrDefault(x => x.Id == currencyId);
                if (cu != null)
                {
                    if (!isDebt)
                    {

                        lblTotalCost.Text = Utility.CalculateExchangeRate(cu.Id, total).ToString();
                        AmountWithExchange = Convert.ToDecimal(lblTotalCost.Text);
                        //if (txtReceiveAmount.Text != null)
                        //{
                        //    receive = Convert.ToDecimal(txtReceiveAmount.Text);
                        //}
                        //lblChanges.Text = (AmountWithExchange - receive).ToString();
                        decimal receive = 0;

                        Decimal.TryParse(txtReceiveAmount.Text, out receive);
                        decimal changes = AmountWithExchange - receive;

                        lblChanges.Text = changes.ToString();
                    }
                    else
                    {

                        lblTotalCost.Text = Utility.CalculateExchangeRate(cu.Id, DebtAmount).ToString();
                        AmountWithExchange = Convert.ToDecimal(lblTotalCost.Text);
                        decimal receive = 0;

                        Decimal.TryParse(txtReceiveAmount.Text, out receive);
                        decimal changes = AmountWithExchange - receive;
                        lblChanges.Text = changes.ToString();
                    }
                    
                }
            }

        }

        private void print()
        {
            //Print Invoice
            #region [ Print ]

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
            int _tAmt = 0;
            foreach (TransactionDetail transaction in DetailList)
            {
                dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                newRow.ItemId = transaction.Product.ProductCode;
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
            else
            {
                reportPath = Application.StartupPath + "\\Epson\\Loc_InvoiceCash.rdlc";
            }

            //reportPath = Application.StartupPath + "\\Reports\\Loc_InvoiceCash.rdlc";
            rv.Reset();
            rv.LocalReport.ReportPath = reportPath;
            rv.LocalReport.DataSources.Add(rds);

            APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

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

            ReportParameter TransactionId = new ReportParameter("TransactionId", printId.ToString());
            rv.LocalReport.SetParameters(TransactionId);

            APP_Data.Counter c = entity.Counters.Where(x => x.Id == MemberShip.CounterId).FirstOrDefault();

            ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
            rv.LocalReport.SetParameters(CounterName);

            ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
            rv.LocalReport.SetParameters(PrintDateTime);

            ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
            rv.LocalReport.SetParameters(CasherName);

            Int64 totalAmountRep = printList.TotalAmount == null ? 0 : Convert.ToInt64(printList.TotalAmount);
            ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
            rv.LocalReport.SetParameters(TotalAmount);

            Int64 taxAmountRep = printList.TaxAmount == null ? 0 : Convert.ToInt64(printList.TaxAmount);
            ReportParameter TaxAmount = new ReportParameter("TaxAmount", taxAmountRep.ToString());
            rv.LocalReport.SetParameters(TaxAmount);

            Int64 disAmountRep = printList.DiscountAmount == null ? 0 : Convert.ToInt64(printList.DiscountAmount);
            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", disAmountRep.ToString());
            rv.LocalReport.SetParameters(DiscountAmount);

            ReportParameter PaidAmount = new ReportParameter("PaidAmount", txtReceiveAmount.Text);
            rv.LocalReport.SetParameters(PaidAmount);

            ReportParameter CurrencyCode = new ReportParameter("CurrencyCode", CurrencySymbol);
            rv.LocalReport.SetParameters(CurrencyCode);

            ReportParameter Change = new ReportParameter("Change", lblChanges.Text);
            rv.LocalReport.SetParameters(Change);

            PrintDoc.PrintReport(rv, "Slip");
            #endregion
        }
        
    }
}

