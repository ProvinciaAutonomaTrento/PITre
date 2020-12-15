using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Rubrica.ConfigCustomAction
{
    /// <summary>
    /// 
    /// </summary>
    public partial class WebConfigurations : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public WebConfigurations(string[] args)
        {
            InitializeComponent();

            if (args != null && args.Length > 0)
            {
                this.WebConfigPath = args[0];

                this.btnBrowse.Enabled = false;
                this.txtWebConfigPath.Text = this.WebConfigPath;
            }
            else
            {
                this.btnBrowse.Enabled = true;
            }
        }

        /// <summary>
        /// Path del file web.config
        /// </summary>
        protected string WebConfigPath
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboDatabaseTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.grdConnectionStringProperties.SelectedObject = this.cboDatabaseTypes.SelectedItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebConfigurations_Load(object sender, EventArgs e)
        {
            this.cboDatabaseTypes.Items.Add(new OracleConnectionStringSettings());
            this.cboDatabaseTypes.Items.Add(new SqlConnectionStringSettings());

            this.cboDatabaseTypes.SelectedItem = this.cboDatabaseTypes.Items[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.WebConfigPath))
            {
                MessageBox.Show("Path web.config non impostato", "Rubrica", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtWebConfigPath.Focus();
            }
            else if (string.IsNullOrEmpty(this.txtLogRootPath.Text))
            {
                MessageBox.Show("Path log non impostato", "Rubrica", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.txtLogRootPath.Focus();
            }
            else
            {
                ConnectionStringSettings settings = (ConnectionStringSettings)this.cboDatabaseTypes.SelectedItem;

                XmlDocument document = new XmlDocument();
                document.Load(this.WebConfigPath);

                XmlNode connectionStringsNode = document.SelectSingleNode("/configuration/connectionStrings");

                if (connectionStringsNode != null)
                    connectionStringsNode.RemoveAll();
                else
                {
                    connectionStringsNode = document.CreateNode(XmlNodeType.Element, "connectionStrings", string.Empty);
                    document.DocumentElement.AppendChild(connectionStringsNode);
                }

                XmlNode connectionStringNode = document.CreateNode(XmlNodeType.Element, "add", string.Empty);

                XmlAttribute attribute = document.CreateAttribute("name");
                attribute.Value = "Rubrica";
                connectionStringNode.Attributes.Append(attribute);

                attribute = document.CreateAttribute("connectionString");
                attribute.Value = settings.GetConnectionString();
                connectionStringNode.Attributes.Append(attribute);

                attribute = document.CreateAttribute("providerName");
                attribute.Value = settings.GetProviderName();
                connectionStringNode.Attributes.Append(attribute);

                connectionStringsNode.AppendChild(connectionStringNode);


                XmlNode appSettingsNode = document.SelectSingleNode("/configuration/appSettings");
                if (appSettingsNode != null)
                    appSettingsNode.RemoveAll();
                else
                {
                    appSettingsNode = document.CreateNode(XmlNodeType.Element, "appSettings", string.Empty);
                    document.DocumentElement.AppendChild(appSettingsNode);
                }

                XmlNode logRootNode = document.CreateNode(XmlNodeType.Element, "add", string.Empty);
                attribute = document.CreateAttribute("key");
                attribute.Value = "LogRootPath";
                logRootNode.Attributes.Append(attribute);

                attribute = document.CreateAttribute("value");
                attribute.Value = this.txtLogRootPath.Text;
                logRootNode.Attributes.Append(attribute);

                appSettingsNode.AppendChild(logRootNode);

                document.Save(this.WebConfigPath);

                this.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "web.config|web.config";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.WebConfigPath = dlg.FileName;
                    this.txtWebConfigPath.Text = this.WebConfigPath;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogRootPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.txtLogRootPath.Text = dlg.SelectedPath;
                }
            }
        }
    }
}
