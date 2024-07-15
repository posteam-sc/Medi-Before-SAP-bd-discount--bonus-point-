using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;

namespace POS
{
    public partial class CustomerDetail : Form
    {
        #region Variables

        POSEntities entity = new POSEntities();
        public int customerId;

        #endregion

        #region Events

        public CustomerDetail()
        {
            InitializeComponent();
        }

        private void CustomerDetail_Load(object sender, EventArgs e)
        {
            Customer cust = (from c in entity.Customers where c.Id == customerId select c).FirstOrDefault<Customer>();

            lblName.Text = cust.Title + " " + cust.Name;
            lblPhoneNumber.Text = cust.PhoneNumber;
            lblNrc.Text = cust.NRC;
            lblAddress.Text = cust.Address;
            lblEmail.Text = cust.Email;
            lblGender.Text = cust.Gender;
          
            lblBirthday.Text = cust.Birthday != null ? Convert.ToDateTime(cust.Birthday).ToString("dd-MM-yyyy") : "-";
            lblCity.Text = cust.City != null ? cust.City.CityName : "-";
            dgvNormalTransaction.AutoGenerateColumns = false;
            dgvVIPTransaction.AutoGenerateColumns = false;
            List<Transaction> transList = cust.Transactions.Where(trans => (trans.IsDeleted == false && trans.IsComplete==false)).ToList();
            List<Transaction> DataBindTransList = new List<Transaction>();
            //Need to get date range for VIP
            dgvVIPTransaction.DataSource = transList;          

            dgvNormalTransaction.DataSource = cust.Transactions.Where(trans => (trans.IsDeleted == null || trans.IsDeleted == false)).ToList();
        }

        private void dgvVIPTransaction_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvVIPTransaction.Rows)
            {
                Transaction ts = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = ts.Id;
                row.Cells[1].Value = ts.PaymentType.Name;
                row.Cells[2].Value = ts.DateTime.ToString("dd-MM-yyyy");
                row.Cells[3].Value = ts.DateTime.ToString("hh:mm");
                row.Cells[4].Value = ts.User.Name;                
                row.Cells[5].Value = ts.TotalAmount;
                
            }
        }

        private void dgvOldTransaction_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvNormalTransaction.Rows)
            {
                Transaction ts = (Transaction)row.DataBoundItem;
                row.Cells[0].Value = ts.Id;
                row.Cells[1].Value = ts.PaymentType.Name;
                row.Cells[2].Value = ts.DateTime.ToString("dd-MM-yyyy");
                row.Cells[3].Value = ts.DateTime.ToString("hh:mm");
                row.Cells[4].Value = ts.User.Name;
                row.Cells[5].Value = ts.TotalAmount;

            }
        }

        #endregion

    }
}
