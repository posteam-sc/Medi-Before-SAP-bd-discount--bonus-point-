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
    public partial class Loc_PaidByGiftCard : Form
    {
        #region Variables

        public List<GiftSystem> GiftList = new List<GiftSystem>();

        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        private List<GiftCard> GiftCardList = new List<GiftCard>();

        private long TotalAmountFromGiftCard = 0;

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int ExtraTax { get; set; }

        public Boolean isDraft { get; set; }

        public string DraftId { get; set; }

        public int CustomerId { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        #endregion

        #region Events

        public Loc_PaidByGiftCard()
        {
            InitializeComponent();
        }

        private void Loc_PaidByGiftCard_Load(object sender, EventArgs e)
        {
             lblTotalCost.Text = (DetailList.Sum(x => x.TotalAmount) - ExtraDiscount + ExtraTax).ToString();
             lblCustomerName.Text = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault().Name;
        }

        private void Loc_PaidByGiftCard_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtGiftCardId);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            
            long TotalCost = (long)DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount;
            long CashReceive = 0;
            Int64.TryParse(txtCash.Text, out CashReceive);

            if ((TotalAmountFromGiftCard + CashReceive) < TotalCost)
            {
                tp.SetToolTip(txtCash, "Error");
                tp.Show("Card and Cash combine doesn't have enought amount for this receipt! " , txtCash);
                hasError = true;
            }

            if (!hasError)
            {

                if (TotalCost - TotalAmountFromGiftCard > 0)
                    CashReceive = TotalCost - TotalAmountFromGiftCard;
                else
                    CashReceive = 0;
                
                System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 3, ExtraTax + Tax, ExtraDiscount + Discount, TotalCost, CashReceive, null, CustomerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);

                                

                string resultId = Id.FirstOrDefault().ToString();
                Transaction insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
                foreach (TransactionDetail detail in DetailList)
                {
                    detail.IsDeleted = false;//Update IsDelete (Null to 0)

                    var detailID = entity.InsertTransactionDetail(insertedTransaction.Id, Convert.ToInt32(detail.ProductId), Convert.ToInt32(detail.Qty), Convert.ToInt32(detail.UnitPrice), Convert.ToDouble(detail.DiscountRate), Convert.ToDouble(detail.TaxRate), Convert.ToInt32(detail.TotalAmount), detail.IsDeleted, Convert.ToDouble(detail.IsDeductedBy), false).SingleOrDefault();
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
                                }
                            }
                        }
                    }
         

                }
                insertedTransaction.IsDeleted = false;
                insertedTransaction.ReceivedCurrencyId = SettingController.DefaultCurrency;

                if (GiftCardList.Count > 0)
                {
                    //attach giftcard information to transaction                
                    foreach (GiftCard gc in GiftCardList)
                    {                     
                        GiftCardInTransaction gic = new GiftCardInTransaction();
                        gic.TransactionId = insertedTransaction.Id;
                        gic.GiftCardId = gc.Id;
                        entity.GiftCardInTransactions.Add(gic);
                        //Clear giftcard in giftcard list

                        GiftCard giftcard = entity.GiftCards.Where(x => x.Id == gc.Id).FirstOrDefault();
                        giftcard.IsUsed = true;
                        giftcard.IsUsedDate = DateTime.Now;
                    }
                }
                //Add promotion gift records for this transaction
                if(GiftList.Count > 0)
                {
                    foreach (GiftSystem gs in GiftList)
                    {
                        AttachGiftSystemForTransaction agft = new AttachGiftSystemForTransaction();
                        agft.AttachGiftSystemId = gs.Id;
                        agft.TransactionId = insertedTransaction.Id;
                        entity.AttachGiftSystemForTransactions.Add(agft);
                    }
                }

                //Clear the draft   
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
                GiftList.Clear();
                //Print Invoice
                PrintInvoice(resultId, insertedTransaction);               

                MessageBox.Show("Payment Completed");
                if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                {
                    Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                    newForm.Clear();
                }
                this.Dispose();
            }
        }

        private void PrintInvoice(string resultId, Transaction insertedTransaction)
        {
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
                    newRow.TotalAmount = (int)transaction.UnitPrice * (int)transaction.Qty; //Edit By ZMH
                    newRow.UnitPrice = "1@" + transaction.UnitPrice.ToString();
                    _tAmt += newRow.TotalAmount;
                    dtReport.AddItemListRow(newRow);
                }

                string reportPath = "";
                ReportViewer rv = new ReportViewer();
                ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);

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

                //reportPath = Application.StartupPath + "\\Reports\\InvoiceGiftcardLoc.rdlc";
                rv.Reset();
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Add(rds);



                APP_Data.Customer cus = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();

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

                ReportParameter TransactionId = new ReportParameter("TransactionId", resultId.ToString());
                rv.LocalReport.SetParameters(TransactionId);

                APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

                ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
                rv.LocalReport.SetParameters(CounterName);

                ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                rv.LocalReport.SetParameters(PrintDateTime);

                ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
                rv.LocalReport.SetParameters(CasherName);


                ReportParameter TaxAmount = new ReportParameter("TaxAmount", insertedTransaction.TaxAmount.ToString());
                rv.LocalReport.SetParameters(TaxAmount);

                ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", insertedTransaction.DiscountAmount.ToString());
                rv.LocalReport.SetParameters(DiscountAmount);

                ReportParameter AmountFromGiftCard = new ReportParameter("AmountFromGiftCard", TotalAmountFromGiftCard.ToString());
                rv.LocalReport.SetParameters(AmountFromGiftCard);



                // ReportParameter TotalAmount = new ReportParameter("TotalAmount", Convert.ToInt32(insertedTransaction.TotalAmount + insertedTransaction.DiscountAmount).ToString());
                ReportParameter TotalAmount = new ReportParameter("TotalAmount", (Convert.ToInt32(insertedTransaction.TotalAmount) - Convert.ToInt32(TotalAmountFromGiftCard)).ToString());
                rv.LocalReport.SetParameters(TotalAmount);

              //  ReportParameter PaidAmount = new ReportParameter("PaidAmount", insertedTransaction.RecieveAmount.ToString());
                ReportParameter PaidAmount = new ReportParameter("PaidAmount", txtCash.Text.ToString());
                rv.LocalReport.SetParameters(PaidAmount);

              //  ReportParameter Change = new ReportParameter("Change", "0");
                ReportParameter Change = new ReportParameter("Change", lblChangesText.Text);
                rv.LocalReport.SetParameters(Change);
            


              //var _giftCardList = dgvGiftCardList.Rows.Cast<DataGridViewRow>().Select(a => a.Cells[1].Value).ToList();

              //string _giftCards = string.Join(", ", _giftCardList.ToList());

              //ReportParameter GiftCardNo = new ReportParameter("GiftCardNo", _giftCards);
              //rv.LocalReport.SetParameters(GiftCardNo);

                for (int i = 0; i <= 1; i++)
                {
                    PrintDoc.PrintReport(rv, "Slip");
                }
            
            #endregion
        }

        private void btnAddGC_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            if (txtGiftCardId.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtGiftCardId, "Error");
                tp.Show("Please fill up gift card number!", txtGiftCardId);
                hasError = true;
            }

            string CardNumber = txtGiftCardId.Text.Trim();
            GiftCard currentCard = (from gcard in entity.GiftCards where gcard.CardNumber == CardNumber && gcard.IsUsed == false && gcard.IsDeleted==false select gcard).FirstOrDefault<GiftCard>();

            //GiftCard is invalid
            if (currentCard == null)
            {
                tp.SetToolTip(txtGiftCardId, "Error");
                tp.Show("Card is already used or invalid!", txtGiftCardId);
                hasError = true;
            }
            else if (DateTime.Now.Date >= currentCard.ExpireDate.Value.Date)
            {
                tp.SetToolTip(txtGiftCardId, "Error");
                tp.Show("Card is already expired!", txtGiftCardId);
                hasError = true;
            }
            else if (currentCard.CustomerId != CustomerId)
            {
                tp.SetToolTip(txtGiftCardId, "Error");
                tp.Show("This card is not belong to current customer", txtGiftCardId);
                hasError = true;
            }
            else
            {
                //if GiftCard Already in the list
                foreach (GiftCard gs in GiftCardList)
                {
                    if (gs.Id == currentCard.Id)
                    {
                        tp.SetToolTip(txtGiftCardId, "Error");
                        tp.Show("Card already in the list", txtGiftCardId);
                        hasError = true;
                    }
                }
            }

            if (!hasError)
            {
                GiftCardList.Add(currentCard);
                dgvGiftCardList.AutoGenerateColumns = false;

                dgvGiftCardList.DataSource = null;
                dgvGiftCardList.DataSource = GiftCardList;
                UpdateTotalCost();
                txtGiftCardId.Text = string.Empty;
            }

        }        

        private void dgvGiftCardList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {                
                //Delete
                if (e.ColumnIndex == 3)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result.Equals(DialogResult.OK))
                    {
                        int index = e.RowIndex;
                        ////DataGridViewRow row = dgvGiftCardList.Rows[e.RowIndex];
                        ////GiftCard giftCardObj = (GiftCard)row.DataBoundItem;
                        ////GiftCardList.Remove(giftCardObj);
                        ////dgvGiftCardList.Rows.RemoveAt(e.RowIndex);
                        ////dgvGiftCardList.CurrentCell = dgvGiftCardList[0, e.RowIndex];
                        ////UpdateTotalCost();

                        GiftCard pdo = GiftCardList[index];
                        GiftCardList.RemoveAt(index);
                        dgvGiftCardList.DataSource = GiftCardList.ToList();                        

                        //Clear_Amt();
                        UpdateTotalCost();
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private void txtCash_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCash_KeyUp(object sender, KeyEventArgs e)
        {
            //UpdateTotalCost();
        }

        #endregion

        #region Function

        private void UpdateTotalCost()
        {
            TotalAmountFromGiftCard = (long)GiftCardList.Sum(x => x.Amount);
            lblAmountFromGiftCard.Text = TotalAmountFromGiftCard.ToString();
            long CashReceive = 0;
            Int64.TryParse(txtCash.Text, out CashReceive);

            if (CashReceive == 0)
            {
                lblChangesText.Text = "0";                
            }
            else if(Convert.ToInt32(lblTotalCost.Text) <= TotalAmountFromGiftCard)
            {
                lblChangesText.Text = CashReceive.ToString();
                
            }
            else
            {
                lblChangesText.Text = ((TotalAmountFromGiftCard + CashReceive) - Convert.ToInt32(lblTotalCost.Text)).ToString();
            }

            long payableamount = Convert.ToInt64(lblTotalCost.Text) - (int)TotalAmountFromGiftCard;
            if (payableamount < 0)
            {
                payableamount = 0;
            }

            lblPayableAmount.Text = payableamount.ToString();
            if (Convert.ToInt32(lblTotalCost.Text) <= TotalAmountFromGiftCard)
            {
                txtCash.Enabled = false;
                txtCash.Text = "0";
            }
            else
            {
                txtCash.Enabled = true;
                txtCash.Text = "0";
            }
            

        }

        #endregion

    }
}
