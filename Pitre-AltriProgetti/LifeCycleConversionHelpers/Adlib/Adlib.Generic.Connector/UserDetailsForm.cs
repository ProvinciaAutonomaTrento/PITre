using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Adlib.Director.SampleApplication
{
    public partial class UserDetailsForm : Form
    {
        private bool _OkPressed = false;
        private string _UserName;
        private string _Password;
        private string _FriendlyName;
        private bool _ShowFriendlyName = true;

        public bool ShowFriendlyName
        {
            get { return _ShowFriendlyName; }
            set { _ShowFriendlyName = value; }
        }

        public string FriendlyName
        {
            get { return _FriendlyName; }
        }
        
        public UserDetailsForm()
        {
            InitializeComponent();
        }

        private void btnSelectIdOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxUserName.Text))
            {
                MessageBox.Show("Username can't be empty");
                return;
            }
            if (string.IsNullOrEmpty(textBoxPwd.Text))
            {
                MessageBox.Show("Password can't be empty");
                return;
            }
            _UserName = textBoxUserName.Text;
            _Password = textBoxPwd.Text;
            _FriendlyName = textBoxFriendlyName.Text;
            _OkPressed = true;
            this.Close();

        }

        private void btnSelectIdCancel_Click(object sender, EventArgs e)
        {
            _OkPressed = false;
            this.Close();
        }

        public bool IsOkPressed()
        {
            return _OkPressed;
        }
        public string UserName()
        {
            return _UserName;
        }
        public string Password()
        {
            return _Password;
        }

        private void UserDetailsForm_Load(object sender, EventArgs e)
        {
            if (_ShowFriendlyName == false)
            {
                textBoxFriendlyName.Enabled = false;
            }
        }

    }
}
