// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class ConfigForm : Form
    {
        private AppConfig _config;

        public ConfigForm(AppConfig config)
        {
            InitializeComponent();
            _config = config;

            // Imposta i valori iniziali
            txtServerUrl.Text = _config.ServerUrl;
            txtUserName.Text = _config.UserName;
            txtUserEmail.Text = _config.UserEmail;
            txtUserRole.Text = _config.UserRole;
            txtCustomData.Text = _config.CustomData;
            chkStartMinimized.Checked = _config.StartMinimized;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Salva i valori nella configurazione
            _config.ServerUrl = txtServerUrl.Text;
            _config.UserName = txtUserName.Text;
            _config.UserEmail = txtUserEmail.Text;
            _config.UserRole = txtUserRole.Text;
            _config.CustomData = txtCustomData.Text;
            _config.StartMinimized = chkStartMinimized.Checked;

            // Salva la configurazione
            _config.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
