using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VerificaSupporto
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VerificaSupportoContainer : UserControl
    {
        public VerificaSupportoContainer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdPeople
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PathSupporto
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PercentualeVerifica
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Interrotta
        {
            get;
            set;
        }

        /// <summary>
        /// Esecuzione logica
        /// </summary>
        public void Execute()
        {
            try
            {
                using (VerificaSupporto.VerificaSupportoProgress progress = new VerificaSupportoProgress(
                                        this.ServiceUrl, this.IdPeople,
                                        this.IdIstanza, this.IdDocumento, 
                                        this.PathSupporto, this.PercentualeVerifica))
                {
                    progress.ShowDialog();

                    this.Success = progress.Success;
                    this.ErrorMessage = progress.ErrorMessage;
                    this.Interrotta = progress.Interrotta;
                }
            }
            catch (Exception ex)
            {
                this.Success = false;
                this.ErrorMessage = ex.Message;
            }
        }
    }
}