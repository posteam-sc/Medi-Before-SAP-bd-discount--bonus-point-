using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.IO.Compression;
using System.Drawing;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using System.IO;


using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading; // Required for this example
using POS.APP_Data;
using System.Text.RegularExpressions;
using System.Data.Objects;


namespace POS
{
    public partial class Centralized : Form
    {
        private POSEntities entity = new POSEntities();
        public Centralized()
        {
            InitializeComponent();
        }
        private void Centralized_Load(object sender, EventArgs e)
        {
            label1.Refresh();
        }
        #region Export

        private void btnExport_Click(object sender, EventArgs e)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter;
            DataSet dsProduct = new DataSet("Product");
            DataSet dsCustomer = new DataSet("Customer");
            DataSet dsTransaction = new DataSet("Transaction");
            DataSet dsTransactionDetail = new DataSet("TransactionDetail");
            DataSet dsBrand = new DataSet("Brand");
            DataSet dsLoc_PointRedeemHistory = new DataSet("Loc_PointRedeemHistory");
            DataSet dsCity = new DataSet("City");
            DataSet dsConsignmentCounter = new DataSet("ConsignmentCounter");
            DataSet dsCounter = new DataSet("Counter");
            DataSet dsProductCategory = new DataSet("ProductCategory");
            DataSet dsProudctSubCategory = new DataSet("ProductSubCategroy");
            DataSet dsUnit = new DataSet("Unit");
            DataSet dsShop = new DataSet("Shop");
            DataSet dsTax = new DataSet("Tax");
            DataSet dsDeletelog = new DataSet("DeleteLog");
            DataSet dsCurrency = new DataSet("Currency");
            DataSet dsExchangeRateForTransaction = new DataSet("ExchangeRateForTransaction");
            DataSet dsPaymentType = new DataSet("PaymentType");
            DataSet dsSetting = new DataSet("Setting");
            DataSet dsUserRole = new DataSet("UserRole");
            DataSet dsUser = new DataSet("User");
            DataSet dsGiftCard = new DataSet("GiftCard");
            DataSet dsGiftCardInTransaction = new DataSet("GiftCardInTransaction");
            DataSet dsRoleManagement = new DataSet("RoleManagement");
            DataSet dsGiftSystem = new DataSet("GiftSystem");
            DataSet dsAttachGiftSystemForTransaction = new DataSet("AttachGiftSystemForTransaction");
            DataSet dsUsePrePaidDebt = new DataSet("UsePrePaidDebt");
            DataSet dsLoc_CustomerPoint = new DataSet("Loc_CustomerPoint");
            DataSet dsNoveltySystem = new DataSet("NoveltySystem");
            DataSet dsProductInNovelty = new DataSet("ProductInNovelty");
            DataSet dsWrapperItem = new DataSet("WrapperItem");
            DataSet dsProductPriceChange = new DataSet("ProductPriceChange");
            DataSet dsSPDetail = new DataSet("SPDetail");
            DataSet dsVIPMemberRule = new DataSet("VIPMemberRule");
            DataSet dsPointDeductionPercentage_History = new DataSet("PointDeductionPercentage_History");
            DataSet dsTransactionPaymentDetail = new DataSet("TransactionPaymentDetails");
            DateTime today = DateTime.Today;
            DateTime TranDate = DateTime.Today.AddMonths(-2);


            string dtToday = today.ToString("yyyy-MM-dd");

            //string dtTranDate = "2024-04-10";
            string dtTranDate = TranDate.ToString("yyyy-MM-dd");


            connetionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            connection = new SqlConnection(connetionString);
            string sqlProduct = "select P.ID,ISNULL(P.Name,'') AS Name,ProductCode,BarCode,Price,ISNULL(Qty,0) AS Qty,ISNULL(BrandId,0) As BrandId,ISNULL(ProductLocation,'') As ProductLocation,ISNULL(P.ProductCategoryId,0) As ProductCategoryId,ISNULL(ProductSubCategoryId,0) As ProductSubCategoryId,ISNULL(UnitId,0) As UnitId,ISNULL(TaxId,0) As TaxId,ISNULL(MinStockQty,0) As MinStockQty,ISNULL(DiscountRate,0.00) As DiscountRate,ISNULL(IsWrapper,'false') As IsWrapper,ISNULL(IsConsignment,'false') As IsConsignment,ISNULL(IsDiscontinue,'false') As IsDiscontinue,ISNULL(IsPromotionProduct,'false') As IsPromotionProduct,ISNULL(IsNovelty,0) As IsNovelty,ISNULL(ConsignmentPrice,0) As ConsignmentPrice,ISNULL(ConsignmentCounterId,0) As ConsignmentCounterId,ISNULL(Size,'') As Size,ISNULL(PurchasePrice,0) As PurchasePrice,ISNULL(IsNotifyMinStock,'false') As IsNotifyMinStock,UpdateDate as UpdateDate, B.Name as BName, PC.Name as CName, ISNULL(PS.Name,'') as SCName, ISNuLL(T.Name, '') as TaxName from Product as P left join Brand as B on P.BrandId = B.Id left join ProductCategory as PC on P.ProductCategoryId = PC.Id left join ProductSubCategory as PS on P.ProductSubCategoryId = PS.Id left join Tax as T on T.Id = P.TaxId";
            string sqlCustomer = "select Cus.*, C.CityName from Customer as Cus left join City as C on Cus.CityId = C.Id where Cus.Id in ((select distinct CustomerId from [Transaction] ts where ts.[DateTime]>='" + dtTranDate + "' and ts.IsComplete=1) UNION (select distinct CustomerId from GiftCard) UNION (select distinct CustomerId from Loc_PointRedeemHistory pr where pr.[DateTime]>='" + dtTranDate + "'))";

            //string sqlCustomer = "select Cus.*, C.CityName from Customer as Cus left join City as C on Cus.CityId = C.Id";
            string sqlTransaction = "select T.ID,T.DateTime,T.UpdatedDate,T.UserId,T.CounterId,T.Type,T.IsPaid,T.IsComplete,T.IsActive,T.IsDeleted,T.Loc_IsCalculatePoint,T.PaymentTypeId,T.TaxAmount,T.DiscountAmount,T.TotalAmount,T.RecieveAmount,ISNULL(T.ParentId,'') As ParentID,T.GiftCardId,T.CustomerId,T.ReceivedCurrencyId,T.ShopId, C.Address, C.Email, C.PhoneNumber, C.CustomerCode, Co.Name as CounterName, U.Name as UName, S.ShopName as ShopName, S.Address as SAddress from [Transaction] as T inner join Customer as C on T.CustomerId = C.Id inner join Counter Co on Co.Id = T.CounterId inner join [User] as U on T.UserId = U.Id left join Shop as S on S.Id = T.ShopId  WHERE T.IsComplete=1 and cast(T.UpdatedDate as date) >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date) ";
            string sqlTransactionDetail = "select TD.*, P.ProductCode from TransactionDetail as TD inner join Product as P on TD.ProductId= P.Id inner join [Transaction] as t on t.Id=TD.TransactionId where t.IsComplete=1 and cast(t.UpdatedDate as date) >= Cast( '" + dtTranDate + "' as date) and cast(t.UpdatedDate as date) <= Cast('" + dtToday + "' as date)";
            string sqlBrand = "select * from Brand";
            string sqlLoc_PointRedeemHistory = "select PR.*, U.Name as UName, C.Address, C.Email, C.PhoneNumber, C.CustomerCode, Ct.Name as CounterName from Loc_PointRedeemHistory as PR inner join Customer as C on PR.CustomerId = C.Id inner join Counter as Ct on PR.CounterId = Ct.Id inner join [User] as U on PR.CasherId = U.Id where PR.DateTime>='" + dtTranDate + "'";
            //string sqlLoc_PointRedeemHistory = "select PR.*, U.Name as UName, C.Address, C.Email, C.PhoneNumber, C.CustomerCode, Ct.Name as CounterName from Loc_PointRedeemHistory as PR inner join Customer as C on PR.CustomerId = C.Id inner join Counter as Ct on PR.CounterId = Ct.Id inner join [User] as U on PR.CasherId = U.Id";
            string sqlCity = "Select * from City";
            string sqlConsignmentCounter = "Select * from ConsignmentCounter";
            string SqlCounter = "Select * from Counter";
            string SqlProductCategory = "Select * from ProductCategory";
            string SqlProductSubCategory = "Select PS.*, PC.Name as CName from ProductSubCategory as PS inner join ProductCategory as PC on PS.ProductCategoryId = PC.Id";
            string SqlUnit = "Select * from Unit";
            string SqlShop = "Select S.*, C.CityName from Shop as S left join City as C on S.CityId = C.Id";
            string SqlTax = "Select * from Tax";
            string SqlDeletelog = "Select DL.*, C.Name as CounterName, U.Name as UName from Deletelog as DL inner join Counter as C on DL.CounterId = C.Id inner join [User] as U on DL.UserId = U.Id inner join [Transaction] as T on T.Id=DL.TransactionId where Cast(T.UpdatedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date) ";
            string SqlCurrency = "Select * from Currency";
            string SqlExchangeRateForTransaction = "Select * from ExchangeRateForTransaction E inner join [Transaction] as T on T.Id=E.TransactionId where Cast(T.UpdatedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date) ";
            string SqlPaymentType = "Select * from PaymentType";
            // string SqlSetting = "Select * from Setting";
            string SqlUserRole = "Select * from UserRole";
            string SqlUser = "Select * from [User]";
            // By ZP
            string SqlGiftCard = "Select G.*, C.Address, C.Email, C.PhoneNumber, C.CustomerCode from GiftCard as G inner join Customer as C on G.CustomerId = C.Id where Cast(G.IsUsedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(G.IsUsedDate as date) <= Cast('" + dtToday + "' as date)  and G.IsDeleted=0 ";
            string SqlGiftCardInTransaction = "select GT.*, G.CardNumber from GiftCardInTransaction as GT inner join GiftCard G on GT.GiftCardId = G.Id inner join [Transaction] as T on T.Id=GT.TransactionId where Cast(T.UpdatedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date)  and G.IsDeleted=0 ";
            string SqlRoleManagement = "Select * from RoleManagement";
            string SqlGiftSystem = "select G.Id,G.Type,G.Name,G.MustBuyCostFrom,G.MustBuyCostTo,G.MustIncludeProductId as MustIncludeProductId ,G.FilterBrandId as FilterBrandId,G.FilterCategoryId as FilterCategoryId, G.FilterSubCategoryId as FilterSubCategoryId  ,G.ValidFrom,G.ValidTo,G.UsePromotionQty,G.PromotionQty,G.GiftProductId,G.PriceForGiftProduct,G.GiftCashAmount,G.DiscountPercentForTransaction,G.UseBrandFilter,G.UseCategoryFilter,G.UseSubCategoryFilter,G.UseProductFilter,G.IsActive,G.UseSizeFilter,G.UseQtyFilter,G.FilterSize,G.FilterQty,p1.ProductCode as MustProductCode, p2.ProductCode as GiftProductCode, B.Name as BName, ISNULL(PC.Name,'') as CName, ISNULL(PS.Name,'') as SCName  from GiftSystem as G left join Product as P1 on P1.Id = G.MustIncludeProductId left join Product as P2 on p2.Id = G.GiftProductId left join Brand as B on B.Id = G.FilterBrandId left join ProductCategory as PC on G.FilterCategoryId = PC.Id left join ProductSubCategory as PS on G.FilterSubCategoryId = PS.Id";
            string SqlAttachGiftSystemForTransaction = "Select AG.*, G.Name from AttachGiftSystemForTransaction as AG inner join GiftSystem as G on AG.AttachGiftSystemId = G.Id inner join [Transaction] as T on T.Id=AG.TransactionId where Cast(T.UpdatedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date) ";
            string SqlUsePrePaidDebt = "Select U.*, C.Name as CounterName, Us.Name as UName from UsePrePaidDebt as U inner join Counter as C on U.CounterId = C.Id inner join [User] as Us on U.CashierId = Us.Id";
            string SqlLoc_CustomerPoint = "Select * from Loc_CustomerPoint";
            string SqlNoveltySystem = "Select N.*, B.Name as BName from NoveltySystem  as N left join Brand as B on N.BrandId = B.Id";
            string SqlProductInNovelty = "Select PN.*, P.ProductCode from ProductInNovelty as  PN inner join Product as P on PN.ProductId = P.Id left join NoveltySystem as NS on NS.Id = NoveltySystemId";
            string SqlWrapperItem = "Select W.*, P1.ProductCode as ParentProductCode, p2.ProductCode as ChildProductCode from WrapperItem as W inner join Product as P1 on p1.Id = W.ParentProductId inner join Product as P2 on p2.Id = w.ChildProductId";
            //string SqlProductPriceChange = "Select PC.*, U.Name as UName, P.ProductCode from ProductPriceChange as PC inner join Product as P on PC.ProductId = P.Id inner join [User] as U on U.Id = PC.UserID;
            string SqlProductPriceChange = "Select PC.*, U.Name as UName, P.ProductCode from ProductPriceChange as PC inner join Product as P on PC.ProductId = P.Id inner join [User] as U on U.Id = PC.UserID where PC.UpdateDate >='" + dtTranDate + "'";
            string SqlSPDetail = "Select SP.*, P1.ProductCode as ParentProductCode, P2.ProductCode as ChildProductCode, T.Id as TID from SPDetail as SP inner join Product as P1 on p1.Id = SP.ParentProductId inner join Product as P2 on p2.Id = SP.ChildProductId inner join TransactionDetail as Td on Td.Id = SP.TransactionDetailID inner join [Transaction] as T on T.Id = Td.TransactionId where Cast(T.UpdatedDate as date)  >= Cast('" + dtTranDate + "' as date) and cast(T.UpdatedDate as date) <= Cast('" + dtToday + "' as date) ";
            string SqlVIPMemberRule = "Select * from VIPMemberRule";
            string SqlPointDeductionPercentage_History = "Select * from PointDeductionPercentage_History";
            string SqlTransactionPaymentDetail = "select TD.*,P.Name from TransactionPaymentDetails as TD inner join PaymentMethod as P on TD.PaymentMethodId= P.Id inner join [Transaction] as t on t.Id=TD.TransactionId where t.IsComplete=1 and cast(t.UpdatedDate as date) >= Cast( '" + dtTranDate + "' as date) and cast(t.UpdatedDate as date) <= Cast('" + dtToday + "' as date)";
            try
            {
                connection.Open();

                //Create combination data
                DataSet dsCombineData = new DataSet("ExportData");
                DataTable Product = new DataTable("Product");
                DataTable Customer = new DataTable("Customer");
                DataTable Transaction = new DataTable("Transaction");
                DataTable TransactionDetail = new DataTable("TransactionDetail");
                DataTable Brand = new DataTable("Brand");
                DataTable Loc_PointRedeemHistory = new DataTable("Loc_PointRedeemHistory");
                DataTable City = new DataTable("City");
                DataTable ConsignmentCounter = new DataTable("ConsignmentCounter");
                DataTable Counter = new DataTable("Counter");
                DataTable ProductCategory = new DataTable("ProductCategory");
                DataTable ProductSubCategory = new DataTable("ProductSubCategory");
                DataTable Shop = new DataTable("Shop");
                DataTable Unit = new DataTable("Unit");
                DataTable Tax = new DataTable("Tax");
                DataTable DeleteLog = new DataTable("DeleteLog");
                DataTable Currency = new DataTable("Currency");
                DataTable ExchangeRateForTransaction = new DataTable("ExchangeRateForTransaction");
                DataTable PaymentType = new DataTable("PaymentType");
                //DataTable Setting = new DataTable("Setting");
                DataTable UserRole = new DataTable("UserRole");
                DataTable User = new DataTable("User");
                DataTable GiftCard = new DataTable("GiftCard");
                DataTable GiftCardInTransaction = new DataTable("GiftCardInTransaction");
                DataTable RoleManagement = new DataTable("RoleManagement");
                DataTable GiftSystem = new DataTable("GiftSystem");
                DataTable AttachGiftSystemForTransaction = new DataTable("AttachGiftSystemForTransaction");
                DataTable UsePrePaidDebt = new DataTable("UsePrePaidDebt");
                DataTable Loc_CustomerPoint = new DataTable("Loc_CustomerPoint");
                DataTable NoveltySystem = new DataTable("NoveltySystem");
                DataTable ProductInNovelty = new DataTable("ProductInNovelty");
                DataTable WrapperItem = new DataTable("WrapperItem");
                DataTable ProductPriceChange = new DataTable("ProductPriceChange");
                DataTable SPDetail = new DataTable("SPDetail");
                DataTable VIPMemberRule = new DataTable("VIPMemberRule");
                DataTable PointDeductionPercentage_History = new DataTable("PointDeductionPercentage_History");
                DataTable TransactionPaymentDetails = new DataTable("TransactionPaymentDetails");

                adapter = new SqlDataAdapter(sqlProduct, connection);
                adapter.Fill(Product);
                adapter = new SqlDataAdapter(sqlCustomer, connection);
                adapter.Fill(Customer);
                adapter = new SqlDataAdapter(sqlTransaction, connection);
                adapter.Fill(Transaction);
                adapter = new SqlDataAdapter(sqlTransactionDetail, connection);
                adapter.Fill(TransactionDetail);
                adapter = new SqlDataAdapter(sqlBrand, connection);
                adapter.Fill(Brand);
                adapter = new SqlDataAdapter(sqlLoc_PointRedeemHistory, connection);
                adapter.Fill(Loc_PointRedeemHistory);
                adapter = new SqlDataAdapter(sqlCity, connection);
                adapter.Fill(City);
                adapter = new SqlDataAdapter(sqlConsignmentCounter, connection);
                adapter.Fill(ConsignmentCounter);
                adapter = new SqlDataAdapter(SqlCounter, connection);
                adapter.Fill(Counter);
                adapter = new SqlDataAdapter(SqlProductCategory, connection);
                adapter.Fill(ProductCategory);
                adapter = new SqlDataAdapter(SqlProductSubCategory, connection);
                adapter.Fill(ProductSubCategory);
                adapter = new SqlDataAdapter(SqlShop, connection);
                adapter.Fill(Shop);
                adapter = new SqlDataAdapter(SqlUnit, connection);
                adapter.Fill(Unit);
                adapter = new SqlDataAdapter(SqlTax, connection);
                adapter.Fill(Tax);
                adapter = new SqlDataAdapter(SqlDeletelog, connection);
                adapter.Fill(DeleteLog);
                adapter = new SqlDataAdapter(SqlCurrency, connection);
                adapter.Fill(Currency);
                adapter = new SqlDataAdapter(SqlExchangeRateForTransaction, connection);
                adapter.Fill(ExchangeRateForTransaction);
                adapter = new SqlDataAdapter(SqlPaymentType, connection);
                adapter.Fill(PaymentType);
                //adapter = new SqlDataAdapter(SqlSetting, connection);
                //adapter.Fill(Setting);
                adapter = new SqlDataAdapter(SqlUserRole, connection);
                adapter.Fill(UserRole);
                adapter = new SqlDataAdapter(SqlUser, connection);
                adapter.Fill(User);
                adapter = new SqlDataAdapter(SqlGiftCard, connection);
                adapter.Fill(GiftCard);
                adapter = new SqlDataAdapter(SqlGiftCardInTransaction, connection);
                adapter.Fill(GiftCardInTransaction);
                adapter = new SqlDataAdapter(SqlRoleManagement, connection);
                adapter.Fill(RoleManagement);
                adapter = new SqlDataAdapter(SqlGiftSystem, connection);
                adapter.Fill(GiftSystem);
                adapter = new SqlDataAdapter(SqlAttachGiftSystemForTransaction, connection);
                adapter.Fill(AttachGiftSystemForTransaction);
                adapter = new SqlDataAdapter(SqlUsePrePaidDebt, connection);
                adapter.Fill(UsePrePaidDebt);
                adapter = new SqlDataAdapter(SqlLoc_CustomerPoint, connection);
                adapter.Fill(Loc_CustomerPoint);
                adapter = new SqlDataAdapter(SqlNoveltySystem, connection);
                adapter.Fill(NoveltySystem);
                adapter = new SqlDataAdapter(SqlProductInNovelty, connection);
                adapter.Fill(ProductInNovelty);
                adapter = new SqlDataAdapter(SqlWrapperItem, connection);
                adapter.Fill(WrapperItem);
                adapter = new SqlDataAdapter(SqlProductPriceChange, connection);
                adapter.Fill(ProductPriceChange);
                adapter = new SqlDataAdapter(SqlSPDetail, connection);
                adapter.Fill(SPDetail);
                adapter = new SqlDataAdapter(SqlVIPMemberRule, connection);
                adapter.Fill(VIPMemberRule);
                adapter = new SqlDataAdapter(SqlPointDeductionPercentage_History, connection);
                adapter.Fill(PointDeductionPercentage_History);

                adapter = new SqlDataAdapter(SqlTransactionPaymentDetail, connection);
                adapter.Fill(TransactionPaymentDetails);

                dsCombineData.Tables.Add(Product);
                dsCombineData.Tables.Add(Customer);
                dsCombineData.Tables.Add(Transaction);
                dsCombineData.Tables.Add(TransactionDetail);
                dsCombineData.Tables.Add(Brand);
                dsCombineData.Tables.Add(Loc_PointRedeemHistory);
                dsCombineData.Tables.Add(City);
                dsCombineData.Tables.Add(ConsignmentCounter);
                dsCombineData.Tables.Add(Counter);
                dsCombineData.Tables.Add(ProductCategory);
                dsCombineData.Tables.Add(ProductSubCategory);
                dsCombineData.Tables.Add(Shop);
                dsCombineData.Tables.Add(Unit);
                dsCombineData.Tables.Add(Tax);
                dsCombineData.Tables.Add(DeleteLog);
                dsCombineData.Tables.Add(Currency);
                dsCombineData.Tables.Add(ExchangeRateForTransaction);
                dsCombineData.Tables.Add(PaymentType);
                // dsCombineData.Tables.Add(Setting);
                dsCombineData.Tables.Add(UserRole);
                dsCombineData.Tables.Add(User);
                dsCombineData.Tables.Add(GiftCard);
                dsCombineData.Tables.Add(GiftCardInTransaction);
                dsCombineData.Tables.Add(RoleManagement);
                dsCombineData.Tables.Add(GiftSystem);
                dsCombineData.Tables.Add(AttachGiftSystemForTransaction);
                dsCombineData.Tables.Add(UsePrePaidDebt);
                dsCombineData.Tables.Add(Loc_CustomerPoint);
                dsCombineData.Tables.Add(NoveltySystem);
                dsCombineData.Tables.Add(ProductInNovelty);
                dsCombineData.Tables.Add(WrapperItem);
                dsCombineData.Tables.Add(ProductPriceChange);
                dsCombineData.Tables.Add(SPDetail);
                dsCombineData.Tables.Add(VIPMemberRule);
                dsCombineData.Tables.Add(PointDeductionPercentage_History);
                dsCombineData.Tables.Add(TransactionPaymentDetails);
                //Create path to save
                string activeDir = @"d:\";
                string newPath = System.IO.Path.Combine(activeDir, "POS_Export");

                if (!System.IO.Directory.Exists(newPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(newPath);
                }

                //Create File name
                string fileName = "D:/POS_Export/" + Environment.MachineName + "_POS_ExportFile[" + DateTime.Now.ToString("dd-MM-yyyy hh-mm tt") + "].xml";
                dsCombineData.WriteXml(fileName);
                string[] encryptFileNameArr = fileName.Split('.');
                string tempFileName = encryptFileNameArr[0] + ".sc";

                //Encrypt File and delete original file
                EncryptFile(fileName, tempFileName);

                connection.Close();

                MessageBox.Show("Done, file saved to " + tempFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region Import

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure that you want to import the data?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (ofdImportFile.ShowDialog(this) == DialogResult.Cancel)
                {
                    return;
                }
                #region Get xml filename and decrypt

                string fileName = ofdImportFile.FileName;
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
                destFileName = destFileName + ".xml";
                if (File.Exists(destFileName)) File.Delete(destFileName);

                DecryptFile(fileName, destFileName);

                #endregion

                // string delimited = @"\G(.+)[\t\u007c](.+)\r?\n";
                //string delimited = @"'([^']*)";

                APP_Data.POSEntities entity = new APP_Data.POSEntities();
                //reading XML file and storing it's data to dataset.

                DataSet dsxml = new DataSet();
                dsxml.ReadXml(destFileName);
                DataTable dtxmlProduct = dsxml.Tables["Product"];
                DataTable dtxmlTransaction = dsxml.Tables["Transaction"];
                DataTable dtxmlCustomer = dsxml.Tables["Customer"];
                DataTable dtxmlTransactionDetail = dsxml.Tables["TransactionDetail"];
                DataTable dtxmlBrand = dsxml.Tables["Brand"];



                DataTable dtxmlLoc_PointRedeemHistory = dsxml.Tables["Loc_PointRedeemHistory"];
                DataTable dtxmlCity = dsxml.Tables["City"];
                DataTable dtxmlConsignmentCounter = dsxml.Tables["ConsignmentCounter"];
                DataTable dtxmlCounter = dsxml.Tables["Counter"];
                DataTable dtxmlProductCategory = dsxml.Tables["ProductCategory"];
                DataTable dtxmlProudctSubCategory = dsxml.Tables["ProductSubCategory"];
                DataTable dtxmLUnit = dsxml.Tables["Unit"];
                DataTable dtxmlShop = dsxml.Tables["Shop"];
                DataTable dtxmlTax = dsxml.Tables["Tax"];
                DataTable dtxmlDeleteLog = dsxml.Tables["DeleteLog"];
                DataTable dtxmlCurrency = dsxml.Tables["Currency"];
                DataTable dtxmlExchangeRateForTransaction = dsxml.Tables["ExchangeRateForTransaction"];
                DataTable dtxmlPaymentType = dsxml.Tables["PaymentType"];
                //  DataTable dtxmlSetting = dsxml.Tables["Setting"];
                DataTable dtxmlUserRole = dsxml.Tables["UserRole"];
                DataTable dtxmlUser = dsxml.Tables["User"];
                DataTable dtxmlGiftCard = dsxml.Tables["GiftCard"];
                DataTable dtxmlGiftCardInTransaction = dsxml.Tables["GiftCardInTransaction"];
                DataTable dtxmlRoleManagement = dsxml.Tables["RoleManagement"];
                DataTable dtxmlGiftSystem = dsxml.Tables["GiftSystem"];
                DataTable dtxmlAttachGiftSystemForTransaction = dsxml.Tables["AttachGiftSystemForTransaction"];
                DataTable dtxmlUsePrePaidDebt = dsxml.Tables["UsePrePaidDebt"];
                DataTable dtxmlLoc_CustomerPoint = dsxml.Tables["Loc_CustomerPoint"];
                DataTable dtxmlNoveltySystem = dsxml.Tables["NoveltySystem"];
                DataTable dtxmlProductInNovelty = dsxml.Tables["ProductInNovelty"];
                DataTable dtxmlWrapperItem = dsxml.Tables["WrapperItem"];
                DataTable dtxmlProductPriceChange = dsxml.Tables["ProductPriceChange"];
                DataTable dtxmlSPDetail = dsxml.Tables["SPDetail"];
                DataTable dtxmlVIPMemberRule = dsxml.Tables["VIPMemberRule"];
                DataTable dtxmlPointDeductRule = dsxml.Tables["PointDeductionPercentage_History"];
                DataTable dtxmlTransactionPaymentDetail = dsxml.Tables["TransactionPaymentDetails"];

                #region Brand

                entity = new APP_Data.POSEntities();
                //Loop through dataRow come from xml and check if the brnad is already exist or brand is new one
                if (dtxmlBrand != null)
                {
                    label1.Text = "Step 1 of 32 : Processing Brand table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlBrand.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;

                    foreach (DataRow dataRowFromXML in dtxmlBrand.Rows)
                    {
                        Progressbar1.PerformStep();
                        Boolean sameRowExist = false;
                        String Name = dataRowFromXML["Name"].ToString();

                        APP_Data.Brand FoundBrand = entity.Brands.Where(x => x.Name == Name).FirstOrDefault();
                        //same brand name found
                        if (FoundBrand != null)
                        {
                            sameRowExist = true;
                        }
                        //Found same row,Update the Current One
                        if (sameRowExist)
                        {
                            GetBrandFromXML(dataRowFromXML, FoundBrand);
                            entity.SaveChanges();

                            String oldBrandId = dataRowFromXML["Id"].ToString();
                            String BName = dataRowFromXML["Name"].ToString();
                            //update in brand DataTables
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("BrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
                                {
                                    o["BrandId"] = FoundBrand.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null)
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("FilterBrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
                                {
                                    o["FilterBrandId"] = FoundBrand.Id.ToString();
                                }
                            }
                            if (dtxmlNoveltySystem != null)
                            {
                                foreach (DataRow o in dtxmlNoveltySystem.Select("BrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
                                {
                                    o["BrandId"] = FoundBrand.Id.ToString();
                                }
                            }
                        }
                        //brand name is not exist in the current database
                        //add new row
                        else
                        {
                            APP_Data.Brand brand = new APP_Data.Brand();
                            GetBrandFromXML(dataRowFromXML, brand);
                            entity.Brands.Add(brand);
                            entity.SaveChanges();
                            //update latest id in other DataTables
                            String oldBrandId = dataRowFromXML["Id"].ToString();
                            String BName = dataRowFromXML["Name"].ToString();
                            //update in brand DataTables
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("BrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
                                {
                                    o["BrandId"] = brand.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null)
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("FilterBrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
                                {
                                    o["FilterBrandId"] = brand.Id.ToString();
                                }
                            }
                            if (dtxmlNoveltySystem != null)
                            {
                                foreach (DataRow o in dtxmlNoveltySystem.Select("BrandId = '" + oldBrandId + "' and BName ='" + BName.Replace("'", "''") + "'"))
                                {
                                    o["BrandId"] = brand.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region City
                entity = new APP_Data.POSEntities();
                //loop through dataRow come from xml and check if the city is already exist or brand is new one
                if (dtxmlCity != null)
                {
                    label1.Text = "Step 2 of 32 : Processing City table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlCity.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;

                    foreach (DataRow dataRowFromXML in dtxmlCity.Rows)
                    {

                        Progressbar1.PerformStep();
                        Boolean sameRowExist = false;
                        string Name = dataRowFromXML["CityName"].ToString();
                        APP_Data.City FoundCity = entity.Cities.Where(x => x.CityName == Name).FirstOrDefault();
                        //found same ctiy name
                        if (FoundCity != null)
                        {
                            sameRowExist = true;
                        }
                        //Found same row ,Update the Current One
                        if (sameRowExist)
                        {
                            GetCityFromXML(dataRowFromXML, FoundCity);
                            entity.SaveChanges();

                            String oldCityId = dataRowFromXML["Id"].ToString();
                            String CName = dataRowFromXML["CityName"].ToString();
                            //update city in City Table
                            if (dtxmlCustomer != null)
                            {
                                foreach (DataRow o in dtxmlCustomer.Select("CityId='" + oldCityId + "' and CityName ='" + CName.Replace("'", "''") + "'"))
                                {
                                    o["CityId"] = FoundCity.Id.ToString();
                                }
                            }
                            if (dtxmlShop != null)
                            {
                                foreach (DataRow o in dtxmlShop.Select("CityId='" + oldCityId + "' and CityName ='" + CName.Replace("'", "''") + "'"))
                                {
                                    o["CityId"] = FoundCity.Id.ToString();
                                }
                            }
                        }
                        //City name is not exist in current database
                        //add new row
                        else
                        {
                            APP_Data.City city = new APP_Data.City();
                            GetCityFromXML(dataRowFromXML, city);
                            entity.Cities.Add(city);
                            entity.SaveChanges();
                            //Update latest id in other datatable
                            String oldCityId = dataRowFromXML["Id"].ToString();
                            String CName = dataRowFromXML["CityName"].ToString();
                            //update city in City Table
                            if (dtxmlCustomer != null)
                            {
                                foreach (DataRow o in dtxmlCustomer.Select("CityId='" + oldCityId + "' and CityId ='" + CName.Replace("'", "''") + "'"))
                                {
                                    o["CityId"] = city.Id.ToString();
                                }
                            }
                            if (dtxmlShop != null)
                            {
                                foreach (DataRow o in dtxmlShop.Select("CityId='" + oldCityId + "' and CityId ='" + CName.Replace("'", "''") + "'"))
                                {
                                    o["CityId"] = city.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region PointDeductionRule
                entity = new APP_Data.POSEntities();
                //loop through dataRow come from xml and check if the city is already exist or brand is new one
                if (dtxmlPointDeductRule != null)
                {
                    label1.Text = "Step 2 of 32 : Processing City table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlPointDeductRule.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;

                    foreach (DataRow dataRowFromXML in dtxmlPointDeductRule.Rows)
                    {

                        Progressbar1.PerformStep();
                        Boolean sameRowExist = false;
                        DateTime startdate = Convert.ToDateTime(dataRowFromXML["StartDate"]);
                        decimal rate = Convert.ToDecimal(dataRowFromXML["DiscountRate"]);
                        APP_Data.PointDeductionPercentage_History Foundrule = entity.PointDeductionPercentage_History.Where(x => x.DiscountRate == rate && x.StartDate == startdate).FirstOrDefault();
                        //found same ctiy name
                        if (Foundrule != null)
                        {
                            sameRowExist = true;
                        }
                        //Found same row ,Update the Current One
                        if (sameRowExist)
                        {
                            GetPointDeductRuleFromXML(dataRowFromXML, Foundrule);
                            entity.SaveChanges();


                        }
                        //City name is not exist in current database
                        //add new row
                        else
                        {
                            APP_Data.PointDeductionPercentage_History pointdeduct = new APP_Data.PointDeductionPercentage_History();
                            GetPointDeductRuleFromXML(dataRowFromXML, pointdeduct);
                            entity.PointDeductionPercentage_History.Add(pointdeduct);
                            entity.SaveChanges();

                        }
                    }
                }
                #endregion


                #region VIPMemberRule

                entity = new APP_Data.POSEntities();
                if (dtxmlVIPMemberRule != null)
                {
                    label1.Text = "Step 3 of 32 : Processing VIP Member Rule table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlVIPMemberRule.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;

                    foreach (DataRow dataFromXML in dtxmlVIPMemberRule.Rows)
                    {
                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        int SameFieldCount = 0;
                        int vIPRuleId = 0;

                        string RuleName = dataFromXML["RuleName"].ToString();
                        long Amount = Convert.ToInt64(dataFromXML["Amount"].ToString());
                        bool IsCalculatePoints = Convert.ToBoolean(dataFromXML["IsCalculatePoints"].ToString());

                        foreach (APP_Data.VIPMemberRule v in entity.VIPMemberRules)
                        {
                            SameFieldCount = 0;

                            if (v.RuleName == RuleName) SameFieldCount++;
                            if (v.Amount == Amount) SameFieldCount++;
                            if (v.IsCalculatePoints == IsCalculatePoints) SameFieldCount++;

                            if (SameFieldCount >= 3)
                            {
                                SameRowExit = true;
                                vIPRuleId = v.Id;
                                break;
                            }
                        }

                        if (SameRowExit)
                        {
                            APP_Data.VIPMemberRule foundVIPMebmerRule = entity.VIPMemberRules.Where(x => x.Id == vIPRuleId).FirstOrDefault();
                            GetVIPMemberRuleFromXML(foundVIPMebmerRule, dataFromXML);
                            entity.SaveChanges();

                            string oldVIpId = dataFromXML["Id"].ToString();
                            if (dtxmlCustomer != null)
                            {
                                foreach (DataRow o in dtxmlCustomer.Select("RuleId='" + oldVIpId + "'"))
                                {
                                    o["RuleId"] = foundVIPMebmerRule.Id.ToString();
                                }
                            }
                        }
                        else
                        {
                            APP_Data.VIPMemberRule vIPobj = new APP_Data.VIPMemberRule();
                            GetVIPMemberRuleFromXML(vIPobj, dataFromXML);
                            entity.VIPMemberRules.Add(vIPobj);
                            entity.SaveChanges();

                            string oldVIpId = dataFromXML["Id"].ToString();
                            if (dtxmlCustomer != null)
                            {
                                foreach (DataRow o in dtxmlCustomer.Select("RuleId='" + oldVIpId + "'"))
                                {
                                    o["RuleId"] = vIPobj.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion


                #region Customer
                entity = new APP_Data.POSEntities();
                //Loop through dataRow come from xml and check if the customer is already exist or a new customer
                if (dtxmlCustomer != null)
                {
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlCustomer.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    label1.Text = "Step 4 of 32 : Processing Customer table please wait!";
                    label1.Refresh();

                    foreach (DataRow dataRowFromXML in dtxmlCustomer.Rows)
                    {
                        Progressbar1.PerformStep();
                        Boolean SameRowExist = false;

                        String CustomerCode = dataRowFromXML["CustomerCode"].ToString();
                        int Id = Convert.ToInt32(dataRowFromXML["Id"].ToString());

                        APP_Data.Customer FoundCustomer = entity.Customers.Where(x => x.CustomerCode == CustomerCode).FirstOrDefault();

                        if (FoundCustomer != null)
                        {
                            SameRowExist = true;
                        }
                        //Found same row,Update the Current One
                        if (SameRowExist)
                        {
                            GetCustomerFromXML(dataRowFromXML, FoundCustomer);
                            entity.SaveChanges();

                            string oldCustomerId = dataRowFromXML["Id"].ToString();

                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("CustomerId = '" + oldCustomerId + "'and CustomerCode ='" + CustomerCode + "'"))
                                {
                                    o["CustomerId"] = FoundCustomer.Id.ToString();
                                    var c = FoundCustomer.Name;
                                }
                            }
                            if (dtxmlGiftCard != null)
                            {
                                foreach (DataRow o in dtxmlGiftCard.Select("CustomerId = '" + oldCustomerId + "'and CustomerCode ='" + CustomerCode + "'"))
                                {
                                    o["CustomerId"] = FoundCustomer.Id.ToString();
                                }
                            }
                            if (dtxmlLoc_PointRedeemHistory != null)
                            {
                                foreach (DataRow o in dtxmlLoc_PointRedeemHistory.Select("CustomerId = '" + oldCustomerId + "'and CustomerCode ='" + CustomerCode + "'"))
                                {
                                    o["CustomerId"] = FoundCustomer.Id.ToString();
                                }
                            }
                        }
                        //Customer Data is not exist in the database
                        //add new row
                        else
                        {
                            APP_Data.Customer cs = new APP_Data.Customer();
                            GetCustomerFromXML(dataRowFromXML, cs);
                            entity.Customers.Add(cs);
                            entity.SaveChanges();
                            //update latest id in other DataTables  

                            //Update in Transaction DataTable
                            string oldCustomerId = dataRowFromXML["Id"].ToString();

                            //if (oldCustomerId == "9550")
                            //{
                            //    string NewId = cs.Id.ToString();
                            //}
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("CustomerId = '" + oldCustomerId + "' and CustomerCode ='" + CustomerCode + "'"))
                                {
                                    o["CustomerId"] = cs.Id.ToString();
                                }
                            }
                            if (dtxmlGiftCard != null)
                            {
                                foreach (DataRow o in dtxmlGiftCard.Select("CustomerId = '" + oldCustomerId + "' and CustomerCode ='" + CustomerCode + "'"))
                                {
                                    o["CustomerId"] = cs.Id.ToString();
                                }
                            }
                            if (dtxmlLoc_PointRedeemHistory != null)
                            {
                                foreach (DataRow o in dtxmlLoc_PointRedeemHistory.Select("CustomerId = '" + oldCustomerId + "' and CustomerCode ='" + CustomerCode + "'"))
                                {
                                    o["CustomerId"] = cs.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Counter

                entity = new APP_Data.POSEntities();
                //Loop through dataRow come from xml and check if the Counter is already exist or Counter is new one
                if (dtxmlCounter != null)
                {
                    label1.Text = "Step 5 of 32 : Processing Counter table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlCounter.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;

                    foreach (DataRow dataRowFromXML in dtxmlCounter.Rows)
                    {

                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        String Name = dataRowFromXML["Name"].ToString();
                        APP_Data.Counter Foundcounter = entity.Counters.Where(x => x.Name == Name).FirstOrDefault();
                        //Same Counter Code Found
                        if (Foundcounter != null)
                        {
                            SameRowExit = true;
                        }
                        //Found same row,Update the Current One
                        if (SameRowExit)
                        {
                            GetCounterFromXML(dataRowFromXML, Foundcounter);
                            entity.SaveChanges();

                            String OldCounterId = dataRowFromXML["Id"].ToString();
                            String counterName = dataRowFromXML["Name"].ToString();
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                                {
                                    o["CounterId"] = Foundcounter.Id.ToString();
                                }
                            }
                            if (dtxmlDeleteLog != null)
                            {
                                foreach (DataRow o in dtxmlDeleteLog.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                                {
                                    o["CounterId"] = Foundcounter.Id.ToString();
                                }
                            }
                            if (dtxmlUsePrePaidDebt != null)
                            {
                                foreach (DataRow o in dtxmlUsePrePaidDebt.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                                {
                                    o["CounterId"] = Foundcounter.Id.ToString();
                                }
                            }
                            if (dtxmlLoc_PointRedeemHistory != null)
                            {
                                foreach (DataRow o in dtxmlLoc_PointRedeemHistory.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                                {
                                    o["CounterId"] = Foundcounter.Id.ToString();
                                }
                            }
                        }
                        //Counter is not exist in the current database
                        //add new row
                        else
                        {
                            APP_Data.Counter counter = new APP_Data.Counter();
                            GetCounterFromXML(dataRowFromXML, counter);
                            entity.Counters.Add(counter);
                            entity.SaveChanges();

                            //update in transaction DataTables
                            String OldCounterId = dataRowFromXML["Id"].ToString();
                            String counterName = dataRowFromXML["Name"].ToString();
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                                {
                                    o["CounterId"] = counter.Id.ToString();
                                }
                            }
                            if (dtxmlDeleteLog != null)
                            {
                                foreach (DataRow o in dtxmlDeleteLog.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                                {
                                    o["CounterId"] = counter.Id.ToString();
                                }
                            }
                            if (dtxmlUsePrePaidDebt != null)
                            {
                                foreach (DataRow o in dtxmlUsePrePaidDebt.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                                {
                                    o["CounterId"] = counter.Id.ToString();
                                }
                            }
                            if (dtxmlLoc_PointRedeemHistory != null)
                            {
                                foreach (DataRow o in dtxmlLoc_PointRedeemHistory.Select("CounterId='" + OldCounterId + "'and CounterName ='" + counterName.Replace("'", "''") + "'"))
                                {
                                    o["CounterId"] = counter.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region UserRole

                entity = new APP_Data.POSEntities();

                if (dtxmlUserRole != null)
                {
                    //loop through dataRow from xml and check if the UserRole is already exist or newone.
                    label1.Text = "Step 6 of 32 : Processing User Role table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlUserRole.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    foreach (DataRow dataFromXML in dtxmlUserRole.Rows)
                    {
                        Progressbar1.PerformStep();

                        bool SameRowExit = false;
                        string RoleName = dataFromXML["RoleName"].ToString();
                        APP_Data.UserRole FoundUserRole = entity.UserRoles.Where(x => x.RoleName == RoleName).FirstOrDefault();
                        //same User Role found
                        if (FoundUserRole != null)
                        {
                            SameRowExit = true;
                        }
                        //Found same row,Update the Current One
                        if (SameRowExit)
                        {
                            GetUserRoleFromXML(dataFromXML, FoundUserRole);
                            entity.SaveChanges();

                            string OldUserRoleId = dataFromXML["Id"].ToString();
                            if (dtxmlUser != null)
                            {
                                foreach (DataRow o in dtxmlUser.Select("UserRoleId='" + OldUserRoleId + "'"))
                                {
                                    o["UserRoleId"] = FoundUserRole.Id.ToString();
                                }
                            }
                            if (dtxmlRoleManagement != null)
                            {
                                foreach (DataRow o in dtxmlRoleManagement.Select("UserRoleId='" + OldUserRoleId + "'"))
                                {
                                    o["UserRoleId"] = FoundUserRole.Id.ToString();
                                }
                            }
                        }
                        //FoundUserRole is not exist in the current database
                        //add new row
                        else
                        {
                            APP_Data.UserRole userRole = new APP_Data.UserRole();
                            GetUserRoleFromXML(dataFromXML, userRole); ;
                            entity.UserRoles.Add(userRole);
                            entity.SaveChanges();

                            //update latest id in other DataTables
                            string OldUserRoleId = dataFromXML["Id"].ToString();
                            //update in transaction DataTables                      
                            if (dtxmlUser != null)
                            {
                                foreach (DataRow o in dtxmlUser.Select("UserRoleId='" + OldUserRoleId + "'"))
                                {
                                    o["UserRoleId"] = userRole.Id.ToString();
                                }
                            }
                            if (dtxmlRoleManagement != null)
                            {
                                foreach (DataRow o in dtxmlRoleManagement.Select("UserRoleId='" + OldUserRoleId + "'"))
                                {
                                    o["UserRoleId"] = userRole.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region User

                entity = new APP_Data.POSEntities();
                if (dtxmlUser != null)
                {
                    label1.Text = "Step 7 of 32 : Processing User table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlUser.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //loop through dataRow from xml and check if the User is already exist or newone.
                    foreach (DataRow dataFromXML in dtxmlUser.Rows)
                    {
                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        string Name = dataFromXML["Name"].ToString();
                        int UserRoleId = Convert.ToInt32(dataFromXML["UserRoleId"].ToString());
                        APP_Data.User FoundUser = entity.Users.Where(x => x.Name == Name && x.UserRoleId == UserRoleId).FirstOrDefault();
                        //same User Role found
                        if (FoundUser != null)
                        {
                            SameRowExit = true;
                        }
                        //Found same row,Update the Current One
                        if (SameRowExit)
                        {
                            GetUserFromXML(dataFromXML, FoundUser);
                            entity.SaveChanges();

                            string OldUserId = dataFromXML["Id"].ToString();
                            string UName = dataFromXML["Name"].ToString();
                            //update in transaction DataTables
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("UserId='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                {
                                    o["UserId"] = FoundUser.Id.ToString();
                                }
                            }
                            if (dtxmlUsePrePaidDebt != null)
                            {
                                foreach (DataRow o in dtxmlUsePrePaidDebt.Select("CashierId='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                {
                                    o["CashierId"] = FoundUser.Id.ToString();
                                }
                            }
                            if (dtxmlDeleteLog != null)
                            {
                                if (dtxmlDeleteLog != null)
                                {
                                    foreach (DataRow o in dtxmlDeleteLog.Select("UserId='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                    {
                                        o["UserId"] = FoundUser.Id.ToString();
                                    }
                                }
                            }
                            if (dtxmlLoc_PointRedeemHistory != null)
                            {
                                foreach (DataRow o in dtxmlLoc_PointRedeemHistory.Select("CasherId='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                {
                                    o["CasherId"] = FoundUser.Id.ToString();
                                }
                            }
                            if (dtxmlProductPriceChange != null)
                            {
                                foreach (DataRow o in dtxmlProductPriceChange.Select("UserID ='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                {
                                    o["UserID"] = FoundUser.Id.ToString();
                                }
                            }
                        }
                        //FoundUserRole is not exist in the current database
                        //add new row
                        else
                        {
                            APP_Data.User user = new APP_Data.User();
                            GetUserFromXML(dataFromXML, user); ;
                            entity.Users.Add(user);
                            entity.SaveChanges();

                            //update latest id in other DataTables
                            string OldUserId = dataFromXML["Id"].ToString();
                            string UName = dataFromXML["Name"].ToString();
                            //update in transaction DataTables
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("UserId='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                {
                                    o["UserId"] = user.Id.ToString();
                                }
                            }
                            if (dtxmlUsePrePaidDebt != null)
                            {
                                foreach (DataRow o in dtxmlUsePrePaidDebt.Select("CashierId='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                {
                                    o["CashierId"] = user.Id.ToString();
                                }
                            }
                            if (dtxmlDeleteLog != null)
                            {
                                if (dtxmlDeleteLog != null)
                                {
                                    foreach (DataRow o in dtxmlDeleteLog.Select("UserId='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                    {
                                        o["UserId"] = user.Id.ToString();
                                    }
                                }
                            }
                            if (dtxmlLoc_PointRedeemHistory != null)
                            {
                                foreach (DataRow o in dtxmlLoc_PointRedeemHistory.Select("CasherId='" + OldUserId + "'and UName ='" + UName.Replace("'", "''") + "'"))
                                {
                                    o["CasherId"] = user.Id.ToString();
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Loc_PointRedeemHistory

                entity = new APP_Data.POSEntities();
                if (dtxmlLoc_PointRedeemHistory != null)
                {
                    label1.Text = "Step 8 of 32 : Processing Point Redeem History table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlLoc_PointRedeemHistory.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //Loop through dataRow come from xml and check if the Loc_PointRedeemHistory is already exist or Loc_PointRedeemHistory is new one
                    foreach (DataRow dataRowFrmXML in dtxmlLoc_PointRedeemHistory.Rows)
                    {

                        Progressbar1.PerformStep();
                        Boolean SameRowExist = false;
                        int CustomerId = Convert.ToInt32(dataRowFrmXML["CustomerId"].ToString());
                        DateTime DateTime = Convert.ToDateTime(dataRowFrmXML["DateTime"].ToString());
                        int redeemAmt = Convert.ToInt32(dataRowFrmXML["RedeemAmount"].ToString());
                        APP_Data.Loc_PointRedeemHistory FoundLoc_PointRedeemHistory = entity.Loc_PointRedeemHistory.Where(x => x.CustomerId == CustomerId && x.DateTime == DateTime && x.RedeemAmount == redeemAmt).FirstOrDefault();
                        //Same Loc_PointRedeemHistory Code Found
                        
                        if (FoundLoc_PointRedeemHistory != null)
                        {
                            SameRowExist = true;
                        }
                        //Found same row,Update the Current One
                        if (SameRowExist)
                        {
                            GetLoc_PointRedeemHistory(dataRowFrmXML, FoundLoc_PointRedeemHistory);
                            entity.SaveChanges();
                        }
                        else
                        {
                            APP_Data.Loc_PointRedeemHistory loc_pointRedeemHistory = new APP_Data.Loc_PointRedeemHistory();
                            GetLoc_PointRedeemHistory(dataRowFrmXML, loc_pointRedeemHistory);
                            entity.Loc_PointRedeemHistory.Add(loc_pointRedeemHistory);
                            entity.SaveChanges();
                        }
                    }
                }

                #endregion

                #region Loc_CustomerPoint

                entity = new APP_Data.POSEntities();
                if (dtxmlLoc_CustomerPoint != null)
                {//Loop through dataRow come from xml and check if the Loc_CustomerPoint is already exist or Loc_CustomerPoint is new one
                    label1.Text = "Step 9 of 32 : Processing Customer Point table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlLoc_CustomerPoint.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    foreach (DataRow dataRowFrmXML in dtxmlLoc_CustomerPoint.Rows)
                    {

                        Progressbar1.PerformStep();
                        Boolean SameRowExist = false;
                        int customerId = Convert.ToInt32(dataRowFrmXML["CustomerId"].ToString());

                        int OldPoint = Convert.ToInt32(dataRowFrmXML["OldPoint"].ToString());
                        int TotalRedeemPoint = Convert.ToInt32(dataRowFrmXML["TotalRedeemPoint"].ToString());

                        APP_Data.Loc_CustomerPoint FoundCustomerPoint = entity.Loc_CustomerPoint.Where(x => x.CustomerId == customerId).FirstOrDefault();
                        //Same Loc_CustomerPoint Code Found
                        if (FoundCustomerPoint != null)
                        {
                            SameRowExist = true;
                        }
                        //Found same row,Update the Current One
                        if (SameRowExist)
                        {
                            if (FoundCustomerPoint.OldPoint < OldPoint && FoundCustomerPoint.TotalRedeemPoint < TotalRedeemPoint)
                            {
                                GetLoc_CustomerPoint(dataRowFrmXML, FoundCustomerPoint);
                                entity.SaveChanges();
                            }
                        }
                        else
                        {
                            APP_Data.Loc_CustomerPoint loc_customerpoint = new APP_Data.Loc_CustomerPoint();
                            GetLoc_CustomerPoint(dataRowFrmXML, loc_customerpoint);
                            entity.Loc_CustomerPoint.Add(loc_customerpoint);
                            entity.SaveChanges();
                        }
                    }
                }

                #endregion

                #region Consigment Counter

                entity = new APP_Data.POSEntities();
                if (dtxmlConsignmentCounter != null)
                {
                    label1.Text = "Step 10 of 32 : Processing Consignment Counter table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlConsignmentCounter.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //loop through dataRow come form xml and check if the Consigment Counter is already exist or Consigment Counter is new one
                    foreach (DataRow dataRowFromXML in dtxmlConsignmentCounter.Rows)
                    {

                        Progressbar1.PerformStep();
                        Boolean sameRowExist = false;
                        string Name = dataRowFromXML["Name"].ToString();
                        string CounterLocation = dataRowFromXML["CounterLocation"].ToString();
                        APP_Data.ConsignmentCounter FoundConsignmentCounter = entity.ConsignmentCounters.Where(x => x.Name == Name && x.CounterLocation == CounterLocation).FirstOrDefault();
                        //fount same consignment counter
                        if (FoundConsignmentCounter != null)
                        {
                            sameRowExist = true;
                        }
                        //Found same row ,Update the Current One
                        if (sameRowExist)
                        {
                            GetConsignmentCounterFromXML(dataRowFromXML, FoundConsignmentCounter);
                            entity.SaveChanges();

                            String OldConsignmentCounterId = dataRowFromXML["Id"].ToString();
                            //update Consigment Counter in City Table
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("ConsignmentCounterId='" + OldConsignmentCounterId + "'"))
                                {
                                    o["ConsignmentCounterId"] = FoundConsignmentCounter.Id.ToString();
                                }
                            }
                        }
                        //Consigment Counter is not exist in current database
                        //add new row
                        else
                        {
                            APP_Data.ConsignmentCounter consignmentCounter = new APP_Data.ConsignmentCounter();
                            GetConsignmentCounterFromXML(dataRowFromXML, consignmentCounter);
                            entity.ConsignmentCounters.Add(consignmentCounter);
                            entity.SaveChanges();
                            //Update latest id in other datatable
                            String OldConsignmentCounterId = dataRowFromXML["Id"].ToString();
                            //update city in City Table
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("ConsignmentCounterId='" + OldConsignmentCounterId + "'"))
                                {
                                    o["ConsignmentCounterId"] = consignmentCounter.Id.ToString();
                                }
                            }
                        }
                    }
                }

                #endregion


                #region Unit

                entity = new APP_Data.POSEntities();
                if (dtxmLUnit != null)
                {
                    label1.Text = "Step 11 of 32 : Processing Unit table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmLUnit.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //loop through dataRow come from xml and check if the Unit is already exist or Unit is new one
                    foreach (DataRow dataRowFromXMl in dtxmLUnit.Rows)
                    {

                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        String UnitName = dataRowFromXMl["UnitName"].ToString();
                        APP_Data.Unit FoundUnit = entity.Units.Where(x => x.UnitName == UnitName).FirstOrDefault();
                        if (FoundUnit != null)
                        {
                            SameRowExit = true;
                        }
                        if (SameRowExit)
                        {
                            GetUnitFromXML(dataRowFromXMl, FoundUnit);
                            entity.SaveChanges();

                            string OldUnitId = dataRowFromXMl["Id"].ToString();
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("UnitId='" + OldUnitId + "'"))
                                {
                                    o["UnitId"] = FoundUnit.Id.ToString();
                                }
                            }
                        }
                        else
                        {
                            APP_Data.Unit unit = new APP_Data.Unit();
                            GetUnitFromXML(dataRowFromXMl, unit);
                            entity.Units.Add(unit);
                            entity.SaveChanges();

                            string OldUnitId = dataRowFromXMl["Id"].ToString();
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("UnitId='" + OldUnitId + "'"))
                                {
                                    o["UnitId"] = unit.Id.ToString();
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Product Category

                entity = new APP_Data.POSEntities();
                if (dtxmlProductCategory != null)
                {
                    label1.Text = "Step 12 of 32 : Processing Product Category table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlProductCategory.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //loop through dataRow come from xml and check if the product category is already exist or product category is new one
                    foreach (DataRow dataRowFromXML in dtxmlProductCategory.Rows)
                    {

                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        String Name = dataRowFromXML["Name"].ToString();
                        String Id = dataRowFromXML["Id"].ToString();

                        APP_Data.ProductCategory FoundProductCategory = entity.ProductCategories.Where(x => x.Name == Name).FirstOrDefault();
                        if (FoundProductCategory != null)
                        {
                            SameRowExit = true;
                        }
                        if (SameRowExit)
                        {
                            GetProductCategoryFromXML(dataRowFromXML, FoundProductCategory);
                            entity.SaveChanges();
                            String OldCategoryId = dataRowFromXML["Id"].ToString();

                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("ProductCategoryId='" + OldCategoryId + "'and CName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["ProductCategoryId"] = FoundProductCategory.Id.ToString();
                                }
                            }
                            if (dtxmlProudctSubCategory != null)
                            {
                                foreach (DataRow o in dtxmlProudctSubCategory.Select("ProductCategoryId='" + OldCategoryId + "'and CName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["ProductCategoryId"] = FoundProductCategory.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null && dtxmlGiftSystem.Columns.Contains("FilterCategoryId"))
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("FilterCategoryId='" + OldCategoryId + "'and CName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["FilterCategoryId"] = FoundProductCategory.Id.ToString();
                                }
                            }
                        }
                        else
                        {
                            APP_Data.ProductCategory productCategory = new APP_Data.ProductCategory();
                            GetProductCategoryFromXML(dataRowFromXML, productCategory);
                            entity.ProductCategories.Add(productCategory);
                            entity.SaveChanges();

                            String OldCategoryId = dataRowFromXML["Id"].ToString();
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("ProductCategoryId='" + OldCategoryId + "'and CName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["ProductCategoryId"] = productCategory.Id.ToString();
                                }
                            }
                            if (dtxmlProudctSubCategory != null)
                            {
                                foreach (DataRow o in dtxmlProudctSubCategory.Select("ProductCategoryId='" + OldCategoryId + "'and CName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["ProductCategoryId"] = productCategory.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null && dtxmlGiftSystem.Columns.Contains("FilterCategoryId"))
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("FilterCategoryId='" + OldCategoryId + "'and CName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["FilterCategoryId"] = productCategory.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region ProductSubCategory

                entity = new APP_Data.POSEntities();
                //loop though dataRow come from xml and check if the counter is already exist or counter is new on
                if (dtxmlProudctSubCategory != null)
                {
                    label1.Text = "Step 13 of 32 : Processing Product Sub Category table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlProudctSubCategory.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    foreach (DataRow dataRowFromXML in dtxmlProudctSubCategory.Rows)
                    {

                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        String Name = dataRowFromXML["Name"].ToString();
                        int ProductCategoryId = Convert.ToInt32(dataRowFromXML["ProductCategoryId"].ToString());

                        APP_Data.ProductSubCategory FoundProductSubCategory = entity.ProductSubCategories.Where(x => x.Name == Name && x.ProductCategoryId == ProductCategoryId).FirstOrDefault();
                        if (FoundProductSubCategory != null)
                        {
                            SameRowExit = true;
                        }
                        if (SameRowExit)
                        {
                            GetProductSubCategoryFromXML(dataRowFromXML, FoundProductSubCategory);
                            entity.SaveChanges();

                            int OldProductSubCategoryId = Convert.ToInt32(dataRowFromXML["Id"].ToString());
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("ProductSubCategoryId='" + OldProductSubCategoryId + "'and SCName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["ProductSubCategoryId"] = FoundProductSubCategory.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null && dtxmlGiftSystem.Columns.Contains("FilterSubCategoryId"))
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("FilterSubCategoryId='" + OldProductSubCategoryId + "'and SCName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["FilterSubCategoryId"] = FoundProductSubCategory.Id.ToString();
                                }
                            }
                        }
                        else
                        {
                            APP_Data.ProductSubCategory productSubCategory = new APP_Data.ProductSubCategory();
                            GetProductSubCategoryFromXML(dataRowFromXML, productSubCategory);
                            entity.ProductSubCategories.Add(productSubCategory);
                            entity.SaveChanges();

                            int OldProductSubCategoryId = Convert.ToInt32(dataRowFromXML["Id"].ToString());
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("ProductSubCategoryId='" + OldProductSubCategoryId + "'and SCName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["ProductSubCategoryId"] = productSubCategory.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null && dtxmlGiftSystem.Columns.Contains("FilterSubCategoryId"))
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("FilterSubCategoryId='" + OldProductSubCategoryId + "'and SCName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["FilterSubCategoryId"] = productSubCategory.Id.ToString();
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Tax
                entity = new APP_Data.POSEntities();

                if (dtxmlTax != null)
                {
                    label1.Text = "Step 14 of 32 : Processing Tax table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlTax.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    foreach (DataRow dataFromXML in dtxmlTax.Rows)
                    {

                        Progressbar1.PerformStep();
                        bool SameRowExist = false;
                        string Name = dataFromXML["Name"].ToString();
                        APP_Data.Tax FoundTax = entity.Taxes.Where(x => x.Name == Name).FirstOrDefault();
                        if (FoundTax != null)
                        {
                            SameRowExist = true;
                        }
                        if (SameRowExist)
                        {
                            GetTaxFromXML(dataFromXML, FoundTax);
                            entity.SaveChanges();

                            string OldTaxId = dataFromXML["Id"].ToString();
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("TaxId='" + OldTaxId + "'and TaxName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["TaxId"] = FoundTax.Id.ToString();
                                }
                            }
                        }
                        else
                        {
                            APP_Data.Tax tax = new APP_Data.Tax();
                            GetTaxFromXML(dataFromXML, tax);
                            entity.Taxes.Add(tax);
                            entity.SaveChanges();

                            string OldTaxId = dataFromXML["Id"].ToString();
                            if (dtxmlProduct != null)
                            {
                                foreach (DataRow o in dtxmlProduct.Select("TaxId='" + OldTaxId + "'and TaxName ='" + Name.Replace("'", "''") + "'"))
                                {
                                    o["TaxId"] = tax.Id.ToString();
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Product
                entity = new APP_Data.POSEntities();

                if (dtxmlProduct != null)
                {
                    label1.Text = "Step 15 of 32 : Processing Product table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlProduct.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //Loop through dataRow come from xml and check if the product is already exist or product is new one
                    foreach (DataRow dataRowFromXML in dtxmlProduct.Rows)
                    {
                        Progressbar1.PerformStep();
                        Boolean SameRowExist = false;
                        String ProductCode = dataRowFromXML["ProductCode"].ToString();
                        String Barcode = dataRowFromXML["BarCode"].ToString();
                        DateTime UpdateDate = Convert.ToDateTime(dataRowFromXML["UpdateDate"].ToString());
                        bool IsWrapperItem = Convert.ToBoolean(dataRowFromXML["IsWrapper"].ToString());
                        string Price = dataRowFromXML["Price"].ToString();

                        APP_Data.Product FoundProduct = entity.Products.Where(x => x.ProductCode == ProductCode).FirstOrDefault();
                        APP_Data.Product FoundProductWithBarcode = entity.Products.Where(x => x.Barcode == Barcode).FirstOrDefault();


                        string id = dataRowFromXML["Id"].ToString();

                        if (id == "46485")
                        {
                            var a = id;
                        }
                        //   Same Product Code Found
                        if (FoundProduct != null)
                        {
                            SameRowExist = true;
                        }
                        //   Same Barcode Found
                        if (FoundProductWithBarcode != null)
                        {
                            SameRowExist = true;
                            FoundProduct = FoundProductWithBarcode;
                        }
                        // Found same row,Update the Current One
                        if (SameRowExist)
                        {
                            GetProductFromXML(dataRowFromXML, FoundProduct);
                            entity.SaveChanges();

                            String oldProductId = dataRowFromXML["Id"].ToString();
                            if (dtxmlTransactionDetail != null)
                            {
                                foreach (DataRow o in dtxmlTransactionDetail.Select("ProductId = '" + oldProductId + "' and ProductCode ='" + ProductCode + "' "))
                                {
                                    o["ProductId"] = FoundProduct.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null && dtxmlGiftSystem.Columns.Contains("MustIncludeProductId"))
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("MustIncludeProductId = '" + oldProductId + "' and MustProductCode ='" + ProductCode + "'"))
                                {
                                    o["MustIncludeProductId"] = FoundProduct.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null && dtxmlGiftSystem.Columns.Contains("GiftProductId"))
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("GiftProductId = '" + oldProductId + "' and GiftProductCode ='" + ProductCode + "'"))
                                {
                                    o["GiftProductId"] = FoundProduct.Id.ToString();
                                }
                            }
                            if (dtxmlProductInNovelty != null)
                            {
                                foreach (DataRow o in dtxmlProductInNovelty.Select("ProductId = '" + oldProductId + "' and ProductCode ='" + ProductCode + "'"))
                                {
                                    o["ProductId"] = FoundProduct.Id.ToString();
                                }
                            }
                            if (dtxmlWrapperItem != null && dtxmlWrapperItem.Columns.Contains("ParentProductId"))
                            {
                                foreach (DataRow o in dtxmlWrapperItem.Select("ParentProductId = '" + oldProductId + "' and ParentProductCode ='" + ProductCode + "'"))
                                {
                                    o["ParentProductId"] = FoundProduct.Id.ToString();
                                }
                            }
                            if (dtxmlWrapperItem != null && dtxmlWrapperItem.Columns.Contains("ChildProductId"))
                            {
                                foreach (DataRow o in dtxmlWrapperItem.Select("ChildProductId = '" + oldProductId + "' and ChildProductCode ='" + ProductCode + "'"))
                                {
                                    o["ChildProductId"] = FoundProduct.Id.ToString();
                                }
                            }
                            if (dtxmlProductPriceChange != null && dtxmlProductPriceChange.Columns.Contains("ProductId"))
                            {
                                foreach (DataRow o in dtxmlProductPriceChange.Select("ProductId='" + oldProductId + "' and ProductCode ='" + ProductCode + "'"))
                                {
                                    o["ProductId"] = FoundProduct.Id.ToString();
                                }
                            }

                            if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("ParentProductID"))
                            {
                                foreach (DataRow o in dtxmlSPDetail.Select("ParentProductID='" + oldProductId + "' and ParentProductCode ='" + ProductCode + "'"))
                                {
                                    o["ParentProductID"] = FoundProduct.Id.ToString();
                                }
                            }
                            if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("ChildProductID"))
                            {
                                foreach (DataRow o in dtxmlSPDetail.Select("ChildProductID='" + oldProductId + "' and ChildProductCode ='" + ProductCode + "'"))
                                {
                                    o["ChildProductID"] = FoundProduct.Id.ToString();
                                }
                            }
                        }
                        //  product code & barcode is not exist in the current database
                        //  add new row
                        else
                        {
                            APP_Data.Product product = new APP_Data.Product();
                            GetProductFromXML(dataRowFromXML, product);
                            product.Qty = 0;
                            entity.Products.Add(product);
                            entity.SaveChanges();
                            // update latest id in other DataTables

                            String oldProductId = dataRowFromXML["Id"].ToString();

                            //  update in transactionDetail DataTables
                            if (dtxmlTransactionDetail != null)
                            {
                                foreach (DataRow o in dtxmlTransactionDetail.Select("ProductId = '" + oldProductId + "' and ProductCode ='" + ProductCode + "' "))
                                {
                                    o["ProductId"] = product.Id.ToString();
                                }
                            }
                            if (dtxmlGiftSystem != null && dtxmlGiftSystem.Columns.Contains("MustIncludeProductId"))
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("MustIncludeProductId = '" + oldProductId + "' and MustProductCode ='" + ProductCode + "'"))
                                {
                                    o["MustIncludeProductId"] = product.Id.ToString();
                                }
                            }

                            if (dtxmlGiftSystem != null && dtxmlGiftSystem.Columns.Contains("GiftProductId"))
                            {
                                foreach (DataRow o in dtxmlGiftSystem.Select("GiftProductId = '" + oldProductId + "' and GiftProductCode ='" + ProductCode + "'"))
                                {
                                    o["GiftProductId"] = product.Id.ToString();
                                }
                            }
                            if (dtxmlProductInNovelty != null)
                            {
                                foreach (DataRow o in dtxmlProductInNovelty.Select("ProductId = '" + oldProductId + "' and ProductCode ='" + ProductCode + "'"))
                                {
                                    o["ProductId"] = product.Id.ToString();
                                }
                            }
                            if (dtxmlWrapperItem != null && dtxmlWrapperItem.Columns.Contains("ParentProductId"))
                            {
                                foreach (DataRow o in dtxmlWrapperItem.Select("ParentProductId = '" + oldProductId + "' and ParentProductCode ='" + ProductCode + "'"))
                                {
                                    o["ParentProductId"] = product.Id.ToString();
                                }
                            }
                            if (dtxmlWrapperItem != null && dtxmlWrapperItem.Columns.Contains("ChildProductId"))
                            {
                                foreach (DataRow o in dtxmlWrapperItem.Select("ChildProductId = '" + oldProductId + "' and ChildProductCode ='" + ProductCode + "'"))
                                {
                                    o["ChildProductId"] = product.Id.ToString();
                                }
                            }
                            if (dtxmlProductPriceChange != null && dtxmlProductPriceChange.Columns.Contains("ProductId"))
                            {
                                foreach (DataRow o in dtxmlProductPriceChange.Select("ProductId='" + oldProductId + "' and ProductCode ='" + ProductCode + "'"))
                                {
                                    o["ProductId"] = product.Id.ToString();
                                }
                            }

                            if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("ParentProductID"))
                            {
                                foreach (DataRow o in dtxmlSPDetail.Select("ParentProductID='" + oldProductId + "' and ParentProductCode ='" + ProductCode + "'"))
                                {
                                    o["ParentProductID"] = product.Id.ToString();
                                }
                            }
                            if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("ChildProductID"))
                            {
                                foreach (DataRow o in dtxmlSPDetail.Select("ChildProductID='" + oldProductId + "' and ChildProductCode ='" + ProductCode + "'"))
                                {
                                    o["ChildProductID"] = product.Id.ToString();
                                }
                            }
                        }
                    }
                }
                if (dtxmlTransactionDetail != null)
                { }

                #endregion

                #region WrapperItem

                entity = new APP_Data.POSEntities();
                if (dtxmlWrapperItem != null)
                {
                    label1.Text = "Step 16 of 32 : Processing Wrapper Item table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlWrapperItem.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    foreach (DataRow dataFromXML in dtxmlWrapperItem.Rows)
                    {
                        label1.Text = "Records Wrapper Item Processing ";
                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        long ParentProductId = Convert.ToInt64(dataFromXML["ParentProductId"].ToString());
                        long ChildProductId = Convert.ToInt64(dataFromXML["ChildProductId"].ToString());

                        APP_Data.WrapperItem FoundWrapper = entity.WrapperItems.Where(x => x.ParentProductId == ParentProductId && x.ChildProductId == ChildProductId).FirstOrDefault();
                        if (FoundWrapper != null)
                        {
                            SameRowExit = true;
                        }
                        if (SameRowExit)
                        {
                            GetWrapperItemFromXML(dataFromXML, FoundWrapper);
                            entity.SaveChanges();
                        }
                        else
                        {
                            APP_Data.WrapperItem wpObj = new APP_Data.WrapperItem();
                            GetWrapperItemFromXML(dataFromXML, wpObj);
                            entity.WrapperItems.Add(wpObj);
                            entity.SaveChanges();
                        }
                    }
                }

                #endregion

                #region Shop
                entity = new APP_Data.POSEntities();
                if (dtxmlShop != null)
                {
                    label1.Text = "Step 17 of 32 : Processing Shop table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlShop.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //loop through dataRow from xml and check if the shop is already exist or newone.
                    foreach (DataRow dataFromXML in dtxmlShop.Rows)
                    {
                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        string ShopName = dataFromXML["ShopName"].ToString();
                        string Address = dataFromXML["Address"].ToString();
                        APP_Data.Shop FoundShop = entity.Shops.Where(x => x.ShopName == ShopName && x.Address == Address).FirstOrDefault();
                        //same shop name found
                        if (FoundShop != null)
                        {
                            SameRowExit = true;
                        }
                        //Found same row,Update the Current One
                        if (SameRowExit)
                        {
                            GetShopFromXML(dataFromXML, FoundShop);
                            entity.SaveChanges();

                            string OldShopId = dataFromXML["Id"].ToString();
                            //update in transaction DataTables
                            if (dtxmlTransaction != null)
                            {

                                foreach (DataRow o in dtxmlTransaction.Select("ShopId='" + OldShopId + "' and ShopName ='" + ShopName.Replace("'", "''") + "' and SAddress ='" + Address + "'"))
                                {
                                    o["ShopId"] = FoundShop.Id.ToString();
                                }
                            }
                        }
                        //shop name is not exist in the current database
                        //add new row
                        else
                        {
                            APP_Data.Shop shop = new APP_Data.Shop();
                            GetShopFromXML(dataFromXML, shop);
                            entity.Shops.Add(shop);
                            entity.SaveChanges();
                            //update latest id in other DataTables
                            string OldShopId = dataFromXML["Id"].ToString();
                            //update in transaction DataTables
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("ShopId='" + OldShopId + "' and ShopName ='" + ShopName.Replace("'", "''") + "' and SAddress ='" + Address + "'"))
                                {
                                    o["ShopId"] = shop.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Currency


                entity = new APP_Data.POSEntities();
                if (dtxmlCurrency != null)
                {
                    label1.Text = "Step 18 of 32 : Processing Currency table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlCurrency.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //through rowdata come from xml and check if the currency is already exists or new one;
                    foreach (DataRow dataFromXML in dtxmlCurrency.Rows)
                    {
                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        string country = dataFromXML["Country"].ToString();
                        int latestExchangeRate = Convert.ToInt32(dataFromXML["LatestExchangeRate"].ToString());
                        APP_Data.Currency FoundCurrency = entity.Currencies.Where(x => x.Country == country).FirstOrDefault();
                        if (FoundCurrency != null)
                        {
                            SameRowExit = true;
                        }
                        if (SameRowExit)
                        {
                            if (latestExchangeRate < FoundCurrency.LatestExchangeRate)
                            {
                                GetCurrencyFromXML(dataFromXML, FoundCurrency);
                                entity.SaveChanges();
                            }

                            int oldCurrencyId = Convert.ToInt32(dataFromXML["Id"].ToString());
                            if (dtxmlExchangeRateForTransaction != null)
                            {
                                foreach (DataRow o in dtxmlExchangeRateForTransaction.Select("CurrencyId='" + oldCurrencyId + "'"))
                                {
                                    o["CurrencyId"] = FoundCurrency.Id.ToString();
                                }
                            }
                            if (dtxmlTransaction != null)
                            {
                                if (dtxmlTransaction.Columns.Contains("ReceivedCurrencyId"))
                                {
                                    foreach (DataRow o in dtxmlTransaction.Select("ReceivedCurrencyId='" + oldCurrencyId + "'"))
                                    {
                                        o["ReceivedCurrencyId"] = FoundCurrency.Id.ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            APP_Data.Currency currency = new APP_Data.Currency();
                            GetCurrencyFromXML(dataFromXML, currency);
                            entity.Currencies.Add(currency);
                            entity.SaveChanges();

                            int oldCurrencyId = Convert.ToInt32(dataFromXML["Id"].ToString());
                            if (dtxmlExchangeRateForTransaction != null)
                            {
                                foreach (DataRow o in dtxmlExchangeRateForTransaction.Select("CurrencyId='" + oldCurrencyId + "'"))
                                {
                                    o["CurrencyId"] = currency.Id.ToString();
                                }
                            }
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("ReceivedCurrencyId='" + oldCurrencyId + "'"))
                                {
                                    o["ReceivedCurrencyId"] = currency.Id.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region PaymentType

                entity = new APP_Data.POSEntities();
                if (dtxmlPaymentType != null)
                {
                    label1.Text = "Step 19 of 32 : Processing Payment Type table please wait!";
                    label1.Refresh();
                    Progressbar1.Minimum = 1;
                    Progressbar1.Maximum = dtxmlPaymentType.Rows.Count;
                    Progressbar1.Value = 1;
                    Progressbar1.Step = 1;
                    //through rowdata come from xml and check if the currency is already exists or new one;
                    foreach (DataRow dataFromXML in dtxmlPaymentType.Rows)
                    {
                        Progressbar1.PerformStep();
                        bool SameRowExit = false;
                        string Name = dataFromXML["Name"].ToString();

                        APP_Data.PaymentType FoundPaymentType = entity.PaymentTypes.Where(x => x.Name == Name).FirstOrDefault();
                        if (FoundPaymentType != null)
                        {
                            SameRowExit = true;
                        }
                        if (SameRowExit)
                        {
                            GetPaymentTypeFromXML(dataFromXML, FoundPaymentType);
                            entity.SaveChanges();

                            int OldPaymentTypeId = Convert.ToInt32(dataFromXML["Id"].ToString());
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("PaymentTypeId='" + OldPaymentTypeId + "'"))
                                {
                                    o["PaymentTypeId"] = FoundPaymentType.Id.ToString();
                                }
                            }
                        }
                        else
                        {
                            APP_Data.PaymentType paymentType = new APP_Data.PaymentType();
                            GetPaymentTypeFromXML(dataFromXML, paymentType);
                            entity.PaymentTypes.Add(paymentType);
                            entity.SaveChanges();

                            int OldPaymentTypeId = Convert.ToInt32(dataFromXML["Id"].ToString());
                            if (dtxmlTransaction != null)
                            {
                                foreach (DataRow o in dtxmlTransaction.Select("PaymentTypeId='" + OldPaymentTypeId + "'"))
                                {
                                    o["PaymentTypeId"] = paymentType.Id.ToString();
                                }
                            }
                        }
                    }
                    #endregion

                    #region Setting

                    //    entity = new APP_Data.POSEntities();
                    //    if (dtxmlSetting != null)
                    //    {
                    //        //through rowdata come from xml and check if the currency is already exists or new one;
                    //        foreach (DataRow dataFromXML in dtxmlSetting.Rows)
                    //        {
                    //            bool SameRowExit = false;
                    //            string Key = dataFromXML["Key"].ToString();
                    //            APP_Data.Setting FoundSetting = entity.Settings.Where(x => x.Key == Key).FirstOrDefault();
                    //            if (FoundSetting != null)
                    //            {
                    //                SameRowExit = true;
                    //            }
                    //            if (SameRowExit)
                    //            {
                    //                GetSettingTypeFromXML(dataFromXML, FoundSetting);
                    //                entity.SaveChanges();

                    //            }
                    //            else
                    //            {
                    //                APP_Data.Setting setting = new APP_Data.Setting();
                    //                GetSettingTypeFromXML(dataFromXML, setting);
                    //                entity.Settings.Add(setting);
                    //                entity.SaveChanges();
                    //            }
                    //        }
                    //    }
                    #endregion

                    #region GiftCard

                    entity = new APP_Data.POSEntities();
                    if (dtxmlGiftCard != null)
                    {
                        label1.Text = "Step 20 of 32 : Processing Gift Card table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlGiftCard.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //through rowdata come from xml and check if the GiftCard is already existed or new one;
                        foreach (DataRow dataFromXML in dtxmlGiftCard.Rows)
                        {

                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            string CardNumber = dataFromXML["CardNumber"].ToString();
                            //bool isUsed = Convert.ToBoolean(dataFromXML["IsUsed"].ToString());
                            //DateTime ExpireDate = Convert.ToDateTime(dataFromXML["ExpireDate"].ToString());
                            //long Amount = Convert.ToInt64(dataFromXML["Amount"].ToString());
                            DateTime IsUsedDate = Convert.ToDateTime(dataFromXML["IsUsedDate"].ToString());
                            // ZP
                            if (CardNumber == "NCYGN2477")
                            {

                            }
                            APP_Data.GiftCard FoundGiftCard = entity.GiftCards.Where(x => x.CardNumber == CardNumber && x.IsDeleted == false).FirstOrDefault();
                            //test

                            if (FoundGiftCard != null)
                            {
                                SameRowExit = true;
                            }
                            if (SameRowExit)
                            {

                                if (FoundGiftCard.IsUsedDate <= IsUsedDate)
                                {
                                    GetGiftCardFromXML(dataFromXML, FoundGiftCard);
                                    entity.SaveChanges();
                                    int OldGiftCardId = Convert.ToInt32(dataFromXML["Id"].ToString());
                                    string GCardNumber = Convert.ToString(dataFromXML["CardNumber"].ToString());
                                    if (dtxmlGiftCardInTransaction != null)
                                    {
                                        foreach (DataRow o in dtxmlGiftCardInTransaction.Select("GiftCardId='" + OldGiftCardId + "' and CardNumber ='" + GCardNumber.Replace("'", "''") + "'"))
                                        {
                                            o["GiftCardId"] = FoundGiftCard.Id.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    int OldGiftCardId = Convert.ToInt32(dataFromXML["Id"].ToString());
                                    string GCardNumber = Convert.ToString(dataFromXML["CardNumber"].ToString());
                                    if (dtxmlGiftCardInTransaction != null)
                                    {
                                        foreach (DataRow o in dtxmlGiftCardInTransaction.Select("GiftCardId='" + OldGiftCardId + "' and CardNumber ='" + GCardNumber.Replace("'", "''") + "'"))
                                        {
                                            o["GiftCardId"] = FoundGiftCard.Id.ToString();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                APP_Data.GiftCard giftcard = new APP_Data.GiftCard();
                                GetGiftCardFromXML(dataFromXML, giftcard);
                                entity.GiftCards.Add(giftcard);
                                entity.SaveChanges();

                                int OldGiftCardId = Convert.ToInt32(dataFromXML["Id"].ToString());
                                string GCardNumber = Convert.ToString(dataFromXML["CardNumber"].ToString());
                                if (dtxmlGiftCardInTransaction != null)
                                {
                                    foreach (DataRow o in dtxmlGiftCardInTransaction.Select("GiftCardId='" + OldGiftCardId + "' and CardNumber ='" + GCardNumber.Replace("'", "''") + "'"))
                                    {
                                        o["GiftCardId"] = giftcard.Id.ToString();
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region GiftSystem

                    entity = new APP_Data.POSEntities();
                    if (dtxmlGiftSystem != null)
                    {//through rowdata come from xml and check if the GiftCard is already existed or new one;
                        label1.Text = "Step 21 of 32 : Processing Gift System table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlGiftSystem.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        foreach (DataRow dataFromXML in dtxmlGiftSystem.Rows)
                        {

                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            string Name = dataFromXML["Name"].ToString();
                            //string SFromdate = dataFromXML["ValidFrom"].ToString();
                            //DateTime Fromdate = Convert.ToDateTime(SFromdate);
                            //Fromdate = Fromdate.Date;
                            //string STodate = dataFromXML["ValidTo"].ToString();
                            //DateTime Todate = Convert.ToDateTime(STodate);
                            //Todate = Todate.Date;

                            APP_Data.GiftSystem FoundGiftSystem = entity.GiftSystems.Where(x => x.Name == Name).FirstOrDefault();

                            if (FoundGiftSystem != null)
                            {
                                SameRowExit = true;
                            }
                            if (SameRowExit)
                            {
                                GetGiftSystemFromXML(dataFromXML, FoundGiftSystem);
                                entity.SaveChanges();

                                string OldGiftSystemId = dataFromXML["Id"].ToString();

                                if (Name.Contains("'"))
                                {
                                    Name = Name.Replace("'", "\"");
                                }

                                if (dtxmlAttachGiftSystemForTransaction != null && dtxmlAttachGiftSystemForTransaction.Columns.Contains("AttachGiftSystemId"))
                                {
                                    foreach (DataRow o in dtxmlAttachGiftSystemForTransaction.Select("AttachGiftSystemId ='" + OldGiftSystemId + "' and Name ='" + Name.Replace("'", "''") + "'"))
                                    {
                                        o["AttachGiftSystemId"] = FoundGiftSystem.Id.ToString();
                                    }
                                }
                            }
                            else
                            {
                                APP_Data.GiftSystem giftSystem = new APP_Data.GiftSystem();
                                GetGiftSystemFromXML(dataFromXML, giftSystem);
                                entity.GiftSystems.Add(giftSystem);
                                entity.SaveChanges();

                                string OldGiftSystemId = dataFromXML["Id"].ToString();
                                if (dtxmlAttachGiftSystemForTransaction != null && dtxmlAttachGiftSystemForTransaction.Columns.Contains("AttachGiftSystemId"))
                                {
                                    foreach (DataRow o in dtxmlAttachGiftSystemForTransaction.Select("AttachGiftSystemId ='" + OldGiftSystemId + "' and Name ='" + Name.Replace("'", "''") + "'"))
                                    {
                                        o["AttachGiftSystemId"] = giftSystem.Id.ToString();
                                    }
                                }
                            }
                        }
                    }
                    #endregion                  

                    #region Transaction


                    entity = new APP_Data.POSEntities();
                    List<int> TranIdList = new List<int>();
                    if (dtxmlTransaction != null)
                    {
                        label1.Text = "Step 22 of 32 : Processing Transaction table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlTransaction.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //Loop through dataRow come from xml and check if the transaction is already exist or a new one
                        DateTime today = DateTime.Today;
                        DateTime TranDate = DateTime.Today.AddMonths(-5);
                        DataTable _TranProcessing = new DataTable();

                        foreach (DataRow dataRowFromXML in dtxmlTransaction.Rows)
                        {

                            Progressbar1.PerformStep();
                            Boolean SameRowExist = false;
                            String Id = dataRowFromXML["Id"].ToString();


                            APP_Data.Transaction FoundTransaction = entity.Transactions.Where(x => x.Id == Id).FirstOrDefault();
                            //Same TransactionId Found
                            if (FoundTransaction != null)
                            {
                                SameRowExist = true;
                            }
                            //Found same row,Update the Current One if xml transactions have more recent updatedDate
                            if (SameRowExist)
                            {
                                DateTime currentUpdateDateFromXML = Convert.ToDateTime(dataRowFromXML["UpdatedDate"].ToString());
                                if (FoundTransaction.UpdatedDate < currentUpdateDateFromXML)
                                {
                                    GetTransactionFromXML(dataRowFromXML, FoundTransaction);
                                    entity.SaveChanges();

                                    string OldTransactionId = dataRowFromXML["Id"].ToString();
                                    if (dtxmlTransactionDetail != null && dtxmlTransactionDetail.Columns.Contains("TransactionId"))
                                    {
                                        foreach (DataRow o in dtxmlTransactionDetail.Select("TransactionId ='" + OldTransactionId + "'"))
                                        {
                                            o["TransactionId"] = FoundTransaction.Id.ToString();
                                        }
                                    }
                                    if (dtxmlAttachGiftSystemForTransaction != null && dtxmlAttachGiftSystemForTransaction.Columns.Contains("TransactionId"))
                                    {
                                        foreach (DataRow o in dtxmlAttachGiftSystemForTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                                        {
                                            o["TransactionId"] = FoundTransaction.Id.ToString();
                                        }
                                    }
                                    if (dtxmlDeleteLog != null && dtxmlDeleteLog.Columns.Contains("TransactionId"))
                                    {
                                        foreach (DataRow o in dtxmlDeleteLog.Select("TransactionId ='" + OldTransactionId + "'"))
                                        {
                                            o["TransactionId"] = FoundTransaction.Id.ToString();
                                        }
                                    }
                                    if (dtxmlExchangeRateForTransaction != null && dtxmlExchangeRateForTransaction.Columns.Contains("TransactionId"))
                                    {
                                        foreach (DataRow o in dtxmlExchangeRateForTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                                        {
                                            o["TransactionId"] = FoundTransaction.Id.ToString();
                                        }
                                    }
                                    if (dtxmlGiftCardInTransaction != null && dtxmlGiftCardInTransaction.Columns.Contains("TransactionId"))
                                    {
                                        foreach (DataRow o in dtxmlGiftCardInTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                                        {
                                            o["TransactionId"] = FoundTransaction.Id.ToString();
                                        }
                                    }
                                    if (dtxmlUsePrePaidDebt != null && dtxmlUsePrePaidDebt.Columns.Contains("TransactionId"))
                                    {
                                        foreach (DataRow o in dtxmlUsePrePaidDebt.Select("CreditTransactionId ='" + OldTransactionId + "'"))
                                        {
                                            o["CreditTransactionId"] = FoundTransaction.Id.ToString();
                                        }
                                    }
                                    if (dtxmlUsePrePaidDebt != null && dtxmlUsePrePaidDebt.Columns.Contains("PrePaidDebtTransactionId"))
                                    {
                                        foreach (DataRow o in dtxmlUsePrePaidDebt.Select("PrePaidDebtTransactionId ='" + OldTransactionId + "'"))
                                        {
                                            o["PrePaidDebtTransactionId"] = FoundTransaction.Id.ToString();
                                        }
                                    }
                                }
                            }
                            //Transaction Id is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.Transaction ts = new APP_Data.Transaction();
                                APP_Data.Transaction tsRefund = new APP_Data.Transaction();

                                //Check  for transaction type whether sale or credit or sale or prepaid or fefund or credit refund 
                                if (dataRowFromXML["Type"].ToString() == "Refund" || dataRowFromXML["Type"].ToString() == "CreditRefund")
                                {
                                    //Get  sale or credit transaction Id of  current refund or credit refund transaction.
                                    string OldParentId = dataRowFromXML["ParentId"].ToString();
                                    //chcek at main Main Database whether having Transaction about parentId of current refund  or credit refund or debt transaction.
                                    APP_Data.Transaction tsParentId = entity.Transactions.Where(x => x.Id == OldParentId).FirstOrDefault();
                                    if (tsParentId != null)
                                    {
                                        GetTransactionFromXML(dataRowFromXML, ts);
                                        entity.Transactions.Add(ts);
                                        entity.SaveChanges();
                                    }
                                    else
                                    {
                                        foreach (DataRow dr in dtxmlTransaction.Select("Id ='" + OldParentId + "'"))
                                        {
                                            if (dr != null && dr["Type"].ToString() != "Refund" && dr["Type"].ToString() != "CreditRefund")
                                            {
                                                GetTransactionFromXML(dr, tsRefund);
                                                entity.Transactions.Add(tsRefund);
                                                entity.SaveChanges();

                                                string OldTransactionIdInner = dr["Id"].ToString();
                                                if (dtxmlTransactionDetail != null && dtxmlTransactionDetail.Columns.Contains("TransactionId"))
                                                {
                                                    foreach (DataRow o in dtxmlTransactionDetail.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                                                    {
                                                        o["TransactionId"] = tsRefund.Id.ToString();
                                                    }
                                                }
                                                if (dtxmlAttachGiftSystemForTransaction != null && dtxmlAttachGiftSystemForTransaction.Columns.Contains("TransactionId"))
                                                {
                                                    foreach (DataRow o in dtxmlAttachGiftSystemForTransaction.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                                                    {
                                                        o["TransactionId"] = tsRefund.Id.ToString();
                                                    }
                                                }
                                                if (dtxmlDeleteLog != null && dtxmlDeleteLog.Columns.Contains("TransactionId"))
                                                {
                                                    foreach (DataRow o in dtxmlDeleteLog.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                                                    {
                                                        o["TransactionId"] = tsRefund.Id.ToString();
                                                    }
                                                }
                                                if (dtxmlExchangeRateForTransaction != null && dtxmlExchangeRateForTransaction.Columns.Contains("TransactionId"))
                                                {
                                                    foreach (DataRow o in dtxmlExchangeRateForTransaction.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                                                    {
                                                        o["TransactionId"] = tsRefund.Id.ToString();
                                                    }
                                                }
                                                if (dtxmlGiftCardInTransaction != null && dtxmlGiftCardInTransaction.Columns.Contains("TransactionId"))
                                                {
                                                    foreach (DataRow o in dtxmlGiftCardInTransaction.Select("TransactionId ='" + OldTransactionIdInner + "'"))
                                                    {
                                                        o["TransactionId"] = tsRefund.Id.ToString();
                                                    }
                                                }
                                                if (dtxmlUsePrePaidDebt != null && dtxmlUsePrePaidDebt.Columns.Contains("TransactionId"))
                                                {
                                                    foreach (DataRow o in dtxmlUsePrePaidDebt.Select("CreditTransactionId ='" + OldTransactionIdInner + "'"))
                                                    {
                                                        o["CreditTransactionId"] = tsRefund.Id.ToString();
                                                    }
                                                }
                                                if (dtxmlUsePrePaidDebt != null && dtxmlUsePrePaidDebt.Columns.Contains("PrePaidDebtTransactionId"))
                                                {
                                                    foreach (DataRow o in dtxmlUsePrePaidDebt.Select("PrePaidDebtTransactionId ='" + OldTransactionIdInner + "'"))
                                                    {
                                                        o["PrePaidDebtTransactionId"] = tsRefund.Id.ToString();
                                                    }
                                                }
                                            }
                                        }
                                        GetTransactionFromXML(dataRowFromXML, ts);
                                        entity.Transactions.Add(ts);
                                        entity.SaveChanges();
                                    }
                                }
                                else
                                {
                                    GetTransactionFromXML(dataRowFromXML, ts);
                                    entity.Transactions.Add(ts);
                                    entity.SaveChanges();
                                }

                                string OldTransactionId = dataRowFromXML["Id"].ToString();

                                if (dtxmlTransactionDetail != null && dtxmlTransactionDetail.Columns.Contains("TransactionId"))
                                {
                                    foreach (DataRow o in dtxmlTransactionDetail.Select("TransactionId ='" + OldTransactionId + "'"))
                                    {
                                        o["TransactionId"] = ts.Id.ToString();
                                    }
                                }
                                if (dtxmlAttachGiftSystemForTransaction != null && dtxmlAttachGiftSystemForTransaction.Columns.Contains("TransactionId"))
                                {
                                    foreach (DataRow o in dtxmlAttachGiftSystemForTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                                    {
                                        o["TransactionId"] = ts.Id.ToString();
                                    }
                                }
                                if (dtxmlDeleteLog != null && dtxmlDeleteLog.Columns.Contains("TransactionId"))
                                {
                                    foreach (DataRow o in dtxmlDeleteLog.Select("TransactionId ='" + OldTransactionId + "'"))
                                    {
                                        o["TransactionId"] = ts.Id.ToString();
                                    }
                                }
                                if (dtxmlExchangeRateForTransaction != null && dtxmlExchangeRateForTransaction.Columns.Contains("TransactionId"))
                                {
                                    foreach (DataRow o in dtxmlExchangeRateForTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                                    {
                                        o["TransactionId"] = ts.Id.ToString();
                                    }
                                }
                                if (dtxmlGiftCardInTransaction != null && dtxmlGiftCardInTransaction.Columns.Contains("TransactionId"))
                                {
                                    foreach (DataRow o in dtxmlGiftCardInTransaction.Select("TransactionId ='" + OldTransactionId + "'"))
                                    {
                                        o["TransactionId"] = ts.Id.ToString();
                                    }
                                }
                                if (dtxmlUsePrePaidDebt != null && dtxmlUsePrePaidDebt.Columns.Contains("TransactionId"))
                                {
                                    foreach (DataRow o in dtxmlUsePrePaidDebt.Select("CreditTransactionId ='" + OldTransactionId + "'"))
                                    {
                                        o["CreditTransactionId"] = ts.Id.ToString();
                                    }
                                }
                                if (dtxmlUsePrePaidDebt != null && dtxmlUsePrePaidDebt.Columns.Contains("PrePaidDebtTransactionId"))
                                {
                                    foreach (DataRow o in dtxmlUsePrePaidDebt.Select("PrePaidDebtTransactionId ='" + OldTransactionId + "'"))
                                    {
                                        o["PrePaidDebtTransactionId"] = ts.Id.ToString();
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region TransactionDetail               
                    entity = new APP_Data.POSEntities();

                    if (dtxmlTransactionDetail != null)
                    {
                        //Loop through dataRow come from xml and check if the transaction is already exist or a new one
                        label1.Text = "Step 23 of 32 : Processing Transaction Detail table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        //Progressbar1.Maximum = dtxmlTransactionDetail.Rows.Count;
                        Progressbar1.Maximum = dtxmlTransactionDetail.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;

                        foreach (DataRow dataRowFromXML in dtxmlTransactionDetail.Rows)
                        {
                            Progressbar1.PerformStep();
                            Boolean SameRowExist = false;
                            String TransactionId = dataRowFromXML["TransactionId"].ToString();
                            long ProductId = Convert.ToInt64(dataRowFromXML["ProductId"].ToString());
                            Boolean IsDelete = Convert.ToBoolean(dataRowFromXML["IsDeleted"].ToString());
                            string ProductCode = dataRowFromXML["ProductCode"].ToString();
                            APP_Data.TransactionDetail FoundTransactionDetail = entity.TransactionDetails.Where(x => x.TransactionId == TransactionId && x.ProductId == ProductId).FirstOrDefault();
                            //Same TransactionId Found
                            if (FoundTransactionDetail != null)
                            {
                                SameRowExist = true;
                            }
                            //Found same row,Update the Current One if xml transactions have more recent updatedDate
                            if (SameRowExist)
                            {
                                GetTransactionDetailFromXML(dataRowFromXML, FoundTransactionDetail);
                                entity.SaveChanges();
                                string OldTransactionDetailId = dataRowFromXML["Id"].ToString();
                                if (dtxmlDeleteLog != null && dtxmlDeleteLog.Columns.Contains("TransactionDetailId"))
                                {
                                    foreach (DataRow o in dtxmlDeleteLog.Select("TransactionDetailId ='" + OldTransactionDetailId + "'"))
                                    {
                                        o["TransactionDetailId"] = FoundTransactionDetail.Id.ToString();
                                    }
                                }
                                if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("TransactionDetailID"))
                                {
                                    foreach (DataRow o in dtxmlSPDetail.Select("TransactionDetailID ='" + OldTransactionDetailId + "' and TID ='" + TransactionId + "' and ParentProductCode ='" + ProductCode + "'"))
                                    {
                                        o["TransactionDetailID"] = FoundTransactionDetail.Id.ToString();
                                    }
                                }
                            }
                            //Transaction Id is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.TransactionDetail ts = new APP_Data.TransactionDetail();
                                GetTransactionDetailFromXML(dataRowFromXML, ts);
                                entity.TransactionDetails.Add(ts);
                                entity.SaveChanges();
                                string OldTransactionDetailId = dataRowFromXML["Id"].ToString();
                                if (dtxmlDeleteLog != null && dtxmlDeleteLog.Columns.Contains("TransactionDetailId"))
                                {
                                    foreach (DataRow o in dtxmlDeleteLog.Select("TransactionDetailId ='" + OldTransactionDetailId + "'"))
                                    {
                                        o["TransactionDetailId"] = ts.Id.ToString();
                                    }
                                }
                                if (dtxmlSPDetail != null && dtxmlSPDetail.Columns.Contains("TransactionDetailID"))
                                {
                                    foreach (DataRow o in dtxmlSPDetail.Select("TransactionDetailID ='" + OldTransactionDetailId + "' and TID ='" + TransactionId + "' and ParentProductCode ='" + ProductCode + "'"))
                                    {
                                        o["TransactionDetailID"] = ts.Id.ToString();
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Transaction Payment Detail
                    entity = new APP_Data.POSEntities();

                    if (dtxmlTransactionPaymentDetail != null)
                    {
                        //Loop through dataRow come from xml and check if the transaction is already exist or a new one
                        label1.Text = "Step 24 of 32 : Processing Transaction Payment Detail table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        //Progressbar1.Maximum = dtxmlTransactionDetail.Rows.Count;
                        Progressbar1.Maximum = dtxmlTransactionPaymentDetail.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;

                        foreach (DataRow dataRowFromXML in dtxmlTransactionPaymentDetail.Rows)
                        {
                            Progressbar1.PerformStep();
                            Boolean SameRowExist = false;
                            String TransactionId = dataRowFromXML["TransactionId"].ToString();
                            int paymentMethodId = Convert.ToInt32(dataRowFromXML["PaymentMethodId"].ToString());
                            //long ProductId = Convert.ToInt64(dataRowFromXML["ProductId"].ToString());
                            //Boolean IsDelete = Convert.ToBoolean(dataRowFromXML["IsDeleted"].ToString());
                            //string ProductCode = dataRowFromXML["ProductCode"].ToString();
                            var FoundTransactionPaymentDetail = entity.TransactionPaymentDetails.Where(x => x.TransactionId == TransactionId && x.PaymentMethodId == paymentMethodId).FirstOrDefault();

                            //Same TransactionId Found
                            if (FoundTransactionPaymentDetail != null)
                            {
                                SameRowExist = true;
                            }
                            //Found same row,Update the Current One if xml transactions have more recent updatedDate
                            if (SameRowExist)
                            {
                                GetTransactionPaymentDetailFromXML(dataRowFromXML, FoundTransactionPaymentDetail);
                                entity.SaveChanges();

                            }
                            //Transaction Id is not exist in current database
                            //add new row
                            else
                            {
                                APP_Data.TransactionPaymentDetail ts = new APP_Data.TransactionPaymentDetail();
                                GetTransactionPaymentDetailFromXML(dataRowFromXML, ts);
                                entity.TransactionPaymentDetails.Add(ts);
                                entity.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    #region GiftCardInTransaction

                    entity = new APP_Data.POSEntities();
                    if (dtxmlGiftCardInTransaction != null)
                    {//through rowdata come from xml and check if the GiftCardInTransaction is already existed or new one;
                        label1.Text = "Step 25 of 32 : Processing Gift Card In Transaction table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlGiftCardInTransaction.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        foreach (DataRow dataFromXML in dtxmlGiftCardInTransaction.Rows)
                        {
                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            int GiftCardId = Convert.ToInt32(dataFromXML["GiftCardId"].ToString());

                            string TransactionId = Convert.ToString(dataFromXML["TransactionId"].ToString());

                            APP_Data.GiftCardInTransaction FoundGiftCardInInTran = entity.GiftCardInTransactions.Where(x => x.GiftCardId == GiftCardId && x.TransactionId == TransactionId).FirstOrDefault();

                            if (FoundGiftCardInInTran != null)
                            {
                                SameRowExit = true;
                            }
                            if (SameRowExit)
                            {
                                GetGiftCardInInTranFromXML(dataFromXML, FoundGiftCardInInTran);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.GiftCardInTransaction giftCardInTran = new APP_Data.GiftCardInTransaction();
                                GetGiftCardInInTranFromXML(dataFromXML, giftCardInTran);
                                entity.GiftCardInTransactions.Add(giftCardInTran);
                                entity.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    #region ExchangeRateForTransaction

                    entity = new APP_Data.POSEntities();

                    if (dtxmlExchangeRateForTransaction != null)
                    {
                        label1.Text = "Step 26 of 32 : Processing Exchange Rate For Transaction table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlExchangeRateForTransaction.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through the dataRow come from xml and check if ExchangeRateForTransaction is already existed or new one
                        foreach (DataRow dataFromXML in dtxmlExchangeRateForTransaction.Rows)
                        {

                            Progressbar1.PerformStep();
                            bool SameRowExist = false;
                            String TransactionId = dataFromXML["TransactionId"].ToString();
                            APP_Data.ExchangeRateForTransaction FoundExchangeRate = entity.ExchangeRateForTransactions.Where(x => x.TransactionId == TransactionId).FirstOrDefault();
                            if (FoundExchangeRate != null)
                            {
                                SameRowExist = true;
                            }
                            if (SameRowExist)
                            {
                                GetExchangeReateForTansction(dataFromXML, FoundExchangeRate);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.ExchangeRateForTransaction exchageRate = new APP_Data.ExchangeRateForTransaction();
                                GetExchangeReateForTansction(dataFromXML, exchageRate);
                                entity.ExchangeRateForTransactions.Add(exchageRate);
                                entity.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region Deletelog

                    entity = new APP_Data.POSEntities();

                    if (dtxmlDeleteLog != null)
                    {
                        label1.Text = "Step 27 of 32 : Processing Delete Log table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlDeleteLog.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow from xml and check if the deletelog is already exist  or a new one
                        foreach (DataRow dataFromXML in dtxmlDeleteLog.Rows)
                        {

                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            string transactionId = dataFromXML["TransactionId"].ToString();
                            APP_Data.DeleteLog FoundDeleteLog = entity.DeleteLogs.Where(x => x.TransactionId == transactionId).FirstOrDefault();

                            if (FoundDeleteLog != null)
                            {
                                SameRowExit = true;
                            }
                            if (SameRowExit)
                            {
                                GetDeleteLogFromXML(dataFromXML, FoundDeleteLog);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.DeleteLog deleteLog = new APP_Data.DeleteLog();
                                GetDeleteLogFromXML(dataFromXML, deleteLog);
                                entity.DeleteLogs.Add(deleteLog);
                                entity.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region AttachGiftSystemForTransaction

                    entity = new APP_Data.POSEntities();
                    if (dtxmlAttachGiftSystemForTransaction != null)
                    {
                        label1.Text = "Step 28 of 32 : Processing Attach Gift System For Transaction table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlAttachGiftSystemForTransaction.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //through rowdata come from xml and check if the GiftCard is already existed or new one;
                        foreach (DataRow dataFromXML in dtxmlAttachGiftSystemForTransaction.Rows)
                        {

                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            string TransactionId = dataFromXML["TransactionId"].ToString();
                            int AttachGiftSystemId = Convert.ToInt32(dataFromXML["AttachGiftSystemId"].ToString());

                            APP_Data.AttachGiftSystemForTransaction FoundAttachGiftSystemForTransaction = entity.AttachGiftSystemForTransactions.Where(x => x.AttachGiftSystemId == AttachGiftSystemId && x.TransactionId == TransactionId).FirstOrDefault();

                            if (FoundAttachGiftSystemForTransaction != null)
                            {
                                SameRowExit = true;
                            }
                            if (SameRowExit)
                            {
                                FoundAttachGiftSystemForTransactionFromXML(dataFromXML, FoundAttachGiftSystemForTransaction);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.AttachGiftSystemForTransaction attachGiftSystemForTransaction = new APP_Data.AttachGiftSystemForTransaction();
                                FoundAttachGiftSystemForTransactionFromXML(dataFromXML, attachGiftSystemForTransaction);
                                entity.AttachGiftSystemForTransactions.Add(attachGiftSystemForTransaction);
                                entity.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    #region UsePrePaidDebt

                    entity = new APP_Data.POSEntities();

                    if (dtxmlUsePrePaidDebt != null)
                    {
                        label1.Text = "Step 29 of 32 : Processing Use Pre Paid Debt table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlUsePrePaidDebt.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //through rowdata come from xml and check if the GiftCard is already existed or new one;
                        foreach (DataRow dataFromXML in dtxmlUsePrePaidDebt.Rows)
                        {
                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            string CreditTransactionId = dataFromXML["CreditTransactionId"].ToString();
                            string PrePaidDebtTransactionId = dataFromXML["PrePaidDebtTransactionId"].ToString();
                            int UseAmount = Convert.ToInt32(dataFromXML["UseAmount"].ToString());

                            //APP_Data.UsePrePaidDebt FoundUserPrePaidDebt = entity.UsePrePaidDebts.Where(x => x.CreditTransactionId == CreditTransactionId && x.PrePaidDebtTransactionId == PrePaidDebtTransactionId && x.UseAmount == UseAmount).FirstOrDefault();
                            APP_Data.UsePrePaidDebt FoundUserPrePaidDebt = entity.UsePrePaidDebts.Where(x => x.CreditTransactionId == CreditTransactionId).FirstOrDefault();
                            if (FoundUserPrePaidDebt != null)
                            {
                                SameRowExit = true;
                            }
                            if (SameRowExit)
                            {
                                UserPrePaidDebtFromXML(dataFromXML, FoundUserPrePaidDebt);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.UsePrePaidDebt usePrePaidDebt = new APP_Data.UsePrePaidDebt();
                                UserPrePaidDebtFromXML(dataFromXML, usePrePaidDebt);
                                entity.UsePrePaidDebts.Add(usePrePaidDebt);
                                entity.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region NoveltySystem

                    entity = new APP_Data.POSEntities();
                    if (dtxmlNoveltySystem != null)
                    {
                        label1.Text = "Step 30 of 32 : Processing Novelty System table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlNoveltySystem.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        //loop through dataRow from xml and check if the deletelog is already exist  or a new one
                        foreach (DataRow dataFromXML in dtxmlNoveltySystem.Rows)
                        {
                            Progressbar1.PerformStep();
                            bool SameRowExit = false;

                            DateTime validfrom = Convert.ToDateTime(dataFromXML["ValidFrom"]).Date;
                            int BrandId = Convert.ToInt32(dataFromXML["BrandId"]);
                            APP_Data.NoveltySystem FoundNovelty = entity.NoveltySystems.Where(x => x.Brand.Id == BrandId && EntityFunctions.TruncateTime(x.ValidFrom) == validfrom).FirstOrDefault();

                            if (FoundNovelty != null) { SameRowExit = true; } else { SameRowExit = false; }
                            if (!SameRowExit)
                            {
                                Boolean newRow = true;
                                APP_Data.NoveltySystem newNov = new APP_Data.NoveltySystem();
                                GetNoveltySystem(dataFromXML, newNov, newRow);
                                entity.NoveltySystems.Add(newNov);
                                entity.SaveChanges();

                                string OldNoveltySystemId = dataFromXML["Id"].ToString();
                                foreach (DataRow o in dtxmlProductInNovelty.Select("NoveltySystemId='" + OldNoveltySystemId + "'"))
                                {
                                    APP_Data.ProductInNovelty newPdNovObj = new APP_Data.ProductInNovelty();
                                    newPdNovObj.ProductId = Convert.ToInt32(o["ProductId"].ToString());
                                    newPdNovObj.IsDeleted = Convert.ToBoolean(o["IsDeleted"].ToString());
                                    newNov.ProductInNovelties.Add(newPdNovObj);
                                    entity.SaveChanges();
                                }
                            }
                            else
                            {
                                Boolean newRow = false;
                                GetNoveltySystem(dataFromXML, FoundNovelty, newRow);
                                entity.SaveChanges();

                                int NoveltySystemId = Convert.ToInt32(FoundNovelty.ProductInNovelties.Select(x => x.NoveltySystemId).FirstOrDefault());

                                List<APP_Data.ProductInNovelty> productInnovelty = entity.ProductInNovelties.Where(x => x.NoveltySystemId == NoveltySystemId).ToList();

                                int ExcelOldNoveltySystemId = Convert.ToInt32(dataFromXML["Id"].ToString());
                                var dtxmlcount = dtxmlProductInNovelty.Select("NoveltySystemId='" + ExcelOldNoveltySystemId + "'").Count();
                                if (dtxmlcount > productInnovelty.Count())
                                {
                                    foreach (DataRow o in dtxmlProductInNovelty.Select("NoveltySystemId='" + ExcelOldNoveltySystemId + "'"))
                                    {

                                        int productid = Convert.ToInt32(o["ProductId"].ToString());
                                        var existedrow = productInnovelty.Where(x => x.NoveltySystemId == NoveltySystemId && x.ProductId == productid).FirstOrDefault();
                                        if (existedrow == null) // add new productIn novelty
                                        {

                                            ProductInNovelty pNObj = new ProductInNovelty();
                                            pNObj.ProductId = Convert.ToInt32(o["ProductId"].ToString());
                                            pNObj.IsDeleted = Convert.ToBoolean(o["IsDeleted"].ToString());
                                            pNObj.NoveltySystemId = NoveltySystemId;
                                            entity.ProductInNovelties.Add(pNObj);

                                            entity.SaveChanges();

                                        }
                                        else
                                        {
                                            ProductInNovelty poldnov = entity.ProductInNovelties.Where(x => x.ProductId == productid && x.NoveltySystemId == NoveltySystemId).FirstOrDefault();
                                            if (poldnov != null)
                                            {
                                                poldnov.IsDeleted = Convert.ToBoolean(o["IsDeleted"].ToString());
                                                entity.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (DataRow o in dtxmlProductInNovelty.Select("NoveltySystemId='" + ExcelOldNoveltySystemId + "'"))
                                    {
                                        int productid = Convert.ToInt32(o["ProductId"].ToString());
                                        ProductInNovelty poldnov = entity.ProductInNovelties.Where(x => x.ProductId == productid && x.NoveltySystemId == NoveltySystemId).FirstOrDefault();
                                        if (poldnov != null)
                                        {
                                            poldnov.IsDeleted = Convert.ToBoolean(o["IsDeleted"].ToString());
                                            entity.SaveChanges();
                                        }

                                    }

                                }



                            }
                        }
                    }
                    #endregion

                    #region ProductPriceChange

                    entity = new APP_Data.POSEntities();

                    if (dtxmlProductPriceChange != null)
                    {
                        label1.Text = "Step 31 of 32 : Processing Product Price Change table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlProductPriceChange.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        foreach (DataRow dataFromXML in dtxmlProductPriceChange.Rows)
                        {
                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            int SameFieldCount = 0;
                            long pdcId = 0;

                            long ProductId = Convert.ToInt64(dataFromXML["ProductId"].ToString());
                            DateTime UpdateDate = Convert.ToDateTime(dataFromXML["UpdateDate"].ToString());
                            long newPrice = Convert.ToInt64(dataFromXML["Price"].ToString());
                            long oldPrice = Convert.ToInt64(dataFromXML["OldPrice"].ToString());

                            foreach (APP_Data.ProductPriceChange pdc in entity.ProductPriceChanges)
                            {
                                SameFieldCount = 0;
                                if (pdc.ProductId == ProductId) SameFieldCount++;
                                if (pdc.UpdateDate == UpdateDate) SameFieldCount++;
                                if (pdc.Price == newPrice) SameFieldCount++;
                                if (pdc.OldPrice == oldPrice) SameFieldCount++;

                                if (SameFieldCount >= 4)
                                {
                                    SameRowExit = true;
                                    pdcId = pdc.Id;
                                    break;
                                }
                            }
                            if (SameRowExit)
                            {
                                APP_Data.ProductPriceChange Foundpdc = entity.ProductPriceChanges.Where(x => x.Id == pdcId).FirstOrDefault();
                                GetProductPriceChangeFromXML(dataFromXML, Foundpdc);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.ProductPriceChange pdcObj = new APP_Data.ProductPriceChange();
                                GetProductPriceChangeFromXML(dataFromXML, pdcObj);
                                entity.ProductPriceChanges.Add(pdcObj);
                                entity.SaveChanges();
                            }
                        }
                    }


                    #endregion

                    #region SPDetail
                    entity = new APP_Data.POSEntities();
                    if (dtxmlSPDetail != null)
                    {
                        label1.Text = "Step 32 of 32 : Processing SP Detail table please wait!";
                        label1.Refresh();
                        Progressbar1.Minimum = 1;
                        Progressbar1.Maximum = dtxmlSPDetail.Rows.Count;
                        Progressbar1.Value = 1;
                        Progressbar1.Step = 1;
                        foreach (DataRow dataFromXML in dtxmlSPDetail.Rows)
                        {
                            Progressbar1.PerformStep();
                            bool SameRowExit = false;
                            int SameFieldCount = 0;
                            string spId = "";

                            int TransactionDetailID = Convert.ToInt32(dataFromXML["TransactionDetailID"].ToString());
                            long ParentProductID = Convert.ToInt64(dataFromXML["ParentProductID"].ToString());
                            long ChildProductID = Convert.ToInt64(dataFromXML["ChildProductID"].ToString());
                            string SPDetailID = dataFromXML["SPDetailID"].ToString();

                            foreach (APP_Data.SPDetail sp in entity.SPDetails)
                            {
                                SameFieldCount = 0;
                                if (sp.TransactionDetailID == TransactionDetailID) SameFieldCount++;
                                if (sp.ParentProductID == ParentProductID) SameFieldCount++;
                                if (sp.ChildProductID == ChildProductID) SameFieldCount++;
                                if (sp.SPDetailID == SPDetailID) SameFieldCount++;

                                if (SameFieldCount >= 4)
                                {
                                    SameRowExit = true;
                                    spId = sp.SPDetailID;
                                    break;
                                }
                            }
                            if (SameRowExit)
                            {
                                APP_Data.SPDetail foundSpDetail = entity.SPDetails.Where(x => x.SPDetailID == spId).FirstOrDefault();
                                GetSPDetailFromXML(dataFromXML, foundSpDetail);
                                entity.SaveChanges();
                            }
                            else
                            {
                                APP_Data.SPDetail spDObj = new APP_Data.SPDetail();
                                GetSPDetailFromXML(dataFromXML, spDObj);
                                entity.SPDetails.Add(spDObj);
                                entity.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    label1.Visible = false;
                    Progressbar1.Visible = false;
                    File.Delete(destFileName);
                    MessageBox.Show("Data updating completed!");
                }
            }
        }

        private void GetVIPMemberRuleFromXML(APP_Data.VIPMemberRule foundVIPMebmerRule, DataRow dataFromXML)
        {
            foundVIPMebmerRule.RuleName = dataFromXML["RuleName"].ToString();
            foundVIPMebmerRule.Amount = Convert.ToInt64(dataFromXML["Amount"].ToString());
            foundVIPMebmerRule.Remark = dataFromXML["Remark"].ToString();
            foundVIPMebmerRule.IsCalculatePoints = Convert.ToBoolean(dataFromXML["IsCalculatePoints"].ToString());
        }

        private void GetSPDetailFromXML(DataRow dataFromXML, APP_Data.SPDetail foundSpDetail)
        {
            if (dataFromXML["ParentProductID"].ToString().Trim() != string.Empty && dataFromXML["ParentProductID"].ToString() != "")
            {
                foundSpDetail.ParentProductID = Convert.ToInt64(dataFromXML["ParentProductID"].ToString());
            }
            if (dataFromXML["ChildProductID"].ToString().Trim() != string.Empty && dataFromXML["ChildProductID"].ToString() != "")
            {
                foundSpDetail.ChildProductID = Convert.ToInt64(dataFromXML["ChildProductID"].ToString());
            }
            if (dataFromXML.Table.Columns.Contains("TransactionDetailID"))
            {
                if (dataFromXML["TransactionDetailID"].ToString().Trim() != string.Empty && dataFromXML["TransactionDetailID"].ToString() != "")
                {
                    foundSpDetail.TransactionDetailID = Convert.ToInt64(dataFromXML["TransactionDetailID"].ToString());
                }
            }
            foundSpDetail.SPDetailID = Convert.ToString(dataFromXML["SPDetailID"].ToString());

            if (dataFromXML["DiscountRate"].ToString().Trim() != string.Empty && dataFromXML["DiscountRate"].ToString() != "")
            {
                foundSpDetail.DiscountRate = Convert.ToDecimal(dataFromXML["DiscountRate"].ToString()); //Repair for error
            }
            if (dataFromXML["Price"].ToString().Trim() != string.Empty && dataFromXML["Price"].ToString() != "")
            {
                foundSpDetail.Price = Convert.ToInt64(dataFromXML["Price"].ToString());
            }
        }

        private void GetProductPriceChangeFromXML(DataRow dataFromXML, APP_Data.ProductPriceChange Foundpdc)
        {
            if (dataFromXML["ProductId"].ToString().Trim() != string.Empty && dataFromXML["ProductId"].ToString() != "")
            {
                Foundpdc.ProductId = Convert.ToInt64(dataFromXML["ProductId"].ToString());
            }
            if (dataFromXML["UpdateDate"].ToString().Trim() != string.Empty && dataFromXML["UpdateDate"].ToString() != "")
            {
                Foundpdc.UpdateDate = Convert.ToDateTime(dataFromXML["UpdateDate"].ToString());
            }
            if (dataFromXML["UserID"].ToString().Trim() != string.Empty && dataFromXML["UserID"].ToString() != "")
            {
                Foundpdc.UserID = Convert.ToInt32(dataFromXML["UserID"].ToString());
            }
            if (dataFromXML["Price"].ToString().Trim() != string.Empty && dataFromXML["Price"].ToString() != "")
            {
                Foundpdc.Price = Convert.ToInt64(dataFromXML["Price"].ToString());
            }
            if (dataFromXML["OldPrice"].ToString().Trim() != string.Empty && dataFromXML["OldPrice"].ToString() != "")
            {
                Foundpdc.OldPrice = Convert.ToInt64(dataFromXML["OldPrice"].ToString());
            }
        }

        private void GetWrapperItemFromXML(DataRow dataFromXML, APP_Data.WrapperItem foundWrapperItem)
        {
            if (dataFromXML["ChildProductId"].ToString().Trim() != String.Empty && dataFromXML["ChildProductId"].ToString() != "")
            {
                foundWrapperItem.ChildProductId = Convert.ToInt32(dataFromXML["ChildProductId"].ToString());
            }
            if (dataFromXML["ParentProductId"].ToString().Trim() != String.Empty && dataFromXML["ParentProductId"].ToString() != "")
            {
                foundWrapperItem.ParentProductId = Convert.ToInt32(dataFromXML["ParentProductId"].ToString());
            }
        }
        private void GetProductInNovaltyFromXML(DataRow dataFromXML, APP_Data.ProductInNovelty FoundPdInNov)
        {
            FoundPdInNov.NoveltySystemId = Convert.ToInt32(dataFromXML["NoveltySystemId"].ToString());
            FoundPdInNov.ProductId = Convert.ToInt32(dataFromXML["ProductId"].ToString());
        }

        private void GetNoveltySystem(DataRow dataFromXML, APP_Data.NoveltySystem foundNoveltySytem, Boolean newRow)
        {
            if (foundNoveltySytem.UpdateDate < Convert.ToDateTime(dataFromXML["UpdateDate"].ToString()) || newRow == true)
            {
                if (dataFromXML["BrandId"].ToString().Trim() != string.Empty && dataFromXML["BrandId"].ToString() != "")
                {
                    foundNoveltySytem.BrandId = Convert.ToInt32(dataFromXML["BrandId"].ToString());
                }
                if (dataFromXML["ValidFrom"].ToString().Trim() != string.Empty && dataFromXML["ValidFrom"].ToString() != "")
                {
                    foundNoveltySytem.ValidFrom = Convert.ToDateTime(dataFromXML["ValidFrom"].ToString());
                }
                if (dataFromXML["ValidTo"].ToString().Trim() != string.Empty && dataFromXML["ValidTo"].ToString() != "")
                {
                    foundNoveltySytem.ValidTo = Convert.ToDateTime(dataFromXML["ValidTo"].ToString());
                }
                if (dataFromXML["UpdateDate"].ToString().Trim() != string.Empty && dataFromXML["UpdateDate"].ToString() != "")
                {
                    foundNoveltySytem.UpdateDate = Convert.ToDateTime(dataFromXML["UpdateDate"].ToString());
                }

            }
        }

        private void GetLoc_CustomerPoint(DataRow dataRowFrmXML, APP_Data.Loc_CustomerPoint FoundCustomerPoint)
        {
            if (dataRowFrmXML["CustomerId"].ToString().Trim() != String.Empty || dataRowFrmXML["CustomerId"].ToString() == "")
            {
                FoundCustomerPoint.CustomerId = Convert.ToInt32(dataRowFrmXML["CustomerId"].ToString());
            }
            if (dataRowFrmXML["OldPoint"].ToString().Trim() != String.Empty || dataRowFrmXML["OldPoint"].ToString() == "")
            {
                FoundCustomerPoint.OldPoint = Convert.ToInt32(dataRowFrmXML["OldPoint"].ToString());
            }
            if (dataRowFrmXML["TotalRedeemPoint"].ToString().Trim() != String.Empty || dataRowFrmXML["TotalRedeemPoint"].ToString() == "")
            {
                FoundCustomerPoint.TotalRedeemPoint = Convert.ToInt32(dataRowFrmXML["TotalRedeemPoint"].ToString());
            }
        }

        private void UserPrePaidDebtFromXML(DataRow dataFromXML, APP_Data.UsePrePaidDebt FoundUserPrePaidDebt)
        {
            FoundUserPrePaidDebt.CreditTransactionId = dataFromXML["CreditTransactionId"].ToString();
            FoundUserPrePaidDebt.PrePaidDebtTransactionId = dataFromXML["PrePaidDebtTransactionId"].ToString();

            if (dataFromXML["UseAmount"].ToString() != string.Empty && dataFromXML["UseAmount"].ToString() != "")
            {
                FoundUserPrePaidDebt.UseAmount = Convert.ToInt32(dataFromXML["UseAmount"].ToString());
            }
            if (dataFromXML["CashierId"].ToString() != string.Empty && dataFromXML["CashierId"].ToString() != "")
            {
                FoundUserPrePaidDebt.CashierId = Convert.ToInt32(dataFromXML["CashierId"].ToString());
            }
            if (dataFromXML["CounterId"].ToString() != string.Empty && dataFromXML["CounterId"].ToString() != "")
            {
                FoundUserPrePaidDebt.CounterId = Convert.ToInt32(dataFromXML["CounterId"].ToString());
            }
        }

        private void FoundAttachGiftSystemForTransactionFromXML(DataRow dataFromXML, APP_Data.AttachGiftSystemForTransaction FoundAttachGiftSystemForTransaction)
        {
            if (dataFromXML["AttachGiftSystemId"].ToString() != string.Empty && dataFromXML["AttachGiftSystemId"].ToString() != "")
            {
                FoundAttachGiftSystemForTransaction.AttachGiftSystemId = Convert.ToInt32(dataFromXML["AttachGiftSystemId"].ToString());
            }
            FoundAttachGiftSystemForTransaction.TransactionId = dataFromXML["TransactionId"].ToString();
        }

        private void GetGiftSystemFromXML(DataRow dataFromXML, APP_Data.GiftSystem FoundGiftSystem)
        {
            FoundGiftSystem.Type = dataFromXML["Type"].ToString();
            FoundGiftSystem.Name = dataFromXML["Name"].ToString();
            if (dataFromXML["MustBuyCostFrom"].ToString().Trim() != string.Empty && dataFromXML["MustBuyCostFrom"].ToString() != "")
            {
                FoundGiftSystem.MustBuyCostFrom = Convert.ToInt64(dataFromXML["MustBuyCostFrom"].ToString());
            }
            if (dataFromXML["MustBuyCostTo"].ToString().Trim() != string.Empty && dataFromXML["MustBuyCostTo"].ToString() != "")
            {
                FoundGiftSystem.MustBuyCostTo = Convert.ToInt64(dataFromXML["MustBuyCostTo"].ToString());
            }
            if (dataFromXML.Table.Columns.Contains("MustIncludeProductId"))
            {
                if (dataFromXML["MustIncludeProductId"].ToString() != "0" && dataFromXML["MustIncludeProductId"].ToString() != "")
                {
                    FoundGiftSystem.MustIncludeProductId = Convert.ToInt64(dataFromXML["MustIncludeProductId"].ToString());
                }
            }
            if (dataFromXML.Table.Columns.Contains("FilterBrandId"))
            {
                if (dataFromXML["FilterBrandId"].ToString() != "0" && dataFromXML["FilterBrandId"].ToString() != "")
                {
                    FoundGiftSystem.FilterBrandId = Convert.ToInt32(dataFromXML["FilterBrandId"].ToString());
                }
            }
            if (dataFromXML.Table.Columns.Contains("FilterCategoryId"))
            {
                if (dataFromXML["FilterCategoryId"].ToString() != "0" && dataFromXML["FilterCategoryId"].ToString() != "")
                {
                    FoundGiftSystem.FilterCategoryId = Convert.ToInt32(dataFromXML["FilterCategoryId"].ToString());
                }
            }
            if (dataFromXML.Table.Columns.Contains("FilterSubCategoryId"))
            {
                if (dataFromXML["FilterSubCategoryId"].ToString() != "0" && dataFromXML["FilterSubCategoryId"].ToString() != "")
                {
                    FoundGiftSystem.FilterSubCategoryId = Convert.ToInt32(dataFromXML["FilterSubCategoryId"].ToString());
                }
            }
            if (dataFromXML["ValidFrom"].ToString() != string.Empty && dataFromXML["ValidFrom"].ToString() != "")
            {
                FoundGiftSystem.ValidFrom = Convert.ToDateTime(dataFromXML["ValidFrom"].ToString());
            }
            if (dataFromXML["ValidTo"].ToString() != string.Empty && dataFromXML["ValidTo"].ToString() != "")
            {
                FoundGiftSystem.ValidTo = Convert.ToDateTime(dataFromXML["ValidTo"].ToString());
            }
            if (dataFromXML["UsePromotionQty"].ToString() != string.Empty && dataFromXML["UsePromotionQty"].ToString() != "")
            {
                FoundGiftSystem.UsePromotionQty = Convert.ToBoolean(dataFromXML["UsePromotionQty"].ToString());
            }
            if (dataFromXML["PromotionQty"].ToString() != string.Empty && dataFromXML["PromotionQty"].ToString() != "")
            {
                FoundGiftSystem.PromotionQty = Convert.ToInt32(dataFromXML["PromotionQty"].ToString());
            }
            if (dataFromXML.Table.Columns.Contains("GiftProductId"))
            {
                if (dataFromXML["GiftProductId"].ToString() != "0" && dataFromXML["GiftProductId"].ToString() != "")
                {
                    FoundGiftSystem.GiftProductId = Convert.ToInt32(dataFromXML["GiftProductId"].ToString());
                }
            }
            if (dataFromXML["PriceForGiftProduct"].ToString() != string.Empty && dataFromXML["PriceForGiftProduct"].ToString() != "")
            {
                FoundGiftSystem.PriceForGiftProduct = Convert.ToInt64(dataFromXML["PriceForGiftProduct"].ToString());
            }
            if (dataFromXML["GiftCashAmount"].ToString() != string.Empty && dataFromXML["GiftCashAmount"].ToString() != "")
            {
                FoundGiftSystem.GiftCashAmount = Convert.ToInt64(dataFromXML["GiftCashAmount"].ToString());
            }
            if (dataFromXML["DiscountPercentForTransaction"].ToString() != string.Empty && dataFromXML["DiscountPercentForTransaction"].ToString() != "")
            {
                FoundGiftSystem.DiscountPercentForTransaction = Convert.ToInt32(dataFromXML["DiscountPercentForTransaction"].ToString());
            }
            if (dataFromXML["UseBrandFilter"].ToString() != string.Empty && dataFromXML["UseBrandFilter"].ToString() != "")
            {
                FoundGiftSystem.UseBrandFilter = Convert.ToBoolean(dataFromXML["UseBrandFilter"].ToString());
            }
            if (dataFromXML["UseCategoryFilter"].ToString() != string.Empty && dataFromXML["UseCategoryFilter"].ToString() != "")
            {
                FoundGiftSystem.UseCategoryFilter = Convert.ToBoolean(dataFromXML["UseCategoryFilter"].ToString());
            }
            if (dataFromXML["UseSubCategoryFilter"].ToString() != string.Empty && dataFromXML["UseSubCategoryFilter"].ToString() != "")
            {
                FoundGiftSystem.UseSubCategoryFilter = Convert.ToBoolean(dataFromXML["UseSubCategoryFilter"].ToString());
            }
            if (dataFromXML["UseProductFilter"].ToString() != string.Empty && dataFromXML["UseProductFilter"].ToString() != "")
            {
                FoundGiftSystem.UseProductFilter = Convert.ToBoolean(dataFromXML["UseProductFilter"].ToString());
            }
            if (dataFromXML["IsActive"].ToString() != string.Empty && dataFromXML["IsActive"].ToString() != "")
            {
                FoundGiftSystem.IsActive = Convert.ToBoolean(dataFromXML["IsActive"].ToString());
            }
            if (dataFromXML.Table.Columns.Contains("UseSizeFilter"))
            {
                if (dataFromXML["UseSizeFilter"].ToString() != string.Empty && dataFromXML["UseSizeFilter"].ToString() != "")
                {
                    FoundGiftSystem.UseSizeFilter = Convert.ToBoolean(dataFromXML["UseSizeFilter"].ToString());
                }
            }
            if (dataFromXML.Table.Columns.Contains("UseQtyFilter"))
            {

                if (dataFromXML["UseQtyFilter"].ToString() != string.Empty && dataFromXML["UseQtyFilter"].ToString() != "")
                {
                    FoundGiftSystem.UseQtyFilter = Convert.ToBoolean(dataFromXML["UseQtyFilter"].ToString());
                }
            }
            if (dataFromXML.Table.Columns.Contains("FilterSize"))
            {
                if (dataFromXML["FilterSize"].ToString() != string.Empty && dataFromXML["FilterSize"].ToString() != "")
                {
                    FoundGiftSystem.FilterSize = Convert.ToInt32(dataFromXML["FilterSize"].ToString());
                }
            }
            if (dataFromXML.Table.Columns.Contains("FilterQty"))
            {
                if (dataFromXML["FilterQty"].ToString() != string.Empty && dataFromXML["FilterQty"].ToString() != "")
                {
                    FoundGiftSystem.FilterQty = Convert.ToInt32(dataFromXML["FilterQty"].ToString());
                }
            }
        }

        private void GetRoleManagementFromXML(DataRow dataFromXML, APP_Data.RoleManagement FoundRoleManagement)
        {
            FoundRoleManagement.RuleFeature = dataFromXML["RuleFeature"].ToString();
            FoundRoleManagement.UserRoleId = Convert.ToInt32(dataFromXML["UserRoleId"].ToString());
            FoundRoleManagement.IsAllowed = Convert.ToBoolean(dataFromXML["IsAllowed"].ToString());
        }

        private void GetGiftCardInInTranFromXML(DataRow dataFromXML, APP_Data.GiftCardInTransaction FoundGiftCardInInTran)
        {
            if (dataFromXML["GiftCardId"].ToString().Trim() != string.Empty && dataFromXML["GiftCardId"].ToString() != "")
            {
                FoundGiftCardInInTran.GiftCardId = Convert.ToInt32(dataFromXML["GiftCardId"].ToString());
            }
            FoundGiftCardInInTran.TransactionId = dataFromXML["TransactionId"].ToString();
        }

        private void GetGiftCardFromXML(DataRow dataFromXML, APP_Data.GiftCard FoundGiftCard)
        {
            // ZP
            if (FoundGiftCard.IsDeleted != true)
            {
                FoundGiftCard.CardNumber = dataFromXML["CardNumber"].ToString();
                if (dataFromXML["Amount"].ToString() != string.Empty && dataFromXML["Amount"].ToString() != "")
                {
                    FoundGiftCard.Amount = Convert.ToInt64(dataFromXML["Amount"].ToString());
                }
                if (dataFromXML["IsUsed"].ToString() != "0" && dataFromXML["IsUsed"].ToString() != "")
                {
                    FoundGiftCard.IsUsed = Convert.ToBoolean(dataFromXML["IsUsed"].ToString());
                }
                if (dataFromXML["ExpireDate"].ToString() != string.Empty && dataFromXML["ExpireDate"].ToString() != "")
                {
                    FoundGiftCard.ExpireDate = Convert.ToDateTime(dataFromXML["ExpireDate"].ToString());
                }
                if (dataFromXML["CustomerId"].ToString() != string.Empty && dataFromXML["CustomerId"].ToString() != "")
                {
                    FoundGiftCard.CustomerId = Convert.ToInt32(dataFromXML["CustomerId"].ToString());
                }
                if (dataFromXML["IsUsedDate"].ToString() != string.Empty && dataFromXML["IsUsedDate"].ToString() != "")
                {
                    FoundGiftCard.IsUsedDate = Convert.ToDateTime(dataFromXML["IsUsedDate"].ToString());//repair at 21.7.2015
                }
                if (dataFromXML["IsDeleted"].ToString() != string.Empty && dataFromXML["IsDeleted"].ToString() != "")
                {
                    FoundGiftCard.IsDeleted = Convert.ToBoolean(dataFromXML["IsDeleted"].ToString());//
                }
            }
        }

        private void GetUserFromXML(DataRow dataFromXML, APP_Data.User FoundUser)
        {
            FoundUser.Name = dataFromXML["Name"].ToString();
            if (dataFromXML["UserRoleId"].ToString().Trim() != String.Empty || dataFromXML["UserRoleId"].ToString() == "")
            {
                FoundUser.UserRoleId = Convert.ToInt32(dataFromXML["UserRoleId"].ToString());
            }
            FoundUser.Password = dataFromXML["Password"].ToString();
            if (dataFromXML["DateTime"].ToString().Trim() != String.Empty || dataFromXML["DateTime"].ToString() == "")
            {
                FoundUser.DateTime = Convert.ToDateTime(dataFromXML["DateTime"].ToString());
            }
        }

        private void GetUserRoleFromXML(DataRow dataFromXML, APP_Data.UserRole FoundUserRole)
        {
            FoundUserRole.RoleName = dataFromXML["RoleName"].ToString();
        }

        private void GetSettingTypeFromXML(DataRow dataFromXML, APP_Data.Setting FoundSetting)
        {
            FoundSetting.Key = dataFromXML["Key"].ToString();
            FoundSetting.Value = dataFromXML["Value"].ToString();
        }

        private void GetPaymentTypeFromXML(DataRow dataFromXML, APP_Data.PaymentType FoundPaymentType)
        {
            FoundPaymentType.Name = dataFromXML["Name"].ToString();
        }

        private void GetExchangeReateForTansction(DataRow dataFromXML, APP_Data.ExchangeRateForTransaction FoundExchangeRate)
        {
            if (dataFromXML["CurrencyId"].ToString().Trim() != string.Empty && dataFromXML["CurrencyId"].ToString() != "")
            {
                FoundExchangeRate.CurrencyId = Convert.ToInt32(dataFromXML["CurrencyId"].ToString());
            }
            if (dataFromXML["ExchangeRate"].ToString().Trim() != string.Empty && dataFromXML["ExchangeRate"].ToString() != "")
            {
                FoundExchangeRate.ExchangeRate = Convert.ToInt32(dataFromXML["ExchangeRate"].ToString());
            }
            FoundExchangeRate.TransactionId = dataFromXML["TransactionId"].ToString();
        }

        private void GetCurrencyFromXML(DataRow dataFromXML, APP_Data.Currency FoundCurrency)
        {
            FoundCurrency.Country = dataFromXML["Country"].ToString();
            FoundCurrency.Symbol = dataFromXML["Symbol"].ToString();
            FoundCurrency.CurrencyCode = dataFromXML["CurrencyCode"].ToString();
            if (dataFromXML["LatestExchangeRate"].ToString().Trim() != string.Empty && dataFromXML["LatestExchangeRate"].ToString() != "")
            {
                FoundCurrency.LatestExchangeRate = Convert.ToInt32(dataFromXML["LatestExchangeRate"].ToString());
            }
        }

        private void GetDeleteLogFromXML(DataRow dataFromXML, APP_Data.DeleteLog FoundDeleteLog)
        {
            if (dataFromXML["UserId"].ToString().Trim() != string.Empty && dataFromXML["UserId"].ToString() != "")
            {
                FoundDeleteLog.UserId = Convert.ToInt32(dataFromXML["UserId"]);
            }
            if (dataFromXML["CounterId"].ToString().Trim() != string.Empty && dataFromXML["CounterId"].ToString() != "")
            {
                FoundDeleteLog.CounterId = Convert.ToInt32(dataFromXML["CounterId"]);
            }
            if (dataFromXML.Table.Columns.Contains("TransactionId"))
            {
                FoundDeleteLog.TransactionId = dataFromXML["TransactionId"].ToString();
            }
            if (dataFromXML.Table.Columns.Contains("TransactionDetailId"))
            {
                if (dataFromXML["TransactionDetailId"].ToString().Trim() != string.Empty && dataFromXML["TransactionDetailId"].ToString() != "")
                {
                    FoundDeleteLog.TransactionDetailId = Convert.ToInt64(dataFromXML["TransactionDetailId"].ToString());
                }
            }
            if (dataFromXML.Table.Columns.Contains("IsParent"))
            {
                if (dataFromXML["IsParent"].ToString().Trim() != string.Empty && dataFromXML["IsParent"].ToString() != "")
                {
                    FoundDeleteLog.IsParent = Convert.ToBoolean(dataFromXML["IsParent"].ToString());
                }
            }
            if (dataFromXML["DeletedDate"].ToString().Trim() != string.Empty && dataFromXML["DeletedDate"].ToString() != "")
            {
                FoundDeleteLog.DeletedDate = Convert.ToDateTime(dataFromXML["DeletedDate"].ToString());
            }
        }

        private void GetTaxFromXML(DataRow dataFromXML, APP_Data.Tax FoundTax)
        {
            FoundTax.Name = dataFromXML["Name"].ToString();
            FoundTax.TaxPercent = Convert.ToDecimal(dataFromXML["TaxPercent"].ToString());
        }

        private void GetShopFromXML(DataRow dataFromXML, APP_Data.Shop FoundShop)
        {
            FoundShop.ShopName = dataFromXML["ShopName"].ToString();
            FoundShop.Address = dataFromXML["Address"].ToString();
            FoundShop.PhoneNumber = dataFromXML["PhoneNumber"].ToString();
            FoundShop.OpeningHours = dataFromXML["OpeningHours"].ToString();
            APP_Data.Shop shop = entity.Shops.Where(x => x.IsDefaultShop == true).FirstOrDefault();
            if (dataFromXML["CityId"].ToString().Trim() != string.Empty && dataFromXML["CityId"].ToString() != "")
            {
                FoundShop.CityId = Convert.ToInt16(dataFromXML["CityId"].ToString());
            }
            FoundShop.ShortCode = dataFromXML["ShortCode"].ToString();
            if (dataFromXML["IsDefaultShop"].ToString().Trim() != string.Empty && dataFromXML["IsDefaultShop"].ToString() != "")
            {

                //FoundShop.IsDefaultShop = Convert.ToBoolean(dataFromXML["IsDefaultShop"].ToString());
                if (FoundShop.ShopName == shop.ShopName)
                {
                    FoundShop.IsDefaultShop = true;
                }
                else
                {
                    FoundShop.IsDefaultShop = false;
                }
            }
        }

        private void GetUnitFromXML(DataRow dataRowFromXMl, APP_Data.Unit FoundUnit)
        {
            FoundUnit.UnitName = dataRowFromXMl["UnitName"].ToString();
        }

        private void GetProductSubCategoryFromXML(DataRow dataRowFromXML, APP_Data.ProductSubCategory FoundProductSubCategory)
        {
            FoundProductSubCategory.Name = dataRowFromXML["Name"].ToString();

            if (dataRowFromXML["ProductCategoryId"].ToString().Trim() != String.Empty && dataRowFromXML["ProductCategoryId"].ToString() != "")
            {
                FoundProductSubCategory.ProductCategoryId = Convert.ToInt32(dataRowFromXML["ProductCategoryId"].ToString());
            }
        }

        private void GetProductCategoryFromXML(DataRow dataRowFromXML, APP_Data.ProductCategory FoundProductCategory)
        {
            FoundProductCategory.Name = dataRowFromXML["Name"].ToString();
        }

        private void GetCounterFromXML(DataRow dataRowFromXML, APP_Data.Counter Foundcounter)
        {
            Foundcounter.Name = dataRowFromXML["Name"].ToString();
        }

        private void GetConsignmentCounterFromXML(DataRow dataRowFromXML, APP_Data.ConsignmentCounter FoundConsignmentCounter)
        {
            FoundConsignmentCounter.Name = dataRowFromXML["Name"].ToString();
            FoundConsignmentCounter.CounterLocation = dataRowFromXML["CounterLocation"].ToString();
        }

        private void GetCityFromXML(DataRow dataRowFromXML, APP_Data.City City)
        {
            City.CityName = dataRowFromXML["CityName"].ToString();
        }

        private void GetPointDeductRuleFromXML(DataRow dataRowFromXML, APP_Data.PointDeductionPercentage_History deductpoint)
        {
            if (dataRowFromXML.Table.Columns.Contains("DiscountRate"))
            {
                if (Convert.ToDecimal(dataRowFromXML["DiscountRate"]) != 0)
                {
                    deductpoint.DiscountRate = Convert.ToDecimal(dataRowFromXML["DiscountRate"]);
                }
            }

            deductpoint.StartDate = Convert.ToDateTime(dataRowFromXML["StartDate"]);
            if (dataRowFromXML.Table.Columns.Contains("EndDate"))
            {
                if (dataRowFromXML["EndDate"] != null && dataRowFromXML["EndDate"].ToString().Trim() != "")
                {
                    deductpoint.EndDate = Convert.ToDateTime(dataRowFromXML["EndDate"]);
                }

            }
            deductpoint.UserId = Convert.ToInt32(dataRowFromXML["UserId"]);
            deductpoint.Counter = Convert.ToInt32(dataRowFromXML["Counter"]);
            deductpoint.Active = Convert.ToBoolean(dataRowFromXML["Active"]);

        }

        private void GetBrandFromXML(DataRow dataRowFromXML, APP_Data.Brand brand)
        {
            brand.Name = dataRowFromXML["Name"].ToString();
        }

        private void GetLoc_PointRedeemHistory(DataRow dataRowFromXML, APP_Data.Loc_PointRedeemHistory loc_pointRedeemHistory)
        {
            if (dataRowFromXML["CustomerId"].ToString().Trim() != String.Empty && dataRowFromXML["CustomerId"].ToString() != "")
            {
                loc_pointRedeemHistory.CustomerId = Convert.ToInt32(dataRowFromXML["CustomerId"].ToString());
            }
            if (dataRowFromXML["RedeemPoint"].ToString().Trim() != String.Empty && dataRowFromXML["RedeemPoint"].ToString() != "")
            {
                loc_pointRedeemHistory.RedeemPoint = Convert.ToInt32(dataRowFromXML["RedeemPoint"].ToString());
            }
            if (dataRowFromXML["RedeemAmount"].ToString().Trim() != String.Empty && dataRowFromXML["RedeemAmount"].ToString() != "")
            {
                loc_pointRedeemHistory.RedeemAmount = Convert.ToInt32(dataRowFromXML["RedeemAmount"].ToString());
            }
            if (dataRowFromXML["DateTime"].ToString().Trim() != String.Empty && dataRowFromXML["DateTime"].ToString() != "")
            {
                loc_pointRedeemHistory.DateTime = Convert.ToDateTime(dataRowFromXML["DateTime"].ToString());
            }
            if (dataRowFromXML["CasherId"].ToString().Trim() != String.Empty && dataRowFromXML["CasherId"].ToString() != "")
            {
                loc_pointRedeemHistory.CasherId = Convert.ToInt32(dataRowFromXML["CasherId"].ToString());
            }
            if (dataRowFromXML["CounterId"].ToString().Trim() != String.Empty && dataRowFromXML["CounterId"].ToString() != "")
            {
                loc_pointRedeemHistory.CounterId = Convert.ToInt32(dataRowFromXML["CounterId"].ToString());
            }
        }

        private void GetProductFromXML(DataRow dataRowFromXML, APP_Data.Product product)
        {
            product.Name = dataRowFromXML["Name"].ToString();
            product.ProductCode = dataRowFromXML["ProductCode"].ToString();
            product.Barcode = dataRowFromXML["BarCode"].ToString();

            if (dataRowFromXML["Price"].ToString().Trim() != String.Empty && dataRowFromXML["Price"].ToString() != "")
            {
                product.Price = Convert.ToInt64(dataRowFromXML["Price"].ToString());
            }
            //   product.Qty = Convert.ToInt32(dataRowFromXML["Qty"].ToString());
            if (dataRowFromXML["BrandId"].ToString() != "0" && dataRowFromXML["BrandId"].ToString() != "")
            {
                product.BrandId = Convert.ToInt32(dataRowFromXML["BrandId"].ToString());
            }
            if (dataRowFromXML["ProductLocation"].ToString() != string.Empty && dataRowFromXML["ProductLocation"].ToString() != "")
            {
                product.ProductLocation = dataRowFromXML["ProductLocation"].ToString();
            }

            if (dataRowFromXML["ProductCategoryId"].ToString().Trim() != "0" && dataRowFromXML["ProductCategoryId"].ToString() != "")
            {
                product.ProductCategoryId = Convert.ToInt32(dataRowFromXML["ProductCategoryId"].ToString());
            }
            if (dataRowFromXML["ProductSubCategoryId"].ToString().Trim() != "0" && dataRowFromXML["ProductSubCategoryId"].ToString() != "")
            {
                product.ProductSubCategoryId = Convert.ToInt32(dataRowFromXML["ProductSubCategoryId"].ToString());
            }
            if (dataRowFromXML["UnitID"].ToString().Trim() != "0" && dataRowFromXML["UnitID"].ToString() != "")
            {
                product.UnitId = Convert.ToInt32(dataRowFromXML["UnitID"].ToString());
            }
            if (dataRowFromXML["TaxId"].ToString().Trim() != String.Empty && dataRowFromXML["TaxId"].ToString() != "")
            {
                product.TaxId = Convert.ToInt32(dataRowFromXML["TaxId"].ToString());
            }
            if (dataRowFromXML["MinStockQty"].ToString().Trim() != String.Empty && dataRowFromXML["MinStockQty"].ToString() != "")
            {
                product.MinStockQty = Convert.ToInt32(dataRowFromXML["MinStockQty"].ToString());
            }
            if (dataRowFromXML["DiscountRate"].ToString().Trim() != String.Empty && dataRowFromXML["DiscountRate"].ToString() != "")
            {
                product.DiscountRate = Convert.ToDecimal(dataRowFromXML["DiscountRate"].ToString());
            }
            if (dataRowFromXML["IsWrapper"].ToString().Trim() != String.Empty && dataRowFromXML["IsWrapper"].ToString() != "")
            {
                product.IsWrapper = Convert.ToBoolean(dataRowFromXML["IsWrapper"].ToString());
            }
            if (dataRowFromXML["IsConsignment"].ToString().Trim() != String.Empty && dataRowFromXML["IsConsignment"].ToString() != "")
            {
                product.IsConsignment = Convert.ToBoolean(dataRowFromXML["IsConsignment"].ToString());
            }
            if (dataRowFromXML["IsDiscontinue"].ToString().Trim() != String.Empty && dataRowFromXML["IsDiscontinue"].ToString() != "")
            {
                product.IsDiscontinue = Convert.ToBoolean(dataRowFromXML["IsDiscontinue"].ToString());
            }
            if (dataRowFromXML["IsPromotionProduct"].ToString().Trim() != String.Empty && dataRowFromXML["IsPromotionProduct"].ToString() != "")
            {
                product.IsPromotionProduct = Convert.ToBoolean(dataRowFromXML["IsPromotionProduct"].ToString());
            }
            if (dataRowFromXML["IsNovelty"].ToString().Trim() != String.Empty && dataRowFromXML["IsNovelty"].ToString() != "")
            {
                product.IsNovelty = Convert.ToBoolean(dataRowFromXML["IsNovelty"].ToString());
            }
            if (dataRowFromXML["ConsignmentPrice"].ToString().Trim() != String.Empty && dataRowFromXML["ConsignmentPrice"].ToString() != "")
            {
                product.ConsignmentPrice = Convert.ToInt64(dataRowFromXML["ConsignmentPrice"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("ConsignmentCounterId"))
            {
                if (dataRowFromXML["ConsignmentCounterId"].ToString().Trim() != "0" && dataRowFromXML["ConsignmentCounterId"].ToString() != "")
                {
                    product.ConsignmentCounterId = Convert.ToInt32(dataRowFromXML["ConsignmentCounterId"].ToString());
                }
            }
            if (dataRowFromXML["Size"].ToString().Trim() != String.Empty && dataRowFromXML["Size"].ToString() != "")
            {
                product.Size = dataRowFromXML["Size"].ToString();
            }
            if (dataRowFromXML["PurchasePrice"].ToString().Trim() != String.Empty && dataRowFromXML["PurchasePrice"].ToString() != "")
            {
                product.PurchasePrice = Convert.ToInt64(dataRowFromXML["PurchasePrice"].ToString());
            }
            if (dataRowFromXML["IsNotifyMinStock"].ToString().Trim() != String.Empty && dataRowFromXML["IsNotifyMinStock"].ToString() != "")
            {
                product.IsNotifyMinStock = Convert.ToBoolean(dataRowFromXML["IsNotifyMinStock"].ToString());
            }
            if (dataRowFromXML["UpdateDate"].ToString().Trim() != String.Empty && dataRowFromXML["UpdateDate"].ToString() != "")
            {
                product.UpdateDate = Convert.ToDateTime(dataRowFromXML["UpdateDate"].ToString());
            }
        }

        private void GetTransactionFromXML(DataRow dataRowFromXML, APP_Data.Transaction transaction)
        {
            transaction.Id = Convert.ToString(dataRowFromXML["Id"].ToString());

            if (dataRowFromXML["DateTime"].ToString().Trim() != string.Empty && dataRowFromXML["DateTime"].ToString() != "")
            {
                transaction.DateTime = Convert.ToDateTime(dataRowFromXML["DateTime"].ToString());
            }
            if (dataRowFromXML["UpdatedDate"].ToString().Trim() != string.Empty && dataRowFromXML["UpdatedDate"].ToString() != "")
            {
                transaction.UpdatedDate = Convert.ToDateTime(dataRowFromXML["UpdatedDate"].ToString());
            }
            if (dataRowFromXML["UserId"].ToString().Trim() != string.Empty && dataRowFromXML["UserId"].ToString() != "")
            {
                transaction.UserId = Convert.ToInt32(dataRowFromXML["UserId"].ToString());
            }
            if (dataRowFromXML["CounterId"].ToString().Trim() != string.Empty && dataRowFromXML["CounterId"].ToString() != "")
            {
                transaction.CounterId = Convert.ToInt32(dataRowFromXML["CounterId"].ToString());
            }
            transaction.Type = Convert.ToString(dataRowFromXML["Type"].ToString());

            if (dataRowFromXML["IsPaid"].ToString().Trim() != string.Empty && dataRowFromXML["IsPaid"].ToString() != "")
            {
                transaction.IsPaid = Convert.ToBoolean(dataRowFromXML["IsPaid"].ToString());
            }
            if (dataRowFromXML["IsComplete"].ToString().Trim() != string.Empty && dataRowFromXML["IsComplete"].ToString() != "")
            {
                transaction.IsComplete = Convert.ToBoolean(dataRowFromXML["IsComplete"].ToString());
            }
            if (dataRowFromXML["IsActive"].ToString().Trim() != string.Empty && dataRowFromXML["IsActive"].ToString() != "")
            {
                transaction.IsActive = Convert.ToBoolean(dataRowFromXML["IsActive"].ToString());
            }
            if (dataRowFromXML["IsDeleted"].ToString().Trim() != string.Empty && dataRowFromXML["IsDeleted"].ToString() != "")
            {
                transaction.IsDeleted = Convert.ToBoolean(dataRowFromXML["IsDeleted"].ToString());
            }
            if (dataRowFromXML["Loc_IsCalculatePoint"].ToString().Trim() != string.Empty && dataRowFromXML["Loc_IsCalculatePoint"].ToString() != "")
            {
                transaction.Loc_IsCalculatePoint = Convert.ToBoolean(dataRowFromXML["Loc_IsCalculatePoint"].ToString());
            }
            if (dataRowFromXML["PaymentTypeId"].ToString().Trim() != string.Empty && dataRowFromXML["PaymentTypeId"].ToString() != "")
            {
                transaction.PaymentTypeId = Convert.ToInt32(dataRowFromXML["PaymentTypeId"].ToString());
            }
            if (dataRowFromXML["TaxAmount"].ToString().Trim() != string.Empty && dataRowFromXML["TaxAmount"].ToString() != "")
            {
                transaction.TaxAmount = Convert.ToInt32(dataRowFromXML["TaxAmount"].ToString());
            }
            if (dataRowFromXML["TotalAmount"].ToString().Trim() != string.Empty && dataRowFromXML["TotalAmount"].ToString() != "")
            {
                transaction.TotalAmount = Convert.ToDecimal(dataRowFromXML["TotalAmount"].ToString());
            }
            if (dataRowFromXML["RecieveAmount"].ToString().Trim() != string.Empty && dataRowFromXML["RecieveAmount"].ToString() != "")
            {
                transaction.RecieveAmount = Convert.ToDecimal(dataRowFromXML["RecieveAmount"].ToString()); ;
            }
            if (dataRowFromXML["DiscountAmount"].ToString().Trim() != string.Empty && dataRowFromXML["DiscountAmount"].ToString() != "")
            {
                transaction.DiscountAmount = Convert.ToInt32(dataRowFromXML["DiscountAmount"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("ParentId"))
            {
                if (dataRowFromXML["ParentId"].ToString().Trim() != string.Empty && dataRowFromXML["ParentId"].ToString() != "")
                {
                    transaction.ParentId = dataRowFromXML["ParentId"].ToString();
                }
            }
            if (dataRowFromXML["CustomerId"].ToString().Trim() != string.Empty && dataRowFromXML["CustomerId"].ToString() != "")
            {
                transaction.CustomerId = Convert.ToInt32(dataRowFromXML["CustomerId"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("ReceivedCurrencyId"))
            {
                if (dataRowFromXML["ReceivedCurrencyId"].ToString().Trim() != string.Empty && dataRowFromXML["ReceivedCurrencyId"].ToString() != "")
                {
                    transaction.ReceivedCurrencyId = Convert.ToInt32(dataRowFromXML["ReceivedCurrencyId"].ToString());
                }
            }
            if (dataRowFromXML["ShopId"].ToString().Trim() != string.Empty && dataRowFromXML["ShopId"].ToString() != "")
            {
                transaction.ShopId = Convert.ToInt32(dataRowFromXML["ShopId"].ToString());
            }
        }

        private void GetTransactionPaymentDetailFromXML(DataRow dataRowFromXML, APP_Data.TransactionPaymentDetail transactionPaymentDetail)
        {
            transactionPaymentDetail.TransactionId = Convert.ToString(dataRowFromXML["TransactionId"].ToString());

            if (dataRowFromXML["PaymentMethodId"].ToString().Trim() != string.Empty && dataRowFromXML["PaymentMethodId"].ToString() != "")
            {
                transactionPaymentDetail.PaymentMethodId = Convert.ToInt32(dataRowFromXML["PaymentMethodId"].ToString());
            }
            if (dataRowFromXML["Amount"].ToString().Trim() != string.Empty && dataRowFromXML["Amount"].ToString() != "")
            {
                transactionPaymentDetail.Amount = Convert.ToInt32(dataRowFromXML["Amount"].ToString());
            }
        }

        private void GetTransactionDetailFromXML(DataRow dataRowFromXML, APP_Data.TransactionDetail transactionDetail)
        {
            transactionDetail.TransactionId = Convert.ToString(dataRowFromXML["TransactionId"].ToString());

            if (dataRowFromXML["ProductId"].ToString().Trim() != string.Empty && dataRowFromXML["ProductId"].ToString() != "")
            {
                transactionDetail.ProductId = Convert.ToInt64(dataRowFromXML["ProductId"].ToString());
            }
            if (dataRowFromXML["Qty"].ToString().Trim() != string.Empty && dataRowFromXML["Qty"].ToString() != "")
            {
                transactionDetail.Qty = Convert.ToInt32(dataRowFromXML["Qty"].ToString());
            }
            if (dataRowFromXML["UnitPrice"].ToString().Trim() != string.Empty && dataRowFromXML["UnitPrice"].ToString() != "")
            {
                transactionDetail.UnitPrice = Convert.ToInt64(dataRowFromXML["UnitPrice"].ToString());
            }
            if (dataRowFromXML["DiscountRate"].ToString().Trim() != string.Empty && dataRowFromXML["DiscountRate"].ToString() != "")
            {
                transactionDetail.DiscountRate = Convert.ToDecimal(dataRowFromXML["DiscountRate"].ToString());
            }
            if (dataRowFromXML["TaxRate"].ToString().Trim() != string.Empty && dataRowFromXML["TaxRate"].ToString() != "")
            {
                transactionDetail.TaxRate = Convert.ToDecimal(dataRowFromXML["TaxRate"].ToString());
            }
            if (dataRowFromXML["TotalAmount"].ToString().Trim() != string.Empty && dataRowFromXML["TotalAmount"].ToString() != "")
            {
                transactionDetail.TotalAmount = Convert.ToInt64(dataRowFromXML["TotalAmount"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("IsDeleted"))
            {
                if (dataRowFromXML["IsDeleted"].ToString().Trim() != string.Empty && dataRowFromXML["IsDeleted"].ToString() != "")
                {
                    transactionDetail.IsDeleted = Convert.ToBoolean(dataRowFromXML["IsDeleted"].ToString());
                }
            }
            //zp
            if (dataRowFromXML.Table.Columns.Contains("IsDeductedBy"))
            {
                if (dataRowFromXML["IsDeductedBy"].ToString().Trim() != string.Empty && dataRowFromXML["IsDeductedBy"].ToString() != "")
                {
                    transactionDetail.IsDeductedBy = Convert.ToDecimal(dataRowFromXML["IsDeductedBy"].ToString());
                }
            }
        }

        private void GetCustomerFromXML(DataRow dataRowFromXML, APP_Data.Customer customer)
        {

            if (dataRowFromXML["CustomerTypeId"].ToString().Trim() != String.Empty && dataRowFromXML["CustomerTypeId"].ToString() != "")
            {
                customer.CustomerTypeId = Convert.ToInt32(dataRowFromXML["CustomerTypeId"].ToString());
            }
            if (dataRowFromXML["Title"].ToString().Trim() != String.Empty && dataRowFromXML["Title"].ToString() != "")
            {
                customer.Title = Convert.ToString(dataRowFromXML["Title"].ToString());
            }
            customer.Name = Convert.ToString(dataRowFromXML["Name"].ToString());
            customer.PhoneNumber = Convert.ToString(dataRowFromXML["PhoneNumber"].ToString());
            customer.Address = Convert.ToString(dataRowFromXML["Address"].ToString());
            customer.NRC = Convert.ToString(dataRowFromXML["NRC"].ToString());
            customer.Email = Convert.ToString(dataRowFromXML["Email"].ToString());
            if (dataRowFromXML.Table.Columns.Contains("VipStartedShop"))
            {
                if (!string.IsNullOrEmpty(dataRowFromXML["VipStartedShop"].ToString()))
                {
                    customer.VipStartedShop = Convert.ToString(dataRowFromXML["VipStartedShop"].ToString());
                }
            }

            if (dataRowFromXML["CityId"].ToString().Trim() != String.Empty && dataRowFromXML["CityId"].ToString() != "")
            {
                customer.CityId = Convert.ToInt32(dataRowFromXML["CityId"].ToString());
            }

            //  customer.TownShip = Convert.ToString(dataRowFromXML["TownShip"].ToString());
            // customer.TownShip = dataRowFromXML["TownShip"].ToString();
            if (dataRowFromXML.Table.Columns.Contains("Gender"))
            {
                if (dataRowFromXML["Gender"].ToString() != "")
                {
                    customer.Gender = Convert.ToString(dataRowFromXML["Gender"].ToString());
                }
            }
            if (dataRowFromXML["Birthday"].ToString() != "")
            {
                customer.Birthday = Convert.ToDateTime(dataRowFromXML["Birthday"].ToString());
            }
            if (dataRowFromXML.Table.Columns.Contains("PromoteDate"))
            {
                if (dataRowFromXML["PromoteDate"].ToString() != "")
                {
                    customer.PromoteDate = Convert.ToDateTime(dataRowFromXML["PromoteDate"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("StartDate"))
            {
                if (dataRowFromXML["StartDate"].ToString() != "")
                {
                    customer.StartDate = Convert.ToDateTime(dataRowFromXML["StartDate"].ToString());
                }
            }
            customer.VIPMemberId = Convert.ToString(dataRowFromXML["VIPMemberId"].ToString());

            if (dataRowFromXML.Table.Columns.Contains("RuleId"))
            {
                if (dataRowFromXML["RuleId"].ToString() != "")
                {
                    customer.RuleId = Convert.ToInt32(dataRowFromXML["RuleId"].ToString());
                }
            }
            if (dataRowFromXML.Table.Columns.Contains("CustomerCode"))
            {
                if (dataRowFromXML["CustomerCode"].ToString() != "")
                {
                    customer.CustomerCode = dataRowFromXML["CustomerCode"].ToString();
                }
            }

            if (dataRowFromXML.Table.Columns.Contains("PreferContact"))
            {
                if (dataRowFromXML["PreferContact"].ToString() != "")
                {
                    customer.PreferContact = dataRowFromXML["PreferContact"].ToString();
                }
            }
        }

        #endregion

        #region EncryptDecrypt

        /// <summary>
        /// Encrypting incomming file.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public static void EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                string password = @"mykey123";
                //string password = @"sap@#123";
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;

                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateEncryptor(key, key), CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                fsIn.Close();
                cs.Close();
                fsCrypt.Close();

                File.Delete(inputFile);
            }
            catch
            {
                MessageBox.Show("Encryption failed!", "Error");
            }
        }

        /// <summary>
        /// Decrypting incomming file.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public static void DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                //string password = @"sap@#123";
                string password = @"mykey123";

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key), CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
                MessageBox.Show("Decryption failed!", "Error");
            }
        }

        #endregion



    }
}
