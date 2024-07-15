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
    public partial class EmailCompilation : Form
    {
        #region Variables
        POSEntities entity = new POSEntities();
        List<Customer> cusList = new List<Customer>();
        string title = string.Empty;
        #endregion
        #region Event
        public EmailCompilation()
        {
            InitializeComponent();
        }

        private void EmailCompilation_Load(object sender, EventArgs e)
        {
            rdbAll.Checked = true;
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void rdbAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void rdbVIP_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Function
        private void LoadData()
        {
            if (rdbAll.Checked)
            {
                cusList = entity.Customers.ToList();
                title = "All Member Email";
            }
            else if (rdbNonVIP.Checked)
            {
                cusList = entity.Customers.Where(x => x.CustomerTypeId == 2).ToList();
                title = "Non VIP Email";
            }
            else if (rdbVIP.Checked)
            {
                cusList = entity.Customers.Where(x => x.CustomerTypeId == 1).ToList();
                title = "VIP Email";
            }
            ShowReportViewer();
            

        }
        private void ShowReportViewer()
        {

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.CustomerInformationDataTable dtCustomerReport = (dsReportTemp.CustomerInformationDataTable)dsReport.Tables["CustomerInformation"];
            foreach (Customer c in cusList)
            {
                dsReportTemp.CustomerInformationRow newRow = dtCustomerReport.NewCustomerInformationRow();
                newRow.Name = c.Name + " " + c.Title;
                newRow.Birthday = (c.Birthday == null) ? "-" : c.Birthday.Value.Date.ToString("dd/MM/yyyy");
                newRow.Gender = (c.Gender == null) ? "-" : c.Gender;
                newRow.NRC = (c.NRC == null) ? "-" : c.NRC;
                newRow.PhoneNumber = (c.PhoneNumber == null) ? "-" : c.PhoneNumber;
                newRow.Email = (c.Email == null) ? "-" : c.Email;
                dtCustomerReport.AddCustomerInformationRow(newRow);
            }
            
            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["CustomerInformation"]);
            string reportPath = Application.StartupPath + "\\Reports\\Customer_EmailCompilation.rdlc";            
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter Header = new ReportParameter("Header", title.ToString());
            reportViewer1.LocalReport.SetParameters(Header);

            reportViewer1.RefreshReport();
        }
        #endregion
    }
}
