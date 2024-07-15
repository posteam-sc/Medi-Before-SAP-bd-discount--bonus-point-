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
    public partial class NewUser : Form
    {
        #region Variables

        public Boolean isEdit { get; set; }

        public int UserId { get; set; }

        private POSEntities entity = new POSEntities();

        private ToolTip tp = new ToolTip();

        #endregion

        #region Events

        public NewUser()
        {
            InitializeComponent();
        }

        private void NewUser_Load(object sender, EventArgs e)
        {
            cboUserRole.DataSource = entity.UserRoles.ToList();
            cboUserRole.DisplayMember = "RoleName";
            cboUserRole.ValueMember = "Id";            

            if (isEdit)
            {
                //Editing here
                User currentUser = (from c in entity.Users where c.Id == UserId select c).FirstOrDefault<User>();
                txtName.Text = currentUser.Name;
                cboUserRole.SelectedValue = currentUser.UserRoleId;
                txtPassword.Text = Utility.DecryptString(currentUser.Password,"SCPos");
                txtConfirmPassword.Text = Utility.DecryptString(currentUser.Password,"SCPos");
                btnSubmit.Image = POS.Properties.Resources.update_big;
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
            if (txtName.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtName, "Error");
                tp.Show("User name cannot be empty!", txtName);
                hasError = true;
            }
            else if (txtPassword.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtPassword, "Error");
                tp.Show("Password cannot be empty!", txtPassword);
                hasError = true;
            }
            else if (txtConfirmPassword.Text.Trim() == string.Empty)
            {
                tp.SetToolTip(txtConfirmPassword, "Error");
                tp.Show("Confirm Passoword cannot be empty!", txtConfirmPassword);
                hasError = true;
            }
            else if (txtConfirmPassword.Text.Trim() != txtPassword.Text.Trim())
            {
                tp.SetToolTip(txtConfirmPassword, "Error");
                tp.Show("Password and confirm password do not match!", txtConfirmPassword);
                hasError = true;
            }

            if (!hasError)
            {
                //Edit
                if (isEdit)
                {
                    int count = (from u in entity.Users where u.Name == txtName.Text && u.Id != UserId select u).ToList().Count;
                    if (count == 0)
                    {
                        User currentUser = (from c in entity.Users where c.Id == UserId select c).FirstOrDefault<User>();
                        currentUser.Name = txtName.Text;
                        currentUser.Password = Utility.EncryptString(txtPassword.Text, "SCPos");
                        if (cboUserRole.SelectedValue != null) currentUser.UserRoleId = Convert.ToInt32(cboUserRole.SelectedValue.ToString());
                        entity.Entry(currentUser).State = EntityState.Modified;
                        entity.SaveChanges();
                        MessageBox.Show("Successfully Update!", "Update");
                        this.Dispose();
                    }
                    else
                    {
                        tp.SetToolTip(txtName, "Error");
                        tp.Show("This user name is already exist!", txtName);
                    }
                }
                //create new user
                else
                {
                    int count = (from u in entity.Users where u.Name == txtName.Text select u).ToList().Count;
                    if (count == 0)
                    {
                        User newUser = new User();
                        newUser.Name = txtName.Text;
                        if (cboUserRole.SelectedValue != null) newUser.UserRoleId = Convert.ToInt32(cboUserRole.SelectedValue.ToString());
                        newUser.Password = Utility.EncryptString(txtPassword.Text, "SCPos");
                        newUser.DateTime = DateTime.Now;
                        entity.Users.Add(newUser);
                        entity.SaveChanges();
                        MessageBox.Show("Successfully Saved!", "Save");
                        this.Dispose();
                    }
                    else
                    {
                        tp.SetToolTip(txtName, "Error");
                        tp.Show("This user name is already exist!", txtName);
                    }
                }
                
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                this.Dispose();
            }
        }

        private void NewUser_MouseMove(object sender, MouseEventArgs e)
        {
            tp.Hide(txtName);
            tp.Hide(txtPassword);
            tp.Hide(txtConfirmPassword);
        }

        #endregion

    }
}
