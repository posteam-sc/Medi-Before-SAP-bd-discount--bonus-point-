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
    public partial class TopSaleReport : Form
    {
        #region Variable

        POSEntities entity = new POSEntities();
        List<TopProductHolder> itemList = new List<TopProductHolder>();

        #endregion

        #region Event
        public TopSaleReport()
        {
            InitializeComponent();
        }

        private void cboCity_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboCounter_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void TopSaleReport_Load(object sender, EventArgs e)
        {
            txtRow.Text = SettingController.DefaultTopSaleRow.ToString() ;
            City_Bind();
            Counter_Bind();
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void rdbQty_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbAmount_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void txtRow_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region [print]
            dsReportTemp dsReport = new dsReportTemp();
            //dsReportTemp.ItemListDataTable dtItemReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["TopItemList"];
            dsReportTemp.TopItemListDataTable dtItemReport = (dsReportTemp.TopItemListDataTable)dsReport.Tables["TopItemList"];
            foreach (TopProductHolder p in itemList)
            {
                //dsReportTemp.ItemListRow newRow = dtItemReport.NewItemListRow();
                dsReportTemp.TopItemListRow newRow = dtItemReport.NewTopItemListRow();
                newRow.ProductCode = p.ProductId.ToString();
                newRow.ProductName = p.Name.ToString();
                newRow.Discount = p.Discount.ToString();
                newRow.Qty = p.Qty.ToString();
                newRow.Amount = p.totalAmount.ToString();
                dtItemReport.AddTopItemListRow(newRow);
                //dtItemReport.AddItemListRow(newRow);
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["TopItemList"]);
            string reportPath = Application.StartupPath + "\\Reports\\BestSellersReport.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

           // ReportParameter ShopName = new ReportParameter("ShopName", "Best Seller Report for " + SettingController.DefaultShop.ShopName);
            string _counter = Utility.Counter_Check(cboCounter);
            ReportParameter ShopName = new ReportParameter("ShopName", "Best Seller Report for " + _counter);
            reportViewer1.LocalReport.SetParameters(ShopName);

            ReportParameter Date = new ReportParameter("Date", " from " + dtpFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtpTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);

            ReportParameter RowAmount = new ReportParameter("RowAmount", txtRow.Text.Trim());
            reportViewer1.LocalReport.SetParameters(RowAmount);
            PrintDoc.PrintReport(reportViewer1);
            #endregion
        }

        #endregion

        #region Function

        private void Counter_Bind()
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


        private void City_Bind()
        {
            List<APP_Data.City> cityList = new List<APP_Data.City>();
            APP_Data.City city1 = new APP_Data.City();
            city1.Id = 0;
            city1.CityName = "Select";
            cityList.Add(city1);
            cityList.AddRange(entity.Cities.ToList());
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";
        }


        private void LoadData()
        {
            int counterId = 0, cityId=0;
            if (cboCounter.SelectedIndex != 0)
            {
                counterId = Convert.ToInt32(cboCounter.SelectedValue);
            }

            if (cboCity.SelectedIndex != 0)
            {
                cityId = Convert.ToInt32(cboCity.SelectedValue);
            }
            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date;
            bool IsAmount = rdbAmount.Checked;
            int totalRow = 0;
            Int32.TryParse(txtRow.Text,out totalRow);
            itemList.Clear();
            System.Data.Objects.ObjectResult<Top100SaleItemList_Result> resultList;
            resultList = entity.Top100SaleItemList(fromDate, toDate, IsAmount, totalRow, cityId, counterId);
            var something = resultList.Select(r => new TopProductHolder
            {
                ProductId = r.ItemId.ToString(),
                Name = r.ItemName,
                Qty = Convert.ToInt32(r.ItemQty),
                totalAmount = Convert.ToInt64(r.ItemTotalAmount)
            }).ToList();
            itemList.AddRange(something);
            //foreach (Top100SaleItemList_Result r in resultList)
            //{
              
            //    TopProductHolder p = new TopProductHolder();
            //    p.ProductId = r.ItemId.ToString();
            //    p.Name = r.ItemName;
            //    p.Discount = r.DisCount;
            //    p.UnitPrice = Convert.ToInt64(r.UnitPrice);
            //    p.Qty = Convert.ToInt32(r.ItemQty);
            //    p.totalAmount = Convert.ToInt64(r.ItemTotalAmount);                
            //    itemList.Add(p);
            //}
            ShowReportViewer();
            lblPeriod.Text = fromDate.ToString() + " to " + toDate.ToString();
        }

         private void ShowReportViewer()
        {

            dsReportTemp dsReport = new dsReportTemp();
            //dsReportTemp.ItemListDataTable dtItemReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["TopItemList"];
             dsReportTemp.TopItemListDataTable dtItemReport = (dsReportTemp.TopItemListDataTable)dsReport.Tables["TopItemList"];
            foreach (TopProductHolder p in itemList)
            {
                //dsReportTemp.ItemListRow newRow = dtItemReport.NewItemListRow();
                dsReportTemp.TopItemListRow newRow = dtItemReport.NewTopItemListRow();
                newRow.ProductCode = p.ProductId.ToString();
                newRow.ProductName = p.Name.ToString();
                newRow.Discount = "0";
                newRow.Qty = p.Qty.ToString();
                newRow.Amount = p.totalAmount.ToString();
                dtItemReport.AddTopItemListRow(newRow);
                //dtItemReport.AddItemListRow(newRow);
            }


            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["TopItemList"]);
            string reportPath = Application.StartupPath + "\\Reports\\BestSellersReport.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            //ReportParameter ShopName = new ReportParameter("ShopName", "Best Seller Report for " + SettingController.DefaultShop.ShopName);
            string _counter = Utility.Counter_Check(cboCounter);
            ReportParameter ShopName = new ReportParameter("ShopName", "Best Seller Report for " + _counter);
            reportViewer1.LocalReport.SetParameters(ShopName);

            ReportParameter Date = new ReportParameter("Date", " from " + dtpFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtpTo.Value.Date.ToString("dd/MM/yyyy"));
            reportViewer1.LocalReport.SetParameters(Date);            

            ReportParameter RowAmount = new ReportParameter("RowAmount", txtRow.Text.Trim());
            reportViewer1.LocalReport.SetParameters(RowAmount);
            reportViewer1.RefreshReport();
        }

        #endregion                

     

    }
}
