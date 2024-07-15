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
    public partial class Loc_ShopInformation : Form
    {
        #region Variables

        public Boolean isEdit { get; set; }

        public int ShopId { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();       

        #endregion

        #region Events

        public Loc_ShopInformation()
        {
            InitializeComponent();
        }

        private void Loc_ShopInformation_Load(object sender, EventArgs e)
        {

            List<APP_Data.City> cityList = entity.Cities.ToList();
            cboCity.DataSource = cityList;
            cboCity.DisplayMember = "CityName";
            cboCity.ValueMember = "Id";

            if (isEdit)
            {
                Shop updateShop = (from s in entity.Shops where s.Id == ShopId select s).FirstOrDefault();
                txtShopName.Text = updateShop.ShopName;
                txtAddress.Text  = updateShop.Address ;
                txtPhone.Text = updateShop.PhoneNumber;
                txtOpeningHours.Text = updateShop.OpeningHours;
                txtShortCode.Text = updateShop.ShortCode;
                cboCity.SelectedValue = updateShop.CityId;  /*Update by HMT*/
                //City city = (from c in entity.Cities where c.Id == updateShop.CityId select c).FirstOrDefault();
                //cboCity.Text = updateShop.City.CityName;                
                entity.Entry(updateShop).State = EntityState.Modified;
                entity.SaveChanges();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            tp.RemoveAll();
            tp.IsBalloon = true;
            tp.ToolTipIcon = ToolTipIcon.Error;
            tp.ToolTipTitle = "Error";
            if (txtShopName.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtShopName, "Error");
                tp.Show("Please fill up shop name!", txtShopName);
                hasError = true;
            }
            else if (txtShortCode.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtShortCode, "Error");
                tp.Show("Please fill up short code!", txtShortCode);
                hasError = true;
            }
            else
            {

                //Check if short code already exist
                Shop duplicateShortCodeShop = (from s in entity.Shops where s.ShortCode == txtShortCode.Text.Trim() select s).FirstOrDefault();
                if (!isEdit)
                {                    
                    if (duplicateShortCodeShop != null)
                    {
                        hasError = true;
                        tp.SetToolTip(txtShortCode, "Error");
                        tp.Show("Short code already exist!", txtShortCode);
                    }
                }
                else
                {
                    Shop updateShop = (from s in entity.Shops where s.Id == ShopId select s).FirstOrDefault();
                    if (updateShop.ShortCode != duplicateShortCodeShop.ShortCode)
                    {
                        hasError = true;
                        tp.SetToolTip(txtShortCode, "Error");
                        tp.Show("Short code already exist!", txtShortCode);
                    }
                }

            }


            if (!hasError)
            {
                if (isEdit)
                {
                    Shop updateShop = (from s in entity.Shops where s.Id == ShopId select s).FirstOrDefault();
                    updateShop.ShopName = txtShopName.Text;
                    updateShop.Address = txtAddress.Text;
                    updateShop.PhoneNumber = txtPhone.Text;                    
                    updateShop.OpeningHours = txtOpeningHours.Text;
                    updateShop.ShortCode = txtShortCode.Text;
                    updateShop.CityId = Convert.ToInt32(cboCity.SelectedValue);                    
                    entity.Entry(updateShop).State = EntityState.Modified;
                    entity.SaveChanges();

                    MessageBox.Show("Successfully Update!", "Update");
                    this.Dispose();
                }
                else
                {
                    Shop shopObj = new Shop();
                    shopObj.ShopName = txtShopName.Text;
                    shopObj.Address = txtAddress.Text;
                    shopObj.PhoneNumber = txtPhone.Text;
                    shopObj.OpeningHours = txtOpeningHours.Text;
                    shopObj.ShortCode = txtShortCode.Text;
                    shopObj.CityId = Convert.ToInt32(cboCity.SelectedValue);
                    entity.Shops.Add(shopObj);
                    entity.SaveChanges();

                    MessageBox.Show("Successfully Saved!", "Save");
                    this.Dispose();
                }

                if (Application.OpenForms["Setting"] != null)
                {
                    Setting newForm = (Setting)Application.OpenForms["Setting"];
                    newForm.UpdateShopList(txtShopName.Text);
                }
            }
        }

        #endregion
    }
}
