/////////////////////////////////////////////////////////////////////////////
//  Copyright (C) 1997-2010 Adlib Software
//  All rights reserved.
/////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Adlib.Director.DirectorWSAWrapper;
using System.IO;

namespace Adlib.Director.SampleApplication
{
    public partial class AddNewMetadataForm : Form
    {
        private MetadataTemplate _MetadataTemplate;
        private DirectorWSAWrapper.JobManagementService.Metadata _MetadataItem = new Adlib.Director.DirectorWSAWrapper.JobManagementService.Metadata();
        private bool _OkPressed = false;
        private MetadataDefinition _SelectedMetadataDef = null;
        private string _SelectedValue = null;
        private string _SelectedData = null;
        private string _PreviouslySelectedText = "";

        public AddNewMetadataForm()
        {
            InitializeComponent();
        }

        public void Init(MetadataTemplate metadataTemplate)
        {
            _MetadataTemplate = metadataTemplate;
        }

        public void SetDataForEdit(string name, string value, string data)
        {
            this.Text = "Edit Metadata Item";
            bool metadataDefExists = false;
            if (_MetadataTemplate != null && _MetadataTemplate.MetadataDefinitionList != null && _MetadataTemplate.MetadataDefinitionList.Count > 0)
            {
                foreach (MetadataDefinition metadataDefItem in _MetadataTemplate.MetadataDefinitionList)
                {
                    if (metadataDefItem != null)
                    {
                        if (metadataDefItem.Name != null && string.Compare(metadataDefItem.Name, name, false) == 0)
                        {
                            _SelectedMetadataDef = metadataDefItem;
                            metadataDefExists = true;
                            break;
                        }
                    }

                }
            }

            if (metadataDefExists == false)
            {
                MetadataDefinition metadataDef = new MetadataDefinition();
                metadataDef.CanEditName = true;
                metadataDef.CanEditValue = true;
                metadataDef.Name = name;
                metadataDef.ValueList.Add(value);

                if (_MetadataTemplate == null)
                {
                    _MetadataTemplate = new MetadataTemplate();
                }
                if (_MetadataTemplate.MetadataDefinitionList == null)
                {
                    _MetadataTemplate.MetadataDefinitionList = new List<MetadataDefinition>();
                }
                _MetadataTemplate.MetadataDefinitionList.Add(metadataDef);
                _SelectedMetadataDef = metadataDef;
            }

            _SelectedData = data;
            _SelectedValue = value;

        }
        public DirectorWSAWrapper.JobManagementService.Metadata GetAddedMetadata()
        {
            return _MetadataItem;
        }
        public bool OkPressed()
        {
            return _OkPressed;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            MetadataDefinition mdDef = (MetadataDefinition)comboBoxName.SelectedItem;
            //MetadataDefinition mdDef = (MetadataDefinition)listBoxMdValues.SelectedItem;
            if (mdDef != null)
            {
                listBoxMdValues.Items.Clear();
                UpdateRichTextBoxValue("", true);

                if (mdDef.ValueList != null && mdDef.ValueList.Count > 0)
                {
                    foreach (string mdValue in mdDef.ValueList)
                    {
                        if (!string.IsNullOrEmpty(mdValue))
                        {
                            listBoxMdValues.Items.Add(mdValue);
                        }
                    }
                    if (_SelectedMetadataDef == null)
                    {
                        if (listBoxMdValues.Items.Count > 0)
                        {
                            listBoxMdValues.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        listBoxMdValues.SelectedIndex = -1;
                    }
                }
                if (listBoxMdValues.Items.Count > 0)
                {
                    buttonAddMdValue.Enabled = true;
                }
                else
                {
                    buttonAddMdValue.Enabled = false;
                }

                if (mdDef.CanEditName)
                {
                    comboBoxName.DropDownStyle = ComboBoxStyle.DropDown;
                }
                else
                {
                    comboBoxName.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                if (mdDef.CanEditValue)
                {
                    richTextBoxMetadataData.Enabled = true;
                    btnAddMetadataBrowse.Enabled = true;
                }
                else
                {
                    richTextBoxMetadataData.Enabled = false;
                    btnAddMetadataBrowse.Enabled = false;
                }
                textBoxDescription.Text = mdDef.Description;
                UpdateRichTextBoxValue(listBoxMdValues.Text);
            }

        }
        private void btnAddMetadataOK_Click(object sender, EventArgs e)
        {
            MetadataDefinition mdDef = (MetadataDefinition)comboBoxName.SelectedItem;

            if (mdDef != null && string.IsNullOrEmpty(richTextBoxMetadataData.Text))
            {
                DialogResult dr = MessageBox.Show("Metadata value is empty. Do you like to save it?", "Metadata Value", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                }
                else
                {
                    return;
                }
                //MessageBox.Show("This metadata item requires a value ('Actial value' field).");
            }
            if (string.IsNullOrEmpty(comboBoxName.Text))
            {
                MessageBox.Show("Metadata 'name' must be specified.");
                return;
            }
            _MetadataItem = new Adlib.Director.DirectorWSAWrapper.JobManagementService.Metadata();
            _MetadataItem.Name = comboBoxName.Text;
            _MetadataItem.Value = richTextBoxMetadataData.Text;
            System.Windows.Forms.ButtonBase bb = (System.Windows.Forms.ButtonBase)sender;

            _OkPressed = true;
            this.Close();
        }

        private void btnAddMetadataCancel_Click(object sender, EventArgs e)
        {
            _OkPressed = false;
            this.Close();
        }

        private void btnAddMetadataBrowse_Click(object sender, EventArgs e)
        {
            StreamReader sr = null;
            string appWorkingPath = null;
            try
            {
                FileInfo fiAssembly = new FileInfo(System.Reflection.Assembly.GetCallingAssembly().Location);
                appWorkingPath = fiAssembly.DirectoryName;
            }
            catch
            {

            }
            try
            {
                if (!string.IsNullOrEmpty(appWorkingPath))
                {
                    openFileDialogData.InitialDirectory = appWorkingPath;
                }
                openFileDialogData.Filter = "All files (*.*)|*.*";
                openFileDialogData.Multiselect = false;
                openFileDialogData.FileName = "";
                if (openFileDialogData.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = openFileDialogData.FileName;
                    if (File.Exists(fileName))
                    {
                        sr = File.OpenText(fileName);
                        string text = sr.ReadToEnd();
                        UpdateRichTextBoxValue(text);
                    }
                    else
                    {
                        MessageBox.Show("File not found: " + fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load file. Error: " + ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        private void AddNewMetadataForm_Shown(object sender, EventArgs e)
        {
            if (_MetadataTemplate != null)
            {
                foreach (MetadataDefinition mdDef in _MetadataTemplate.MetadataDefinitionList)
                {
                    if (mdDef != null)
                    {
                        comboBoxName.Items.Add(mdDef);
                    }
                }
            }
            if (_SelectedMetadataDef != null)
            {
                comboBoxName.SelectedItem = _SelectedMetadataDef;
            }
            richTextBoxMetadataData.Enabled = false;

            UpdateSelection();
            if (_SelectedData != null)
            {
                UpdateRichTextBoxValue(_SelectedData);
            }
            if (_SelectedValue != null)
            {
                bool found = false;
                for (int i = 0; i < listBoxMdValues.Items.Count; i++)
                {
                    string str = (string)listBoxMdValues.Items[i];
                    if (str != null && string.Compare(str, _SelectedValue) == 0)
                    {
                        listBoxMdValues.SelectedIndex = i;
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    //listBoxMdValues.SelectedText = _SelectedValue;
                }
            }
        }

        private void buttonAddMdValue_Click(object sender, EventArgs e)
        {
            UpdateRichTextBoxValue(listBoxMdValues.Text);
        }

        private void UpdateRichTextBoxValue(string text)
        {
            UpdateRichTextBoxValue(text, false);
        }
        private void UpdateRichTextBoxValue(string text, bool overwrite)
        {
            if (!overwrite && !string.IsNullOrEmpty(richTextBoxMetadataData.Text) &&
                richTextBoxMetadataData.Text != _PreviouslySelectedText && !string.IsNullOrEmpty(_PreviouslySelectedText))
            {
                DialogResult dr = MessageBox.Show("You typed different text in 'Actual value' window. Do you like to overwrite it?", "Metadata Value", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    richTextBoxMetadataData.Text = text;
                    _PreviouslySelectedText = richTextBoxMetadataData.Text;
                }
            }
            else
            {
                richTextBoxMetadataData.Text = text;
                _PreviouslySelectedText = richTextBoxMetadataData.Text;
            }

        }

        private void listBoxMdValues_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxMdValues.Items.Count > 0)
            {
                UpdateRichTextBoxValue(listBoxMdValues.Text);
            }
        }

    }
}
