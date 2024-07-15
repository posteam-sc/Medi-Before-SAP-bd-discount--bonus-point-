using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.APP_Data;
namespace POS
{
    public partial class Setting : Form
    {
        #region Variable
        POSEntities entity = new POSEntities();
        private ToolTip tp = new ToolTip();
        #endregion

        public Setting()
        {
            InitializeComponent();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            #region Printer

            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                cboBarcodePrinter.Items.Add(printerName);
                cboA4Printer.Items.Add(printerName);
                cboSlipPrinter.Items.Add(printerName);
            }

            if (DefaultPrinter.BarcodePrinter != null)
            {
                cboBarcodePrinter.Text = DefaultPrinter.BarcodePrinter;
            }
            if (DefaultPrinter.A4Printer != null)
            {
                cboA4Printer.Text = DefaultPrinter.A4Printer;
            }
            if (DefaultPrinter.SlipPrinter != null)
            {
                cboSlipPrinter.Text = DefaultPrinter.SlipPrinter;
            }

            #endregion

            #region taxPercent
            List<Tax> taxList = entity.Taxes.ToList();
            List<Tax> result = new List<Tax>();
            foreach (Tax r in taxList)
            {
                Tax t = new Tax();
                t.Id = r.Id;
                t.Name = r.Name + " and " + r.TaxPercent;
                t.TaxPercent = r.TaxPercent;
                result.Add(t);
            }
            cboTaxList.DataSource = result;
            cboTaxList.DisplayMember = "Name";
            cboTaxList.ValueMember = "Id";
            if (SettingController.DefaultTaxRate != null)
            {
                int id = Convert.ToInt32(SettingController.DefaultTaxRate);
                Tax defaultTax = (from t in entity.Taxes where t.Id == id select t).FirstOrDefault();
                cboTaxList.Text = defaultTax.Name + " and " + defaultTax.TaxPercent;
            }
            #endregion

            #region city
            List<APP_Data.City> cityList = entity.Cities.ToList();
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";
            if (SettingController.DefaultVIPMemberRule != 0)
            {
                int id = Convert.ToInt32(SettingController.DefaultVIPMemberRule);
                APP_Data.VIPMemberRule viprule = entity.VIPMemberRules.Where(x => x.Id == id).FirstOrDefault();
                cboMemberRule.Text = viprule.RuleName;
            }
            #endregion

            #region shop

            cboShopList.DataSource = entity.Shops.ToList();
            cboShopList.DisplayMember = "ShopName";
            cboShopList.ValueMember = "Id";
            cboShopList.Text = SettingController.DefaultShop.ShopName;

            #endregion
            #region Expire Month

            txtExpireMonth.Text = SettingController.DefaultExpireMonth.ToString();
            #endregion


            #region currency
            List<Currency> currencyList = new List<Currency>();
            currencyList.AddRange(entity.Currencies.ToList());
            cboCurrency.DataSource = currencyList;
            cboCurrency.DisplayMember = "CurrencyCode";
            cboCurrency.ValueMember = "Id";
            if (SettingController.DefaultCurrency != 0)
            {
                int id = Convert.ToInt32(SettingController.DefaultCurrency);
                Currency curreObj = entity.Currencies.FirstOrDefault(x => x.Id == id);
                cboCurrency.Text = curreObj.CurrencyCode;
            }
            //txtExchangeRate.Text = SettingController.DefaultExchangeRate.ToString();
            #endregion

            #region VIPMemberRule
            Bind_VIPMember();
            #endregion

        }

        public void Bind_VIPMember()
        {
            List<APP_Data.VIPMemberRule> VIPMemberRuleList = entity.VIPMemberRules.ToList();
            cboMemberRule.DataSource = VIPMemberRuleList;
            cboMemberRule.DisplayMember = "RuleName";
            cboMemberRule.ValueMember = "Id";
            if (SettingController.DefaultVIPMemberRule != 0)
            {
                int id = Convert.ToInt32(SettingController.DefaultVIPMemberRule);
                APP_Data.VIPMemberRule viprule = entity.VIPMemberRules.Where(x => x.Id == id).FirstOrDefault();
                cboMemberRule.Text = viprule.RuleName;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Boolean hasError = false;

            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            //Validation
            //if (Convert.ToInt32(cboCurrency.SelectedValue) == 0)
            //{
            //    tp.SetToolTip(txtExchangeRate, "Error");
            //    tp.Show("Please select current exchange rate!", txtExchangeRate);
            //    hasError = true;
            //}
            if (Convert.ToInt32(txtExpireMonth.Text) > 12 || Convert.ToInt32(txtExpireMonth.Text) <= 0)
            {
                tp.SetToolTip(txtExpireMonth, "Error");
                tp.Show("Expire Month Not Greater Than 12 Or Not be Zero!", txtExpireMonth);
                hasError = true;
            }

            if (!hasError)
            {
                DefaultPrinter.BarcodePrinter = cboBarcodePrinter.Text;
                DefaultPrinter.A4Printer = cboA4Printer.Text;
                DefaultPrinter.SlipPrinter = cboSlipPrinter.Text;

                SettingController.DefaultTaxRate = cboTaxList.SelectedValue.ToString();

                int topcity = 0;
                Int32.TryParse(cboCity.SelectedValue.ToString(), out topcity);
                SettingController.DefaultCity = topcity;

                int Id = Convert.ToInt32(cboCurrency.SelectedValue);
                SettingController.DefaultCurrency = Id;

                //Default Expire Month 
                int month = Convert.ToInt32(txtExpireMonth.Text);
                SettingController.DefaultExpireMonth = month;

                int topSalesRow = 0;
                Int32.TryParse(txtDefaultSalesRow.Text, out topSalesRow);
                SettingController.DefaultTopSaleRow = topSalesRow;

                int topVIPRule = 0;
                Int32.TryParse(cboMemberRule.SelectedValue.ToString(), out topVIPRule);
                SettingController.DefaultVIPMemberRule = topVIPRule;

                List<Shop> shopList = entity.Shops.Where(x => x.IsDefaultShop == true).ToList();
                foreach (Shop shop in shopList)
                {
                    shop.IsDefaultShop = false;
                }
                //if (cboCurrency.SelectedIndex > 0)
                //{
                //    int Id = Convert.ToInt32(cboCurrency.SelectedValue);
                //    int exchangeRate = 0;
                //    Int32.TryParse(txtExchangeRate.Text, out exchangeRate);
                //    SettingController.SetExchangeRate(Id, exchangeRate);
                //}
                int defaultShopId = Convert.ToInt32(cboShopList.SelectedValue);
                Shop defaultShop = entity.Shops.Where(x => x.Id == defaultShopId).FirstOrDefault();
                defaultShop.IsDefaultShop = true;
                entity.SaveChanges();

                MessageBox.Show("Successfully Saved!");
                this.Dispose();
            }
        }

        private void btnNewTax_Click(object sender, EventArgs e)
        {
            Taxes newForm = new Taxes();
            newForm.ShowDialog();
        }

        private void btnNewCity_Click(object sender, EventArgs e)
        {
            City newForm = new City();
            newForm.ShowDialog();
        }

        private void cboShopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Shop currentShop = (Shop)cboShopList.SelectedItem;
            currentShop.City = entity.Cities.Where(c => c.Id == currentShop.CityId).FirstOrDefault();
            lblShopName.Text = currentShop.ShopName;
            lblAddress.Text = currentShop.Address;
            lblOpeningHours.Text = currentShop.OpeningHours;
            lblPhoneNumber.Text = currentShop.PhoneNumber;
            lblCity.Text = currentShop.City.CityName;
        }

        private void btnEditShop_Click(object sender, EventArgs e)
        {
            Loc_ShopInformation newForm = new Loc_ShopInformation();
            newForm.ShopId = Convert.ToInt32(cboShopList.SelectedValue);
            newForm.isEdit = true;
            newForm.ShowDialog();
        }

        private void btnAddShop_Click(object sender, EventArgs e)
        {
            Loc_ShopInformation newForm = new Loc_ShopInformation();
            newForm.ShowDialog();
        }

        #region Function
        public void ReLoadData()
        {
            #region taxPercent
            List<Tax> taxList = entity.Taxes.ToList();
            List<Tax> result = new List<Tax>();
            foreach (Tax r in taxList)
            {
                Tax t = new Tax();
                t.Id = r.Id;
                t.Name = r.Name + " and " + r.TaxPercent;
                t.TaxPercent = r.TaxPercent;
                result.Add(t);
            }
            cboTaxList.DataSource = result;
            cboTaxList.DisplayMember = "Name";
            cboTaxList.ValueMember = "Id";
            if (SettingController.DefaultTaxRate != null)
            {
                int id = Convert.ToInt32(SettingController.DefaultTaxRate);
                Tax defaultTax = (from t in entity.Taxes where t.Id == id select t).FirstOrDefault();
                cboTaxList.Text = defaultTax.Name + " and " + defaultTax.TaxPercent;
            }
            #endregion

            #region city
            List<APP_Data.City> cityList = entity.Cities.ToList();
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";
            if (SettingController.DefaultCity != null)
            {
                int id = Convert.ToInt32(SettingController.DefaultCity);
                APP_Data.City city = entity.Cities.Where(x => x.Id == id).FirstOrDefault();
                cboCity.Text = city.CityName;
            }
            #endregion
        }
        #endregion

        public void UpdateShopList(string newShopName)
        {

            cboShopList.DataSource = entity.Shops.ToList();
            cboShopList.DisplayMember = "ShopName";
            cboShopList.ValueMember = "Id";
            cboShopList.Text = newShopName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MemberRule newForm = new MemberRule();
            newForm.ShowDialog();
        }

        private void chkDeduct_CheckedChanged(object sender, EventArgs e)
        {//LHST
            if (chkDeduct.Checked == true)
            {
                txtpercentageTOdeduct.Enabled = true;
            }
            else
            {
                txtpercentageTOdeduct.Enabled = false;
                txtpercentageTOdeduct.Text = "";
            }
        }

        private void groupBox8_MouseHover(object sender, EventArgs e)
        {
            ttPromoRule.Show("Not Calulate Points After ... % you entered", this.groupBox8);
        }

        private void txtpercentageTOdeduct_MouseHover(object sender, EventArgs e)
        {
            ttPromoRule.Show("Not Calulate Points After ... % you entered", this.txtpercentageTOdeduct);
        }

        private void chkDeduct_MouseHover(object sender, EventArgs e)
        {
            ttPromoRule.Show("Not Calulate Points After ... % you entered", this.chkDeduct);
        }


        private void txtExpireMonth_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
    }
}
