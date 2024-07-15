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
    public partial class OutstandingCustomerReport : Form
    {
        #region Variable
        POSEntities entity = new POSEntities();
        CustomerInfoHolder[] cInfoList;
        CustomerInfoHolder[] FinalResult;

        int count = 0;

        List<Customer> cusList = new List<Customer>();
        List<Customer> customerList;
        #endregion
        public OutstandingCustomerReport()
        {
            InitializeComponent();
        }

        private void OutstandingCustomerReport_Load(object sender, EventArgs e)
        {

           // var _customer = entity.Customers.Select(c => c.Name).ToArray();

            //auto complete textbox
            ////txtName.AutoCompleteMode = AutoCompleteMode.Suggest;
            ////txtName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            ////AutoCompleteStringCollection DataCollection = new AutoCompleteStringCollection();
            ////DataCollection.AddRange(_customer);
            ////txtName.AutoCompleteCustomSource = DataCollection;
            Bind_Customer();


            this.reportViewer1.RefreshReport();
            LoadData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cboName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }


        #region Function

        private void LoadData()
        {
          
            if (cboName.SelectedIndex == 0)
            {
           // customerList = (from c in entity.Customers select c).ToList();
                customerList = (from c in cusList
                                join t in entity.Transactions on c.Id equals t.CustomerId
                                where t.Type == "Credit" || t.Type == "CreditRefund" || t.Type == "Prepaid"
                                select c).Distinct().ToList();
            // customerList=cusList;
            }
            else
            {
                int custID = Convert.ToInt32(cboName.SelectedValue);
            // customerList = (from c in entity.Customers where c.Id == custID select c).ToList();
                //customerList = (from c in entity.Customers where c.Name == name
                //                join t in entity.Transactions on c.Id equals t.CustomerId
                //                where t.Type == "Credit" || t.Type == "CreditRefund" || t.Type == "Prepaid"
                //                select c).ToList();

                customerList = (from c in cusList
                                join t in entity.Transactions on c.Id equals t.CustomerId
                                where (t.Type == "Credit" || t.Type == "CreditRefund" || t.Type == "Prepaid") && (c.Id == custID) select c).Distinct().ToList();
            }
            cInfoList = new CustomerInfoHolder[customerList.Count];
            int index = 0; int outer, inner;
            foreach (Customer c in customerList)
            {
                int totalDebt = 0, totalPrepaid = 0;
                cInfoList[index] = new CustomerInfoHolder();
                cInfoList[index].Id = c.Id;
                cInfoList[index].Title = c.Title;
                cInfoList[index].Name = c.Name;
                cInfoList[index].PhNo = c.PhoneNumber;
                cInfoList[index].Address = c.Address;
                List<Transaction> rtList = new List<Transaction>();

            
                foreach (Transaction tf in c.Transactions)
                {
                    if (tf.IsPaid == false && tf.IsDeleted==false)
                    {
                        totalDebt += (int)((tf.TotalAmount) - tf.RecieveAmount);
                        rtList = (from rt in entity.Transactions where rt.Type == TransactionType.CreditRefund && rt.ParentId == tf.Id select rt).ToList();

                        if (rtList.Count > 0)
                        {
                            foreach (Transaction rt in rtList)
                            {
                                totalDebt -= (int)rt.RecieveAmount;
                            }
                        }

                        totalDebt -= Convert.ToInt32(tf.UsePrePaidDebts.Sum(x => x.UseAmount).Value);

                    }

                    if (tf.Type == TransactionType.Prepaid && tf.IsActive == false)
                    {
                        totalPrepaid += (int)tf.RecieveAmount;
                        int useAmount = 0;
                        if (tf.UsePrePaidDebts1 != null)
                        {
                            foreach (UsePrePaidDebt useObj in tf.UsePrePaidDebts1)
                            {
                                useAmount += (int)useObj.UseAmount;
                            }
                        }
                        totalPrepaid -= useAmount;
                    }
                    
                    //totalDebt += (int)((tf.TotalAmount ) - tf.RecieveAmount);
                    totalDebt -= totalPrepaid;
                }

                //totalDebt -= totalPrepaid;
                cInfoList[index].OutstandingAmount = totalDebt;
                index++;
            }
            for (outer = index - 1; outer >= 1; outer--)
            {
                for (inner = 0; inner < outer; inner++)
                {
                    if (cInfoList[inner].OutstandingAmount < cInfoList[inner + 1].OutstandingAmount)
                    {
                        CustomerInfoHolder temp = cInfoList[inner];
                        cInfoList[inner] = cInfoList[inner + 1];
                        cInfoList[inner + 1] = temp;
                    }
                }
            }

            //FinalResult = new CustomerInfoHolder[cInfoList.Length];
            //for (int i = 0; i < cInfoList.Length; i++)
            //{
            //    if (cInfoList[i].OutstandingAmount > 0)
            //    {
            //        FinalResult[count] = cInfoList[i];
            //        count++;
            //    }
            //}
            ShowReportViewer();
        }

        private void Bind_Customer()
        {
            
            APP_Data.Customer cust = new APP_Data.Customer();
            cust.Id = 0;
            cust.Name = "All";
            cusList.Add(cust);
            cusList.AddRange(entity.Customers.OrderBy(x=>x.Name).ToList());
            
            cboName.DataSource = cusList;
            cboName.DisplayMember = "Name";
            cboName.ValueMember = "Id";
           // cboName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cboName.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void ShowReportViewer()
        {
            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.OutstandingCustomerDataTable dtOutstandingCusReport = (dsReportTemp.OutstandingCustomerDataTable)dsReport.Tables["OutstandingCustomer"];
            for (int index = 0; index < cInfoList.Length; index++)
            {
                if (cInfoList[index].OutstandingAmount != 0)
                {
                    dsReportTemp.OutstandingCustomerRow newRow = dtOutstandingCusReport.NewOutstandingCustomerRow();
                    CustomerInfoHolder c = cInfoList[index];
                    newRow.CustomerId = c.Id.ToString();
                    newRow.Name = c.Name + " " + c.Title;
                    newRow.PhoneNo = c.PhNo;
                    newRow.Address = c.Address;
                    newRow.OutstandingAmount = (int)c.OutstandingAmount;
                    dtOutstandingCusReport.AddOutstandingCustomerRow(newRow);
                }
            }

           // var _reportList = (from c in cInfoList where c.OutstandingAmount > 0 select c).ToList();
         

          ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["OutstandingCustomer"]);
         //   ReportDataSource rds = new ReportDataSource("DataSet1", _reportList);
            string reportPath = Application.StartupPath + "\\Reports\\OutstandingReport.rdlc";
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            ReportParameter HeadTitle = new ReportParameter("HeadTitle", "Customer Information and their outstanding amount for " + SettingController.DefaultShop.ShopName);
            reportViewer1.LocalReport.SetParameters(HeadTitle);

            reportViewer1.RefreshReport();

        }

        #endregion

        private void cboName_KeyPress(object sender, KeyPressEventArgs e)
        {
            cboName.DroppedDown = true;
        }

        private void cboName_KeyDown(object sender, KeyEventArgs e)
        {
            cboName.DroppedDown = true;
        }

     
        
    }
}
