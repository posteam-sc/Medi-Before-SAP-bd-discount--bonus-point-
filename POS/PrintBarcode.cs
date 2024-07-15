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
    public partial class PrintBarcode : Form
    {
        #region Variable

        public int productId;
        POSEntities entity = new POSEntities();
        Product product = new Product();
        #endregion

        #region Event

        public PrintBarcode()
        {
            InitializeComponent();
        }

        private void PrintBarcode_Load(object sender, EventArgs e)
        {
            product = (from p in entity.Products where p.Id == productId select p).FirstOrDefault();
            lblBarCode.Text = product.Barcode.ToString();
            lblId.Text = product.Barcode.ToString();
            lblItemName.Text = product.Name;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region [ Print ]

            string reportPath =string.Empty;
            ReportViewer reportViewer = new ReportViewer();
            reportPath = Application.StartupPath + "\\Reports\\Sticker.rdlc";
            reportViewer.Reset();
            reportViewer.LocalReport.ReportPath = reportPath;

            ReportParameter ProductName = new ReportParameter("ProductName", product.Name);
            reportViewer.LocalReport.SetParameters(ProductName);

            ReportParameter ProductId = new ReportParameter("ProductId", product.Id.ToString());
            reportViewer.LocalReport.SetParameters(ProductId);

            ReportParameter ProductIdBarcode = new ReportParameter("ProductIdBarcode", product.Id.ToString());
            reportViewer.LocalReport.SetParameters(ProductIdBarcode);

            PrintDoc.PrintReport(reportViewer, "BarcodeSticker");
            #endregion
        }
        #endregion      
    }
}
