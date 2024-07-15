using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;
using Microsoft.Reporting.WinForms;
using System.Threading;

namespace POS
{
    public partial class Sales : Form
    {
        #region Variables

        public string mssg = "";

        private POSEntities entity = new POSEntities();

        private Boolean isDraft = false;

        private String DraftId = string.Empty;

        public int CurrentCustomerId = 0;
        bool isload = false;
        bool isCredit = false;
        bool IsBirthday = false;
        decimal disRate = 0;

        public EventArgs e { get; set; }

        private List<GiftSystem> GiftList = new List<GiftSystem>();
        private List<GiftSystem> GivenGiftList = new List<GiftSystem>();

        int proCount = 0;

        public int _rowIndex;

        List<int> _priceList;
        public static decimal birthdayDiscount = 0;
        #endregion

        #region Events

        public Sales()
        {
            InitializeComponent();

        }

        public static IQueryable<Product> iTempP { get; set; }

        public static void getCommonProduct()
        {
            POSEntities entity = new POSEntities();
            iTempP = (from p in entity.Products join pd in entity.ProductCategories on p.ProductCategoryId equals pd.Id where !pd.Name.Contains("GWP") select p).Distinct();
        }

        public enum sCol
        {
            BarCode,
            SKU,
            Qty,
            ItemName,
            BatchNo,
            SalePrice,
            DisPercent,
            Tax,
            Cost,
            Delete,
            pId,
            status
        }

        delegate void DelLoadProductNameList();

        private void LoadProductNameList()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.cboProductName.InvokeRequired)
            {

                DelLoadProductNameList d = new DelLoadProductNameList(LoadProductNameListDe);
                this.Invoke(d);
            }

        }

        private void LoadProductNameListDe()
        {
            List<Product> productList = new List<Product>();
            Product product = new Product();
            product.Id = 0;
            product.Name = "";
            if (iTempP == null)
            {
                getCommonProduct();
            }
            //if (iTempStockFillingFromSAP == null || iTempStockFillingFromSAP.Count() < 1)
            //{
            //    getCommonStockFillingFromSAP();
            //}
            productList.Add(product);//not contain the GWP product when sale
            productList.AddRange(iTempP.ToList());
            cboProductName.DataSource = productList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "Id";
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void Sales_Load(object sender, EventArgs e)
        {
            this.cboPaymentMethod.TextChanged -= new EventHandler(cboPaymentMethod_TextChanged);
            cboPaymentMethod.SelectedIndexChanged -= cboPaymentMethod_SelectedIndexChanged;
            dgvSearchProductList.AutoGenerateColumns = false;
            cboPaymentMethod.DataSource = entity.PaymentMethods.Where(x => x.PaymentParentId == null).ToList();
            cboPaymentMethod.DisplayMember = "Name";
            cboPaymentMethod.ValueMember = "Id";

            List<Product> productList = new List<Product>();
            Product product = new Product();
            product.Id = 0;
            product.Name = "";
            productList.Add(product);//not contain the GWP product when sale
            productList.AddRange((from p in entity.Products join pd in entity.ProductCategories on p.ProductCategoryId equals pd.Id where !pd.Name.Contains("GWP") select p).Distinct().ToList());
            cboProductName.DataSource = productList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "Id";
            Thread tLoadProduct = new Thread(LoadProductNameList);
            // Set the priority of threads
            tLoadProduct.Priority = ThreadPriority.Normal;
            
            tLoadProduct.Start();
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;
            ReloadCustomerList();
            dgvSalesItem.Focus();
            lblGift.Visible = false;
            plGift.Visible = false;
            isload = false;
            this.cboPaymentMethod.TextChanged += new EventHandler(cboPaymentMethod_TextChanged);
            cboPaymentMethod.SelectedIndexChanged += cboPaymentMethod_SelectedIndexChanged;
            cboPaymentMethod_SelectedIndexChanged(sender, e);
            birthdayDiscount = SettingController.birthday_discount;
        }

        private void dgvSalesItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _rowIndex = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 8)
                {
                    object deleteProductCode = dgvSalesItem[1, e.RowIndex].Value;

                    //If product code is null, this is just new role without product. Do not need to delete the row.
                    if (deleteProductCode != null)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            dgvSalesItem.Rows.RemoveAt(e.RowIndex);
                            UpdateTotalCost();
                            dgvSalesItem.CurrentCell = dgvSalesItem[0, e.RowIndex];
                        }
                    }
                }
                else if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2)
                {
                    dgvSalesItem.CurrentCell = dgvSalesItem.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dgvSalesItem.BeginEdit(true);
                }
            }
        }

        private void dgvSalesItem_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _rowIndex = e.RowIndex;
            entity = new POSEntities();
            if (e.RowIndex >= 0)
            {

                DataGridViewRow row = dgvSalesItem.Rows[e.RowIndex];
                dgvSalesItem.CommitEdit(new DataGridViewDataErrorContexts());
                if (row.Cells[0].Value != null || row.Cells[1].Value != null)
                {

                    //Barcode Change
                    if (e.ColumnIndex == 0)
                    {
                        string currentBarcode = row.Cells[0].Value.ToString();

                        //get current product
                        Product pro = (from p in entity.Products where p.Barcode == currentBarcode select p).FirstOrDefault<Product>();
                        if (pro != null)
                        {
                            //fill the current row with the product information
                            isload = true;
                            row.Cells[1].Value = pro.ProductCode;
                            row.Cells[2].Value = 1;
                            row.Cells[3].Value = pro.Name;
                            row.Cells[4].Value = pro.Price;
                            row.Cells[5].Value = pro.DiscountRate;
                            row.Cells[6].Value = pro.Tax.TaxPercent;
                            row.Cells[7].Value = getActualCost(pro);
                            row.Cells[9].Value = pro.Id;
                        }
                        else
                        {
                            //remove current row if input have no associate product
                            MessageBox.Show("Wrong item code");
                            mssg = "Wrong";
                        }
                    }
                    //Product Code Change
                    else if (e.ColumnIndex == 1)
                    {
                        string currentProductCode = row.Cells[1].Value.ToString();

                        //get current product //not contain the GWP product when sale
                        Product pro = (from p in entity.Products join pd in entity.ProductCategories on p.ProductCategoryId equals pd.Id where p.ProductCode == currentProductCode && !pd.Name.Contains("GWP") select p).FirstOrDefault<Product>();
                        if (pro != null)
                        {
                            isload = true;
                            //fill the current row with the product information
                            row.Cells[9].Value = pro.Id;
                            row.Cells[0].Value = pro.Barcode;
                            row.Cells[1].Value = currentProductCode;
                            row.Cells[2].Value = 1;
                            row.Cells[3].Value = pro.Name;
                            row.Cells[4].Value = pro.Price;
                            row.Cells[5].Value = pro.DiscountRate;
                            row.Cells[6].Value = pro.Tax.TaxPercent;
                            row.Cells[7].Value = getActualCost(pro);

                        }
                        else
                        {
                            //remove current row if input have no associate product
                            MessageBox.Show("Wrong item code");
                            mssg = "Wrong";
                        }

                        //check if current row isn't topmost
                        if (e.RowIndex > 0 && mssg == "")
                        {
                            Check_ProductCode_Exist(currentProductCode);
                        }
                    }
                    //Qty Changes
                    else if (e.ColumnIndex == 2)
                    {

                        if (isload == false)
                        {
                            Decimal currentDiscountRate = 0;

                            int discountRate = 0;

                            currentDiscountRate = Convert.ToDecimal(row.Cells[5].Value);
                            try
                            {
                                if (currentDiscountRate.ToString() != null && currentDiscountRate != 0)
                                {
                                    currentDiscountRate = Convert.ToDecimal(row.Cells[5].Value.ToString());
                                    discountRate = Convert.ToInt32(currentDiscountRate);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Input Discount rate have invalid keywords.");
                                row.Cells[5].Value = "0.00";
                            }


                            string currentProductCode = row.Cells[1].Value.ToString();



                            //get current Project by Id
                            Product pro = (from p in entity.Products where p.ProductCode == currentProductCode select p).FirstOrDefault<Product>();


                            int currentQty = 1;
                            try
                            {
                                //get updated qty
                                currentQty = Convert.ToInt32(row.Cells[2].Value);
                                if (currentQty.ToString() != null && currentQty != 0)
                                {
                                    row.DefaultCellStyle.BackColor = Color.White;
                                }
                                else
                                {
                                    row.DefaultCellStyle.BackColor = Color.Red;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Input quantity have invalid keywords.");
                                row.Cells[2].Value = "1";
                            }
                            row.Cells[7].Value = currentQty * getActualCost(pro, discountRate);
                        }
                        else
                        {
                            string currentProductCode = row.Cells[1].Value.ToString();
                            //get current Project by Id
                            Product pro = (from p in entity.Products where p.ProductCode == currentProductCode select p).FirstOrDefault<Product>();

                            int currentQty = 1;
                            try
                            {
                                //get updated qty
                                currentQty = Convert.ToInt32(row.Cells[2].Value);
                                if (currentQty.ToString() != null && currentQty != 0)
                                {
                                    row.DefaultCellStyle.BackColor = Color.White;
                                }
                                else
                                {
                                    row.DefaultCellStyle.BackColor = Color.Red;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Input quantity have invalid keywords.");
                                row.Cells[2].Value = "1";
                            }
                            // by SYM
                            //        row.Cells[4].Value = getActualCost(pro);
                            row.Cells[7].Value = currentQty * getActualCost(pro);
                            isload = false;

                        }


                    }
                    //Discount Rate Change
                    else if (e.ColumnIndex == 5)
                    {
                        string currentProductCode = row.Cells[1].Value.ToString();
                        //get current Project by Id
                        Product pro = (from p in entity.Products where p.ProductCode == currentProductCode select p).FirstOrDefault<Product>();


                        int currentQty = 1;
                        try
                        {
                            //get updated qty
                            currentQty = Convert.ToInt32(row.Cells[2].Value);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Input quantity have invalid keywords.");
                            row.Cells[2].Value = "1";
                        }

                        decimal DiscountRate = 0;

                        try
                        {
                            //get updated qty
                            // Decimal.TryParse(row.Cells[5].Value.ToString(), out DiscountRate);
                            DiscountRate = Convert.ToDecimal(row.Cells[5].Value);
                            if (DiscountRate > 100)
                            {
                                row.Cells[5].Value = 100;
                                DiscountRate = 100;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Input Discount rate have invalid keywords.");
                            row.Cells[5].Value = "0.00";
                        }

                        row.Cells[7].Value = currentQty * getActualCost(pro, DiscountRate);
                    }

                    if (mssg == "")
                    {
                        Cell_ReadOnly();
                    }
                    UpdateTotalCost();
                }
                else
                {
                    dgvSalesItem.CurrentCell = dgvSalesItem[0, e.RowIndex];
                    MessageBox.Show("You need to input product code or barcode firstly in order to add product quentity!");
                    mssg = "Wrong";
                }
            }
        }

        private void btnPaid_Click(object sender, EventArgs e)
        {
            List<TransactionDetail> DetailList = GetTranscationListFromDataGridView();
            if (DetailList.Count() != 0)
            {

                List<int> index = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                                   where r.Cells[2].Value == null || r.Cells[2].Value.ToString() == String.Empty || r.Cells[2].Value.ToString() == "0"
                                   select r.Index).ToList();


                index.RemoveAt(index.Count - 1);

                if (index.Count > 0)
                {

                    foreach (var a in index)
                    {
                        dgvSalesItem.Rows[a].DefaultCellStyle.BackColor = Color.Red;

                    }
                    return;
                }

                if (cboCustomer.SelectedIndex > 0)
                {
                    //check if gift has
                    if (GiftList.Count > 0)
                    {
                        GivenGiftList.Clear();
                        for (int i = 0; i < chkGiftList.Items.Count; i++)
                        {
                            if (chkGiftList.GetItemChecked(i) == true)
                            {
                                GivenGiftList.Add(GiftList[i]);
                            }
                        }
                    }
                    else
                    {
                        GivenGiftList.Clear();
                    }
                    if (GivenGiftList.Count > 0)
                    {
                        foreach (GiftSystem gObj in GivenGiftList)
                        {
                            if (gObj.Product1 != null)
                            {
                                TransactionDetail tDObj = new TransactionDetail();
                                tDObj.ProductId = gObj.Product1.Id;
                                tDObj.TotalAmount = gObj.PriceForGiftProduct;
                                tDObj.DiscountRate = 0;
                                tDObj.TaxRate = 0;
                                tDObj.Qty = 1;
                                tDObj.UnitPrice = 0;
                                DetailList.Add(tDObj);
                            }
                        }
                    }

                    #region MultiPayment
                    Boolean hasError = false;
                    int _extraDiscount = 0;
                    Int32.TryParse(txtAdditionalDiscount.Text, out _extraDiscount);
                    int _extraTax = 0;
                    Int32.TryParse(txtExtraTax.Text, out _extraTax);


                    Currency cu = entity.Currencies.FirstOrDefault(x => x.Id == 1);

                    Transaction insertedTransaction = new Transaction();
                    int paidAmount = 0; bool isFoc = false; int giftcardAmt = 0; int creditAmount = 0;
                    foreach (DataGridViewRow row in dgvPaymentType.Rows)
                    {
                        if (row.Cells[5].Value == null)
                        {
                            paidAmount = 0;
                            isFoc = true;
                        }
                        else
                        {
                            if (row.Cells[2].Value.ToString() == "Gift Card")
                            {
                                giftcardAmt += Convert.ToInt32(row.Cells[5].Value);
                            }
                            if (row.Cells[2].Value.ToString() == "Credit")
                            {
                                creditAmount += Convert.ToInt32(row.Cells[5].Value);
                            }
                            paidAmount += Convert.ToInt32(row.Cells[5].Value);

                        }
                    }

                    int totalAmount = Convert.ToInt32(lblTotal.Text);
                    if (paidAmount == 0 && isFoc == false)
                    {
                        MessageBox.Show("Please fill up receive amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        hasError = true;
                    }
                    else if (totalAmount > (paidAmount + _extraDiscount) && isFoc == false)
                    {
                        MessageBox.Show("Receive amount must be greater than total cost!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        hasError = true;
                    }

                    if (!hasError)
                    {
                        System.Data.Objects.ObjectResult<String> Id;

                        long totalCost = (long)DetailList.Sum(x => x.TotalAmount) - _extraDiscount;
                        if (!isFoc)
                        {
                            //Sale
                            if (creditAmount == 0)
                            {
                                Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 7, _extraTax + Convert.ToInt32(lblTaxTotal.Text), _extraDiscount + Convert.ToInt32(lblDiscountTotal.Text), totalCost, paidAmount - giftcardAmt, null, Convert.ToInt32(cboCustomer.SelectedValue), SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                            }
                            //Credit
                            else
                            {
                                Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Credit, true, true, 2, _extraTax + Convert.ToInt32(lblTaxTotal.Text), _extraDiscount + Convert.ToInt32(lblDiscountTotal.Text), totalCost, paidAmount - (giftcardAmt + creditAmount), null, Convert.ToInt32(cboCustomer.SelectedValue), SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                            }
                        }
                        //FOC
                        else
                        {
                            Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 7, 0, 0, 0, 0, null, Convert.ToInt32(cboCustomer.SelectedValue), SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                        }
                        entity = new POSEntities();
                        string resultId = Id.FirstOrDefault().ToString();
                        insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                        string TId = insertedTransaction.Id;
                        insertedTransaction.IsDeleted = false;
                        insertedTransaction.ReceivedCurrencyId = 1;


                        foreach (TransactionDetail detail in DetailList)
                        {
                            detail.IsDeleted = false;//Update IsDelete (Null to 0)
                           
                            detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                            if (!string.IsNullOrEmpty(txtVIPID.Text) && cboCustomer.SelectedValue != null && (int)cboCustomer.SelectedValue > 0 && IsBirthday && detail.DiscountRate >= SettingController.birthday_discount)
                            {
                                IsBirthday = true;

                            }
                            //    var detailID = entity.InsertTransactionDetail(TId, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, detail.ConsignmentPrice, IsConsignmentPaid).SingleOrDefault();
                            var detailID = entity.InsertTransactionDetail(TId, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, Convert.ToDouble(detail.IsDeductedBy), IsBirthday).SingleOrDefault();



                            var detailforBDdiscount = entity.TransactionDetails.Find(detailID);

                            detail.Product.Qty = detail.Product.Qty - detail.Qty;

                            //save in stocktransaction



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
                                        entity.insertSPDetail(spDetail.TransactionDetailID, spDetail.ParentProductID, spDetail.ChildProductID, spDetail.Price, spDetail.DiscountRate, "PC");
                                        //entity.SPDetails.Add(spDetail);
                                    }
                                }
                            }

                            entity.SaveChanges();
                        }
                        //save in stocktransaction


                        if (giftcardAmt != 0)
                        {
                            foreach (DataGridViewRow row in dgvPaymentType.Rows)
                            {
                                if (row.Cells[2].Value.ToString() == "Gift Card")
                                {
                                    string cardNumber = row.Cells[3].Value.ToString();
                                    int giftcardid = entity.GiftCards.Where(x => x.CardNumber.Trim() == cardNumber).Select(x => x.Id).FirstOrDefault();

                                    if (giftcardid != 0)
                                    {
                                        GiftCardInTransaction gic = new GiftCardInTransaction();
                                        gic.TransactionId = TId;
                                        gic.GiftCardId = giftcardid;
                                        entity.GiftCardInTransactions.Add(gic);
                                        //Clear giftcard in giftcard list

                                        GiftCard giftcard = entity.GiftCards.Where(x => x.Id == giftcardid).FirstOrDefault();
                                        giftcard.IsUsed = true;
                                    }

                                }
                            }
                        }


                        List<MultiPayment> multiPaymentList = new List<MultiPayment>();
                        foreach (DataGridViewRow row in dgvPaymentType.Rows)
                        {
                            if (multiPaymentList.Count != 0)
                            {

                                var data = multiPaymentList.Where(x => x.id == (int)row.Cells[0].Value).FirstOrDefault();
                                if (data != null)
                                {
                                    data.amount += Convert.ToInt32(row.Cells[5].Value);
                                }
                                else
                                {
                                    MultiPayment multiPayment = new MultiPayment();
                                    multiPayment.id = Convert.ToInt32(row.Cells[0].Value);
                                    multiPayment.paymentName = Convert.ToString(row.Cells[2].Value);
                                    multiPayment.amount = Convert.ToInt32(row.Cells[5].Value);
                                    multiPaymentList.Add(multiPayment);
                                }
                            }
                            else
                            {
                                MultiPayment multiPayment = new MultiPayment();
                                multiPayment.id = Convert.ToInt32(row.Cells[0].Value);
                                multiPayment.paymentName = Convert.ToString(row.Cells[2].Value);
                                multiPayment.amount = Convert.ToInt32(row.Cells[5].Value);
                                multiPaymentList.Add(multiPayment);
                            }
                        }

                        foreach (var item in multiPaymentList)
                        {
                            if (item.paymentName == "Credit")
                            {
                                isCredit = true;
                            }
                            TransactionPaymentDetail tranPaymentDetail = new TransactionPaymentDetail();
                            tranPaymentDetail.TransactionId = TId;
                            tranPaymentDetail.PaymentMethodId = item.id;
                            tranPaymentDetail.Amount = item.amount;
                            entity.TransactionPaymentDetails.Add(tranPaymentDetail);
                            entity.SaveChanges();
                        }

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

                        ExchangeRateForTransaction ex = new ExchangeRateForTransaction();
                        ex.TransactionId = TId;
                        ex.CurrencyId = cu.Id;
                        ex.ExchangeRate = Convert.ToInt32(cu.LatestExchangeRate);
                        entity.ExchangeRateForTransactions.Add(ex);
                        entity.SaveChanges();

                        #region [ Print ]


                        dsReportTemp dsReport = new dsReportTemp();
                        dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
                        dsReportTemp.MultiPaymentDataTable multiReport = (dsReportTemp.MultiPaymentDataTable)dsReport.Tables["MultiPayment"];
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


                        foreach (var item in multiPaymentList)
                        {
                            dsReportTemp.MultiPaymentRow newRow = multiReport.NewMultiPaymentRow();
                            newRow.PaymentMethod = item.paymentName;
                            newRow.Amount = Convert.ToString(item.amount);
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
                        Int64 totalAmountRep = insertedTransaction.TotalAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TotalAmount);
                        ReportParameter TotalAmount = new ReportParameter("TotalAmount", totalAmountRep.ToString());
                        rv.LocalReport.SetParameters(TotalAmount);

                        Int64 taxAmountRep = insertedTransaction.TaxAmount == null ? 0 : Convert.ToInt64(insertedTransaction.TaxAmount);
                        ReportParameter TaxAmount = new ReportParameter("TaxAmount", taxAmountRep.ToString());
                        rv.LocalReport.SetParameters(TaxAmount);

                        Int64 disAmountRep = insertedTransaction.DiscountAmount == null ? 0 : Convert.ToInt64(insertedTransaction.DiscountAmount);
                        ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", disAmountRep.ToString());
                        rv.LocalReport.SetParameters(DiscountAmount);

                        ReportParameter PaidAmount = new ReportParameter("PaidAmount", (paidAmount - giftcardAmt).ToString());
                        rv.LocalReport.SetParameters(PaidAmount);

                        var CurrencySymbol = "Ks";
                        ReportParameter CurrencyCode = new ReportParameter("CurrencyCode", CurrencySymbol);
                        rv.LocalReport.SetParameters(CurrencyCode);

                        ReportParameter Change = new ReportParameter("Change", lblChanges.Text);
                        rv.LocalReport.SetParameters(Change);
                        for (int i = 0; i <= 1; i++)
                        {
                            PrintDoc.PrintReport(rv, "Slip");
                        }
                        #endregion
                        DialogResult result = MessageBox.Show("Payment Completed", "mPOS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {

                            Clear();
                        }

                    }


                    #endregion


                    #region Single Payment
                    //Cash
                    if (false)
                    {
                        Loc_PaidByCash form = new Loc_PaidByCash();
                        form.DetailList = DetailList;
                        form.GiftList = GivenGiftList;
                        int extraDiscount = 0;
                        Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                        int giftDiscount = 0;
                        Int32.TryParse(txtGiftDiscount.Text, out giftDiscount);
                        int tax = 0;
                        Int32.TryParse(txtExtraTax.Text, out tax);
                        form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                        form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                        form.isDraft = isDraft;
                        form.DraftId = DraftId;
                        //if cashier doesn't select customer, leave it as null.
                        if (cboCustomer.SelectedIndex != 0)
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                        else
                            form.CustomerId = null;
                        form.ExtraTax = tax;
                        form.ExtraDiscount = extraDiscount + giftDiscount;
                        form.isDebt = false;
                        form.ShowDialog();
                    }
                    //Credit
                    else if (false)
                    {
                        Loc_PaidByCredit form = new Loc_PaidByCredit();
                        form.DetailList = DetailList;
                        form.GiftList = GivenGiftList;
                        int extraDiscount = 0;
                        Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                        int giftDiscount = 0;
                        Int32.TryParse(txtGiftDiscount.Text, out giftDiscount);
                        int tax = 0;
                        Int32.TryParse(txtExtraTax.Text, out tax);
                        form.isDraft = isDraft;
                        form.DraftId = DraftId;
                        //if cashier doesn't select customer, leave it as null.
                        if (cboCustomer.SelectedIndex != 0)
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                        else
                            form.CustomerId = null;

                        form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                        form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                        form.ExtraTax = tax;
                        form.ExtraDiscount = extraDiscount + giftDiscount;
                        form.ShowDialog();
                    }
                    //GiftCard
                    else if (false)
                    {
                        Loc_PaidByGiftCard form = new Loc_PaidByGiftCard();
                        form.GiftList = GivenGiftList;
                        form.DetailList = DetailList;
                        int extraDiscount = 0;
                        int discount = 0;
                        Int32.TryParse(txtAdditionalDiscount.Text, out discount);
                        int GiftDiscount = 0;
                        Int32.TryParse(txtGiftDiscount.Text, out GiftDiscount);
                        extraDiscount = discount + GiftDiscount;
                        int tax = 0;
                        Int32.TryParse(txtExtraTax.Text, out tax);
                        form.isDraft = isDraft;
                        form.DraftId = DraftId;
                        //if cashier doesn't select customer, leave it as null.
                        if (cboCustomer.SelectedIndex != 0)
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                        else
                        {
                            MessageBox.Show("Please fill up customer name!");
                            return;
                        }
                        form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                        form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                        form.ExtraTax = tax;
                        form.ExtraDiscount = extraDiscount;
                        form.ShowDialog();
                    }
                    //FOC
                    else if (false)
                    {
                        Loc_FOC form = new Loc_FOC();
                        form.DetailList = DetailList;
                        form.GiftList = GivenGiftList;
                        form.Type = 4;
                        int extraDiscount = 0;
                        Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                        int giftDiscount = 0;
                        Int32.TryParse(txtGiftDiscount.Text, out giftDiscount);
                        int tax = 0;
                        Int32.TryParse(txtExtraTax.Text, out tax);
                        form.isDraft = isDraft;
                        form.DraftId = DraftId;
                        //if cashier doesn't select customer, leave it as null.
                        if (cboCustomer.SelectedIndex != 0)
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                        else
                            form.CustomerId = null;
                        form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                        form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                        form.ExtraTax = tax;
                        form.ExtraDiscount = extraDiscount + giftDiscount;
                        form.ShowDialog();
                    }
                    //Paid by MPU
                    else if (false)
                    {
                        PaidByMPU form = new PaidByMPU();
                        form.DetailList = DetailList;
                        int extraDiscount = 0;
                        Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                        int giftDiscount = 0;
                        Int32.TryParse(txtGiftDiscount.Text, out giftDiscount);
                        int tax = 0;
                        Int32.TryParse(txtExtraTax.Text, out tax);
                        form.isDraft = isDraft;
                        form.DraftId = DraftId;
                        //if cashier doesn't select customer, leave it as null.
                        if (cboCustomer.SelectedIndex != 0)
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                        else
                            form.CustomerId = null;
                        form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                        form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                        form.ExtraTax = tax;
                        form.ExtraDiscount = extraDiscount + giftDiscount;
                        form.ShowDialog();
                    }
                    else if (false)
                    {
                        Loc_FOC form = new Loc_FOC();
                        form.DetailList = DetailList;
                        form.GiftList = GivenGiftList;
                        form.Type = 6;
                        int extraDiscount = 0;
                        Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);
                        int giftDiscount = 0;
                        Int32.TryParse(txtGiftDiscount.Text, out giftDiscount);
                        int tax = 0;
                        Int32.TryParse(txtExtraTax.Text, out tax);
                        form.isDraft = isDraft;
                        form.DraftId = DraftId;
                        //if cashier doesn't select customer, leave it as null.
                        if (cboCustomer.SelectedIndex != 0)
                            form.CustomerId = Convert.ToInt32(cboCustomer.SelectedValue.ToString());
                        else
                            form.CustomerId = null;
                        form.Discount = Convert.ToInt32(lblDiscountTotal.Text);
                        form.Tax = Convert.ToInt32(lblTaxTotal.Text);
                        form.ExtraTax = tax;
                        form.ExtraDiscount = extraDiscount + giftDiscount;
                        form.ShowDialog();
                    }

                    #endregion
                }
                else
                {
                    MessageBox.Show("Please select customer!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("You haven't select any item to paid");
            }
        }

        private void btnLoadDraft_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("This action will erase current sale data. Would you like to continue?", "Load", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                DraftList form = new DraftList();
                form.Show();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //will only work if the grid have data row
            //datagrid count header as a row, so we have to check there is more than one row
            if (dgvSalesItem.Rows.Count > 1)
            {
                List<TransactionDetail> DetailList = GetTranscationListFromDataGridView();

                int extraDiscount = 0;
                Int32.TryParse(txtAdditionalDiscount.Text, out extraDiscount);

                int tax = 0;
                Int32.TryParse(txtExtraTax.Text, out tax);
                int cusId = Convert.ToInt32(cboCustomer.SelectedValue);
                if (cusId <= 0)
                {
                    cusId = 9128;//Customer Default Id is different between Pearl and Main Office.Now using Id is gernal default Id that clude in this two shop.
                }
                System.Data.Objects.ObjectResult<String> Id;
                Id = entity.InsertDraft(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 1, tax, extraDiscount, DetailList.Sum(x => x.TotalAmount) + tax - extraDiscount, 0, null, cusId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
                string resultId = Id.FirstOrDefault().ToString();
                Transaction insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                foreach (TransactionDetail detail in DetailList)
                {
                    insertedTransaction.TransactionDetails.Add(detail);
                }
                entity.SaveChanges();  //// 111111
                Clear();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string productName = cboProductName.Text.Trim();
            List<Product> productList = (from p in entity.Products
                                         join pd in entity.ProductCategories
                      on p.ProductCategoryId equals pd.Id
                                         where !pd.Name.Contains("GWP")//not contain the GWP product when sale
                                         && p.Name.Contains(productName)
                                         select p).Distinct().ToList();
            if (productList.Count > 0)
            {
                dgvSearchProductList.DataSource = productList;
                dgvSearchProductList.Focus();
            }
            else
            {
                MessageBox.Show("Item not found!", "Cannot find");
                dgvSearchProductList.DataSource = null;
                return;
            }
        }

        private void dgvSearchProductList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int currentProductId = Convert.ToInt32(dgvSearchProductList.Rows[e.RowIndex].Cells[0].Value);
                int count = dgvSalesItem.Rows.Count;
                if (e.ColumnIndex == 1)
                {
                    entity = new POSEntities();
                    Product pro = (from p in entity.Products where p.Id == currentProductId select p).FirstOrDefault<Product>();
                    if (pro != null)
                    {

                        DataGridViewRow row = (DataGridViewRow)dgvSalesItem.Rows[count - 1].Clone();
                        row.Cells[0].Value = pro.Barcode;
                        row.Cells[1].Value = pro.ProductCode;
                        row.Cells[2].Value = 1;
                        row.Cells[3].Value = pro.Name;
                        row.Cells[4].Value = pro.Price;
                        if (IsBirthday)
                        {
                            disRate = birthdayDiscount;

                        }
                        else
                        {
                            disRate = pro.DiscountRate;
                        }
                        row.Cells[5].Value = disRate;
                        row.Cells[6].Value = pro.Tax.TaxPercent;
                        row.Cells[7].Value = getActualCost(pro, disRate);
                        row.Cells[9].Value = currentProductId;
                        dgvSalesItem.Rows.Add(row);

                        _rowIndex = dgvSalesItem.Rows.Count - 2;
                        cboProductName.SelectedIndex = 0;
                        dgvSearchProductList.DataSource = "";
                        dgvSearchProductList.ClearSelection();
                        dgvSalesItem.Focus();
                        Check_ProductCode_Exist(pro.ProductCode);

                        Cell_ReadOnly();
                    }
                    else
                    {

                        MessageBox.Show("Item not found!", "Cannot find");
                    }

                    UpdateTotalCost();
                }
            }
        }

        private void dgvSearchProductList_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyData == Keys.Enter && dgvSearchProductList.CurrentCell != null)
            {
                int Row = dgvSearchProductList.CurrentCell.RowIndex;
                int Column = dgvSearchProductList.CurrentCell.ColumnIndex;
                int currentProductId = Convert.ToInt32(dgvSearchProductList.Rows[Row].Cells[0].Value);
                int count = dgvSalesItem.Rows.Count;
                if (Column == 1)
                {
                    Product pro = (from p in entity.Products where p.Id == currentProductId select p).FirstOrDefault<Product>();
                    if (pro != null)
                    {
                        
                        //fill the new row with the product information
                        //dgvSalesItem.Rows.Add();
                        //int newRowIndex = dgvSalesItem.NewRowIndex;

                        DataGridViewRow row = (DataGridViewRow)dgvSalesItem.Rows[count - 1].Clone();
                        row.Cells[0].Value = pro.Barcode;
                        row.Cells[1].Value = pro.ProductCode;
                        row.Cells[2].Value = 1;
                        row.Cells[3].Value = pro.Name;
                        row.Cells[4].Value = pro.Price;
                        if (IsBirthday)
                        {
                            disRate = birthdayDiscount;
                        }
                        else
                        {
                            disRate = pro.DiscountRate;
                        }
                        row.Cells[5].Value = disRate;
                        row.Cells[6].Value = pro.Tax.TaxPercent;
                        row.Cells[7].Value = getActualCost(pro, disRate);
                        row.Cells[9].Value = currentProductId;
                        dgvSalesItem.Rows.Add(row);
                        cboProductName.SelectedIndex = 0;
                        dgvSearchProductList.DataSource = "";
                        dgvSearchProductList.ClearSelection();
                        dgvSalesItem.Focus();
                        //dgvSalesItem.CurrentCell = dgvSalesItem.Rows[count].Cells[0];
                        Check_ProductCode_Exist(pro.ProductCode);

                        Cell_ReadOnly();
                    }
                    else
                    {

                        MessageBox.Show("Item not found!", "Cannot find");
                    }

                    UpdateTotalCost();
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F2))
            {
                cboProductName.Focus();
                return true;
            }
            else if (keyData == (Keys.F1))
            {
                btnPaid_Click(this.btnPaid, e);
                return true;
            }
            else if (keyData == Keys.End)
            {
                txtAdditionalDiscount.Focus();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void cboProductName_KeyDown(object sender, KeyEventArgs e)
        {
            this.AcceptButton = btnSearch;
        }

        private void txtAdditionalDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtExtraTax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void lbAdvanceSearch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CustomerSearch form = new CustomerSearch();
            form.ShowDialog();
        }

        private void cboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCustomer.SelectedIndex != 0)
            {
                SetCurrentCustomer(Convert.ToInt32(cboCustomer.SelectedValue.ToString()));
                dgvPaymentType.Rows.Clear();
            }
            else
            {
                //Clear customer data
                CurrentCustomerId = 0;
                lblCustomerName.Text = "-";
                lblEmail.Text = "-";
                lblNRIC.Text = "-";
                lblPhoneNumber.Text = "-";
            }
        }

        private void btnAddNewCustomer_Click(object sender, EventArgs e)
        {
            //Role Management
            RoleManagementController controller = new RoleManagementController();
            controller.Load(MemberShip.UserRoleId);
            if (controller.Customer.Add || MemberShip.isAdmin)
            {

                NewCustomer form = new NewCustomer();
                form.isEdit = false;
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You are not allowed to add new customer", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void dgvSalesItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                int col = dgvSalesItem.CurrentCell.ColumnIndex;
                int row = dgvSalesItem.CurrentCell.RowIndex;

                if (col == 8)
                {
                    object deleteProductCode = dgvSalesItem[1, row].Value;

                    //If product code is null, this is just new role without product. Do not need to delete the row.
                    if (deleteProductCode != null)
                    {

                        DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result.Equals(DialogResult.OK))
                        {
                            dgvSalesItem.Rows.RemoveAt(row);
                            UpdateTotalCost();
                            dgvSalesItem.CurrentCell = dgvSalesItem[0, row];

                        }
                    }
                }
                e.Handled = true;
            }
        }
        #endregion

        #region Function

        private List<TransactionDetail> GetTranscationListFromDataGridView()
        {
            List<TransactionDetail> DetailList = new List<TransactionDetail>();
            PointDeductionPercentage_History pdp = entity.PointDeductionPercentage_History.Where(p => p.Active == true).FirstOrDefault();
            foreach (DataGridViewRow row in dgvSalesItem.Rows)
            {
                if (!row.IsNewRow && row.Cells[9].Value != null && row.Cells[0].Value != null && row.Cells[1].Value != null && row.Cells[2].Value != null)
                {
                    TransactionDetail transDetail = new TransactionDetail();

                    int qty = 0, productId = 0;
                    bool alreadyinclude = false;
                    decimal discountRate = 0;
                    Int32.TryParse(row.Cells[9].Value.ToString(), out productId);
                    Int32.TryParse(row.Cells[2].Value.ToString(), out qty);
                    Decimal.TryParse(row.Cells[5].Value.ToString(), out discountRate);

                    //Check if the product is already include in above row
                    foreach (TransactionDetail td in DetailList)
                    {
                        if (td.ProductId == productId && td.DiscountRate == discountRate)
                        {
                            Product tempProd = (from p in entity.Products where p.Id == productId select p).FirstOrDefault<Product>();
                            td.Qty = td.Qty + qty;


                            //td.TotalAmount = Convert.ToInt64(tempProd) * Convert.ToInt64( td.Qty);
                            // by SYM
                            td.TotalAmount = Convert.ToInt64(Convert.ToDecimal(row.Cells[7].Value));
                            alreadyinclude = true;
                        }
                    }

                    if (!alreadyinclude)
                    {
                        //Check productId is valid or not.
                        Product pro = (from p in entity.Products where p.Id == productId select p).FirstOrDefault<Product>();
                        if (pro != null)
                        {
                            transDetail.ProductId = pro.Id;
                            transDetail.UnitPrice = pro.Price;
                            transDetail.DiscountRate = discountRate;
                            transDetail.TaxRate = Convert.ToDecimal(pro.Tax.TaxPercent);
                            transDetail.Qty = qty;

                            // this expression statement is used to control double promotion rules by Discount percentage
                            // Customer must be VIP member , Double promotion rule must be activated , the discount rate of product must not be Zero when selling 
                            //by LHST
                            transDetail.IsDeductedBy = txtVIPID.Text.Trim() != "" && pdp != null && transDetail.DiscountRate != 0 ? pdp.DiscountRate : (decimal?)null;

                            //transDetail.TotalAmount = Convert.ToInt64(getActualCost(pro, discountRate)) * qty;
                            // by SYM
                            transDetail.TotalAmount = Convert.ToInt64(Convert.ToDecimal(row.Cells[7].Value));
                            transDetail.IsDeleted = false;
                            DetailList.Add(transDetail);
                        }
                    }
                }
            }

            return DetailList;
        }

        private void UpdateTotalCost()

        {
            int discount = 0, tax = 0, total = 0, totalqty = 0;

            int count = Convert.ToInt32(dgvSalesItem.Rows.Count - 1);

            foreach (DataGridViewRow dgrow in dgvSalesItem.Rows)

            {
                //check if the current one is new empty row
                if (!dgrow.IsNewRow && dgrow.Cells[1].Value != null)
                {
                    string rowProductCode = string.Empty;

                    int qty = 0;
                    rowProductCode = dgrow.Cells[1].Value.ToString().Trim();
                    if (rowProductCode != string.Empty && dgrow.Cells[2].Value != null && dgrow.Cells[10].Value == null)
                    {
                        //Get qty
                        Int32.TryParse(dgrow.Cells[2].Value.ToString(), out qty);
                        Product pro = (from p in entity.Products where p.ProductCode == rowProductCode select p).FirstOrDefault<Product>();


                        decimal productDiscount = 0;
                        if (dgrow.Cells[5].Value != null)
                        {
                            Decimal.TryParse(dgrow.Cells[5].Value.ToString(), out productDiscount);
                        }
                        else
                        {
                            if (IsBirthday)
                            {
                                productDiscount = birthdayDiscount;
                            }
                            else
                            {
                                productDiscount = pro.DiscountRate;
                            }
                        }

                        total += (int)Math.Ceiling(Convert.ToDecimal(dgrow.Cells[7].Value));
                        discount += (int)Math.Ceiling(getDiscountAmount(pro.Price, productDiscount) * qty);
                        tax += (int)Math.Ceiling(getTaxAmount(pro) * qty);
                        totalqty += qty;
                    }
                }
            }

            // by SYM//TotalAmount
            lblTotal.Text = total.ToString();
            lblDiscountTotal.Text = discount.ToString();
            lblTaxTotal.Text = tax.ToString();
            lblQty.Text = totalqty.ToString();

            #region GiftSystem

            bool HasGift = false; bool IsProduct = false, IsBrand = false, IsCategory = false, IsSubCategory = false, IsQtyValid = true, IsCost = true, IsSize = true, IsFilterQty = true;
            GiftList.Clear();

            DateTime CurrentDate = DateTime.Now.Date;
            List<GiftSystem> GiftSysList = entity.GiftSystems.Where(x => x.ValidTo >= CurrentDate && x.ValidFrom <= CurrentDate && x.IsActive == true).ToList();

            foreach (GiftSystem giftObj in GiftSysList)
            {
                IsQtyValid = false; IsCost = false;
                HasGift = false; IsProduct = false; IsBrand = false; IsCategory = false; IsSubCategory = false; IsSize = false; IsFilterQty = false;
                if (giftObj != null)
                {
                    if (giftObj.UsePromotionQty == true)
                    {
                        List<AttachGiftSystemForTransaction> attachList = entity.AttachGiftSystemForTransactions.Where(x => x.AttachGiftSystemId == giftObj.Id).ToList();
                        if (attachList.Count >= giftObj.PromotionQty)
                        {
                            IsQtyValid = false;
                        }
                        else
                        {
                            IsQtyValid = true;
                        }
                    }
                    else
                    {
                        IsQtyValid = true;
                    }

                    if (giftObj.MustBuyCostFrom > 0 && giftObj.MustBuyCostTo > 0)
                    {
                        if (total < giftObj.MustBuyCostFrom || total > giftObj.MustBuyCostTo)
                        {
                            IsCost = false;
                        }
                        else
                        {
                            IsCost = true;
                        }

                    }
                    else if (giftObj.MustBuyCostFrom > 0 && giftObj.MustBuyCostTo == 0)
                    {
                        if (total < giftObj.MustBuyCostFrom)
                        {
                            IsCost = false;
                        }
                        else
                        {
                            IsCost = true;
                        }
                    }
                    else if (giftObj.MustBuyCostFrom == 0 && giftObj.MustBuyCostTo == 0)
                    {
                        IsCost = true;
                    }

                    if (IsQtyValid == true && IsCost == true)
                    {
                        proCount = 0;

                        #region 
                        foreach (DataGridViewRow dgrow in dgvSalesItem.Rows)
                        {
                            IsProduct = false; IsBrand = false; IsCategory = false; IsSubCategory = false; IsSize = false; IsFilterQty = false;
                            int currentProductId = Convert.ToInt32(dgrow.Cells[9].Value);
                            int Qty = Convert.ToInt32(dgrow.Cells[2].Value);
                            Product pro = entity.Products.Where(x => x.Id == currentProductId).FirstOrDefault();
                            if (pro != null)
                            {
                                if (giftObj.UseProductFilter == true)
                                {
                                    if (pro.Id == giftObj.MustIncludeProductId)
                                    {
                                        IsProduct = true;
                                    }
                                }
                                if (giftObj.UseBrandFilter == true)
                                {
                                    if (pro.BrandId == giftObj.FilterBrandId)
                                    {
                                        IsBrand = true;
                                    }
                                }
                                if (giftObj.UseCategoryFilter == true)
                                {
                                    if (pro.ProductCategoryId == giftObj.FilterCategoryId)
                                    {
                                        IsCategory = true;
                                    }
                                }
                                if (giftObj.UseSubCategoryFilter == true)
                                {
                                    if (pro.ProductSubCategoryId == giftObj.FilterSubCategoryId)
                                    {
                                        IsSubCategory = true;
                                    }
                                }
                                if (giftObj.UseSizeFilter == true)
                                {
                                    if (pro.Size != null)
                                    {
                                        if (pro.Size == giftObj.FilterSize.ToString())
                                        {
                                            IsSize = true;
                                        }
                                    }
                                }

                                if (giftObj.UseQtyFilter == true)
                                {
                                    if (giftObj.UseProductFilter == IsProduct && giftObj.UseBrandFilter == IsBrand && giftObj.UseCategoryFilter == IsCategory && giftObj.UseSubCategoryFilter == IsSubCategory && giftObj.UseSizeFilter == IsSize)
                                    {
                                        proCount += Qty;
                                    }

                                    if (proCount >= giftObj.FilterQty)
                                    {
                                        IsFilterQty = true;
                                    }

                                }
                                if (giftObj.UseProductFilter == IsProduct && giftObj.UseBrandFilter == IsBrand && giftObj.UseCategoryFilter == IsCategory && giftObj.UseSubCategoryFilter == IsSubCategory && giftObj.UseSizeFilter == IsSize && giftObj.UseQtyFilter == IsFilterQty)
                                {
                                    HasGift = true;
                                }
                            }
                        }

                        #endregion
                        if (HasGift == true)
                        {
                            GiftList.Add(giftObj);
                            if (giftObj.UseQtyFilter == true)
                            {
                                proCount = 0;
                            }
                        }
                    }
                }
            }

            if (GiftList.Count > 0)
            {
                plGift.Visible = true;
                chkGiftList.Items.Clear();
                lblGift.Visible = true;

                foreach (GiftSystem gObj in GiftList)
                {
                    if (gObj.Product1 != null)
                    {
                        if (gObj.PriceForGiftProduct == 0)
                        {
                            chkGiftList.Items.Add(gObj.Product1.Name + " is given for gift");
                        }
                        else
                        {
                            chkGiftList.Items.Add(gObj.Product1.Name + " is purchased with price " + gObj.PriceForGiftProduct);
                        }
                    }
                    else if (gObj.GiftCashAmount > 0)
                    {
                        chkGiftList.Items.Add("Discount amount " + gObj.GiftCashAmount + " is given for current transaction");
                    }
                    else if (gObj.DiscountPercentForTransaction > 0)
                    {
                        chkGiftList.Items.Add("Discount percent " + gObj.GiftCashAmount + " is given for current transaction");
                    }
                }
                plGift.Controls.Add(chkGiftList);
            }
            else
            {
                lblGift.Visible = false;
                plGift.Visible = false;
            }

            #endregion

            CalculateChargesAmount();
        }

        private decimal getActualCost(Product prod)
        {
            decimal? actualCost = 0;
            //decrease discount ammount            
            actualCost = prod.Price - ((prod.Price / 100) * prod.DiscountRate);



            //add tax ammount            
            actualCost = actualCost + ((prod.Price / 100) * prod.Tax.TaxPercent);
            return (decimal)actualCost;
        }

        private decimal getActualCost(Product prod, decimal discountRate)
        {

            decimal? actualCost = 0;
            //decrease discount ammount            
            actualCost = prod.Price - ((prod.Price / 100) * discountRate);
            //add tax ammount            
            actualCost = actualCost + ((prod.Price / 100) * prod.Tax.TaxPercent);
            return (decimal)actualCost;
        }
        private decimal getActualCost(long productPrice, decimal productDiscount, decimal tax)
        {
            decimal? actualCost = 0;
            //decrease discount ammount            
            actualCost = productPrice - ((productPrice / 100) * productDiscount);
            //add tax ammount            
            actualCost = actualCost + ((productPrice / 100) * tax);
            return (decimal)actualCost;
        }
        private decimal getDiscountAmount(long productPrice, decimal productDiscount)
        {
            return (((decimal)productPrice / 100) * productDiscount);
        }

        private decimal getTaxAmount(Product prod)
        {
            return ((prod.Price / 100) * Convert.ToDecimal(prod.Tax.TaxPercent));
        }

        private decimal getTaxAmount(long productPrice, decimal tax)
        {
            return ((productPrice / 100) * Convert.ToDecimal(tax));
        }
        public void LoadDraft(string TransactionId)
        {
            entity = new POSEntities();
            Clear();
            DraftId = TransactionId;
            isload = false;



            Transaction draft = (from ts in entity.Transactions where ts.Id == TransactionId && ts.IsComplete == false select ts).FirstOrDefault<Transaction>();

            if (draft != null)
            {
                //pre add the rows
                //dgvSalesItem.Rows.Insert(0, draft.TransactionDetails.Count());

                var _tranDetails = (from a in entity.TransactionDetails where a.TransactionId == TransactionId select a).ToList();
                dgvSalesItem.Rows.Insert(0, _tranDetails.Count());

                int index = 0;
                //foreach (TransactionDetail detail in draft.TransactionDetails)
                foreach (TransactionDetail detail in _tranDetails)
                {
                    //If product still exist
                    if (detail.Product != null)
                    {
                        isload = true;

                        DataGridViewRow row = dgvSalesItem.Rows[index];
                        //fill the current row with the product information                       
                        row.Cells[0].Value = detail.Product.Barcode;
                        row.Cells[1].Value = detail.Product.ProductCode;
                        row.Cells[2].Value = detail.Qty;
                        row.Cells[3].Value = detail.Product.Name;
                        row.Cells[4].Value = detail.Product.Price;
                        row.Cells[5].Value = detail.DiscountRate;
                        row.Cells[6].Value = detail.Product.Tax.TaxPercent;
                        row.Cells[7].Value = getActualCost(detail.Product, detail.DiscountRate) * detail.Qty;
                        row.Cells[9].Value = detail.ProductId;
                        index++;

                    }
                }

                txtAdditionalDiscount.Text = draft.DiscountAmount.ToString();
                txtExtraTax.Text = draft.TaxAmount.ToString();
                if (draft.Customer != null)
                {
                    SetCurrentCustomer((int)draft.CustomerId);
                }
                UpdateTotalCost();
            }
            else
            {
                //no associate transaction
                MessageBox.Show("The item doesn't exist anymore!");
            }

            isDraft = true;
        }

        public void DeleteCopy(string TransactionId)
        {
            Clear();
            DraftId = TransactionId;
            Transaction draft = (from ts in entity.Transactions where ts.Id == TransactionId select ts).FirstOrDefault<Transaction>();
            decimal disTotal = 0, taxTotal = 0;
            //Delete transaction
            draft.IsDeleted = true;
            draft.UpdatedDate = DateTime.Now;

            if (draft.PaymentTypeId == 3)
            {
                List<GiftCardInTransaction> gList = entity.GiftCardInTransactions.Where(x => x.TransactionId == TransactionId).ToList();
                foreach (GiftCardInTransaction gt in gList)
                {
                    GiftCard g = entity.GiftCards.Where(x => x.Id == gt.GiftCardId).FirstOrDefault();
                    g.IsUsed = false;
                    g.IsUsedDate = DateTime.Now;
                    entity.SaveChanges(); ////22222222

                }
            }

            foreach (TransactionDetail detail in draft.TransactionDetails.Where(x => x.IsDeleted != true))
            {
                detail.IsDeleted = true;
                detail.Product.Qty = detail.Product.Qty + detail.Qty;

                if (detail.Product.IsWrapper == true)
                {
                    List<WrapperItem> wplist = detail.Product.WrapperItems.ToList();

                    foreach (WrapperItem wp in wplist)
                    {
                        wp.Product1.Qty = wp.Product1.Qty + detail.Qty;
                    }
                }
            }
            DeleteLog dl = new DeleteLog();
            dl.DeletedDate = DateTime.Now;
            dl.CounterId = MemberShip.CounterId;
            dl.UserId = MemberShip.UserId;
            dl.IsParent = true;
            dl.TransactionId = draft.Id;
            entity.DeleteLogs.Add(dl);
            entity.SaveChanges(); ////// 3333333

            Transaction parenttransaction = entity.Transactions.Where(x => x.Id == TransactionId).FirstOrDefault();
            foreach (TransactionDetail td in parenttransaction.TransactionDetails)
            {
                parenttransaction.TotalAmount = parenttransaction.TotalAmount - td.TotalAmount;
            }
            entity.SaveChanges(); /////44444444

            //copy transaction
            if (draft != null)
            {
                //pre add the rows
                dgvSalesItem.Rows.Insert(0, draft.TransactionDetails.Count());

                int index = 0;
                foreach (TransactionDetail detail in draft.TransactionDetails)
                {
                    //If product still exist
                    if (detail.Product != null)
                    {
                        DataGridViewRow row = dgvSalesItem.Rows[index];
                        //fill the current row with the product information
                        row.Cells[0].Value = detail.Product.Barcode;
                        row.Cells[1].Value = detail.Product.ProductCode;
                        row.Cells[2].Value = detail.Qty;
                        row.Cells[3].Value = detail.Product.Name;
                        row.Cells[4].Value = detail.UnitPrice;
                        row.Cells[5].Value = detail.DiscountRate;
                        row.Cells[6].Value = detail.TaxRate;
                        row.Cells[7].Value = getActualCost(Convert.ToInt64(detail.UnitPrice), detail.DiscountRate, detail.TaxRate) * detail.Qty;
                        disTotal += Convert.ToInt64(getDiscountAmount(Convert.ToInt64(detail.UnitPrice), detail.DiscountRate) * detail.Qty);
                        taxTotal += Convert.ToInt64(getTaxAmount(Convert.ToInt64(detail.UnitPrice), detail.TaxRate) * detail.Qty);
                        row.Cells[9].Value = detail.ProductId;
                        index++;
                    }

                }

                txtAdditionalDiscount.Text = (draft.DiscountAmount - disTotal).ToString();
                txtExtraTax.Text = (draft.TaxAmount - taxTotal).ToString();
                if (draft.Customer != null)
                {
                    SetCurrentCustomer((int)draft.CustomerId);
                }
                UpdateTotalCost();


            }


        }
        public void Clear()
        {
            CurrentCustomerId = 0;

            dgvSalesItem.Rows.Clear();
            dgvSalesItem.Focus();
            txtAdditionalDiscount.Text = "0";
            txtExtraTax.Text = "0";
            lblTotal.Text = "0";
            lblTaxTotal.Text = "0";
            lblDiscountTotal.Text = "0";
            isDraft = false;
            DraftId = string.Empty;
            dgvSearchProductList.DataSource = "";
            cboProductName.SelectedIndex = 0;
            List<Product> productList = new List<Product>();
            Product productObj = new Product();
            productObj.Id = 0;
            productObj.Name = "";
            productList.Add(productObj);
            productList.AddRange((from p in entity.Products select p).ToList());
            cboProductName.DataSource = productList;
            cboProductName.DisplayMember = "Name";
            cboProductName.ValueMember = "Id";
            cboProductName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProductName.AutoCompleteSource = AutoCompleteSource.ListItems;
            lblGift.Enabled = false;
            chkGiftList.Items.Clear();
            cboCustomer.SelectedIndex = 0;
            txtVIPID.Text = "";
            ReloadCustomerList();
            txtGiftDiscount.Text = "0";
            lblQty.Text = "0";
            cboPaymentMethod.SelectedIndex = 0;
            cboPaymentType.SelectedIndex = 0;
            txtAmount.Clear();
            dgvPaymentType.Rows.Clear();
            cboPaymentMethod.Enabled = true;
            cboPaymentType.Enabled = true;
            txtAmount.Enabled = true;
            btnPaymentAdd.Enabled = true;
            lblChanges.Text = "0";
            lblbday.Text = "-";
            lblBDMessage.ResetText();
            lblBDMessage.Text = "";
            lblbday.BackColor = System.Drawing.Color.Transparent;
            IsBirthday = false;
            disRate = 0;
        }

        public void SetCurrentCustomer(Int32 CustomerId)
        {
            CurrentCustomerId = CustomerId;
            Customer currentCustomer = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
            if (currentCustomer != null)
            {
                lblCustomerName.Text = currentCustomer.Title + " " + currentCustomer.Name;
                lblNRIC.Text = currentCustomer.NRC;
                lblPhoneNumber.Text = currentCustomer.PhoneNumber;
                lblEmail.Text = currentCustomer.Email;
                cboCustomer.Text = currentCustomer.Name;
                cboCustomer.SelectedItem = currentCustomer;
                cboCustomer.SelectedValue = currentCustomer.Id;
                txtVIPID.Text = currentCustomer.VIPMemberId;
                lblbday.Text = currentCustomer.Birthday == null ? "-" : ((DateTime)currentCustomer.Birthday).ToString("dd-MMM-yyyy");

                // update 7 dec-2022 khs
                if (currentCustomer.Birthday == null)
                {
                    lblbday.Text = "-";
                    lblBDMessage.ResetText();
                    IsBirthday = false;
                    disRate = 0;
                    lblbday.BackColor = System.Drawing.Color.Transparent;
                    if (dgvSalesItem.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgvSalesItem.Rows.Count - 1; i++)
                        {
                            if (dgvSalesItem.Rows[i].Cells[(int)sCol.pId].Value != null && !string.IsNullOrEmpty(dgvSalesItem.Rows[i].Cells[(int)sCol.pId].Value.ToString()))
                            {
                                string GridProductCOde = (string)dgvSalesItem.Rows[i].Cells[1].Value;
                                Product itemp = iTempP.Where(p => p.ProductCode == GridProductCOde).FirstOrDefault();
                                if (itemp != null && itemp.DiscountRate == 0)
                                {

                                    dgvSalesItem.Rows[i].Cells[(int)sCol.DisPercent].Value = "0.0";
                                    int qty = Convert.ToInt32(dgvSalesItem.Rows[i].Cells[(int)sCol.Qty].Value);
                                    dgvSalesItem.Rows[i].Cells[(int)sCol.Cost].Value = getActualCost(itemp, itemp.DiscountRate) * qty;
                                }
                            }
                            else
                            {
                                dgvSalesItem.Rows.RemoveAt(i);
                            }
                        }
                        UpdateTotalCost();
                    }
                }
                else
                {
                    var bod = Convert.ToDateTime(currentCustomer.Birthday).ToString("dd-MMM-yyyy");
                    lblbday.Text = bod.ToString();

                    int count = dgvSalesItem.Rows.Count;
                    if (Convert.ToDateTime(lblbday.Text).Month == System.DateTime.Now.Month && !string.IsNullOrEmpty(currentCustomer.VIPMemberId))
                    {
                        Application.DoEvents();
                        //int cusId = Convert.ToInt32(cboCustomer.SelectedValue);
                        //var bdList = (from t in entity.Transactions where t.CustomerId == cusId && t.BDDiscountAmt != 0 select t).ToList();
                        //var bdPeryear = entity.Transactions.Where(t => t.CustomerId == CustomerId && t.DateTime.Year == DateTime.Now.Year && false == t.IsDeleted && t.Loc_IsCalculatePoint == true && t.DiscountAmount > 0).Join(;
                        var bdPeryear = from t in entity.Transactions
                                        join td in entity.TransactionDetails on t.Id equals td.TransactionId
                                        where t.CustomerId == CustomerId && t.DateTime.Year == DateTime.Now.Year && false == t.IsDeleted && t.Loc_IsCalculatePoint == true && t.DiscountAmount > 0
                                        && td.BdDiscounted == true
                                        select t.Id;

                        if ((bdPeryear == null || bdPeryear.Count() == 0) && birthdayDiscount > 0)
                        {
                            //var a=bdPeryear.Where(x=>x.DateTime.Month==DateTime.Now.Month).FirstOrDefault().TransactionDetails
                            lblbday.BackColor = System.Drawing.Color.Yellow;
                            IsBirthday = true;
                            disRate = birthdayDiscount;
                            if (dgvSalesItem.Rows.Count > 0)
                            {

                                for (int i = 0; i < count - 1; i++)
                                {
                                    if (Convert.ToDecimal(dgvSalesItem.Rows[i].Cells[6].Value) == 0)
                                    {
                                        //bool isFoc = false;
                                        //if (dgvSalesItem.Rows[i].Cells[(int)sCol.FOC].Value != null && dgvSalesItem.Rows[i].Cells[(int)sCol.colFOC].Value.ToString() == "FOC")
                                        //{
                                        //    isFoc = true;
                                        //}
                                        dgvSalesItem.Rows[i].Cells[6].Value = birthdayDiscount;
                                        string GridProductCOde = (string)dgvSalesItem.Rows[i].Cells[1].Value;
                                        Product itemp = iTempP.Where(p => p.ProductCode == GridProductCOde).FirstOrDefault();
                                        if (itemp != null && itemp.DiscountRate == 0 )
                                        {
                                            int qty = Convert.ToInt32(dgvSalesItem.Rows[i].Cells[(int)sCol.Qty].Value);
                                            dgvSalesItem.Rows[i].Cells[(int)sCol.Cost].Value = getActualCost(itemp, birthdayDiscount) * qty;
                                        }
                                    }
                                }
                                lblBDMessage.Visible = true;
                                lblBDMessage.Text = "Birthday Discount ( " + birthdayDiscount + "% ) will be discounted for a Transaction.";
                                UpdateTotalCost();
                            }
                        }
                        else
                        {
                            lblBDMessage.ResetText();
                            lblBDMessage.Text = "";
                            lblbday.BackColor = System.Drawing.Color.Transparent;
                            IsBirthday = false;
                            disRate = 0;
                        }
                    }
                    else
                    {
                        lblBDMessage.ResetText();
                        lblBDMessage.Text = "";
                        IsBirthday = false;
                        disRate = 0;
                        lblbday.BackColor = System.Drawing.Color.Transparent;
                        if (dgvSalesItem.Rows.Count > 0)
                        {
                            for (int i = 0; i < count - 1; i++)
                            {
                                string gridProductCode = (string)dgvSalesItem.Rows[i].Cells[1].Value.ToString();
                                if (iTempP.Where(p => p.ProductCode == gridProductCode).FirstOrDefault().DiscountRate == 0)
                                {
                                    dgvSalesItem.Rows[i].Cells[6].Value = "0.0";
                                }
                            }
                            UpdateTotalCost();


                        }
                    }
                }
                //end here khs
            }
        }

        public void ReloadCustomerList()
        {

            //Add Customer List with default option
            entity = new POSEntities();
            List<APP_Data.Customer> customerList = new List<APP_Data.Customer>();
            APP_Data.Customer customer = new APP_Data.Customer();
            customer.Id = 0;
            customer.Name = "None";
            customerList.Add(customer);
            customerList.AddRange(entity.Customers.OrderBy(x => x.Name).ToList());
            cboCustomer.DataSource = customerList;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";
        }


        private void Cell_ReadOnly()
        {
            if (_rowIndex != -1)
            {
                DataGridViewRow row = dgvSalesItem.Rows[_rowIndex];
                if (_rowIndex > 0)
                {
                    if (row.Cells[1].Value != null)
                    {
                        string currentProductCode = row.Cells[1].Value.ToString();
                        List<string> _productList = dgvSalesItem.Rows
                               .OfType<DataGridViewRow>()
                               .Where(r => r.Cells[1].Value != null)
                               .Select(r => r.Cells[1].Value.ToString())
                               .ToList();

                        List<string> _checkProList = new List<string>();

                        _checkProList = (from p in _productList where p.Contains(currentProductCode) select p).ToList();
                        _checkProList.RemoveAt(_checkProList.Count - 1);
                        if (_checkProList.Count == 0)
                        {
                            dgvSalesItem.Rows[_rowIndex].Cells[0].ReadOnly = true;
                            dgvSalesItem.Rows[_rowIndex].Cells[1].ReadOnly = true;
                        }
                    }
                }
                else
                {
                    dgvSalesItem.Rows[_rowIndex].Cells[0].ReadOnly = true;
                    dgvSalesItem.Rows[_rowIndex].Cells[1].ReadOnly = true;
                }

            }
            else
            {
                dgvSalesItem.Rows[0].Cells[0].ReadOnly = true;
                dgvSalesItem.Rows[0].Cells[1].ReadOnly = true;
            }

            dgvSalesItem.CurrentCell = dgvSalesItem[0, dgvSalesItem.Rows.Count - 1];

        }



        private bool Check_ProductCode_Exist(string currentProductCode)
        {
            bool check = false;
            List<int> _indexCount = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                                     where r.Cells[1].Value != null && r.Cells[1].Value.ToString() == currentProductCode
                                     select r.Index).ToList();
            if (_indexCount.Count > 1)
            {
                _indexCount.RemoveAt(_indexCount.Count - 1);
                int index = (from r in dgvSalesItem.Rows.Cast<DataGridViewRow>()
                             where r.Cells[1].Value != null && r.Cells[1].Value.ToString() == currentProductCode
                             select r.Index).FirstOrDefault();
                dgvSalesItem.Rows[index].Cells[2].Value = Convert.ToInt32(dgvSalesItem.Rows[index].Cells[2].Value) + 1;
                BeginInvoke(new Action(delegate { dgvSalesItem.Rows.RemoveAt(dgvSalesItem.Rows.Count - 2); }));
                dgvSalesItem.Rows[dgvSalesItem.Rows.Count - 2].Cells[10].Value = "Delete";
                check = true;
            }
            return check;
        }

        #endregion                                                       

        private void chkGiftList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int i = chkGiftList.SelectedIndex;
            if (i >= 0 && i < GiftList.Count)
            {
                if (chkGiftList.GetItemChecked(i) == false)
                {
                    long total = Convert.ToInt64(lblTotal.Text);
                    int DisAmount = Convert.ToInt32(txtGiftDiscount.Text);
                    if (GiftList[i].Product1 != null)
                    {
                        total += GiftList[i].PriceForGiftProduct;
                    }
                    else if (GiftList[i].GiftCashAmount > 0)
                    {
                        total -= (long)GiftList[i].GiftCashAmount;
                        DisAmount += (int)GiftList[i].GiftCashAmount;
                    }
                    else if (GiftList[i].DiscountPercentForTransaction > 0)
                    {
                        total -= (long)(total * (GiftList[i].GiftCashAmount / 100));
                        DisAmount += (int)(total * (GiftList[i].GiftCashAmount / 100));
                    }
                    // by SYM//TotalAmount
                    lblTotal.Text = total.ToString();
                    txtGiftDiscount.Text = DisAmount.ToString();
                }
                else
                {
                    long total = Convert.ToInt64(lblTotal.Text);
                    int DisAmount = Convert.ToInt32(txtGiftDiscount.Text);
                    if (GiftList[i].Product1 != null)
                    {
                        total -= GiftList[i].PriceForGiftProduct;
                    }
                    else if (GiftList[i].GiftCashAmount > 0)
                    {
                        total += (long)GiftList[i].GiftCashAmount;
                        DisAmount -= (int)GiftList[i].GiftCashAmount;
                    }
                    else if (GiftList[i].DiscountPercentForTransaction > 0)
                    {
                        total += (long)(total * (GiftList[i].GiftCashAmount / 100));
                        DisAmount -= (int)(total * (GiftList[i].GiftCashAmount / 100));
                    }
                    // by SYM//TotalAmount
                    lblTotal.Text = total.ToString();
                    txtGiftDiscount.Text = DisAmount.ToString();
                }
            }
        }

        private void txtVIPID_KeyDown(object sender, KeyEventArgs e)
        {
            this.AcceptButton = null;

            if (e.KeyData == Keys.Enter)
            {
                string VIPID = txtVIPID.Text;
                Customer cus = entity.Customers.Where(x => x.VIPMemberId == VIPID && x.CustomerTypeId == 1).FirstOrDefault();
                if (cus != null)
                {
                    SetCurrentCustomer(cus.Id);
                }
                else
                {
                    MessageBox.Show("VIP Member ID not found!", "Cannot find");
                    //Clear customer data
                    CurrentCustomerId = 0;
                    lblCustomerName.Text = "-";
                    lblEmail.Text = "-";
                    lblNRIC.Text = "-";
                    lblPhoneNumber.Text = "-";
                    cboCustomer.SelectedIndex = 0;
                }
            }
        }

        private void dgvSalesItem_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSalesItem.Rows[e.RowIndex];
                dgvSalesItem.CommitEdit(new DataGridViewDataErrorContexts());
                if (row.Cells[0].Value == null && row.Cells[1].Value == null && row.Cells[2].Value == null && row.Cells[5].Value == null)
                {
                    if (row.Cells[8].Value != null)
                    {
                        BeginInvoke(new Action(delegate { dgvSalesItem.Rows.RemoveAt(e.RowIndex); }));
                    }
                }
                else if (mssg == "Wrong")
                {
                    if (row.Cells[8].Value != null)
                    {
                        BeginInvoke(new Action(delegate { dgvSalesItem.Rows.RemoveAt(e.RowIndex); }));
                        if (row.Cells[0].Value != null)
                        {
                            dgvSalesItem.CurrentCell = dgvSalesItem[0, e.RowIndex];
                        }
                        else if (row.Cells[1].Value != null)
                        {
                            dgvSalesItem.CurrentCell = dgvSalesItem[1, e.RowIndex];
                        }
                        mssg = "";
                    }
                }
            }
        }

        private void cboCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            cboCustomer.DroppedDown = true;
        }

        private void cboCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            cboCustomer.DroppedDown = true;
        }

        private void cboPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedId = Convert.ToInt32(cboPaymentMethod.SelectedValue);
            cboPaymentType.DataSource = null;
            var paymentType = entity.PaymentMethods.Where(x => x.PaymentParentId == selectedId).ToList();
            if (paymentType.Count == 0)
            {
                cboPaymentType.DataSource = entity.PaymentMethods.Where(x => x.Id == selectedId).ToList();
            }
            else
            {
                cboPaymentType.DataSource = paymentType;
            }

            cboPaymentType.DisplayMember = "Name";
            cboPaymentType.ValueMember = "Id";
            cboPaymentType.SelectedIndex = 0;
            txtAmount.Enabled = cboPaymentType.Text.Trim() == "FOC" ? false : true;
        }
        private void CalculateChargesAmount()
        {
            int paidAmount = 0; bool isFoc = false;
            int discount = 0;
            if (!String.IsNullOrWhiteSpace(txtAdditionalDiscount.Text))
            {
                discount = Convert.ToInt32(txtAdditionalDiscount.Text);
            }

            foreach (DataGridViewRow row in dgvPaymentType.Rows)
            {
                if (row.Cells[5].Value == null)
                {
                    paidAmount = 0;
                    isFoc = true;
                }
                else
                {
                    paidAmount += Convert.ToInt32(row.Cells[5].Value);

                }
            }
            int totalAmount = Convert.ToInt32(lblTotal.Text);
            int changesAmount = totalAmount - (paidAmount + discount);
            lblChanges.Text = changesAmount >= 0 ? changesAmount.ToString() : (changesAmount * -1).ToString();
            labelChanges.Text = changesAmount >= 0 ? "Payable Amount" : "Changes";
            if (isFoc)
            {
                lblChanges.Text = "0";
            }
        }

        private void btnPaymentAdd_Click(object sender, EventArgs e)
        {
            if (cboPaymentType.SelectedIndex != -1 && cboPaymentType.Text.Trim() != "FOC")
            {
                if (!string.IsNullOrWhiteSpace(txtAmount.Text))
                {
                    if (cboPaymentType.Text.Trim() == "Gift Card")
                    {
                        Boolean hasError = false;
                        string CardNumber = txtAmount.Text.Trim();
                        GiftCard currentCard = (from gcard in entity.GiftCards where gcard.CardNumber == CardNumber && gcard.IsUsed == false && gcard.IsDeleted == false select gcard).FirstOrDefault<GiftCard>();

                        //GiftCard is invalid
                        if (currentCard == null)
                        {
                            MessageBox.Show("Card is already used or invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            hasError = true;
                        }
                        else if (currentCard.CustomerId != Convert.ToInt32(cboCustomer.SelectedValue))
                        {
                            MessageBox.Show("This card is not belong to current customer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            hasError = true;
                        }
                        else
                        {
                            //if GiftCard Already in the list
                            foreach (DataGridViewRow row in dgvPaymentType.Rows)
                            {
                                if (Convert.ToString(row.Cells[1].Value).Trim() == "Gift Card")
                                {
                                    if (Convert.ToString(row.Cells[3].Value).Trim() == currentCard.CardNumber.Trim())
                                    {
                                        MessageBox.Show("Card already in the list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        hasError = true;
                                    }
                                }

                            }
                        }

                        if (!hasError)
                        {
                            dgvPaymentType.Rows.Add(Convert.ToInt32(cboPaymentType.SelectedValue), cboPaymentMethod.Text.Trim(), cboPaymentType.Text.Trim(), txtAmount.Text, "Delete", currentCard.Amount);
                            cboPaymentMethod.SelectedIndex = 0;
                            cboPaymentType.SelectedIndex = 0;
                            txtAmount.Clear();
                            CalculateChargesAmount();
                        }
                    }
                    else
                    {
                        dgvPaymentType.Rows.Add(Convert.ToInt32(cboPaymentType.SelectedValue), cboPaymentMethod.Text.Trim(), cboPaymentType.Text.Trim(), txtAmount.Text, "Delete", txtAmount.Text);
                        cboPaymentMethod.SelectedIndex = 0;
                        cboPaymentType.SelectedIndex = 0;
                        txtAmount.Clear();
                        CalculateChargesAmount();
                    }

                }
            }
            //For FOC
            else
            {
                DialogResult result = MessageBox.Show("Are you sure FOC invoice", "FOC", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result.Equals(DialogResult.OK))
                {

                    dgvPaymentType.Rows.Clear();
                    dgvPaymentType.Rows.Add(Convert.ToInt32(cboPaymentType.SelectedValue), cboPaymentMethod.Text.Trim(), cboPaymentType.Text.Trim(), txtAmount.Text, "Delete", null);
                    cboPaymentMethod.Enabled = false;
                    cboPaymentType.Enabled = false;
                    btnPaymentAdd.Enabled = false;
                    lblChanges.Text = "0";
                }
            }
            int finalrows = dgvPaymentType.Rows.Count;
        }

        private void cboPaymentMethod_TextChanged(object sender, EventArgs e)
        {
            //UpdateTotalCost();
        }

        private void txtAdditionalDiscount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                // Check_MType();//SD
                UpdateTotalCost();
                SendKeys.Send("{TAB}");

            }
        }

        private void txtAdditionalDiscount_Leave(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }

        private void cboPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAmount.Clear();
        }

        private void dgvPaymentType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowindex = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                //Delete
                if (e.ColumnIndex == 4)
                {
                    string paymentTypeName = Convert.ToString(dgvPaymentType[1, e.RowIndex].Value);

                    dgvPaymentType.Rows.RemoveAt(e.RowIndex);
                    CalculateChargesAmount();

                    if (paymentTypeName.Trim() == "FOC")
                    {
                        cboPaymentMethod.Enabled = true;
                        cboPaymentMethod.SelectedIndex = 0;
                        cboPaymentType.Enabled = true;
                        cboPaymentType.SelectedIndex = 0;
                        btnPaymentAdd.Enabled = true;
                    }
                }
            }
        }

        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyData == Keys.Enter)
            {
                btnPaymentAdd_Click(sender, e);
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cboPaymentMethod.Text.Trim() != "Gift Card")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }
        public class MultiPayment
        {
            public int id;
            public string paymentName;
            public int amount;
        }
    }
}
