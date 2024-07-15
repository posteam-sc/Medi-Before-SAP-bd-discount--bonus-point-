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
    public partial class PaidByFOC : Form
    {
        #region Variables

        public List<TransactionDetail> DetailList = new List<TransactionDetail>();

        public List<GiftSystem> GiftList = new List<GiftSystem>();

        public int Discount { get; set; }

        public int Tax { get; set; }

        public int ExtraDiscount { get; set; }

        public int ExtraTax { get; set; }

        public Boolean isDraft { get; set; }        

        public string DraftId { get; set; }

        public int? CustomerId { get; set; }
        
        private POSEntities entity = new POSEntities();

        public int Type {get; set;}

        #endregion
        public PaidByFOC()
        {
            InitializeComponent();
        }

        private void PaidByFOC_Load(object sender, EventArgs e)
        {
            long totalCost = (long)DetailList.Sum(x => x.TotalAmount) + ExtraTax - ExtraDiscount;
            System.Data.Objects.ObjectResult<String> Id = entity.InsertTransaction(DateTime.Now, MemberShip.UserId, MemberShip.CounterId, TransactionType.Sale, true, true, 4, ExtraTax + Tax, ExtraDiscount + Discount, totalCost, 0, null, CustomerId, SettingController.DefaultShop.ShortCode, SettingController.DefaultShop.Id);
            
            string resultId = Id.FirstOrDefault().ToString();
            Transaction insertedTransaction = (from trans in entity.Transactions where trans.Id == resultId select trans).FirstOrDefault<Transaction>();
            foreach (TransactionDetail detail in DetailList)
            {
                detail.Product = (from prod in entity.Products where prod.Id == (long)detail.ProductId select prod).FirstOrDefault();
                detail.Product.Qty = detail.Product.Qty - detail.Qty;
                insertedTransaction.TransactionDetails.Add(detail);
            }
            if (isDraft)
            {
                Transaction draft = (from trans in entity.Transactions where trans.Id == DraftId select trans).FirstOrDefault<Transaction>();
                if (draft != null)
                {
                    draft.TransactionDetails.Clear();
                    var Detail = entity.TransactionDetails.Where(d => d.TransactionId == draft.Id);
                    foreach (var d in Detail)
                    {
                        entity.TransactionDetails.Remove(d);
                    }
                    entity.Transactions.Remove(draft);
                }
            }

            //Add promotion gift records for this transaction
            if (GiftList.Count > 0)
            {
                foreach (GiftSystem gs in GiftList)
                {
                    AttachGiftSystemForTransaction agft = new AttachGiftSystemForTransaction();
                    agft.AttachGiftSystemId = gs.Id;
                    agft.TransactionId = insertedTransaction.Id;
                    entity.AttachGiftSystemForTransactions.Add(agft);
                }
            }

            entity.SaveChanges();
            //Print Invoice
            #region [ Print ]

            dsReportTemp dsReport = new dsReportTemp();
            dsReportTemp.ItemListDataTable dtReport = (dsReportTemp.ItemListDataTable)dsReport.Tables["ItemList"];
            foreach (TransactionDetail transaction in DetailList)
            {
                dsReportTemp.ItemListRow newRow = dtReport.NewItemListRow();
                newRow.Name = transaction.Product.Name;
                newRow.Qty = transaction.Qty.ToString();
                newRow.TotalAmount = (int)transaction.TotalAmount;
                dtReport.AddItemListRow(newRow);
            }

            string reportPath = "";
            ReportViewer rv = new ReportViewer();
            ReportDataSource rds = new ReportDataSource("DataSet1", dsReport.Tables["ItemList"]);
            reportPath = Application.StartupPath + "\\Reports\\InvoiceFOC.rdlc";
            rv.Reset();
            rv.LocalReport.ReportPath = reportPath;
            rv.LocalReport.DataSources.Add(rds);


            ReportParameter ShopName = new ReportParameter("ShopName", SettingController.DefaultShop.ShopName);
            rv.LocalReport.SetParameters(ShopName);
           

            ReportParameter BranchName = new ReportParameter("BranchName", SettingController.DefaultShop.Address);
            rv.LocalReport.SetParameters(BranchName);
            


            ReportParameter Phone = new ReportParameter("Phone", SettingController.DefaultShop.PhoneNumber);
            rv.LocalReport.SetParameters(Phone);

            ReportParameter OpeningHours = new ReportParameter("OpeningHours", SettingController.DefaultShop.OpeningHours);
            rv.LocalReport.SetParameters(OpeningHours);

            ReportParameter TransactionId = new ReportParameter("TransactionId", resultId.ToString());
            rv.LocalReport.SetParameters(TransactionId);

            APP_Data.Counter c = entity.Counters.FirstOrDefault(x => x.Id == MemberShip.CounterId);

            ReportParameter CounterName = new ReportParameter("CounterName", c.Name);
            rv.LocalReport.SetParameters(CounterName);

            ReportParameter PrintDateTime = new ReportParameter("PrintDateTime", DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
            rv.LocalReport.SetParameters(PrintDateTime);

            ReportParameter CasherName = new ReportParameter("CasherName", MemberShip.UserName);
            rv.LocalReport.SetParameters(CasherName);

            ReportParameter TotalAmount = new ReportParameter("TotalAmount", insertedTransaction.TotalAmount.ToString());
            rv.LocalReport.SetParameters(TotalAmount);

            ReportParameter TaxAmount = new ReportParameter("TaxAmount", insertedTransaction.TaxAmount.ToString());
            rv.LocalReport.SetParameters(TaxAmount);

            ReportParameter DiscountAmount = new ReportParameter("DiscountAmount", insertedTransaction.DiscountAmount.ToString());
            rv.LocalReport.SetParameters(DiscountAmount);

            ReportParameter PaidAmount = new ReportParameter("PaidAmount", "0");
            rv.LocalReport.SetParameters(PaidAmount);

            ReportParameter Change = new ReportParameter("Change", "0");
            rv.LocalReport.SetParameters(Change);


            PrintDoc.PrintReport(rv, "Slip");
            #endregion


            //MessageBox.Show("Payment Completed");
            
                if (System.Windows.Forms.Application.OpenForms["Sales"] != null)
                {
                    Sales newForm = (Sales)System.Windows.Forms.Application.OpenForms["Sales"];
                    newForm.Clear();
                }
            
            this.Dispose();
        }
    }
}
