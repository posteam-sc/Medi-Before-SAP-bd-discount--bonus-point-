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
    public partial class Loc_PaidByCredit : Form
    {
        #region Variables

        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        public List<GiftSystem> GiftList = new List<GiftSystem>();

        public string DId { get; set; }

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int? CustomerId { get; set; }

        public int ExtraTax { get; set; }

        public Boolean isDraft { get; set; }

        public string DraftId { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        //private long outstandingBalance = 0;
        long total;
        long OldOutstandingAmount = 0;
        int PrepaidDebt = 0;

        #endregion

        #region Event
        public Loc_PaidByCredit()
        {
            InitializeComponent();
        }

        private void Loc_PaidByCredit_Load(object sender, EventArgs e)
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
            total = (long)(DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax);
            lblTotalCost.Text = Utility.CalculateExchangeRate(id, total).ToString();
            lblAccuralCost.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax).ToString();
            lblNetPayable.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax).ToString();
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
            else if (Convert.ToInt32(txtReceiveAmount.Text) >= Convert.ToInt32(lblTotalCost.Text))
            {
                DialogResult result = MessageBox.Show("Receive Amount is greater than Total cost. Are you sure want to change Cash Type! ", "mPOS", MessageBoxButtons.OKCancel);
                if (result.Equals(DialogResult.OK))
                {
                    hasError = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Receive Amount should be less than Total cost.");
                    txtReceiveAmount.Focus();
                    hasError = true;
                }

            }

            if (!hasError)
            {
               
                decimal totalAmount = 0; decimal totalCost = 0; decimal receiveAmount = 0;
                decimal.TryParse(txtReceiveAmount.Text, out receiveAmount);
                
                long tCost = (long)(DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax);
                decimal actualCost = (decimal)tCost;
                totalCost = tCost;
                decimal PayDebtAmount = 0;
                decimal.TryParse(lblPrePaid.Text, out PayDebtAmount);
                totalAmount = receiveAmount;
                //Currency cu = entity.Currencies.FirstOrDefault(x => x.Id == currencyId);
                if (lblNetPayableTitle.Text == "Change")
                {
                    totalAmount -= Convert.ToDecimal(lblNetPayable.Text);
                    receiveAmount -= Convert.ToDecimal(lblNetPayable.Text);
                }
                if (chkIsPrePaid.Checked)
                {
                    totalAmount += PayDebtAmount;
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

                //insert credit Transaction
                System.Data.Objects.ObjectResult<string> Id;
                Transaction insertedTransaction = new Transaction();
               
                string resultId;
                #region add sale, debt, prepaid transaction when receiveamount is greater than totalCost
                if (receiveAmount > totalCost)
                {
                    Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, ExtraTax + Tax, ExtraDiscount + Discount, actualCost, totalCost, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                    resultId = Id.FirstOrDefault().ToString();
                    insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                    string TId = insertedTransaction.Id;
                    insertedTransaction.IsDeleted = false;
                    insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                    foreach (TransactionDetail detail in DetailList)
                    {

                        var detailID = entity.InsertTransactionDetail(TId, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, Convert.ToDouble(detail.IsDeductedBy), false).SingleOrDefault();
                        detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                        detail.IsDeleted = false;//Update IsDelete (Null to 0)
                        detail.Product.Qty = detail.Product.Qty - detail.Qty;
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
                                }
                            }
                        }

                    }
                    totalAmount -= totalCost;
                    receiveAmount -= totalCost;
                    //bool prepaidCredit = false;
                    entity.SaveChanges();
                    //ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                    //ex.TransactionId = TId;
                    //ex.CurrencyId = cu.Id;
                    //ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                    //entity.ExchangeRateForTransactions.Add(ex);
                    //entity.SaveChanges();
                    #region current transation paid and remain amount is paid other transaction
                    if (totalAmount != 0)
                    {
                        //prepaidCredit = true;
                        decimal tamount = 0;long Ramount = 0;
                        decimal differentAmount = totalAmount;
                        decimal DebtAmount = 0;
                        List<Transaction> tList = new List<Transaction>();
                        List<Transaction> RefundList = new List<Transaction>();
                        foreach (Transaction t in transList)
                        {
                            tamount = (long)t.TotalAmount - (long)t.RecieveAmount;
                            RefundList = (from tr in entity.Transactions where tr.ParentId == t.Id && tr.Type == TransactionType.CreditRefund select tr).ToList();
                            if (RefundList.Count > 0)
                            {
                                foreach (Transaction TRefund in RefundList)
                                {
                                    Ramount -= (long)TRefund.RecieveAmount;
                                }
                                //tamount = Utility.CalculateExchangeRate(currencyId, Ramount);
                                tamount = Ramount;
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

                        if (tList.Count > 0)
                        {
                            foreach (Transaction t in tList)
                            {
                                decimal CreditAmount = 0;
                                //CreditAmount = Utility.CalculateExchangeRate(currencyId, (decimal)t.TotalAmount) - Utility.CalculateExchangeRate(currencyId, (decimal)t.RecieveAmount);
                                CreditAmount = (decimal)t.TotalAmount - (decimal)t.RecieveAmount;
                                RefundList = (from tr in entity.Transactions where tr.ParentId == t.Id && tr.Type == TransactionType.CreditRefund select tr).ToList();
                                if (RefundList.Count > 0)
                                {
                                    foreach (Transaction TRefund in RefundList)
                                    {
                                        //CreditAmount -= Utility.CalculateExchangeRate(currencyId, (decimal)TRefund.RecieveAmount);
                                        CreditAmount -= (decimal)TRefund.RecieveAmount;
                                    }
                                }
                                if (CreditAmount <= totalAmount)
                                {
                                    Transaction CreditT = (from tr in entity.Transactions where tr.Id == t.Id select tr).FirstOrDefault<Transaction>();
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
                                        foreach (Transaction PrePaidDebtTrans in prePaidTranList)
                                        {
                                            decimal PrePaidamount = 0;

                                            decimal useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                            //PrePaidamount = Utility.CalculateExchangeRate(currencyId, (decimal)PrePaidDebtTrans.TotalAmount) - Utility.CalculateExchangeRate(currencyId, (decimal)useAmount);
                                            PrePaidamount = (decimal)PrePaidDebtTrans.TotalAmount - (decimal)useAmount;
                                            if (CreditAmount >= PrePaidamount)
                                            {
                                                PrePaidDebtTrans.IsActive = true;
                                                UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                                usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                                usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                                usePrePaidDObj.CreditTransactionId = t.Id;
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
                                                usePrePaidDObj.CreditTransactionId = t.Id;
                                                usePrePaidDObj.CashierId = MemberShip.UserId;
                                                usePrePaidDObj.CounterId = MemberShip.CounterId;
                                                entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                                entity.SaveChanges();
                                                //CreditAmount -= PrePaidamount;
                                                CreditAmount = 0;
                                            }
                                        }

                                    }

                                    prePaidTranList = (from PDT in entity.Transactions where PDT.Type == TransactionType.Prepaid && PDT.IsActive == false select PDT).ToList();
                                }

                            }
                            if (DebtAmount > 0)
                            {
                                System.Data.Objects.ObjectResult<string> DebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Settlement, true, true, 1, 0, 0, DebtAmount, DebtAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                                entity.SaveChanges();
                            }
                        }
                        else
                        {
                            totalAmount -= PrepaidDebt;
                            receiveAmount -= PrepaidDebt;
                            //System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.PrepaidDebt, true, false, 1, 0, 0, totalAmount, totalAmount, null, customerId);
                            //entity.SaveChanges();
                        }

                    }
                    #endregion

                    if (receiveAmount > 0)
                    {
                        System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Prepaid, true, false, 1, 0, 0, receiveAmount, receiveAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                        entity.SaveChanges();
                    }
                }
                #endregion

                #region add credit Transaction
                else
                {
                    if (totalAmount >= totalCost)
                    {
                        if (chkIsPrePaid.Checked == true)
                        {
                            Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount, receiveAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                            totalAmount -= receiveAmount;
                            totalCost -= receiveAmount;
                            resultId = Id.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                            foreach (TransactionDetail detail in DetailList)
                            {

                                var detailID = entity.InsertTransactionDetail(insertedTransaction.Id, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, Convert.ToDouble(detail.IsDeductedBy), false).SingleOrDefault();
                                detail.IsDeleted = false;//Update IsDelete (Null to 0)
                                detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                detail.Product.Qty = detail.Product.Qty - detail.Qty;
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
                                        }
                                    }
                                }

                            }
                            insertedTransaction.IsDeleted = false;
                            insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                            entity.SaveChanges();
                            foreach (Transaction PrePaidDebtTrans in prePaidTranList)
                            {
                                long PrePaidamount = 0;
                                //int useAmount = 0;
                                int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                if (totalCost >= PrePaidamount)
                                {
                                    PrePaidDebtTrans.IsActive = true;
                                    totalCost -= PrePaidamount;
                                    totalAmount -= PrePaidamount;
                                    entity.SaveChanges();
                                    UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                    usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                    usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                    usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                    usePrePaidDObj.CashierId = MemberShip.UserId;
                                    usePrePaidDObj.CounterId = MemberShip.CounterId;
                                    entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                    entity.SaveChanges();
                                }
                                else
                                {
                                    UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                    usePrePaidDObj.UseAmount = (int)totalCost;
                                    usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                    usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                    usePrePaidDObj.CashierId = MemberShip.UserId;
                                    usePrePaidDObj.CounterId = MemberShip.CounterId;
                                    entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                    entity.SaveChanges();
                                    totalAmount -= totalCost;
                                    totalCost = 0;
                                }

                            }
                        }
                        else
                        {
                            if (chkIsPrePaid.Checked == true)
                            {
                                Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount, receiveAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);

                                totalAmount -= receiveAmount;
                                totalCost -= receiveAmount;
                                resultId = Id.FirstOrDefault().ToString();
                                insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                                foreach (TransactionDetail detail in DetailList)
                                {
                                    detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                    detail.Product.Qty = detail.Product.Qty - detail.Qty;
                                    if (detail.Product.Brand.Name == "Special Promotion")
                                    {
                                        List<WrapperItem> wList = detail.Product.WrapperItems.ToList();
                                        if (wList.Count > 0)
                                        {
                                            foreach (WrapperItem w in wList)
                                            {
                                                Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                                                wpObj.Qty = wpObj.Qty - detail.Qty;

                                            }
                                        }
                                    }
                                    //detail.IsDeleted = false;
                                    insertedTransaction.TransactionDetails.Add(detail);
                                }
                                insertedTransaction.IsDeleted = false;
                                insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                                entity.SaveChanges();
                                foreach (Transaction PrePaidDebtTrans in prePaidTranList)
                                {
                                    long PrePaidamount = 0;
                                    //int useAmount = 0;
                                    int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                    PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                    if (totalCost >= PrePaidamount)
                                    {
                                        PrePaidDebtTrans.IsActive = true;
                                        totalCost -= PrePaidamount;
                                        totalAmount -= PrePaidamount;
                                        entity.SaveChanges();
                                        UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                        usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                        usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                        usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                        usePrePaidDObj.CashierId = MemberShip.UserId;
                                        usePrePaidDObj.CounterId = MemberShip.CounterId;
                                        entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                        entity.SaveChanges();
                                    }
                                    else
                                    {
                                        UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                        usePrePaidDObj.UseAmount = (int)totalCost;
                                        usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                        usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                        usePrePaidDObj.CashierId = MemberShip.UserId;
                                        usePrePaidDObj.CounterId = MemberShip.CounterId;
                                        entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                        entity.SaveChanges();
                                        totalAmount -= totalCost;
                                        totalCost = 0;
                                    }

                                }
                            }
                            else
                            {
                                Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount, receiveAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                                totalAmount -= receiveAmount;
                                totalCost -= receiveAmount;
                                resultId = Id.FirstOrDefault().ToString();
                                insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                                foreach (TransactionDetail detail in DetailList)
                                {
                                    detail.IsDeleted = false;//Update IsDelete (Null to 0)
                                    detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                    detail.Product.Qty = detail.Product.Qty - detail.Qty;
                                    if (detail.Product.Brand.Name == "Special Promotion")
                                    {
                                        List<WrapperItem> wList = detail.Product.WrapperItems.ToList();
                                        if (wList.Count > 0)
                                        {
                                            foreach (WrapperItem w in wList)
                                            {
                                                Product wpObj = (from p in entity.Products where p.Id == w.ChildProductId select p).FirstOrDefault();
                                                wpObj.Qty = wpObj.Qty - detail.Qty;

                                            }
                                        }
                                    }
                                    detail.IsDeleted = false;
                                    insertedTransaction.TransactionDetails.Add(detail);
                                }
                                insertedTransaction.IsDeleted = false;
                                insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                                entity.SaveChanges();
                            }
                        }
                        //if (totalAmount > 0)
                        //{
                        //    System.Data.Objects.ObjectResult<string> PreDebtId = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.PrepaidDebt, true, false, 1, 0, 0, totalAmount, totalAmount, null, customerId);
                        //    entity.SaveChanges();
                        //}
                    }
                    else
                    {
                        if (chkIsPrePaid.Checked == true)
                        {
                            //Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount, receiveAmount, null, customerId);
                            Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount, receiveAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                            totalAmount -= receiveAmount;
                            totalCost -= receiveAmount;
                            resultId = Id.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                            foreach (TransactionDetail detail in DetailList)
                            {
                                detail.IsDeleted = false;//Update IsDelete (Null to 0)
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

                                            }
                                        }
                                    }
                                }
                               
                                // detail.IsDeleted = false;
                                insertedTransaction.TransactionDetails.Add(detail);
                            }
                            insertedTransaction.IsDeleted = false;
                            insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                            entity.SaveChanges();
                            foreach (Transaction PrePaidDebtTrans in prePaidTranList)
                            {
                                long PrePaidamount = 0;
                                //int useAmount = 0;
                                int useAmount = (PrePaidDebtTrans.UsePrePaidDebts1 == null) ? 0 : (int)PrePaidDebtTrans.UsePrePaidDebts1.Sum(x => x.UseAmount);
                                PrePaidamount = (long)PrePaidDebtTrans.TotalAmount - useAmount;
                                if (totalCost >= PrePaidamount)
                                {
                                    PrePaidDebtTrans.IsActive = true;
                                    totalCost -= PrePaidamount;
                                    totalAmount -= PrePaidamount;
                                    entity.SaveChanges();
                                    UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                    usePrePaidDObj.UseAmount = (int)PrePaidamount;
                                    usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                    usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                    usePrePaidDObj.CashierId = MemberShip.UserId;
                                    usePrePaidDObj.CounterId = MemberShip.CounterId;
                                    entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                    entity.SaveChanges();
                                }
                                else
                                {
                                    UsePrePaidDebt usePrePaidDObj = new UsePrePaidDebt();
                                    usePrePaidDObj.UseAmount = (int)totalCost;
                                    usePrePaidDObj.PrePaidDebtTransactionId = PrePaidDebtTrans.Id;
                                    usePrePaidDObj.CreditTransactionId = insertedTransaction.Id;
                                    usePrePaidDObj.CashierId = MemberShip.UserId;
                                    usePrePaidDObj.CounterId = MemberShip.CounterId;
                                    entity.UsePrePaidDebts.Add(usePrePaidDObj);
                                    entity.SaveChanges();
                                    totalAmount -= totalCost;
                                    totalCost = 0;
                                }

                            }
                        }
                        else
                        {
                            Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, false, true, 2, ExtraTax + Tax, ExtraDiscount + Discount, DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount, totalAmount, null, customerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                            resultId = Id.FirstOrDefault().ToString();
                            insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();

                            insertedTransaction.IsDeleted = false;
                            insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;
                            foreach (TransactionDetail detail in DetailList)
                            {
                                detail.IsDeleted = false;//Update IsDelete (Null to 0)
                                detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                                detail.Product.Qty = detail.Product.Qty - detail.Qty;
                                //detail.IsDeleted = false;
                                insertedTransaction.TransactionDetails.Add(detail);
                            }
                            entity.SaveChanges();
                        }
                    }
                }
                #endregion

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

                    entity.SaveChanges();
                }
                GiftList.Clear();
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
                        newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty;
                        newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                        _tAmt += newRow.TotalAmount;
                        dtReport.AddItemListRow(newRow);
                    }

                    string reportPath = "";
                    ReportViewer rv = new ReportViewer();
                    ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);

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


                    string _Point = Loc_CustomerPointSystem.GetPointFromCustomerId(Convert.ToInt32(cboCustomerList.SelectedValue)).ToString();
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

                    APP_Data.Counter counter = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                    ReportParameter CounterName = new ReportParameter("CounterName", counter.Name);
                    rv.LocalReport.SetParameters(CounterName);

                    ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                    rv.LocalReport.SetParameters(PrintDateTime);

                    ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                    rv.LocalReport.SetParameters(CasherName);

                   // ReportParameter TotalAmount = new ReportParameter("TotalAmount",  Convert.ToInt32(insertedTransaction.TotalAmount + insertedTransaction.DiscountAmount).ToString());
                    ReportParameter TotalAmount = new ReportParameter("TotalAmount", Convert.ToInt32(insertedTransaction.TotalAmount ).ToString());
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

                    ReportParameter NetPayable = new ReportParameter("NetPayable", Convert.ToInt32(OldOutstandingAmount + insertedTransaction.TotalAmount - PayDebtAmount).ToString());
                    rv.LocalReport.SetParameters(NetPayable);

                    ReportParameter Balance = new ReportParameter("Balance", Convert.ToInt32((OldOutstandingAmount + insertedTransaction.TotalAmount - PayDebtAmount) - Convert.ToInt64(txtReceiveAmount.Text)).ToString());
                    rv.LocalReport.SetParameters(Balance);

                      int cuId= Convert.ToInt32(cboCustomerList.SelectedValue);
                      var _title = (from c in entity.Customers where c.Id == cuId select c.Title).FirstOrDefault();
                    ReportParameter CustomerName = new ReportParameter("CustomerName", _title + " " + cboCustomerList.Text);
                    rv.LocalReport.SetParameters(CustomerName);
                    for (int i = 0; i <= 1; i++)
                    {
                        PrintDoc.PrintReport(rv, "Slip");
                    }
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cboCustomerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrepaidDebt = 0;
            int currencyId =Convert.ToInt32(cboCurrency.SelectedValue);
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
                        int useAmount = (ts.UsePrePaidDebts1 == null) ? 0 : (int)ts.UsePrePaidDebts1.Sum(x => x.UseAmount);
                        PrepaidDebt -= useAmount;
                    }
                }


                if (OldOutstandingAmount < 0)
                {
                    lblPreviousBalance.Text = "0";
                    lblPrePaid.Text = Utility.CalculateExchangeRate(currencyId, PrepaidDebt).ToString(); 
                    total = (long)(DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax);
                    lblTotalCost.Text = Utility.CalculateExchangeRate(currencyId,total).ToString();
                    lblNetPayable.Text = Utility.CalculateExchangeRate(currencyId,total).ToString();
                }
                else
                {
                    lblPreviousBalance.Text = OldOutstandingAmount.ToString();
                    lblPrePaid.Text = Utility.CalculateExchangeRate(currencyId, PrepaidDebt).ToString(); 
                    total = (long)((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) + OldOutstandingAmount - PrepaidDebt);
                    lblTotalCost.Text = Utility.CalculateExchangeRate(currencyId,total).ToString();
                    lblNetPayable.Text = Utility.CalculateExchangeRate(currencyId, total).ToString();
                }
                //to 
                decimal amount = 0;
                decimal.TryParse(txtReceiveAmount.Text, out amount);
                decimal totalCost = 0;
                decimal.TryParse(lblTotalCost.Text, out totalCost);

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
                total = (long)((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) + 0);
                lblTotalCost.Text = Utility.CalculateExchangeRate(currencyId, total).ToString();
                lblNetPayable.Text = Utility.CalculateExchangeRate(currencyId, total).ToString();

                //to 
                decimal amount = 0;
                decimal.TryParse(txtReceiveAmount.Text, out amount);

                if (amount >= (decimal.Parse(lblTotalCost.Text)))
                {
                    decimal value = Convert.ToDecimal(lblTotalCost.Text);
                    lblNetPayableTitle.Text = "Change";
                    lblNetPayable.Text = (amount - value).ToString();
                }
                else
                {
                    decimal value = Convert.ToDecimal(lblTotalCost.Text);
                    lblNetPayableTitle.Text = "NetPayable";
                    lblNetPayable.Text = (value - amount).ToString();
                }
            }
        }

        private void txtReceiveAmount_KeyUp(object sender, KeyEventArgs e)
        {
            decimal amount = 0;
            decimal.TryParse(txtReceiveAmount.Text, out amount);
            decimal totalCost = 0;
            decimal.TryParse(lblTotalCost.Text, out totalCost);

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

        private void Loc_PaidByCredit_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(cboCustomerList);
            tp.Hide(txtReceiveAmount);
        }

        private void txtReceiveAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && char.IsLetter('.'))
            {
                e.Handled = true;
            }
        }

        private void chkIsPrePaid_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsPrePaid.Checked == false)
            {
                lblTotalCost.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) + OldOutstandingAmount).ToString();
                lblNetPayable.Text = ((DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax) + OldOutstandingAmount).ToString();
            }
        }

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

        private void cboCurrency_SelectedValueChanged(object sender, EventArgs e)
        {
            int currencyId = 0;                
            Int32.TryParse(cboCurrency.SelectedValue.ToString(),out currencyId);
            if (currencyId != 0)
            {
                
                Currency cu = entity.Currencies.FirstOrDefault(x => x.Id == currencyId);
                if (cu != null)
                {
                    lblTotalCost.Text = Utility.CalculateExchangeRate(cu.Id, total).ToString();
                    lblNetPayable.Text = Utility.CalculateExchangeRate(cu.Id, total).ToString();
                }
            }
        }
    }
}
