using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Reporting.WinForms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using POS.APP_Data;
using POS.POSInterfaceServiceReference;
using System.Data;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace POS
{
    class Utility
    {
        public class mac
        {
            public string MacName { get; set; }
            public string MacAddress { get; set; }
        }

        public static void Push_DataToWebServices(DateTime fromTime, DateTime toTime)
        {// By SYM

            if (SettingController.At_JunctionCity == "1")
            {
                //toTime = DateTime.Now;
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
                dt.Load(dataReader);
                //fromTime = DateTime.Now;
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
                            Col_15 = dt.Rows[i]["saleType"].ToString() + "s",
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

                    string _msg = return_status + '-' + record_Received;
                    MessageBox.Show(_msg);


                }
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
                string password = @"myKey123";

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

        /// <summary>
        /// Encrypting incomming file.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public static void EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                string password = @"myKey123";
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
            }
            catch
            {
                MessageBox.Show("Encryption failed!", "Error");
            }
        }

        /// <summary>
        /// Decrypt the input string ( Eg: EncryptString("ABC", string.Empty); )  
        /// </summary>
        public static string EncryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }

        /// <summary>
        /// Decrypt the input string ( Eg: DecryptString("LoBCnf0JCg8=", string.Empty); )  
        /// </summary>
        public static string DecryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        public static string GetSystemMACID()
        {
            string systemName = System.Windows.Forms.SystemInformation.ComputerName;
            try
            {
                ManagementScope theScope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
                System.Management.ObjectQuery theQuery = new System.Management.ObjectQuery("SELECT * FROM Win32_NetworkAdapter");
                ManagementObjectSearcher theSearcher = new ManagementObjectSearcher(theScope, theQuery);
                ManagementObjectCollection theCollectionOfResults = theSearcher.Get();

                foreach (ManagementObject theCurrentObject in theCollectionOfResults)
                {
                    if (theCurrentObject["MACAddress"] != null)
                    {
                        string macAdd = theCurrentObject["MACAddress"].ToString();
                        return macAdd.Replace(':', '-');
                    }
                }
            }
            catch (ManagementException e)
            {
            }
            catch (System.UnauthorizedAccessException e)
            {

            }
            return string.Empty;
        }

        public static List<mac> ManualGetSystemMACID()
        {
            List<mac> macList = new List<mac>();
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface adapter in nics)
                {
                    macList.Add(new mac { MacName = adapter.Description, MacAddress = adapter.GetPhysicalAddress().ToString() });
                }
                return macList;

            }
            catch (ManagementException e)
            {
                MessageBox.Show(e.ToString());
            }
            catch (System.UnauthorizedAccessException e)
            {
                MessageBox.Show(e.ToString());
            }
            return macList;
        }

        public static Boolean IsRegister()
        {
            POSEntities entity = new POSEntities();
            //string MacId = Utility.GetSystemMACID();
            //Authorize currentKey = (from a in entity.Authorizes where a.macAddress == MacId select a).FirstOrDefault<Authorize>();
            //if (currentKey != null)
            //{
            //    return true;
            //}

            foreach (Authorize item in entity.Authorizes.ToList())
            {
                if (item.macAddress != null)
                {
                    string currentKey = Utility.DecryptString(item.macAddress, "ABCD");
                    if (currentKey != "")
                    {
                        foreach (var mac in Utility.ManualGetSystemMACID())
                        {
                            if (!String.IsNullOrWhiteSpace(mac.MacAddress))
                            {
                                string checkmac = Regex.Replace(mac.MacAddress, ".{2}", "$0-").Substring(0, 17);
                                if (currentKey == checkmac)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        //Calculate Exchange Rate
        public static decimal CalculateExchangeRate(int Id, decimal totalAmount)
        {
            POSEntities entity = new POSEntities();
            Currency currencyObj = entity.Currencies.FirstOrDefault(x => x.Id == Id);
            decimal value;
            if (currencyObj.CurrencyCode == "MMK")
            {
                value = (decimal)currencyObj.LatestExchangeRate * totalAmount;
                return value;
            }
            else if (currencyObj.CurrencyCode == "USD")
            {
                decimal num = (decimal)(((decimal)totalAmount / (decimal)currencyObj.LatestExchangeRate));
                value = Math.Ceiling(num * (decimal)Math.Pow(10, 2)) / (decimal)Math.Pow(10, 2);
                return value;
            }
            return 0;
        }

        //Save TransactionId and current exchange rate
        public static void AddExchangeRateForTransaction(int currrencyId, string transId)
        {
            POSEntities entity = new POSEntities();
            Currency currencyObj = entity.Currencies.FirstOrDefault(x => x.Id == currrencyId);
            ExchangeRateForTransaction ExRTObj = new ExchangeRateForTransaction();
            ExRTObj.CurrencyId = currrencyId;
            ExRTObj.TransactionId = transId;
            ExRTObj.ExchangeRate = Convert.ToInt32(currencyObj.LatestExchangeRate);
            entity.ExchangeRateForTransactions.Add(ExRTObj);
            entity.SaveChanges();
        }


        //Counter_Select
        public static string Counter_Check(ComboBox cboCounter)
        {
            string _counter = "";
            if (cboCounter.Enabled == true)
            {
                if (cboCounter.SelectedIndex > 0)
                {
                    _counter = cboCounter.Text;
                }
                else
                {
                    _counter = "All";
                }
            }
            else
            {
                _counter = "All";
            }
            return _counter;
        }

    }

    /// <summary>
    ///  call payment type by name 
    /// </summary>
    public enum CustomerPaymentType
    {
        Cash = 1,
        Credit,
        GiftCard,
        FOC,
        MPU

    }
    public static class Loc_CustomerPointSystem
    {
        public static int GetPointFromCustomerId(int CustomerId)
        {
            POSEntities entity = new POSEntities();
            Customer currentCustomer = entity.Customers.Where(x => x.Id == CustomerId).FirstOrDefault();
            PointDeductionPercentage_History pdp = entity.PointDeductionPercentage_History.Where(t => t.Active == true).FirstOrDefault();

            int point = 0;
            int totalRedeemPoint = 0;
            //Point are given only to VIP customer
            if (currentCustomer.CustomerTypeId == 1)
            {
                long totalSale = 0; long FirstVIPInvoiceTotal = 0;

                //Only Calculate point for transactions which user allowed to point
                List<Transaction> transactionList = currentCustomer.Transactions.Where(x => x.Loc_IsCalculatePoint == true && x.IsDeleted == false && x.IsComplete == true).ToList();
                Transaction transaction = new Transaction();
                //If current customer's promoted date is null,
                //customer must be VIP from the very start
                if (currentCustomer.PromoteDate != null)
                {
                    DateTime promoteDate = currentCustomer.PromoteDate.Value.Date;
                    transactionList = transactionList.Where(x => x.DateTime.Date >= promoteDate).ToList();
                }
                //All bonus points must be redeemed on/before 31 March of every year.
                int month = SettingController.DefaultExpireMonth;

                if (DateTime.Now.Year == 2021 && DateTime.Now.Month < 6)
                {
                    transactionList = transactionList.Where(x => (x.DateTime.Month > 3 && x.DateTime.Year == (DateTime.Now.Year - 1)) || x.DateTime.Year == DateTime.Now.Year).ToList();
                }
                else if ((DateTime.Now.Year == 2021 && DateTime.Now.Month > 5) || (DateTime.Now.Year == 2022 && DateTime.Now.Month < 4))
                {
                    if (DateTime.Now.Year == 2021)
                    {
                        transactionList = transactionList.Where(x => (x.DateTime.Month > 5 && x.DateTime.Year == DateTime.Now.Year)).ToList();
                    }
                    else
                    {
                        transactionList = transactionList.Where(x => (x.DateTime.Month > 5 && x.DateTime.Year == (DateTime.Now.Year - 1)) || (x.DateTime.Year == DateTime.Now.Year)).ToList();
                    }
                }
                // Updated by Lele 2024-Mar-11, extend Pts expired date from 2023-Apr-01 to 2024-May-31
                else if ((DateTime.Now.Year == 2023 && DateTime.Now.Month > 3) || (DateTime.Now.Year == 2024 && DateTime.Now.Month < 7))
                {
                    if (DateTime.Now.Year == 2023)
                    {
                        transactionList = transactionList.Where(x => (x.DateTime.Month > 3 && x.DateTime.Year == DateTime.Now.Year)).ToList();
                    }
                    else
                    {
                        transactionList = transactionList.Where(x => (x.DateTime.Month > 3 && x.DateTime.Year == (DateTime.Now.Year - 1)) || (x.DateTime.Year == DateTime.Now.Year)).ToList();
                    }
                }// Updated by Lele 2024-Mar-11, extend Pts expired date from 2023-Apr-01 to 2024-May-31
                else
                {
                    if (month == 12)
                    {
                        transactionList = transactionList.Where(x => x.DateTime.Year == DateTime.Now.Year).ToList();
                    }

                    else if (DateTime.Now.Month > month)
                    {
                        transactionList = transactionList.Where(x => x.DateTime.Month > month && x.DateTime.Year == DateTime.Now.Year).ToList();
                    }
                    else
                    {
                        transactionList = transactionList.Where(x => (x.DateTime.Month > month && x.DateTime.Year == (DateTime.Now.Year - 1)) || x.DateTime.Year == DateTime.Now.Year).ToList();

                    }
                }

                if (currentCustomer.VIPMemberRule.IsCalculatePoints == false)
                {
                    transaction = transactionList.Where(x => x.PaymentTypeId != (int)CustomerPaymentType.GiftCard &&
                        x.PaymentTypeId != (int)CustomerPaymentType.FOC && x.Type != TransactionType.Refund).FirstOrDefault();
                }

                if (transaction != null)
                {
                    if (transaction.DateTime.Date == currentCustomer.PromoteDate.Value.Date)
                    {
                        FirstVIPInvoiceTotal = Convert.ToInt32(transaction.TotalAmount);
                    }
                }


                //this statement is calculation for double promotion rule
                long discountItemAmount = 0;
                foreach (Transaction Items in transactionList)
                {  //IsDeductedby must not be null and equal to Zero  then   The discount rate of product must be greater than the rate of Double Promothion RULE                        
                    discountItemAmount += (long)Items.TransactionDetails.Where(t => (t.BdDiscounted == null || t.BdDiscounted == false) && t.IsDeductedBy != (decimal?)null && t.IsDeductedBy != 0 && decimal.Round(t.DiscountRate) >= decimal.Round(Convert.ToDecimal(t.IsDeductedBy))).Sum(s => s.TotalAmount);
                }

                //Exclude gift card payment for now
                //Also Exclude RefundTransactions
                //FOC Shouldn't include too
                totalSale = (long)transactionList.Where(x => x.PaymentTypeId != (int)CustomerPaymentType.GiftCard && x.PaymentTypeId != (int)CustomerPaymentType.FOC &&
                    x.Type != TransactionType.Refund).Sum(x => x.RecieveAmount);

                //Reduce refund transactions amount
                totalSale = totalSale - (long)(transactionList.Where(x => x.Type == TransactionType.Refund).Sum(x => x.TotalAmount));

                //Giftcard payment are not point calculatable,
                //Just extra amount from giftcard payment are able to convert to point
                totalSale += (long)transactionList.Where(x => x.PaymentTypeId == (int)CustomerPaymentType.GiftCard).Sum(x => x.RecieveAmount);

                totalSale -= FirstVIPInvoiceTotal;

                totalSale -= discountItemAmount;
                //Every 10,000 Kyats spent at shop earns you will get 1 point
                point = (int)totalSale / 10000;

                //Exclude Redeem Point
                //Calculate total redeem point for current year
                #region Calculate redeem point for current year

                List<Loc_PointRedeemHistory> plist = currentCustomer.Loc_PointRedeemHistory.ToList();
                if (plist.Count > 0)
                {
                    if (DateTime.Now.Year == 2021 && DateTime.Now.Month < 6)
                    {
                        foreach (Loc_PointRedeemHistory po in plist)
                        {
                            if (po.DateTime.Year == ((DateTime.Now.Year - 1)) && po.DateTime.Month > 3)
                            {
                                totalRedeemPoint += po.RedeemPoint;
                            }
                            if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month < 6)
                            {
                                totalRedeemPoint += po.RedeemPoint;
                            }
                        }
                    }
                    else if ((DateTime.Now.Year == 2021 && DateTime.Now.Month > 5) || (DateTime.Now.Year == 2022 && DateTime.Now.Month < 4))
                    {
                        if (DateTime.Now.Year == 2021)
                        {
                            foreach (Loc_PointRedeemHistory po in plist)
                            {
                                if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month > 5)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                            }
                        }
                        else
                        {
                            foreach (Loc_PointRedeemHistory po in plist)
                            {
                                if (po.DateTime.Year == ((DateTime.Now.Year - 1)) && po.DateTime.Month > 5)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                                if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month < 4)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                            }
                        }
                    }
                    // Updated by Lele 2024-Mar-11, extend Pts expired date from 2023-Apr-01 to 2024-May-31
                    else if ((DateTime.Now.Year == 2023 && DateTime.Now.Month > 3) || (DateTime.Now.Year == 2024 && DateTime.Now.Month < 7))
                    {
                        if (DateTime.Now.Year == 2023)
                        {
                            foreach (Loc_PointRedeemHistory po in plist)
                            {
                                if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month > 3)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                            }
                        }
                        else
                        {
                            foreach (Loc_PointRedeemHistory po in plist)
                            {
                                if (po.DateTime.Year == ((DateTime.Now.Year - 1)) && po.DateTime.Month > 3)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                                if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month < 7)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                            }
                        }
                    }
                    // Updated by Lele 2024-Mar-11, extend Pts expired date from 2023-Apr-01 to 2024-May-31
                    else
                    {
                        if (DateTime.Now.Month < month + 1)
                        {
                            foreach (Loc_PointRedeemHistory po in plist)
                            {

                                if (po.DateTime.Year == ((DateTime.Now.Year - 1)) && po.DateTime.Month > month)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                                if (po.DateTime.Year == (DateTime.Now.Year) && po.DateTime.Month < month + 1)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                            }
                        }
                        else
                        {
                            foreach (Loc_PointRedeemHistory po in plist)
                            {
                                if (po.DateTime.Year == ((DateTime.Now.Year)) && po.DateTime.Month > month)
                                {
                                    totalRedeemPoint += po.RedeemPoint;
                                }
                                //if (po.DateTime.Year == (DateTime.Now.Year + 1) && po.DateTime.Month < month + 1)
                                //{
                                //    totalRedeemPoint += po.RedeemPoint;
                                //}
                            }
                        }
                    }

                    #endregion

                    //previous one
                    point = point - totalRedeemPoint;
                    ////handle the nagtive value
                    //point = point < 0 ? 0 : point;


                }
            }
            return point;
        }


    }

    public class VIPPoint
    {
        public string Counter;

        public int BonudPoint;
        public int TotalSale;
    }


    public static class MemberShip
    {
        public static string UserName;
        public static string UserRole;
        public static int UserRoleId;
        public static int UserId;
        public static bool isLogin;
        public static bool isAdmin;
        public static int CounterId;
    }

    public static class TransactionType
    {
        public static string Sale
        {
            get { return "Sale"; }
        }
        public static string Refund
        {
            get { return "Refund"; }
        }
        public static string Settlement
        {
            get { return "Settlement"; }
        }
        public static string Credit
        {
            get { return "Credit"; }
        }
        public static string CreditRefund
        {
            get { return "CreditRefund"; }
        }
        public static string Prepaid
        {
            get { return "Prepaid"; }
        }
    }

    public class TransactionDetailByItemHolder
    {
        public string ItemId;
        public string Name;
        public string TransactionId;
        public string TransactionType;
        public int Qty;
        public int TotalAmount;
        public DateTime date;
    }

    public class TopProductHolder
    {
        public string ProductId;
        public string Name;
        public decimal Discount;
        public int Qty;
        public long UnitPrice;
        public long totalAmount;

    }
    public class CustomerInfoHolder
    {
        public int Id;
        public string Title;
        public string Name;
        public string PhNo;
        public string Address;
        public long OutstandingAmount;
        public long RefundAmount;
    }
    public class OutstandingInfoHolder
    {
        public int Id;
        public string Name;
        public string PhNo;
        public long RefundAmount;
        public long OutstandingAmount;
    }
    public class ReportItemSummary
    {
        public string Id;
        public string Name;
        public int Qty;
        public int UnitPrice;
        public long totalAmount;
        public int PaymentId;
        public string Size;
    }

    public class VIP_Member_Weekly_Summary
    {
        public string Name;
        public int InvoiceQty;
        public int Amount;
        public int Qty;
        public int Novelty_Qty;
        public int GWP_Qty;
        public string IsVIP;
    }

    public class GWP_Transactions_Controller
    {
        public string Name;
        public string TransactionNo;
        public string ItemCode;
        public string GiftName;
        public decimal Discount;
        public int Qty;
        public decimal TotalAmount;
    }

    public class GWPSet_Controller
    {
        public int Id;
        public string Name;
        public int Qty;
        public Int64 Amount;
    }
    public class SaleByRangeOrSegmentController
    {
        public int Id;
        public string BrandName;
        public decimal PeriodTotal;
        public decimal StartYearlyTotal;
    }
    public class SaleBreakDownController
    {
        public int bId;
        public string Name;
        public decimal Sales;
        public decimal BreakDown;
        public int saleQty;
        public decimal Refund;
        public int refundQty;
    }

    public class SpecialPromotionController
    {
        public int bId;
        public string Name;
        public decimal Sales;
        public decimal BreakDown;
        public int saleQty;
        public decimal Refund;
        public int refundQty;
    }
    public class ProductReportController
    {
        public string SKUCode;
        public string ProductName;
        public string BrandName;
        public int TotalQty;
        public string Segment;
        public string SubSegment;
        public string Line;
        public bool IsDiscontinous;

    }

    public class NoveltySaleController
    {
        public string SKUCode;
        public string ProductName;
        public int TotalQty;
        public int TotalAmount;
        public int UnitPrice;
    }

    public class AverageMonthlySaleController
    {
        public string ProductCode;
        public string ProductName;
        public string Unit;
        public int JanQty;
        public int FebQty;
        public int MarQty;
        public int AprQty;
        public int MayQty;
        public int JunQty;
        public int JulQty;
        public int AugQty;
        public int SepQty;
        public int OctQty;
        public int NovQty;
        public int DecQty;
        public int TotalQty;
        public decimal AvgQty;
        public int SellingPrice;
        public Int64 TotalAmount;
        public string Remark;
    }


    public class RoleManagementController
    {
        public RoleManagementModel Product { get; set; }
        public RoleManagementModel Brand { get; set; }
        public RoleManagementModel GiftCard { get; set; }
        public RoleManagementModel Customer { get; set; }
        public RoleManagementModel Supplier { get; set; }
        public RoleManagementModel Category { get; set; }
        public RoleManagementModel SubCategory { get; set; }
        public RoleManagementModel Counter { get; set; }
        public RoleManagementModel Promotion { get; set; }
        public RoleManagementModel Novelty { get; set; }

        //Reports
        public Boolean TransactionReport { get; set; }
        public Boolean ItemSummaryReport { get; set; }
        public Boolean TaxSummaryReport { get; set; }
        public Boolean ReorderPointReport { get; set; }
        public Boolean TransactionDetailReport { get; set; }
        public Boolean OutstandingCustomerReport { get; set; }
        public Boolean TopBestSellerReport { get; set; }
        public Boolean TransactionSummaryReport { get; set; }
        public Boolean GWPTransaction { get; set; }
        public Boolean MemberWeekly { get; set; }
        public Boolean MemberInfo { get; set; }
        public Boolean EmailCompilation { get; set; }
        public Boolean SaleBreakDown { get; set; }
        public Boolean SaleByRangeAndSegment { get; set; }

        private int UserRoleId { get; set; }

        public RoleManagementController()
        {
            Product = new RoleManagementModel();
            Brand = new RoleManagementModel();
            GiftCard = new RoleManagementModel();
            Customer = new RoleManagementModel();
            Supplier = new RoleManagementModel();
            Category = new RoleManagementModel();
            SubCategory = new RoleManagementModel();
            Counter = new RoleManagementModel();
            Promotion = new RoleManagementModel();
            Novelty = new RoleManagementModel();
        }

        public void Load(int roleId)
        {
            UserRoleId = roleId;

            POSEntities entity = new POSEntities();
            //Product
            Product.View = LoadRules(entity, "product_view");
            Product.EditOrDelete = LoadRules(entity, "product_edit");
            Product.Add = LoadRules(entity, "product_add");
            //Brand
            Brand.View = LoadRules(entity, "brand_view");
            Brand.EditOrDelete = LoadRules(entity, "brand_edit");
            Brand.Add = LoadRules(entity, "brand_add");
            //GiftCard
            GiftCard.View = LoadRules(entity, "giftcard_view");
            GiftCard.Add = LoadRules(entity, "giftcard_add");
            GiftCard.EditOrDelete = LoadRules(entity, "giftcard_edit");
            //Customer
            Customer.View = LoadRules(entity, "customer_view");
            Customer.EditOrDelete = LoadRules(entity, "customer_edit");
            Customer.Add = LoadRules(entity, "customer_add");
            //Supplier
            Supplier.View = LoadRules(entity, "supplier_view");
            Supplier.EditOrDelete = LoadRules(entity, "supplier_edit");
            Supplier.Add = LoadRules(entity, "supplier_add");
            //Category
            Category.View = LoadRules(entity, "category_view");
            Category.EditOrDelete = LoadRules(entity, "category_edit");
            Category.Add = LoadRules(entity, "category_add");
            //Sub Category
            SubCategory.View = LoadRules(entity, "subcategory_view");
            SubCategory.EditOrDelete = LoadRules(entity, "subcategory_edit");
            SubCategory.Add = LoadRules(entity, "subcategory_add");
            //Counter
            Counter.EditOrDelete = LoadRules(entity, "counter_edit");
            Counter.Add = LoadRules(entity, "counter_add");
            //Promotion
            Promotion.View = LoadRules(entity, "promotion_view");
            Promotion.EditOrDelete = LoadRules(entity, "promotion_edit");
            Promotion.Add = LoadRules(entity, "promotion_add");
            //Novelty
            Novelty.View = LoadRules(entity, "novelty_view");
            Novelty.EditOrDelete = LoadRules(entity, "novelty_edit");
            Novelty.Add = LoadRules(entity, "novelty_add");

            //Reports
            TransactionReport = LoadRules(entity, "transactionReport");
            ItemSummaryReport = LoadRules(entity, "itemSummaryReport");
            TaxSummaryReport = LoadRules(entity, "taxSummaryReport");
            ReorderPointReport = LoadRules(entity, "reorderPointReport");
            TransactionDetailReport = LoadRules(entity, "transactionDetailReport");
            OutstandingCustomerReport = LoadRules(entity, "outstandingCustomerReport");
            TopBestSellerReport = LoadRules(entity, "topBestSellerReport");
            TransactionSummaryReport = LoadRules(entity, "transactionSummaryReport");
            GWPTransaction = LoadRules(entity, "GWPTransactionReport");
            MemberWeekly = LoadRules(entity, "memberWeeklyReport");
            MemberInfo = LoadRules(entity, "memberInfoReport");
            EmailCompilation = LoadRules(entity, "emailCompilationReport");
            SaleBreakDown = LoadRules(entity, "SaleBreakDownReport");
            SaleByRangeAndSegment = LoadRules(entity, "SaleByRangeAndSegmentReport");
        }

        public void Save(int roleId)
        {
            UserRoleId = roleId;
            POSEntities entity = new POSEntities();

            //Delete old entry for this userroldId firstly
            List<APP_Data.RoleManagement> RulesListById = entity.RoleManagements.Where(x => x.UserRoleId == UserRoleId).ToList();
            foreach (APP_Data.RoleManagement rule in RulesListById)
            {
                entity.RoleManagements.Remove(rule);
            }

            //Product
            CreateRules(entity, Product.View, "product_view");
            CreateRules(entity, Product.EditOrDelete, "product_edit");
            CreateRules(entity, Product.Add, "product_add");
            //Brand
            CreateRules(entity, Brand.View, "brand_view");
            CreateRules(entity, Brand.EditOrDelete, "brand_edit");
            CreateRules(entity, Brand.Add, "brand_add");
            //GiftCard
            CreateRules(entity, GiftCard.View, "giftcard_view");
            CreateRules(entity, GiftCard.Add, "giftcard_add");
            CreateRules(entity, GiftCard.EditOrDelete, "giftcard_edit");
            //Customer
            CreateRules(entity, Customer.View, "customer_view");
            CreateRules(entity, Customer.EditOrDelete, "customer_edit");
            CreateRules(entity, Customer.Add, "customer_add");
            //Supplier
            CreateRules(entity, Supplier.View, "supplier_view");
            CreateRules(entity, Supplier.EditOrDelete, "supplier_edit");
            CreateRules(entity, Supplier.Add, "supplier_add");
            //Category
            CreateRules(entity, Category.View, "category_view");
            CreateRules(entity, Category.EditOrDelete, "category_edit");
            CreateRules(entity, Category.Add, "category_add");
            //Sub Category
            CreateRules(entity, SubCategory.View, "subcategory_view");
            CreateRules(entity, SubCategory.EditOrDelete, "subcategory_edit");
            CreateRules(entity, SubCategory.Add, "subcategory_add");
            //Counter
            CreateRules(entity, Counter.EditOrDelete, "counter_edit");
            CreateRules(entity, Counter.Add, "counter_add");
            //Promotion
            CreateRules(entity, Promotion.View, "promotion_view");
            CreateRules(entity, Promotion.EditOrDelete, "promotion_edit");
            CreateRules(entity, Promotion.Add, "promotion_add");
            //Novelty
            CreateRules(entity, Novelty.View, "novelty_view");
            CreateRules(entity, Novelty.EditOrDelete, "novelty_edit");
            CreateRules(entity, Novelty.Add, "novelty_add");

            //Reports
            CreateRules(entity, TransactionReport, "transactionReport");
            CreateRules(entity, ItemSummaryReport, "itemSummaryReport");
            CreateRules(entity, TaxSummaryReport, "taxSummaryReport");
            CreateRules(entity, ReorderPointReport, "reorderPointReport");
            CreateRules(entity, TransactionDetailReport, "transactionDetailReport");
            CreateRules(entity, OutstandingCustomerReport, "outstandingCustomerReport");
            CreateRules(entity, TopBestSellerReport, "topBestSellerReport");
            CreateRules(entity, TransactionSummaryReport, "transactionSummaryReport");
            CreateRules(entity, GWPTransaction, "GWPTransactionReport");
            CreateRules(entity, MemberWeekly, "memberWeeklyReport");
            CreateRules(entity, MemberInfo, "memberInfoReport");
            CreateRules(entity, EmailCompilation, "emailCompilationReport");
            CreateRules(entity, SaleBreakDown, "SaleBreakDownReport");
            CreateRules(entity, SaleByRangeAndSegment, "SaleByRangeAndSegmentReport");
            entity.SaveChanges();
        }

        private void CreateRules(POSEntities entity, Boolean IsAllowed, String Rule)
        {
            APP_Data.RoleManagement obj = new APP_Data.RoleManagement();
            obj.UserRoleId = UserRoleId;
            obj.IsAllowed = IsAllowed;
            obj.RuleFeature = Rule;

            entity.RoleManagements.Add(obj);
        }

        private Boolean LoadRules(POSEntities entity, String Rule)
        {
            APP_Data.RoleManagement obj = entity.RoleManagements.Where(x => x.RuleFeature == Rule && x.UserRoleId == UserRoleId).FirstOrDefault();
            Boolean result = false;
            if (obj != null) result = obj.IsAllowed;

            return result;
        }
    }

    public class RoleManagementModel
    {
        public Boolean View { get; set; }
        public Boolean EditOrDelete { get; set; }
        public Boolean Add { get; set; }

    }

    public static class SettingController
    {
        public static decimal birthday_discount
        {
            get
            {
                POSEntities db = new POSEntities();

                var currentset = db.Settings.Where(x => x.Key == "birthday_discount").FirstOrDefault();
                if (currentset != null)
                {
                    return Convert.ToDecimal(currentset.Value);
                }
                return 0;
            }
            set
            {
                POSEntities db = new POSEntities();
                var currentset = db.Settings.Where(x => x.Key == "birthday_discount").FirstOrDefault();
                if (currentset == null)
                {
                    currentset = new APP_Data.Setting();
                    currentset.Key = "birthday_discount";
                    currentset.Value = value.ToString();
                    db.Settings.Add(currentset);
                }
                else
                {
                    currentset.Value = value.ToString();
                }
                db.SaveChanges();
            }

        }

        public static string Application_ID
        {// by SYM
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "applicationId");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
        }

        public static string POS_ID
        {// by SYM
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "pos-Id");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
        }

        public static string Mall_Code
        {// by SYM
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "mall-code");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
        }

        public static string Application_Key
        {// by SYM
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "applicationKey");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
        }

        public static string Encrypted_Key
        {// by SYM
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "encryptedKey");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
        }


        public static string At_JunctionCity
        {// by SYM
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "At_JunctionCity");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
        }
        public static Shop DefaultShop
        {
            get
            {
                POSEntities entity = new POSEntities();
                Shop defaultShop = entity.Shops.Where(x => x.IsDefaultShop == true).FirstOrDefault();
                return defaultShop;
            }
        }

        public static string DefaultTaxRate
        {
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_tax_rate");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
            set
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_tax_rate");
                if (currentSet == null)
                {
                    currentSet = new APP_Data.Setting();
                    currentSet.Key = "default_tax_rate";
                    currentSet.Value = value.ToString();
                    entity.Settings.Add(currentSet);
                }
                else
                {
                    currentSet.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }

        public static int DefaultTopSaleRow
        {
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_top_sale_row");
                if (currentSet != null)
                {
                    return Convert.ToInt32(currentSet.Value);
                }

                return 0;
            }
            set
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_top_sale_row");
                if (currentSet == null)
                {
                    currentSet = new APP_Data.Setting();
                    currentSet.Key = "default_top_sale_row";
                    currentSet.Value = value.ToString();
                    entity.Settings.Add(currentSet);
                }
                else
                {
                    currentSet.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }

        public static int DefaultCurrency
        {
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_currency");
                if (currentSet != null)
                {
                    return Convert.ToInt32(currentSet.Value);
                }
                return 0;
            }

            set
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentset = entity.Settings.FirstOrDefault(x => x.Key == "default_currency");
                if (currentset == null)
                {
                    currentset = new APP_Data.Setting();
                    currentset.Key = "default_currency";
                    currentset.Value = value.ToString();
                    entity.Settings.Add(currentset);
                }
                else
                {
                    currentset.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }

        public static int GetExchangeRate(int Id)
        {
            POSEntities entity = new POSEntities();
            Currency currentCurrency = entity.Currencies.FirstOrDefault(x => x.Id == Id);
            if (currentCurrency != null)
            {
                return Convert.ToInt32(currentCurrency.LatestExchangeRate);
            }
            return 0;
        }
        public static void SetExchangeRate(int Id, int value)
        {
            POSEntities entity = new POSEntities();
            Currency currentCurrency = entity.Currencies.FirstOrDefault(x => x.Id == Id);
            currentCurrency.LatestExchangeRate = value;
            entity.SaveChanges();
        }
        public static int DefaultCity
        {
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_city_id");
                if (currentSet != null)
                {
                    return Convert.ToInt32(currentSet.Value);
                }
                return 0;
            }
            set
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_city_id");
                if (currentSet == null)
                {
                    currentSet = new APP_Data.Setting();
                    currentSet.Key = "default_city_id";
                    currentSet.Value = value.ToString();
                    entity.Settings.Add(currentSet);
                }
                else
                {
                    currentSet.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }

        public static int DefaultExpireMonth
        {
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_expire_month");
                if (currentSet != null)
                {
                    return Convert.ToInt32(currentSet.Value);
                }
                return 0;
            }
            set
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_expire_month");
                if (currentSet == null)
                {
                    currentSet = new APP_Data.Setting();
                    currentSet.Key = "default_expire_month";
                    currentSet.Value = value.ToString();
                    entity.Settings.Add(currentSet);
                }
                else
                {
                    currentSet.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }


        public static int DefaultVIPMemberRule
        {
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_VIPrule_id");
                if (currentSet != null)
                {
                    return Convert.ToInt32(currentSet.Value);
                }
                return 0;
            }
            set
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "default_VIPrule_id");
                if (currentSet == null)
                {
                    currentSet = new APP_Data.Setting();
                    currentSet.Key = "default_VIPrule_id";
                    currentSet.Value = value.ToString();
                    entity.Settings.Add(currentSet);
                }
                else
                {
                    currentSet.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }
    }

    public static class DatabaseControlSetting
    {
        public static string _ServerName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["_ServerName"];
            }
        }
        /// <summary>
        /// Get or Set the Database's Name
        /// </summary>
        public static string _DBName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["_DBName"];
            }
        }
        /// <summary>
        /// Get or Set the Database's Login User
        /// </summary>
        public static string _DBUser
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["_DBUser"];
            }
        }
        /// <summary>
        /// Get or Set the Database's Login Password
        /// </summary>
        public static string _DBPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["_DBPassword"];
            }
        }

    }


    public class RestoreHelper
    {
        public RestoreHelper()
        {

        }

        public void RestoreDatabase(String databaseName, String backUpFile, String serverName, String userName, String password)
        {
            ServerConnection connection = new ServerConnection(serverName, userName, password);
            Server sqlServer = new Server(connection);
            string dbaddr = string.Empty;
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                dbaddr = System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            }

            SqlConnection conn = new SqlConnection(dbaddr);
            string s = conn.State.ToString();
            conn.Close();
            SqlConnection.ClearPool(conn);
            Restore rstDatabase = new Restore();
            rstDatabase.Action = RestoreActionType.Database;
            rstDatabase.Database = databaseName;
            BackupDeviceItem bkpDevice = new BackupDeviceItem(backUpFile, DeviceType.File);
            rstDatabase.Devices.Add(bkpDevice);
            rstDatabase.ReplaceDatabase = true;
            rstDatabase.Complete += new ServerMessageEventHandler(sqlRestore_Complete);
            rstDatabase.PercentCompleteNotification = 10;
            rstDatabase.PercentComplete += new PercentCompleteEventHandler(sqlRestore_PercentComplete);
            rstDatabase.SqlRestore(sqlServer);
            sqlServer.Refresh();
        }

        public event EventHandler<PercentCompleteEventArgs> PercentComplete;

        void sqlRestore_PercentComplete(object sender, PercentCompleteEventArgs e)
        {
            if (PercentComplete != null)
                PercentComplete(sender, e);
        }

        public event EventHandler<ServerMessageEventArgs> Complete;

        void sqlRestore_Complete(object sender, ServerMessageEventArgs e)
        {
            if (Complete != null)
                Complete(sender, e);
        }
    }

    public class BackupHelper
    {
        public BackupHelper()
        {

        }

        public void BackupDatabase(String databaseName, String userName, String password, String serverName, String destinationPath, ref bool isBackUp)
        {
            Backup sqlBackup = new Backup();

            sqlBackup.Action = BackupActionType.Database;
            sqlBackup.BackupSetDescription = "ArchiveDataBase:" + DateTime.Now.ToShortDateString();
            sqlBackup.BackupSetName = "Archive";

            sqlBackup.Database = databaseName.Trim();

            BackupDeviceItem deviceItem = new BackupDeviceItem(destinationPath, DeviceType.File);
            ServerConnection connection = new ServerConnection(serverName, userName, password);
            Server sqlServer = new Server(connection);

            Database db = sqlServer.Databases[databaseName.Trim()];

            sqlBackup.Initialize = true;
            sqlBackup.Checksum = true;
            sqlBackup.ContinueAfterError = true;

            sqlBackup.Devices.Add(deviceItem);
            sqlBackup.Incremental = false;

            sqlBackup.ExpirationDate = DateTime.Now.AddDays(30);
            sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;

            sqlBackup.FormatMedia = false;
            try
            {
                sqlBackup.SqlBackup(sqlServer);
                isBackUp = true;
            }
            catch
            {
                MessageBox.Show("Please check the database if it's properly installed.");
            }
        }
    }

    #region PrintFunctions

    public static class PrintDoc
    {
        private static Boolean isStickerSize = false;
        private static Boolean isSlipSize = false;
        private static IList<Stream> m_streams;
        private static int m_currentPageIndex;

        #region Printing Functions

        private static void Print()
        {
            try
            {
                if (m_streams == null || m_streams.Count == 0)
                    return;

                PrintDocument printDoc = new PrintDocument();

                if (isStickerSize)
                    printDoc.PrinterSettings.PrinterName = DefaultPrinter.BarcodePrinter;
                else if (isSlipSize)
                    printDoc.PrinterSettings.PrinterName = DefaultPrinter.SlipPrinter;
                else
                    printDoc.PrinterSettings.PrinterName = DefaultPrinter.A4Printer;

                if (!printDoc.PrinterSettings.IsValid)
                {
                    string msg = String.Format("Can't find printer \"{0}\".", DefaultPrinter.A4Printer);
                    System.Diagnostics.Debug.WriteLine(msg);
                    return;
                }
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                printDoc.Print();

                printDoc.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void PrintReport(ReportViewer rv)
        {
            isStickerSize = false;
            m_currentPageIndex = 0;
            m_streams = null;
            Export(rv.LocalReport);
            Print();
            //  Dispose();
            rv.LocalReport.ReleaseSandboxAppDomain();
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="Type">BarcodeStricker||Slip </param>
        public static void PrintReport(ReportViewer mreportViewer, string Type)
        {
            m_currentPageIndex = 0;
            m_streams = null;
            if (Type == "BarcodeSTicker")
                isStickerSize = true;
            else
                isSlipSize = true;
            Export(mreportViewer.LocalReport);
            Print();
            mreportViewer.LocalReport.ReleaseSandboxAppDomain();
        }

        // Export the given report as an EMF (Enhanced Metafile) file.
        private static void Export(LocalReport report)
        {
            string deviceInfo = string.Empty;
            if (isStickerSize)
            {
                deviceInfo =
                  @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>3in</PageWidth>                
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
                </DeviceInfo>";
            }
            else if (isSlipSize)
            {
                deviceInfo =
                  @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>3in</PageWidth>                
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
             <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";
            }
            else
            {
                deviceInfo =
                  @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>8in</PageWidth>
                <PageHeight>10.5in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";
            }
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }

        private static void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);
            ev.Graphics.DrawImage(pageImage, ev.PageBounds);
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        private static Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        #endregion
    }

    public static class ExportReport
    {
        public static void Excel(ReportViewer mreportViwer, String FileName)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = mreportViwer.LocalReport.Render(
               "Excel", null, out mimeType, out encoding,
                out extension,
               out streamids, out warnings);
            try
            {
                FileStream fs = new FileStream(@"D:\Reports\" + FileName + DateTime.Now.ToString("ddMMyyyy") + ".xls", FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                MessageBox.Show(@"Report file is saved in D\Reports\" + FileName + DateTime.Now.ToString("ddMMyyyy") + ".xls", "Saving Complete");
            }
            catch (DirectoryNotFoundException message)
            {
                MessageBox.Show(@"The file patch (D:\Reports) isn't exist. Please check and create Reports folder in the Drive D>" + message.InnerException, "Error");
            }
        }
    }

    public static class DefaultPrinter
    {
        public static string BarcodePrinter
        {
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "barcode_printer");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
            set
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "barcode_printer");
                if (currentSet == null)
                {
                    currentSet = new APP_Data.Setting();
                    currentSet.Key = "barcode_printer";
                    currentSet.Value = value.ToString();
                    entity.Settings.Add(currentSet);
                }
                else
                {
                    currentSet.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }

        public static string A4Printer
        {
            get
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "a4_printer");
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
            set
            {
                POSEntities entity = new POSEntities();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == "a4_printer");
                if (currentSet == null)
                {
                    currentSet = new APP_Data.Setting();
                    currentSet.Key = "a4_printer";
                    currentSet.Value = value.ToString();
                    entity.Settings.Add(currentSet);
                }
                else
                {
                    currentSet.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }

        public static string SlipPrinter
        {
            get
            {
                POSEntities entity = new POSEntities();
                string key = "slip_printer_counter" + MemberShip.CounterId.ToString();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == key);
                if (currentSet != null)
                {
                    return Convert.ToString(currentSet.Value);
                }

                return string.Empty;
            }
            set
            {
                POSEntities entity = new POSEntities();
                string key = "slip_printer_counter" + MemberShip.CounterId.ToString();
                APP_Data.Setting currentSet = entity.Settings.FirstOrDefault(x => x.Key == key);
                if (currentSet == null)
                {
                    currentSet = new APP_Data.Setting();
                    currentSet.Key = key;
                    currentSet.Value = value.ToString();
                    entity.Settings.Add(currentSet);
                }
                else
                {
                    currentSet.Value = value.ToString();
                }
                entity.SaveChanges();
            }
        }
    }

    #endregion


}

