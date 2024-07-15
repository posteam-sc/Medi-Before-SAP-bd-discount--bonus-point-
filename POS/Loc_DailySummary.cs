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
    public partial class Loc_DailySummary : Form
    {
        #region Variable

        POSEntities entity = new POSEntities();
        //List<Product> itemList = new List<Product>();
        List<ReportItemSummary> itemList = new List<ReportItemSummary>();
        List<ReportItemSummary> IList = new List<ReportItemSummary>();
        long CashTotal = 0, CreditTotal = 0, FOCAmount = 0, MPUAmount = 0, TesterAmount = 0, GiftCardAmount = 0, Total = 0, CreditReceive = 0; long UseGiftAmount = 0;
        List<Transaction> AllTranslist = new List<Transaction>();
        #endregion        

        #region Event
        public Loc_DailySummary()
        {
            InitializeComponent();
        }

        private void Loc_DailySummary_Load(object sender, EventArgs e)
        {
            LoadData();
            this.reportViewer1.RefreshReport();
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
                gbPaymentType.Enabled = false;
            }
            LoadData();
        }

        #endregion

        #region Function

        private void LoadData()
        {
            DateTime fromDate = dtFrom.Value.Date;
            DateTime toDate = dtTo.Value.Date;
            bool IsSale = rdbSale.Checked;
            bool IsCash = chkCash.Checked, IsCredit = chkCredit.Checked, IsFOC = chkFOC.Checked, IsMPU = chkMPU.Checked, IsGiftCard = chkGiftCard.Checked, IsTester = chkTester.Checked;
            CashTotal = 0; CreditTotal = 0; FOCAmount = 0; MPUAmount = 0; TesterAmount = 0; GiftCardAmount = 0; Total = 0;
            IList.Clear();
            itemList.Clear();
            System.Data.Objects.ObjectResult<SelectItemListByDate_Result> resultList;
            //System.Data.Objects.ObjectResult<SelectItemListByDate_Result> FinalResultList ;
            //FinalResultList = null;
            List<Transaction> transList = new List<Transaction>();
            if (IsSale)
            {
                transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Sale || t.Type == TransactionType.Credit) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
            }
            else
            {
                transList = (from t in entity.Transactions where EntityFunctions.TruncateTime((DateTime)t.DateTime) >= fromDate && EntityFunctions.TruncateTime((DateTime)t.DateTime) <= toDate && t.IsComplete == true && t.IsActive == true && (t.Type == TransactionType.Refund || t.Type == TransactionType.CreditRefund) && (t.IsDeleted == null || t.IsDeleted == false) select t).ToList<Transaction>();
            }

            List<ReportItemSummary> FinalResultList = new List<ReportItemSummary>();
            resultList = entity.SelectItemListByDate(fromDate, toDate, IsSale,0);
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
                FinalResultList.Add(p);
            }
            AllTranslist.Clear();
            CreditReceive = 0;
            UseGiftAmount = 0;
            if (IsSale == true)
            {
                if (IsCash)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 1).ToList());
                    CashTotal += FinalResultList.Where(x => x.PaymentId == 1).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 1).ToList());
                }
                if (IsCredit)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 2).ToList());
                    CreditTotal += FinalResultList.Where(x => x.PaymentId == 2).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 2).ToList());
                    CreditReceive += Convert.ToInt64(transList.Where(x => x.PaymentTypeId == 2).Sum(x => x.RecieveAmount));
                }
                if (IsGiftCard)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 3).ToList());
                    GiftCardAmount += FinalResultList.Where(x => x.PaymentId == 3).Sum(x => x.totalAmount);
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
                }
                if (IsFOC)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 4).ToList());
                    FOCAmount += FinalResultList.Where(x => x.PaymentId == 4).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 4).ToList());
                }
                if (IsMPU)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 5).ToList());
                    MPUAmount += FinalResultList.Where(x => x.PaymentId == 5).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 5).ToList());
                }
                if (IsTester)
                {
                    itemList.AddRange(FinalResultList.Where(x => x.PaymentId == 6).ToList());
                    TesterAmount += FinalResultList.Where(x => x.PaymentId == 6).Sum(x => x.totalAmount);
                    AllTranslist.AddRange(transList.Where(x => x.PaymentTypeId == 6).ToList());
                }
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
            Total = CashTotal + CreditTotal + FOCAmount + MPUAmount + GiftCardAmount + TesterAmount;
            CashTotal += GiftCardAmount;
            //To Find DiscountAmountOfAllTransactions
            decimal OverAllDis = 0;
            foreach (Transaction t in AllTranslist)
            {
                List<TransactionDetail> tdList = new List<TransactionDetail>();
                tdList = t.TransactionDetails.ToList();
                decimal itemDis = 0;
                foreach (TransactionDetail td in tdList)
                {
                    itemDis += Convert.ToDecimal((td.UnitPrice * (td.DiscountRate / 100)) * td.Qty);
                }
                OverAllDis += Convert.ToDecimal(t.DiscountAmount - itemDis);
            }
            decimal actualAmount = (Convert.ToDecimal(CashTotal + CreditReceive) - (OverAllDis + UseGiftAmount));

            ReportDataSource rds = new ReportDataSource("ItemSummary", dsReport.Tables["LO'c_ItemSummary"]);
            string reportPath = Application.StartupPath + "\\Reports\\LO'C_Daily_Summary.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter ItemReportTitle = new ReportParameter("ItemReportTitle", gbList.Text + " for " + SettingController.DefaultShop.ShopName);
            reportViewer1.LocalReport.SetParameters(ItemReportTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            ReportParameter TotalAmount = new ReportParameter("TotalAmount", Total.ToString());
            reportViewer1.LocalReport.SetParameters(TotalAmount);

            ReportParameter CreditAmount = new ReportParameter("CreditAmount", (CreditTotal - CreditReceive).ToString());
            reportViewer1.LocalReport.SetParameters(CreditAmount);

            ReportParameter CashAmount = new ReportParameter("CashAmount", (CashTotal + CreditReceive).ToString());
            reportViewer1.LocalReport.SetParameters(CashAmount);

            ReportParameter DisAmount = new ReportParameter("DisAmount", OverAllDis.ToString());
            reportViewer1.LocalReport.SetParameters(DisAmount);

            ReportParameter UsedGiftAmount = new ReportParameter("UsedGiftAmount", UseGiftAmount.ToString());
            reportViewer1.LocalReport.SetParameters(UsedGiftAmount);

            ReportParameter FOC = new ReportParameter("FOC", FOCAmount.ToString());
            reportViewer1.LocalReport.SetParameters(FOC);

            ReportParameter MPU = new ReportParameter("MPU", MPUAmount.ToString());
            reportViewer1.LocalReport.SetParameters(MPU);

            ReportParameter Tester = new ReportParameter("Tester", TesterAmount.ToString());
            reportViewer1.LocalReport.SetParameters(Tester);

            ReportParameter ActualAmount = new ReportParameter("ActualAmount", actualAmount.ToString());
            reportViewer1.LocalReport.SetParameters(ActualAmount);

            reportViewer1.RefreshReport();
        }
        #endregion
    }
}
