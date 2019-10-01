////////////////////////////////////////////////////////////////////////////
/*
Copyright (C) 1997-2010 Adlib Software
All rights reserved.

DISCLAIMER OF WARRANTIES:
 
Permission is granted to copy this Sample Code for internal use only, 
provided that this permission notice and warranty disclaimer appears in all copies.
 
SAMPLE CODE IS LICENSED TO YOU AS-IS.
 
ADLIB SOFTWARE AND ITS SUPPLIERS AND LICENSORS DISCLAIM ALL WARRANTIES, 
EITHER EXPRESS OR IMPLIED, IN SUCH SAMPLE CODE, INCLUDING THE WARRANTY OF 
NON-INFRINGEMENT AND THE IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A 
PARTICULAR PURPOSE. IN NO EVENT WILL ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS 
BE LIABLE FOR ANY DAMAGES ARISING OUT OF THE USE OF OR INABILITY TO USE THE SAMPLE 
APPLICATION OR SAMPLE CODE, DISTRIBUTION OF THE SAMPLE APPLICATION OR SAMPLE CODE, 
OR COMBINATION OF THE SAMPLE APPLICATION OR SAMPLE CODE WITH ANY OTHER CODE. 
IN NO EVENT SHALL ADLIB SOFTWARE OR ITS LICENSORS AND SUPPLIERS BE LIABLE FOR ANY 
LOST REVENUE, LOST PROFITS OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, 
INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, 
EVEN IF ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
*/
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Adlib.Director.DirectorWSAWrapper.JobManagementService;

namespace Adlib.Director.SampleApplication
{
    public partial class EditMetadata : Form
    {
        private Metadata _MetadataItem = null;
        private bool _OkPressed = false;
        public EditMetadata()
        {
            InitializeComponent();
        }
        public void Init(ref Metadata metadataToEdit)
        {
            _MetadataItem = metadataToEdit;
        }
        public bool OkPressed()
        {
            return _OkPressed;
        }
        private void btnEditMetadataOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxMetadataName.Text))
            {
                MessageBox.Show("Metadata 'name' must be specified.");
                return;
            }
            _MetadataItem.Name = textBoxMetadataName.Text;
            _MetadataItem.Value = textBoxMetadataValue.Text;
            if (!string.IsNullOrEmpty(richTextBoxMetadataData.Text))
            {
                _MetadataItem.Value = richTextBoxMetadataData.Text;
            }
            _OkPressed = true;
            this.Close();
        }

        private void btnEditMetadataBrowse_Click(object sender, EventArgs e)
        {
            StreamReader sr = null;
            try
            {
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
                        richTextBoxMetadataData.Text = text;
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

        private void btnEditMetadataCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EditMetadata_Load(object sender, EventArgs e)
        {

        }

        private void EditMetadata_Shown(object sender, EventArgs e)
        {
            if (_MetadataItem != null)
            {
                textBoxMetadataName.Text = _MetadataItem.Name;
                textBoxMetadataValue.Text = _MetadataItem.Value;
                richTextBoxMetadataData.Text = _MetadataItem.Value;

            }
        }
    }
}
