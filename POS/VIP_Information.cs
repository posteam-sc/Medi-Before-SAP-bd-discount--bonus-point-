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
using System.Data.Objects;

namespace POS
{
    public partial class VIP_Information : Form
    {
        #region Variables
        POSEntities entity = new POSEntities();
        List<Customer> cusList = new List<Customer>();
        List<Customer> _tempCusList = new List<Customer>();
        Boolean _status = false;
        #endregion
        #region Event
        public VIP_Information()
        {
            InitializeComponent();
        }

        private void VIP_Information_Load(object sender, EventArgs e)
        {
            //rdbAll.Checked = true;
            //gbYesNo.Enabled = false;
            //rdbYes.Checked = true;
           // gbPeriod.Enabled = false;
            cboPreferContact.SelectedIndex = 0;


            List<APP_Data.Shop> shoplist = new List<APP_Data.Shop>();
            APP_Data.Shop shopobj = new APP_Data.Shop();
            shopobj.ShortCode = "0";
            shopobj.ShopName = "Select";
            shoplist.Add(shopobj);
            shoplist.AddRange(entity.Shops.Where(x => x.ShortCode != "-").GroupBy(x => x.ShortCode).Select(x => x.FirstOrDefault()).ToList());
            vipstartedin.DataSource = shoplist;
            vipstartedin.DisplayMember = "ShopName";
            vipstartedin.ValueMember = "ShortCode";
            City_BInd();
            _status = true;
            First_Load();
            LoadData();
            this.reportViewer1.RefreshReport();
        }

        private void cboCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }


        private void rdbAll_CheckedChanged(object sender, EventArgs e)
        {
            gbPeriod.Enabled = false;
            gbYesNo.Enabled = false;
            gpselectshop.Visible = false;
            LoadData();
            
        }

        private void rdbVIP_CheckedChanged(object sender, EventArgs e)
        {
            rdbYes.Checked = false;
            rdbNo.Checked = true;
            gbYesNo.Enabled = true;
            gbPeriod.Enabled = false;
            gpselectshop.Visible = true;
            LoadData();
        }

        private void rdbNonVIP_CheckedChanged(object sender, EventArgs e)
        {
            gbPeriod.Enabled = false;
            gbYesNo.Enabled = false;
            gpselectshop.Visible = false;
            LoadData();
        }

       

        private void rdbYes_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbYes.Checked)
            {
                gbPeriod.Enabled = true;
                LoadData();
            }
        }

        private void rdbNo_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbNo.Checked)
            {
                gbPeriod.Enabled = false;
                LoadData();
            }
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Function
    
        private void City_BInd()
        {
            List<APP_Data.City> cityList = new List<APP_Data.City>();
            APP_Data.City city1 = new APP_Data.City();
            city1.Id = 0;
            city1.CityName = "ALL";
            cityList.Add(city1);
            cityList.AddRange(entity.Cities.ToList());
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";
        }

        private void First_Load()
        {
            _tempCusList = entity.Customers.ToList();
            Bonus_Point();
        }

        private void LoadData()
        {
            if (_status == true)
            {
                int cityId = 0;
                if (cboCity.SelectedIndex > 0)
                {
                    cityId = Convert.ToInt32(cboCity.SelectedValue);
                }
                string selectshop = vipstartedin.SelectedValue.ToString();
              //  DateTime fromDate = dtFrom.Value.Date;
                //DateTime toDate = dtTo.Value.Date;
                 //DateTime fromDate = new DateTime();
                // DateTime toDate = new DateTime();
                DateTime fromDate = dtFrom.Value.Date;
                DateTime toDate = dtTo.Value.Date;
                bool IsPeriodYes = rdbYes.Checked;
                string _preferContact=cboPreferContact.Text;

                if (rdbAll.Checked)
                {
                    cusList = _tempCusList.Where(x => ((cityId == 0 && 1 == 1) || (cityId != 0 && x.CityId == cityId)) && ((_preferContact == "ALL" && 1 == 1) || (_preferContact != "ALL" && x.PreferContact == _preferContact))).ToList();
                 
                }
                else if (rdbNonVIP.Checked)
                {
                 //   cusList = entity.Customers.Where(x => (x.CustomerTypeId) == 2 && ((cityId == 0 && 1 == 1) || (cityId != 0 && x.CityId == cityId)) && ((_preferContact=="ALL" && 1==1) || (_preferContact!="ALL" && x.PreferContact==_preferContact))).ToList();
                    cusList = _tempCusList.Where(x => (x.CustomerTypeId) == 2 && ((cityId == 0 && 1 == 1) || (cityId != 0 && x.CityId == cityId)) && ((_preferContact == "ALL" && 1 == 1) || (_preferContact != "ALL" && x.PreferContact == _preferContact))).ToList();
                }
                else if (rdbVIP.Checked)
                {
                    if (IsPeriodYes)
                    {
                      //  cusList = entity.Customers.Where(x => (x.CustomerTypeId) == 1 && ((cityId == 0 && 1 == 1) || (cityId != 0 && x.CityId == cityId)) && ((_preferContact == "ALL" && 1 == 1) || (_preferContact != "ALL" && x.PreferContact == _preferContact))).ToList().Where(x => x.PromoteDate >= fromDate && x.PromoteDate <= toDate).ToList();
                        //cusList = _tempCusList.Where(x => (x.CustomerTypeId) == 1 && ((cityId == 0 && 1 == 1) || (cityId != 0 && x.CityId == cityId)) && ((_preferContact == "ALL" && 1 == 1) || (_preferContact != "ALL" && x.PreferContact == _preferContact))).ToList().Where(x => EntityFunctions.TruncateTime(x.PromoteDate.Value) >= fromDate.Date && EntityFunctions.TruncateTime(x.PromoteDate.Value) <= toDate.Date).ToList();
                        cusList = (from x in entity.Customers
                                   join t in entity.Transactions on x.Id equals t.CustomerId into q
                                   from xq in q.DefaultIfEmpty()

                                   where x.CustomerTypeId == 1 && ((cityId == 0 && 1 == 1) || (cityId != 0 && x.CityId == cityId)) &&
                                       ((_preferContact == "ALL" && 1 == 1) || (_preferContact != "ALL" && x.PreferContact == _preferContact))
                                       && (EntityFunctions.TruncateTime(x.PromoteDate.Value) >= fromDate && EntityFunctions.TruncateTime(x.PromoteDate.Value) <= toDate) && ((selectshop == "0" && 1 == 1) || (selectshop != "0" && x.VipStartedShop == selectshop))
                                        
                                    select x).Distinct().ToList();
                    }
                    else
                    {
                      //  cusList = entity.Customers.Where(x => (x.CustomerTypeId) == 1 && ((cityId == 0 && 1 == 1) || (cityId != 0 && x.CityId == cityId)) && ((_preferContact == "ALL" && 1 == 1) || (_preferContact != "ALL" && x.PreferContact == _preferContact))).ToList();
                        cusList = _tempCusList.Where(x => (x.CustomerTypeId) == 1 && ((selectshop == "0" && 1 == 1) || (selectshop != "0" && x.VipStartedShop == selectshop)) && ((cityId == 0 && 1 == 1) || (cityId != 0 && x.CityId == cityId)) && ((_preferContact == "ALL" && 1 == 1) || (_preferContact != "ALL" && x.PreferContact == _preferContact))).ToList();
                    }
                }
             
             
                ShowReportViewer();
            }
            

        }

        private void Bonus_Point()
        {
            foreach (var c in _tempCusList)
            {
                c.BonusPoint = Loc_CustomerPointSystem.GetPointFromCustomerId(c.Id).ToString();
                Application.DoEvents();
                Cursor.Current = Cursors.WaitCursor;
            }
        }

        private void ShowReportViewer()
        {

            //dsReportTemp dsReport = new dsReportTemp();            
            //dsReportTemp.CustomerInformationDataTable dtCustomerReport = (dsReportTemp.CustomerInformationDataTable)dsReport.Tables["CustomerInformation"];
            //foreach (Customer c in cusList)
            //{
            //    dsReportTemp.CustomerInformationRow newRow = dtCustomerReport.NewCustomerInformationRow();
            //    newRow.Name = c.Name + " " + c.Title;
            //    newRow.Birthday = (c.Birthday == null) ? "-" : c.Birthday.Value.Date.ToString("dd/MM/yyyy");
            //    newRow.Gender = (c.Gender == null) ? "-" : c.Gender;
            //    newRow.NRC = (c.NRC == null) ? "-" : c.NRC;
            //    newRow.PhNo = (c.PhoneNumber == null) ? "-" : c.PhoneNumber;
            //    newRow.Email = (c.Email == null) ? "-" : c.Email;
            //    newRow.Address = (c.Address == null || c.Address == "") ? "-" : c.Address;
            //    //newRow.PromoteDate = (c.PromoteDate == null) ? "-" : c.PromoteDate.ToString();
            //    newRow.PromoteDate = Convert.ToDateTime(c.PromoteDate);
              // newRow.BonusPoint = Loc_CustomerPointSystem.GetPointFromCustomerId(c.Id).ToString();
            //    dtCustomerReport.AddCustomerInformationRow(newRow);
            //}
           // dtCustomerReport = cusList as dsReportTemp.CustomerInformationDataTable();
          //  ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["CustomerInformation"]);


          
            ReportDataSource rds = new ReportDataSource();
            rds.Name = "DataSet1";
            rds.Value = cusList;
            
           // ReportDataSource rds = new ReportDataSource("DataSet1", cusList);
            string reportPath = string.Empty;
            reportPath = Application.StartupPath + "\\Reports\\AllMemberInformation.rdlc";
            string _customerType = "";
            if (rdbAll.Checked)
            {
            //    reportPath = Application.StartupPath + "\\Reports\\AllMemberInformation.rdlc";
                _customerType = "All Customer Information  ";
            }
            else if (rdbVIP.Checked)
            {
               // reportPath = Application.StartupPath + "\\Reports\\VIPMenberInformation.rdlc";
                _customerType = "VIP Customer Information  ";
            }
            else if (rdbNonVIP.Checked)
            {
               // reportPath = Application.StartupPath + "\\Reports\\AllMemberInformation.rdlc";
                _customerType = "Non VIP Customer Information  ";
            }
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter CustomerType = new ReportParameter("CustomerType", _customerType);
            reportViewer1.LocalReport.SetParameters(CustomerType);

            ReportParameter PreferContact = new ReportParameter("PreferContact", cboPreferContact.Text);
            reportViewer1.LocalReport.SetParameters(PreferContact);

            //ReportParameter Date = new ReportParameter("Date", " from " + dtFrom.Value.Date.ToString("dd/MM/yyyy") + " To " + dtTo.Value.Date.ToString("dd/MM/yyyy"));
            //reportViewer1.LocalReport.SetParameters(Date);

            reportViewer1.RefreshReport();
        }
        #endregion

        private void cboCounter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void vipstartedin_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
