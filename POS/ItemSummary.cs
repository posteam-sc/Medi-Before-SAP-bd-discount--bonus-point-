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
    public partial class ItemSummary : Form
    {
        #region Variable

        POSEntities entity = new POSEntities();
        List<Product> itemList = new List<Product>();

        #endregion

        #region Event

        public ItemSummary()
        {
            InitializeComponent();
        }

        private void ItemSummary_Load(object sender, EventArgs e)
        {
            
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void rdbSale_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        
        private void rdbRefund_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region [ Print ]

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.ItemListDataTable dtItemReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];

            foreach (Product p in itemList)
            {
                dsReportTemp.ItemListRow newRow = dtItemReport.NewItemListRow();
                newRow.ItemId = p.Id.ToString();
                newRow.Name = p.Name;
                newRow.Qty = p.Qty.ToString();
                newRow.TotalAmount = Convert.ToInt32(p.Price);
                dtItemReport.AddItemListRow(newRow);
            }
            string reportPath = "";
            ReportViewer rv = new ReportViewer();
            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
            reportPath = Application.StartupPath + "\\Reports\\ItemReport.rdlc";
            rv.Reset();
            rv.LocalReport.ReportPath = reportPath;
            rv.LocalReport.DataSources.Add(rds);

            ReportParameter ItemReportTitle = new ReportParameter("ItemReportTitle", gbList.Text + " for " + SettingController.DefaultShop.ShopName);
            rv.LocalReport.SetParameters(ItemReportTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            PrintDoc.PrintReport(rv);
            #endregion
        }

        #endregion

        #region Function

        private void LoadData()
        {
            
            DateTime fromDate = dtFrom.Value.Date;
            DateTime toDate = dtTo.Value.Date;
            bool IsSale = rdbSale.Checked;
            
            itemList.Clear();
            System.Data.Objects.ObjectResult<SelectItemListByDate_Result> resultList;
            resultList = entity.SelectItemListByDate(fromDate, toDate, IsSale, 0);
            foreach (SelectItemListByDate_Result r in resultList)
            {
                Product p = new Product();
                p.ProductCode = r.ItemId;
                p.Name = r.ItemName;
                p.Qty = r.ItemQty;
                p.Price = Convert.ToInt32(r.ItemTotalAmount);
                itemList.Add(p);
            }
            if (IsSale)
            {
                gbList.Text = "Item Sale Report";

            }
            else
            {
                gbList.Text = "Item Refund Report";

            }
            ShowReportViewer();
           
        }

        private void ShowReportViewer()
        {

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.ItemListDataTable dtItemReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];

            foreach (Product p in itemList)
            {
                dsReportTemp.ItemListRow newRow = dtItemReport.NewItemListRow();
                newRow.ItemId = p.ProductCode;
                newRow.Name = p.Name;
                newRow.Qty = p.Qty.ToString();
                newRow.TotalAmount = Convert.ToInt32(p.Price);
                dtItemReport.AddItemListRow(newRow);
            }


            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
            string reportPath = Application.StartupPath + "\\Reports\\ItemReport.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter ItemReportTitle = new ReportParameter("ItemReportTitle", gbList.Text + " for " + SettingController.DefaultShop.ShopName);
            reportViewer1.LocalReport.SetParameters(ItemReportTitle);

            ReportParameter Date = new ReportParameter("Date", " from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            reportViewer1.RefreshReport();
        }
        #endregion                


       

    }
}
