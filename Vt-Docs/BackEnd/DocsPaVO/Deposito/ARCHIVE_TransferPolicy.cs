using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Deposito
{
    /// <summary>
    /// This is the Data Transfer Object class for ARCHIVE_TransferPolicy.
    /// </summary>
    public partial class ARCHIVE_TransferPolicy
    {

        #region Fields

        private Int32 _System_ID;
        private String _Description;
        private Int32 _Enabled;
        private Int32? _Transfer_ID;
        private Int32 _TransferPolicyType_ID;
        private Int32 _TransferPolicyState_ID;
        private Int32? _Registro_ID;
        private Int32? _UO_ID;
        private Int32? _IncludiSottoalberoUO;
        private Int32? _Tipologia_ID;
        private Int32? _Titolario_ID;
        private String _ClasseTitolario;
        private Int32? _IncludiSottoalberoClasseTit;
        private Int32? _AnnoCreazioneDa;
        private Int32? _AnnoCreazioneA;
        private Int32? _AnnoProtocollazioneDa;
        private Int32? _AnnoProtocollazioneA;
        private Int32? _AnnoChiusuraDa;
        private Int32? _AnnoChiusuraA;
        #endregion

        #region Properties

        /// <summary>
        /// The database automatically generates this value.
        /// </summary>
        public virtual Int32 System_ID
        {

            get
            {
                return _System_ID;
            }

            set
            {
                _System_ID = value;
            }
        }

        public virtual String Description
        {

            get
            {
                return _Description;
            }

            set
            {
                _Description = value;
            }
        }

        public virtual Int32 Enabled
        {

            get
            {
                return _Enabled;
            }

            set
            {
                _Enabled = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? Transfer_ID
        {

            get
            {
                return _Transfer_ID;
            }

            set
            {
                _Transfer_ID = value;
            }
        }

        public virtual Int32 TransferPolicyType_ID
        {

            get
            {
                return _TransferPolicyType_ID;
            }

            set
            {
                _TransferPolicyType_ID = value;
            }
        }

        public virtual Int32 TransferPolicyState_ID
        {

            get
            {
                return _TransferPolicyState_ID;
            }

            set
            {
                _TransferPolicyState_ID = value;
            }
        }


        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? Registro_ID
        {

            get
            {
                return _Registro_ID;
            }

            set
            {
                _Registro_ID = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? UO_ID
        {

            get
            {
                return _UO_ID;
            }

            set
            {
                _UO_ID = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? IncludiSottoalberoUO
        {

            get
            {
                return _IncludiSottoalberoUO;
            }

            set
            {
                _IncludiSottoalberoUO = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? Tipologia_ID
        {

            get
            {
                return _Tipologia_ID;
            }

            set
            {
                _Tipologia_ID = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? Titolario_ID
        {

            get
            {
                return _Titolario_ID;
            }

            set
            {
                _Titolario_ID = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual String ClasseTitolario
        {

            get
            {
                return _ClasseTitolario;
            }

            set
            {
                _ClasseTitolario = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? IncludiSottoalberoClasseTit
        {

            get
            {
                return _IncludiSottoalberoClasseTit;
            }

            set
            {
                _IncludiSottoalberoClasseTit = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? AnnoCreazioneDa
        {

            get
            {
                return _AnnoCreazioneDa;
            }

            set
            {
                _AnnoCreazioneDa = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? AnnoCreazioneA
        {

            get
            {
                return _AnnoCreazioneA;
            }

            set
            {
                _AnnoCreazioneA = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? AnnoProtocollazioneDa
        {

            get
            {
                return _AnnoProtocollazioneDa;
            }

            set
            {
                _AnnoProtocollazioneDa = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? AnnoProtocollazioneA
        {

            get
            {
                return _AnnoProtocollazioneA;
            }

            set
            {
                _AnnoProtocollazioneA = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? AnnoChiusuraDa
        {

            get
            {
                return _AnnoChiusuraDa;
            }

            set
            {
                _AnnoChiusuraDa = value;
            }
        }

        /// <summary>
        /// This is not a required field.
        /// </summary>
        public virtual Int32? AnnoChiusuraA
        {

            get
            {
                return _AnnoChiusuraA;
            }

            set
            {
                _AnnoChiusuraA = value;
            }
        }

        #endregion

        #region Default Constructor

        public ARCHIVE_TransferPolicy()
        {
        }

        #endregion

        #region Constructors

        public ARCHIVE_TransferPolicy(Int32 system_ID, String description, Int32 enabled, Int32? transfer_ID, Int32 transferPolicyType_ID,Int32 transferPolicyState_ID,Int32? registro_ID, Int32? uO_ID, Int32? includiSottoalberoUO, Int32? tipologia_ID, Int32? titolario_ID, String classeTitolario, Int32? includiSottoalberoClasseTit, Int32? annoCreazioneDa, Int32? annoCreazioneA, Int32? annoProtocollazioneDa, Int32? annoProtocollazioneA, Int32? annoChiusuraDa, Int32? annoChiusuraA)
        {
            System_ID = system_ID;
            Description = description;
            Enabled = enabled;
            Transfer_ID = transfer_ID;
            TransferPolicyType_ID = transferPolicyType_ID;
            TransferPolicyState_ID = transferPolicyState_ID;
            Registro_ID = registro_ID;
            UO_ID = uO_ID;
            IncludiSottoalberoUO = includiSottoalberoUO;
            Tipologia_ID = tipologia_ID;
            Titolario_ID = titolario_ID;
            ClasseTitolario = classeTitolario;
            IncludiSottoalberoClasseTit = includiSottoalberoClasseTit;
            AnnoCreazioneDa = annoCreazioneDa;
            AnnoCreazioneA = annoCreazioneA;
            AnnoProtocollazioneDa = annoProtocollazioneDa;
            AnnoProtocollazioneA = annoProtocollazioneA;
            AnnoChiusuraDa = annoChiusuraDa;
            AnnoChiusuraA = annoChiusuraA;
        }

        #endregion

    }
}
