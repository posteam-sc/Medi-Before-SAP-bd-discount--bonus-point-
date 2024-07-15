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
using Microsoft.Reporting.WinForms;
using POS.APP_Data;

namespace POS
{
    public partial class Loc_ItemSummary : Form
    {
        #region Variable

        POSEntities entity = new POSEntities();
        //List<Product> itemList = new List<Product>();
        List<ReportItemSummary> itemList = new List<ReportItemSummary>();
        List<ReportItemSummary> IList = new List<ReportItemSummary>();
        long CashTotal = 0, CreditTotal = 0, FOCAmount = 0, MPUAmount = 0, TesterAmount = 0, GiftCardAmount = 0, Total = 0, CreditReceive = 0; long UseGiftAmount = 0; long newPaymentGAmount = 0;
        int globalCard = 0, pay = 0, bankTransfer = 0;
        List<Transaction> AllTranslist = new List<Transaction>();
        decimal totalSettlement = 0;
        List<Transaction> DtransList = new List<Transaction>();
        #endregion

        #region Event
        public Loc_ItemSummary()
        {
            InitializeComponent();
        }

        private void Loc_ItemSummary_Load(object sender, EventArgs e)
        {
            Counter_BInd();
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void cboCounter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbSale_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbRefund_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbRefund.Checked)
            {
                //  gbPaymentType.Enabled = false;
            }
            LoadData();
        }

        private void chkCash_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkGiftCard_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkMPU_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkCredit_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkFOC_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void chkTester_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion



        #region Function

        private void Counter_BInd()
        {
            List<APP_Data.Counter> counterList = new List<APP_Data.Counter>();
            APP_Data.Counter counterObj = new APP_Data.Counter();
            counterObj.Id = 0;
            counterObj.Name = "Select";
            counterList.Add(counterObj);
            counterList.AddRange((from c in entity.Counters orderby c.Id select c).ToList());
            cboCounter.DataSource = counterList;
            cboCounter.DisplayMember = "Name";
            cboCounter.ValueMember = "Id";
        }

        private void LoadData()
        {
            DateTime fromDate = dtFrom.Value.Date;
            DateTime toDate = dtTo.Value.Date;
            bool IsSale = rdbSale.Checked;
            // bool IsCash = chkCash.Checked, IsCredit = chkCredit.Checked, IsFOC = chkFOC.Checked, IsMPU = chkMPU.Checked, IsGiftCard = chkGiftCard.Checked, IsTester = chkTester.Checked;
            bool IsCash = true, IsCredit = true, IsFOC = true, IsMPU = true, IsGiftCard = true, IsTester = true;
            CashTotal = 0; CreditTotal = 0; FOCAmount = 0; MPUAmount = 0; TesterAmount = 0; GiftCardAmount = 0; Total = 0;
            globalCard = 0; pay = 0; bankTransfer = 0;
            IList.Clear();
            itemList.Clear();
            System.Data.Objects.ObjectResult<SelectItemListByDate_Result> resultList;
            //System.Data.Objects.ObjectResult<SelectItemListByDate_Result> FinalResultList ;
            //FinalResultList = null;
            List<Transaction> transList = new List<Transaction>();
            int counterId = 0;
            if (cboCounter.SelectedIndex > 0)
            {
                counterId = Convert.ToInt32(cboCounter.SelectedValue);
            }

            if (IsSale)
            {
                transList = (from t in entity.Transactions
                             where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate
                             && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true
                             && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit)
                             && (t.IsDeleted == null || t.IsDeleted == false) && ((counterId != 0 && t.CounterId == counterId) || (counterId == 0 && 1 == 1))
                             select t).ToList<Transaction>();
            }
            else
            {
                transList = (from t in entity.Transactions
                             where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true
   && t.IsActive == true && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) && (t.IsDeleted == null || t.IsDeleted == false)
   && ((counterId != 0 && t.CounterId == counterId) || (counterId == 0 && 1 == 1))
                             select t).ToList<Transaction>();
            }

            DtransList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && (t.Type == TransactionType.Settlement || t.Type == TransactionType.Prepaid) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();

            List<ReportItemSummary> FinalResultList = new List<ReportItemSummary>();
            resultList = entity.SelectItemListByDate(fromDate, toDate, IsSale, counterId);
            foreach (SelectItemListByDate_Result r in resultList)
            {
                ReportItemSummary p = new ReportItemSummary();
                p.Id = r.ItemId;
                p.Name = r.ItemName;
                p.Qty = (int)r.ItemQty;
                p.UnitPrice = Convert.ToInt32(r.UnitPrice);
                p.totalAmount = Convert.ToInt64(r.ItemTotalAmount);
                p.PaymentId = (int)r.PaymentTypeId;
                p.Size = r.Size;
                itemList.Add(p);
            }

            AllTranslist.Clear();
            CreditReceive = 0;
            UseGiftAmount = 0;
            if (IsSale == true)
            {
                if (IsCash)
                {


                    CashTotal = (long)transList.Where(x => x.PaymentTypeId == 1).Sum(x => x.TotalAmount);
                    CashTotal += (from t in entity.TransactionPaymentDetails
                                  join td in entity.Transactions on t.TransactionId equals td.Id
                                  join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                  where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                  && td.IsDeleted == false && p.Name == "Cash" && ((counterId != 0 && td.CounterId == counterId) || (counterId == 0 && 1 == 1))
                                  select t.Amount).ToList().Sum();
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 1).ToList());
                }
                if (IsCredit)
                {

                    CreditTotal += FinalResultList.Where(x => x.PaymentId == 2).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 2).ToList());
                    CreditReceive += Convert.ToInt64(transList.Where(x => x.PaymentTypeId == 2).Sum(x => x.RecieveAmount));
                }
                if (IsGiftCard)
                {

                    GiftCardAmount += (long)transList.Where(x => x.PaymentTypeId == 3).Sum(x => x.TotalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 3).ToList());

                    List<Transaction> GTransList = transList.Where(x => x.PaymentTypeId == 3).ToList();
                    foreach (Transaction t in GTransList)
                    {
                        List<GiftCardInTransaction> GList = t.GiftCardInTransactions.ToList();
                        foreach (GiftCardInTransaction g in GList)
                        {
                            UseGiftAmount += Convert.ToInt64(g.GiftCard.Amount);
                        }
                    }
                    newPaymentGAmount = (from t in entity.TransactionPaymentDetails
                                      join td in entity.Transactions on t.TransactionId equals td.Id
                                      join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                      where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                      && td.IsDeleted == false && p.Name == "Gift Card" && ((counterId != 0 && td.CounterId == counterId) || (counterId == 0 && 1 == 1))
                                      select t.Amount).ToList().Sum();
                }
                if (IsFOC)
                {

                    FOCAmount += FinalResultList.Where(x => x.PaymentId == 4).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 4).ToList());
                }
                if (IsMPU)
                {
                    MPUAmount = (long)transList.Where(x => x.PaymentTypeId == 5).Sum(x => x.RecieveAmount);
                    MPUAmount += (from t in entity.TransactionPaymentDetails
                                  join td in entity.Transactions on t.TransactionId equals td.Id
                                  join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                  where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                  && td.IsDeleted == false && p.PaymentParentId == 3 && ((counterId != 0 && td.CounterId == counterId) || (counterId == 0 && 1 == 1))
                                  select t.Amount).ToList().Sum();
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 5).ToList());
                }
                if (IsTester)
                {

                    TesterAmount += FinalResultList.Where(x => x.PaymentId == 6).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 6).ToList());
                }
                globalCard = (int)(from t in entity.TransactionPaymentDetails
                                   join td in entity.Transactions on t.TransactionId equals td.Id
                                   join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                   where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                   && td.IsDeleted == false && p.PaymentParentId == 4 && ((counterId != 0 && td.CounterId == counterId) || (counterId == 0 && 1 == 1))
                                   select t.Amount).ToList().Sum();
                bankTransfer = (int)(from t in entity.TransactionPaymentDetails
                                     join td in entity.Transactions on t.TransactionId equals td.Id
                                     join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                                     where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                                     && td.IsDeleted == false && p.PaymentParentId == 5 && ((counterId != 0 && td.CounterId == counterId) || (counterId == 0 && 1 == 1))
                                     select t.Amount).ToList().Sum();
                pay = (int)(from t in entity.TransactionPaymentDetails
                            join td in entity.Transactions on t.TransactionId equals td.Id
                            join p in entity.PaymentMethods on t.PaymentMethodId equals p.Id
                            where EntityFunctions.TruncateTime((DateTime)td.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)td.DateTime) <= toDate
                            && td.IsDeleted == false && p.PaymentParentId == 6 && ((counterId != 0 && td.CounterId == counterId) || (counterId == 0 && 1 == 1))
                            select t.Amount).ToList().Sum();
                AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 7).ToList());

                //var tmp = itemList.GroupBy(x => x.Id);
                //var result = tmp.Select(y => new
                //{
                //    Id = y.Key,
                //    Name = 
                //});
                //itemList = itemList.GroupBy(x => x.Id).SelectMany( x => x).ToList();

                foreach (ReportItemSummary r in itemList)
                {
                    Boolean already = false;
                    foreach (ReportItemSummary s in IList)
                    {
                        if (r.Id == s.Id && r.UnitPrice == s.UnitPrice)
                        {
                            s.Qty += r.Qty;
                            s.totalAmount += r.totalAmount;
                            already = true;
                        }
                    }
                    if (!already)
                    {
                        IList.Add(r);
                    }
                }
            }
            else
            {
                //itemList = (List<ReportItemSummary>)itemList.GroupBy(x => x.Id);
                foreach (ReportItemSummary r in FinalResultList)
                {
                    ReportItemSummary p = new ReportItemSummary();
                    p.Id = r.Id;
                    p.Name = r.Name;
                    p.Qty = (int)r.Qty;
                    p.UnitPrice = Convert.ToInt32(r.UnitPrice);
                    p.totalAmount = Convert.ToInt64(r.totalAmount);
                    p.PaymentId = (int)r.PaymentId;
                    CashTotal += Convert.ToInt64(r.totalAmount);
                    itemList.Add(p);
                }

                //Grop By Item
                foreach (ReportItemSummary r in itemList)
                {
                    Boolean already = false;
                    foreach (ReportItemSummary s in IList)
                    {
                        if (r.Id == s.Id && r.UnitPrice == s.UnitPrice)
                        {
                            s.Qty += r.Qty;
                            s.totalAmount += r.totalAmount;
                            already = true;
                        }
                    }
                    if (!already)
                    {
                        IList.Add(r);
                    }
                }
            }

            if (IsSale)
            {
                gbList.Text = "Daily Sales Report";

            }
            else
            {
                gbList.Text = "Daily Refunds Report";

            }
            ShowReportViewer();
        }

        private void ShowReportViewer()
        {

            dsReportTemp dsReport = new dsReportTemp();
            //dsReportTemp.ItemListDataTable dtItemReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["LO'c_ItemSummary"];
            dsReportTemp._LO_c_ItemSummaryDataTable dtItemReport = (dsReportTemp._LO_c_ItemSummaryDataTable)dsReport.Tables["LO'c_ItemSummary"];
            foreach (ReportItemSummary p in IList)
            {
                //dsReportTemp.ItemListRow newRow = dtItemReport.NewItemListRow();
                dsReportTemp._LO_c_ItemSummaryRow newRow = dtItemReport.New_LO_c_ItemSummaryRow();
                newRow.ItemCode = p.Id;
                newRow.Name = p.Name;
                newRow.Size = p.Size;
                newRow.Qty = p.Qty.ToString();
                newRow.UnitPrice = p.UnitPrice.ToString();
                newRow.TotalAmount = Convert.ToInt64(p.totalAmount);
                dtItemReport.Add_LO_c_ItemSummaryRow(newRow);
            }

            CashTotal += GiftCardAmount;


            if (rdbSale.Checked)
            {
                totalSettlement = DtransList.Sum(x => x.TotalAmount).Value;
            }
            else
            {
                totalSettlement = 0;
            }

            //To Find DiscountAmountOfAllTransactions
            decimal OverAllDis = 0;
            decimal OverAllDisMpu = 0;
            foreach (Transaction t in AllTranslist)
            {
                List<TransactionDetail> tdList = new List<TransactionDetail>();
                tdList = t.TransactionDetails.ToList();
                decimal itemDis = 0;
                if (t.PaymentTypeId != 5)
                {
                    foreach (TransactionDetail td in tdList)
                    {
                        itemDis += Convert.ToDecimal((td.UnitPrice * (td.DiscountRate / 100)) * td.Qty);
                    }
                    OverAllDis += Convert.ToDecimal(t.DiscountAmount);
                }
                else
                {
                    foreach (TransactionDetail td in tdList)
                    {
                        itemDis += Convert.ToDecimal((td.UnitPrice * (td.DiscountRate / 100)) * td.Qty);
                    }
                    OverAllDisMpu += Convert.ToDecimal(t.DiscountAmount);
                }

            }
            // decimal actualAmount = (Convert.ToDecimal(CashTotal+CreditReceive) - (OverAllDis+UseGiftAmount));
            decimal actualAmount = (Convert.ToDecimal(CashTotal - UseGiftAmount));
            Total = CashTotal + CreditTotal + FOCAmount + MPUAmount + TesterAmount + bankTransfer + globalCard + pay +newPaymentGAmount ;
            ReportDataSource rds = new ReportDataSource("ItemSummary", dsReport.Tables["LO'c_ItemSummary"]);
            //ReportDataSource rds = new ReportDataSource("ItemSummary", IList);
            string reportPath = Application.StartupPath + "\\Reports\\LO'C_Daily_Summary.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);


            //ReportParameter ItemReportTitle = new ReportParameter("ItemReportTitle", gbList.Text + " for " + SettingController.DefaultShop.ShopName);
            string _counter = Utility.Counter_Check(cboCounter);
            ReportParameter ItemReportTitle = new ReportParameter("ItemReportTitle", gbList.Text + " for " + _counter);
            reportViewer1.LocalReport.SetParameters(ItemReportTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            ReportParameter TotalAmount = new ReportParameter("TotalAmount", Total.ToString());
            reportViewer1.LocalReport.SetParameters(TotalAmount);


            ReportParameter CreditAmount = new ReportParameter("CreditAmount", (CreditTotal - CreditReceive).ToString());
            reportViewer1.LocalReport.SetParameters(CreditAmount);

            ReportParameter CashAmount = new ReportParameter("CashAmount", (CashTotal + CreditReceive).ToString());
            reportViewer1.LocalReport.SetParameters(CashAmount);

            ReportParameter DisAmount = new ReportParameter("DisAmount", (OverAllDis + OverAllDisMpu).ToString());
            reportViewer1.LocalReport.SetParameters(DisAmount);
            ReportParameter MpuDisAmount = new ReportParameter("MpuDisAmount", OverAllDisMpu.ToString());
            reportViewer1.LocalReport.SetParameters(MpuDisAmount);
            ReportParameter CashDisAmount = new ReportParameter("CashDisAmount", "0");
            reportViewer1.LocalReport.SetParameters(CashDisAmount);
            ReportParameter CreditDisAmount = new ReportParameter("CreditDisAmount", "0");
            reportViewer1.LocalReport.SetParameters(CreditDisAmount);


            ReportParameter UsedGiftAmount = new ReportParameter("UsedGiftAmount", (UseGiftAmount + newPaymentGAmount).ToString());
            reportViewer1.LocalReport.SetParameters(UsedGiftAmount);

            ReportParameter FOC = new ReportParameter("FOC", FOCAmount.ToString());
            reportViewer1.LocalReport.SetParameters(FOC);

            //if (OverAllDisMpu != 0)
            //{
            //    MPUAmount -= (long)OverAllDisMpu;
            //}
            ReportParameter MPU = new ReportParameter("MPU", MPUAmount.ToString());
            reportViewer1.LocalReport.SetParameters(MPU);
            ReportParameter GlobalCard = new ReportParameter("GlobalCard", globalCard.ToString());
            reportViewer1.LocalReport.SetParameters(GlobalCard);
            ReportParameter BankTransfer = new ReportParameter("BankTransfer", bankTransfer.ToString());
            reportViewer1.LocalReport.SetParameters(BankTransfer);
            ReportParameter Pay = new ReportParameter("Pay", pay.ToString());
            reportViewer1.LocalReport.SetParameters(Pay);

            ReportParameter Tester = new ReportParameter("Tester", TesterAmount.ToString());
            reportViewer1.LocalReport.SetParameters(Tester);


            ReportParameter TotalSettlement = new ReportParameter("TotalSettlement", totalSettlement.ToString());
            reportViewer1.LocalReport.SetParameters(TotalSettlement);


            ReportParameter ActualAmount = new ReportParameter("ActualAmount", actualAmount.ToString());
            reportViewer1.LocalReport.SetParameters(ActualAmount);

            reportViewer1.RefreshReport();
        }
        #endregion

    }
}
