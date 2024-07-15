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
    public partial class VIP_Member_Weekly_Transaction : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();
        List<VIP_Member_Weekly_Summary> resultList = new List<VIP_Member_Weekly_Summary>();
        string title = string.Empty;
        System.Data.Objects.ObjectResult<VIPReportForNoveltyAndGWP_Result> rList;
        Boolean IsStart = false;
        #endregion
        public VIP_Member_Weekly_Transaction()
        {
            InitializeComponent();
        }

        private void VIP_Member_Weekly_Transaction_Load(object sender, EventArgs e)
        {
            Counter_BInd();
            City_BInd();
            IsStart = true;
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void rdbEach_VIp_CheckedChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void rdbVIPs_CheckedChanged(object sender, EventArgs e)
        {
            //LoadData();
        }
        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void cboCounter_SelectedIndexChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void cboCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            //LoadData();
        }

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


        private void City_BInd()
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
            if (IsStart)
            {
                int counterId = 0, cityId = 0;
                if (cboCounter.SelectedIndex > 0)
                {
                    counterId = Convert.ToInt32(cboCounter.SelectedValue);
                }

                if (cboCity.SelectedIndex > 0)
                {
                    cityId = Convert.ToInt32(cboCity.SelectedValue);
                }
                DateTime fromDate = dtFrom.Value.Date;
                DateTime toDate = dtTo.Value.Date;
                bool IsVIPs = rdbVIPs.Checked;
                resultList.Clear();

                if (!IsVIPs)
                {
                    rList = entity.VIPReportForNoveltyAndGWP(1, fromDate, toDate, counterId, cityId);
                    title = "VIP Weekly Transactions";
                }
                else
                {
                    rList = entity.VIPReportForNoveltyAndGWP(2, fromDate, toDate, counterId, cityId);
                    title = "Non VIP Weekly Transactions";
                }
                //////foreach (VIPReportForNoveltyAndGWP_Result r in rList)
                //////{
                //////    VIP_Member_Weekly_Summary rObj = new VIP_Member_Weekly_Summary();
                //////    rObj.Name = r.Name;
                //////    rObj.InvoiceQty = (r.InvoiceQty == null)? 0: Convert.ToInt32(r.InvoiceQty);
                //////    rObj.Amount = (r.Total == null) ? 0 : Convert.ToInt32(r.Total);
                //////    rObj.Qty = (r.productQty == null) ? 0 : Convert.ToInt32(r.productQty);
                //////    rObj.Novelty_Qty = (r.NV_Qty == null) ? 0 : Convert.ToInt32(r.NV_Qty);
                //////    rObj.GWP_Qty = (r.GWPQty == null) ? 0 : Convert.ToInt32(r.GWPQty);
                //////    rObj.IsVIP = (r.IsVIP == null)? "" :r.IsVIP.ToString();
                //////    resultList.Add(rObj);
                //////}

                ShowReportViewer();
            }
        }
        private void ShowReportViewer()
        {

            dsReportTemp dsReport = new dsReportTemp();            
            dsReportTemp.VIP_MemberDataTable dtVIPReport = (dsReportTemp.VIP_MemberDataTable)dsReport.Tables["VIP_Member"];
            ////foreach (VIP_Member_Weekly_Summary v in resultList)
            ////{
            ////    dsReportTemp.VIP_MemberRow newRow = dtVIPReport.NewVIP_MemberRow();
            ////    newRow.Name = v.Name;
            ////    newRow.Invoice_Qty = v.InvoiceQty;
            ////    newRow.Amount = v.Amount;
            ////    newRow.Qty = v.Qty;
            ////    newRow.Novelty_Qty = v.Novelty_Qty;
            ////    newRow.GWP_Qty = v.GWP_Qty;
            ////    newRow.IsVIP = v.IsVIP;
            ////    dtVIPReport.AddVIP_MemberRow(newRow);
            ////}

            ReportDataSource rds = new ReportDataSource();
            rds.Name = "DataSet1";
            rds.Value = rList;
            ////ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["VIP_Member"]);
            string reportPath = Application.StartupPath + "\\Reports\\VIP_Member_Weekly_Invoice.rdlc";
            
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            //ReportParameter ItemReportTitle = new ReportParameter("ItemReportTitle", gbList.Text + " for " + SettingController.DefaultShop.ShopName);
            //reportViewer1.LocalReport.SetParameters(ItemReportTitle);

            ReportParameter Header = new ReportParameter("Header", title.ToString());
            reportViewer1.LocalReport.SetParameters(Header);

            reportViewer1.RefreshReport();
        }

        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
