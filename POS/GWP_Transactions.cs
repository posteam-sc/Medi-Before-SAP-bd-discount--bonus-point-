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
    public partial class NoveltiesSale : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();
        List<GWP_Transactions_Controller> FinalResultList = new List<GWP_Transactions_Controller>();
        List<GWPSet_Controller> GWPSetList = new List<GWPSet_Controller>();
        int InvoiceCount = 0;
        int ProductCount = 0;
        #endregion
        #region Event
        public NoveltiesSale()
        {
            InitializeComponent();
        }

        private void GWP_Transactions_Load(object sender, EventArgs e)
        {
            rdbVIP.Checked = true;
            Counter_BInd();
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

        private void rdbVIP_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbNon_VIP_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboCounter_SelectedIndexChanged(object sender, EventArgs e)
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
            int counterId = 0;
            if (cboCounter.SelectedIndex > 0)
            {
                counterId = Convert.ToInt32(cboCounter.SelectedValue);
            }

            InvoiceCount = 0; ProductCount = 0;
            DateTime fromDate = dtFrom.Value.Date;
            DateTime toDate = dtTo.Value.Date;
            int cusTypeId = (rdbVIP.Checked) ? 1 : 2;
            string TId = string.Empty;
            FinalResultList.Clear();
            GWPSetList.Clear();
            List<GWP_Transactions_Controller> GWPTransList = new List<GWP_Transactions_Controller>();
            System.Data.Objects.ObjectResult<GetGWPTransactions_Result> result = entity.GetGWPTransactions(cusTypeId, fromDate, toDate,counterId);

            #region select GWP Transactions
            foreach (GetGWPTransactions_Result r in result)
            {
                //check that transaction is in the attachGiftSystemForTransaction
                List<AttachGiftSystemForTransaction> attachGiftList = entity.AttachGiftSystemForTransactions.Where(x => x.TransactionId == r.InvoiceNo).ToList();
                if (attachGiftList.Count > 0)
                {
                    //count transaction
                    if (TId != r.InvoiceNo)
                    {

                        InvoiceCount++;
                        TId = r.InvoiceNo;
                    }

                    Product pObj = entity.Products.FirstOrDefault(x => x.Id == r.productId);
                    if (pObj.IsWrapper == true)
                    {
                        List<WrapperItem> wrappObj = entity.WrapperItems.Where(x => x.ParentProductId == pObj.Id).ToList();
                        List<Product> pList = new List<Product>();
                        foreach (WrapperItem w in wrappObj)
                        {
                            pList.Add(w.Product1);
                        }
                        foreach (Product p in pList)
                        {
                            GWP_Transactions_Controller GWPTransObj = new GWP_Transactions_Controller();
                            GWPTransObj.Name = r.Name;
                            GWPTransObj.TransactionNo = r.InvoiceNo;
                            //GWPTransObj.ItemCode = p.Name;
                            GWPTransObj.ItemCode = p.ProductCode;
                            GWPTransObj.GiftName = (r.GiftName == null) ? "" : r.GiftName;
                            GWPTransObj.Discount = r.Dis;
                            GWPTransObj.Qty = Convert.ToInt32(r.Qty);
                            GWPTransObj.TotalAmount = Convert.ToDecimal(p.Price) * Convert.ToInt32(r.Qty) - (((Convert.ToDecimal(p.Price) * Convert.ToInt32(r.Qty)) / 100) * r.Dis);
                            FinalResultList.Add(GWPTransObj);
                            ProductCount++;
                        }
                    }
                    else
                    {
                        GWP_Transactions_Controller GWPTransObj = new GWP_Transactions_Controller();
                        GWPTransObj.Name = (r.Name == null) ? "Unknown" : r.Name;
                        GWPTransObj.TransactionNo = r.InvoiceNo;
                        //GWPTransObj.ItemCode = pObj.Name;
                        GWPTransObj.ItemCode = pObj.ProductCode;
                        GWPTransObj.GiftName = (r.GiftName == null) ? "" : r.GiftName;
                        GWPTransObj.Discount = r.Dis;
                        GWPTransObj.Qty = Convert.ToInt32(r.Qty);
                        GWPTransObj.TotalAmount = Convert.ToDecimal(r.Total);
                        FinalResultList.Add(GWPTransObj);
                        ProductCount++;
                    }


                }
            }
            #endregion

            #region GWP Total Qty and Amount by time period
            System.Data.Objects.ObjectResult<GetGWPSetQtyAndAmount_Result> GWPSetResult = entity.GetGWPSetQtyAndAmount(cusTypeId, fromDate, toDate, counterId);
            foreach (GetGWPSetQtyAndAmount_Result r in GWPSetResult)
            {
                GWPSet_Controller GWPObj = new GWPSet_Controller();
                GWPObj.Id = r.Id;
                GWPObj.Name = r.Name;
                GWPObj.Qty = Convert.ToInt32(r.Qty);
                GWPObj.Amount = Convert.ToInt64(r.Amount);
                GWPSetList.Add(GWPObj);
            }
            #endregion
            ShowReportViewer();
        }

        private void ShowReportViewer()
        {
            string Member = string.Empty;
            if (rdbVIP.Checked)
            {
                Member = "VIP Members";
            }
            else
            {
                Member = "Non VIP Members";
            }
            dsReportTemp dsReport = new dsReportTemp();
            //dsReportTemp.VIP_MemberDataTable dtVIPReport = (dsReportTemp.VIP_MemberDataTable)dsReport.Tables["VIP_Member"];
            dsReportTemp.GWPTransactionDataTable dtGWPTransReport = (dsReportTemp.GWPTransactionDataTable)dsReport.Tables["GWPTransaction"];
            foreach (GWP_Transactions_Controller g in FinalResultList)
            {
                dsReportTemp.GWPTransactionRow newRow = dtGWPTransReport.NewGWPTransactionRow();
                newRow.Name = g.Name;
                newRow.InvoiceNo = g.TransactionNo;
                newRow.ProductCode = g.ItemCode;
                newRow.GiftName = g.GiftName;
                newRow.Dis = g.Discount;
                newRow.Qty = g.Qty;
                newRow.TotalAmount = g.TotalAmount;
                dtGWPTransReport.AddGWPTransactionRow(newRow);
            }


            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["GWPTransaction"]);

            dsReportTemp dsReportGWP = new dsReportTemp();
            dsReportTemp.GWPSetDataTable dtGWPSetReport = (dsReportTemp.GWPSetDataTable)dsReportGWP.Tables["GWPSet"];
            foreach (GWPSet_Controller s in GWPSetList)
            {
                dsReportTemp.GWPSetRow newRow = dtGWPSetReport.NewGWPSetRow();
                newRow.Id = s.Id.ToString();
                newRow.Name = s.Name;
                newRow.Qty = s.Qty.ToString();
                newRow.Amount = s.Amount.ToString();
                dtGWPSetReport.AddGWPSetRow(newRow);
            }
            ReportDataSource rds2 = new ReportDataSource("DataSet2", dsReportGWP.Tables["GWPSet"]);

            string reportPath = Application.StartupPath + "\\Reports\\GWP_Transactions.rdlc";

            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.DataSources.Add(rds2);

            ReportParameter TransCount = new ReportParameter("TransCount", InvoiceCount.ToString());
            reportViewer1.LocalReport.SetParameters(TransCount);

            ReportParameter ItemCount = new ReportParameter("ItemCount", ProductCount.ToString());
            reportViewer1.LocalReport.SetParameters(ItemCount);

            ReportParameter Header = new ReportParameter("Header", "GWP Transaction of "+Member +" from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Header);

            reportViewer1.RefreshReport();
        }
        #endregion

       
    }
}
