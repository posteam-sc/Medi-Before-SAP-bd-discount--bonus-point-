using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;
using POS.POSInterfaceServiceReference;
using System.Data.SqlClient;
using System.Configuration;

namespace POS
{
    public partial class MDIParent : Form 
    {
        #region Events

        // By SYM
        DateTime fromTime;
        DateTime toTime;
        bool isRunning = false;

        private int childFormNumber = 0;

        #region Form Events

        public MDIParent()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {// By SYM
            string address = "https://jc-dc.junctionv-aap.com/wsSSI/wsSSIWebService.asmx";
            IsAddressAvailable(address);
            if (isRunning == true)
            {
                Push_DataToWebService();
            }

        }

        public void IsAddressAvailable(string address)
        {// By SYM
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                client.DownloadData(address);
                isRunning = true;
            }
            catch
            {
                isRunning = false;
            }

        }

        public void Push_DataToWebService()
        {// By SYM  

            if (SettingController.At_JunctionCity=="1")
            {
                toTime = DateTime.Now;
                Parameters _parameters = new Parameters();
                Transactions _transaction = new Transactions();
                List<Transactions> _transactionList = new List<Transactions>();

                SqlConnection _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);
                _conn.Open();
                SqlCommand _com = new SqlCommand("JC_POSInterface", _conn);
                _com.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromTime;
                _com.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toTime;
                _com.CommandType = CommandType.StoredProcedure;
                SqlDataReader dataReader = _com.ExecuteReader();
                DataTable dt = new DataTable("Transactions");
                fromTime = DateTime.Now;
                dt.Load(dataReader);

                _conn.Close();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        _transactionList.Add(new Transactions()
                        {
                            Col_01 = dt.Rows[i]["mallCode"].ToString(),
                            Col_02 = dt.Rows[i]["posID"].ToString(),
                            Col_03 = dt.Rows[i]["transactionDate"].ToString(),
                            Col_04 = dt.Rows[i]["transactionTime"].ToString(),
                            Col_05 = dt.Rows[i]["transactionNo"].ToString(),
                            Col_06 = dt.Rows[i]["itemQty"].ToString(),
                            Col_07 = dt.Rows[i]["saleAmtCurrency"].ToString(),
                            Col_08 = dt.Rows[i]["totAmtNoTax"].ToString(),
                            Col_09 = dt.Rows[i]["TotAmtWithTax"].ToString(),
                            Col_10 = dt.Rows[i]["tax"].ToString(),
                            Col_11 = dt.Rows[i]["serviceChargeAmt"].ToString(),
                            Col_12 = dt.Rows[i]["payAmtCurrency"].ToString(),
                            Col_13 = dt.Rows[i]["paymentAmt"].ToString(),
                            Col_14 = dt.Rows[i]["paymentType"].ToString(),
                            Col_15 = dt.Rows[i]["saleType"].ToString()+"s",
                            Col_16 = "",
                            Col_17 = "",
                            Col_18 = "",
                            Col_19 = "",
                            Col_20 = ""

                        });
                    }
                    _parameters.Application_ID = SettingController.Application_ID;
                    _parameters.Col_01 = SettingController.Mall_Code;
                    _parameters.Col_02 = SettingController.POS_ID;
                    _parameters.Col_03 = "";
                    string _todayDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    _parameters.TimeStamp = _todayDate.ToString();
                    _parameters.Columns = _transactionList.ToArray();


                    wsSSIAuthentication _wsSSIAuthentication = new wsSSIAuthentication();
                    _wsSSIAuthentication.applicationKey = SettingController.Application_Key;
                    _wsSSIAuthentication.encryptedKey = SettingController.Encrypted_Key;

                   
                    
                    POSInterfaceServiceReference.wsSSIWebServiceSoapClient soapClient = new POSInterfaceServiceReference.wsSSIWebServiceSoapClient();
                    POSInterfaceServiceReference.ws_SSI_SendDataRS rqResponse = soapClient.ws_SSI_SendDataRQ(_wsSSIAuthentication, _parameters);

                    string _transactionID = rqResponse.TransactionID;
                    string return_status = rqResponse.ReturnStatus;
                    int record_Received = rqResponse.RecordsReceived;
                    int record_Imported = rqResponse.RecordsImported;
                    string error_Detail = rqResponse.ErrorDetails;
                    string defective_RowNos = rqResponse.DefectiveRowNos;

                    //string _msg = return_status + '-' + record_Received;
                    //MessageBox.Show(_msg);


                }
            }

        }

        private void MDIParent_Load(object sender, EventArgs e)
        {
            if (!Utility.IsRegister())
            {
                Register regform = new Register();
                regform.WindowState = FormWindowState.Maximized;
                regform.MdiParent = this;
                regform.Show();
                this.menuStrip.Enabled = false;
            }
            else if (!MemberShip.isLogin)
            {
                Login loginForm = new Login();
                loginForm.WindowState = FormWindowState.Maximized;
                loginForm.MdiParent = this;
                loginForm.Show();
                this.menuStrip.Enabled = false;
            }
            else
            {
                Sales form = new Sales();
                form.WindowState = FormWindowState.Maximized;
                form.MdiParent = this;
                form.Show();
            }
            timer1.Interval = 7200000;//4hour
            timer1.Tick += new System.EventHandler(timer1_Tick);
            timer1.Start();

            // By SYM // JC POS UI Interface

            if (SettingController.At_JunctionCity=="1")
            {
                Timer MyTimer = new Timer();
                MyTimer.Interval = (15 * 60 * 1000); // 15 mins
                MyTimer.Tick += new EventHandler(MyTimer_Tick);
                MyTimer.Start();
                fromTime = DateTime.Now;
            }
        }

        private void MDIParent_FormClosing(object sender, FormClosingEventArgs e)
        {
            toolStripStatusLabel.Text = "Saving data.. Please wait";

            // By SYM
            string address = "https://jc-dc.junctionv-aap.com/wsSSI/wsSSIWebService.asmx";
            IsAddressAvailable(address);
            if (isRunning == true)
            {
                Push_DataToWebService();
            }

            //Only main server will make backup
            if (DatabaseControlSetting._ServerName.ToUpper() == System.Environment.MachineName.ToUpper())
            {
                Backup(true);
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {

        }

        #endregion

        #region Menu Click Events

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void productTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductSubCategory newform = new ProductSubCategory();
            newform.ShowDialog();
        }

    

        private void customerListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OutstandingCustomerList form = new OutstandingCustomerList();
            form.ShowDialog();
        }

        private void productCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductCategory form = new ProductCategory();
            form.ShowDialog();
        }

        private void addNewCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewCustomer form = new NewCustomer();
            form.ShowDialog();
        }

        private void brandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Brand newForm = new Brand();
            newForm.ShowDialog();
        }

        private void counterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Counter newForm = new Counter();
            newForm.ShowDialog();
        }

        private void userListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserControl form = new UserControl();
            form.ShowDialog();
        }

        private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewUser form = new NewUser();
            form.ShowDialog();
        }
        private void transactionSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransactionSummary newform = new TransactionSummary();
            newform.ShowDialog();
        }

        private void newProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProduct newForm = new NewProduct();
            newForm.ShowDialog();
        }

        private void productListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemList newForm = new ItemList();
            newForm.ShowDialog();
        }

        private void giftCardControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GiftCardControl newForm = new GiftCardControl();
            newForm.ShowDialog();
        }

        private void transactionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransactionList newForm = new TransactionList();
            newForm.ShowDialog();
        }

        private void startDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartDay form = new StartDay();
            form.ShowDialog();
        }

        private void endDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EndDay form = new EndDay();
            form.ShowDialog();
        }

        private void refundListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefundList newform = new RefundList();
            newform.ShowDialog();
        }

        private void unitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnitForm newform = new UnitForm();
            newform.ShowDialog();
        }

        

        private void creditTransactionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreditTransactionList newForm = new CreditTransactionList();
            newForm.ShowDialog();
        }

        private void transactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransactionReport_FOC_MPU newform = new TransactionReport_FOC_MPU();
            newform.ShowDialog();
        }

        private void itemSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemSummary newform = new ItemSummary();
            newform.ShowDialog();
        }

        private void roleManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoleManagement newform = new RoleManagement();
            newform.ShowDialog();
        }

        private void taxesSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaxesSummary newform = new TaxesSummary();
            newform.ShowDialog();
        }

        private void addCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            City newForm = new City();
            newForm.ShowDialog();
        }

        private void deleteLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteLogForm newform = new DeleteLogForm();
            newform.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting newform = new Setting();
            newform.ShowDialog();
        }

        private void itemPurchaseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseOrderItem newform = new PurchaseOrderItem();
            newform.ShowDialog();
        }

        private void transactionDetailByItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransactionDetailByItem newform = new TransactionDetailByItem();
            newform.ShowDialog();
        }

        private void taxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Taxes newform = new Taxes();
            newform.ShowDialog();
        }

        private void outstandingCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AverageMonthlySaleReport_frm newform = new AverageMonthlySaleReport_frm();
            newform.ShowDialog();
        }

        private void addNewProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProduct newForm = new NewProduct();
            newForm.ShowDialog();
        }

        private void productCategoryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ProductCategory form = new ProductCategory();
            form.ShowDialog();
        }

        private void productSubCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductSubCategory newform = new ProductSubCategory();
            newform.ShowDialog();
        }

        private void brandToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Brand newForm = new Brand();
            newForm.ShowDialog();
        }

        private void productListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ItemList newForm = new ItemList();
            newForm.ShowDialog();
        }

        private void addNewUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewUser form = new NewUser();
            form.ShowDialog();
        }

        private void userListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UserControl form = new UserControl();
            form.ShowDialog();
        }

        private void roleManagementToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RoleManagement newform = new RoleManagement();
            newform.ShowDialog();
        }

        private void configurationSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting newform = new Setting();
            newform.ShowDialog();
        }

        private void counterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Counter newForm = new Counter();
            newForm.ShowDialog();
        }

        private void giftCardContToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Loc_GiftCardRegister newForm = new Loc_GiftCardRegister();
            newForm.ShowDialog();
        }

        private void measurementUnitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnitForm newform = new UnitForm();
            newform.ShowDialog();
        }

        private void taxRatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Taxes newform = new Taxes();
            newform.ShowDialog();
        }

        private void transactionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TransactionList newForm = new TransactionList();
            newForm.ShowDialog();
        }

        private void creditTransactionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CreditTransactionList newForm = new CreditTransactionList();
            newForm.ShowDialog();
        }

        private void refundListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RefundList newform = new RefundList();
            newform.ShowDialog();
        }

      

        private void form1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.ShowDialog();
        }

        private void addSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewSupplier newForm = new NewSupplier();
            newForm.ShowDialog();
        }

        private void supplierListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierList newForm = new SupplierList();
            newForm.ShowDialog();
        }

        private void productPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseInput newForm = new PurchaseInput();
            newForm.ShowDialog();
        }

        private void newPurchaseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseInput newForm = new PurchaseInput();
            newForm.ShowDialog();
        }

        private void purchaseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        

        

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopSaleReport newForm = new TopSaleReport();
            newForm.ShowDialog();
        }

        private void promotionSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PromotionSystem newForm = new PromotionSystem();
            newForm.ShowDialog();
        }

        private void promotionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Promotion_List newForm = new Promotion_List();
            newForm.ShowDialog();
        }

        private void customerListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Loc_CustomerList newForm = new Loc_CustomerList();
            newForm.ShowDialog();
        }

        private void changeCurrencyExchangeRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExchangeRate newForm = new ExchangeRate();
            newForm.ShowDialog();
        }

        #endregion

        #region Database Export Import

        private void databaseExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Backup(true);
        }

        private void databaseImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = Backup(false);
            Restore(ref fileName);
        }

        #endregion

        #region Login/Logout

        private void logInToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Boolean isAlreadyHave = false;

            foreach (Form child in this.MdiChildren)
            {
                if (child.Text == "Login")
                {
                    child.Activate();
                    isAlreadyHave = true;
                }
                else
                {
                    child.Close();
                }
            }
            if (!isAlreadyHave)
            {
                Login f = new Login();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                Boolean isAlreadyHave = false;

                foreach (Form child in this.MdiChildren)
                {
                    if (child.Text == "LogIn")
                    {
                        child.Activate();
                        isAlreadyHave = true;
                    }
                    else
                    {
                        child.Close();
                    }
                }
                MemberShip.UserId = 0;
                MemberShip.UserName = "";
                MemberShip.UserRole = null;
                //MemberShip.isAdmin = false;
                //MemberShip.isLogin = false;

                if (!isAlreadyHave)
                {
                    Login f = new Login();
                    f.MdiParent = this;
                    f.WindowState = FormWindowState.Maximized;
                    f.Show();

                    //DisableControls();
                }
                this.menuStrip.Enabled = false;
                toolStripStatusLabel.Text = string.Empty;
                logOutToolStripMenuItem.Visible = false;
                logInToolStripMenuItem1.Visible = true;
            }
        }

        #endregion

        #endregion

        #region Functions

        private void Restore(ref string fileName)
        {
            if (MessageBox.Show("Are you sure that you want to restore", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (ofdDBBackup.ShowDialog(this) == DialogResult.Cancel)
                {
                    return;
                }

                POSEntities entity = new POSEntities();
                entity.ClearDBConnections();

                fileName = ofdDBBackup.FileName;
                string destFileName = string.Empty;
                string[] fnArr1 = fileName.Split('_');
                string[] fnArr2 = fileName.Split('.');
                string[] fnArr3 = fileName.Split('\\');
                string filePath = string.Empty;
                for (int i = 0; i < fnArr3.Length - 1; i++)
                {
                    if (i + 1 != fnArr3.Length - 1)
                    {
                        filePath += fnArr3[i] + "/";
                    }
                    else
                    {
                        filePath += fnArr3[i];
                    }
                }

                /*--Decrypt DB--*/
                for (int i = 0; i < fnArr1.Length - 1; i++)
                {
                    if (i + 1 != fnArr1.Length - 1)
                    {
                        destFileName += fnArr1[i] + "_";
                    }
                    else
                    {
                        destFileName += fnArr1[i];
                    }
                }
                destFileName = destFileName + "." + fnArr2[1];
                if (File.Exists(destFileName)) File.Delete(destFileName);

                Utility.DecryptFile(fileName, destFileName);

                /*--Restore DB--*/
                RestoreHelper restoreHelper = new RestoreHelper();

                string[] tempConString = Properties.Settings.Default.MyConnectionString.Split(';');
                string[] userNameArr = tempConString[tempConString.Length - 2].Split('=');
                string[] passwordArr = tempConString[tempConString.Length - 1].Split('=');

                //restoreHelper.RestoreDatabase(Utility._DBName, destFileName + "." + fnArr2[1], Utility._ServerName, userNameArr[userNameArr.Length - 1], passwordArr[passwordArr.Length - 1]);
                restoreHelper.RestoreDatabase(DatabaseControlSetting._DBName, destFileName, DatabaseControlSetting._ServerName, DatabaseControlSetting._DBUser, DatabaseControlSetting._DBPassword);
                try
                {
                    if (File.Exists(destFileName))
                    {
                        File.Delete(destFileName);
                    }
                    MessageBox.Show("Successfully Restored..");
                }
                catch
                {
                    MessageBox.Show("Can't remove temp files", "Error!!");
                }
            }
        }

        private string Backup(Boolean IsManual)
        {
            string activeDir = @"d:\";

            /*-- Create a new subfolder under the current active folder --*/
            string newPath;
            if (IsManual)
            {
                newPath = System.IO.Path.Combine(activeDir, "Manual_Backups");
            }
            else
            {
                newPath = System.IO.Path.Combine(activeDir, "DB_Backups");
            }

            if (!System.IO.Directory.Exists(newPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(newPath);
                //di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            /*-- Backup DB --*/
            BackupHelper backHelper = new BackupHelper();
            string fileName;

            if (IsManual)
                fileName = "D:/Manual_Backups/" + DatabaseControlSetting._DBName + "[" + DateTime.Now.ToString("dd-MM-yyyy hh-mm tt") + "].bak";
            else
                fileName = "D:/DB_Backups/" + DatabaseControlSetting._DBName + "[" + DateTime.Now.ToString("dd-MM-yyyy hh-mm tt") + "].bak";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            bool isBackup = false;
            backHelper.BackupDatabase(DatabaseControlSetting._DBName, DatabaseControlSetting._DBUser, DatabaseControlSetting._DBPassword, DatabaseControlSetting._ServerName, fileName, ref isBackup);

            /*-- Encrypt DB --*/
            string[] fileNameArr = fileName.Split('\\');
            string[] encryptFileNameArr = fileName.Split('.');
            string tempFileName = encryptFileNameArr[0] + "_encrypted." + encryptFileNameArr[1];

            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
            if (isBackup)
            {
                Utility.EncryptFile(fileName, tempFileName);
            }

            try
            {
                File.Delete(fileName);
                if (isBackup)
                {
                    MessageBox.Show("Successfully Exported to " + newPath);
                }
            }
            catch
            {
                MessageBox.Show("Can't remove temporary files");
            }
            return fileName;
        }

        #endregion                

        private void lOCItemSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Loc_ItemSummary newForm = new Loc_ItemSummary();
            newForm.ShowDialog();
            ////Loc_DailySummary newForm = new Loc_DailySummary();
            ////newForm.ShowDialog();
            //ItemSummary newForm = new ItemSummary();
            //newForm.ShowDialog();
        }

        private void noveltySystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NoveltySystem newForm = new NoveltySystem();
            newForm.ShowDialog();
        }

        private void noveltyListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Novelty_List newForm = new Novelty_List();
            newForm.ShowDialog();
        }

        private void lOCVIPReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VIP_Information newForm = new VIP_Information();
            newForm.ShowDialog();
        }

        private void lOCEmailCompilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmailCompilation newForm = new EmailCompilation();
            newForm.ShowDialog();
        }

        private void lOCVIPMemberWeeklyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VIP_Member_Weekly_Transaction newForm = new VIP_Member_Weekly_Transaction();
            newForm.ShowDialog();
        }

        private void lOCGWPTransactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NoveltiesSale newForm = new NoveltiesSale();
            newForm.ShowDialog();
        }

        private void lOCSalesRangeAndSegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaleByRangeAndSegment newForm = new SaleByRangeAndSegment();
            newForm.ShowDialog();
        }
       

        private void salesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaleBreakDown newForm = new SaleBreakDown();
            newForm.ShowDialog();
        }

        private void lOCSaleBreakDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
           SaleBreakDown newForm = new SaleBreakDown();
            newForm.ShowDialog();
        }

        

        private void promotionSystemToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PromotionSystem newForm = new PromotionSystem();
            newForm.ShowDialog();
        }

        private void promotionListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Promotion_List newForm = new Promotion_List();
            newForm.ShowDialog();
        }

        private void noveltySystemToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NoveltySystem newForm = new NoveltySystem();
            newForm.ShowDialog();
        }

        private void noveltyListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Novelty_List newForm = new Novelty_List();
            newForm.ShowDialog();
        }

        private void productReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductReprotFrm newForm = new ProductReprotFrm();
            newForm.ShowDialog();
        }


        private void saleToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if ((Application.OpenForms["Sales"] as Sales) != null)
            {
                //Form is already open
            }
            else
            {
                Sales form = new Sales();
                form.WindowState = FormWindowState.Maximized;
                form.MdiParent = this;
                form.Show();
            }
          
        }

        private void addVIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MemberRule newForm = new MemberRule();
            newForm.ShowDialog();
        }

        private void userManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("MPOS_20.3.2015.pdf");
            System.Diagnostics.Process.Start("MPOS.chm");
            
        }

        private void priceChangeHistoryListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PriceChangeHistoryList newForm = new PriceChangeHistoryList();
            newForm.ShowDialog();
        }

        private void centralizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Backup(false);
            Centralized newform = new Centralized();
            newform.ShowDialog();
        }

        private void averageMonthlySaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AverageMonthlySaleReport_frm newform = new AverageMonthlySaleReport_frm();
            newform.ShowDialog();
        }

        private void outstandingCustomerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OutstandingCustomerReport newform = new OutstandingCustomerReport();
            newform.ShowDialog();
        }

        private string BackupAuto()
        {
            string activeDir = @"d:\";

            /*-- Create a new subfolder under the current active folder --*/
            string newPath;

            newPath = System.IO.Path.Combine(activeDir, "Auto_Backups");

            if (!System.IO.Directory.Exists(newPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(newPath);
                //di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            /*-- Backup DB --*/
            BackupHelper backHelper = new BackupHelper();
            string fileName;
            fileName = "D:/Auto_Backups/" + DatabaseControlSetting._DBName + "[" + DateTime.Now.ToString("dd-MM-yyyy hh-mm tt") + "].bak";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            bool isBackup = false;
            backHelper.BackupDatabase(DatabaseControlSetting._DBName, DatabaseControlSetting._DBUser, DatabaseControlSetting._DBPassword, DatabaseControlSetting._ServerName, fileName, ref isBackup);

            /*-- Encrypt DB --*/
            string[] fileNameArr = fileName.Split('\\');
            string[] encryptFileNameArr = fileName.Split('.');
            string tempFileName = encryptFileNameArr[0] + "_encrypted." + encryptFileNameArr[1];

            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
            if (isBackup)
            {
                Utility.EncryptFile(fileName, tempFileName);
            }

            try
            {
                File.Delete(fileName);
            }
            catch
            {
                MessageBox.Show("Can't remove temporary files");
            }
            return fileName;
        }
      
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DatabaseControlSetting._ServerName.ToUpper() == System.Environment.MachineName.ToUpper())
            {
                BackupAuto();
            }
        }
        private void MDIParent_Activated(object sender, EventArgs e)
        {
            if (DatabaseControlSetting._ServerName.ToUpper() == System.Environment.MachineName.ToUpper())
            {
                timer1.Start();
            }

        }

        private void noveltiesSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNoveltiesSales nv = new frmNoveltiesSales();
            nv.ShowDialog();
        }

        private void uIInterfaceForJunctionCityToolStripMenuItem_Click(object sender, EventArgs e)
        {// By SYM
            LOC_POS_UI_Interface_For_JunctionCity _loc_UI_Interface = new LOC_POS_UI_Interface_For_JunctionCity();
            _loc_UI_Interface.ShowDialog();
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPointDeductionPercentage_History _PointDeduction = new frmPointDeductionPercentage_History();
            _PointDeduction.ShowDialog();
        }

        private void priceChangeWithExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPriceChangeWithExcel priceChangeForm = new frmPriceChangeWithExcel();
            priceChangeForm.ShowDialog();
        }
    }
}
