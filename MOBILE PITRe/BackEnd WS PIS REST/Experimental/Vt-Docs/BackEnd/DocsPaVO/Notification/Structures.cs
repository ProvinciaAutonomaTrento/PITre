using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Notification
{

        public struct ListDomainObject
        {
            public const string DOCUMENT = "DOCUMENTO";
            public const string FOLDER = "FASCICOLO";
            public const string JOB = "JOB";
        }

        /// <summary>
        /// Tipo notifica(informativa, operativa)
        /// </summary>
        public struct NotificationType
        {
            public const char OPERATIONAL = 'O';
            public const char INFORMATION = 'I';
        }

        public struct Multiplicity
        {
            public const string ONE = "ONE";
            public const string ALL = "ALL";
        }

        public struct TagItem
        {
            public const string LINE = "<line>";
            public const string CLOSE_LINE = "</line>";
            public const string LABEL = "<label>";
            public const string CLOSE_LABEL = "</label>";
            public const string COLORRED = "<colorRed>";
            public const string CLOSE_COLORRED = "</colorRed>";
            public const string COLORRED_STRIKE = "<colorRedStrike>";
            public const string CLOSE_COLORRED_STRIKE = "</colorRedStrike>";
        }

        public struct TypeProtocol
        {
            public const string INTERNO = "I";
            public const string ARRIVO = "A";
            public const string PARTENZA = "P";
            public const string GRIGIO = "G";
            public const string STAMPAREG = "R";
            public const string LABEL_INTERNO = "lblInterno";
            public const string LABEL_ARRIVO = "lblArrivo";
            public const string LABEL_PARTENZA = "lblPartenza";
            public const string LABEL_GRIGIO = "lblGrigio";
            public const string LABEL_STAMPAREG = "lblStampaReg";
        }

        /// <summary>
        /// Elemento modificato
        /// </summary>
        public enum TypeOperation
        { 
            RECORD_PREDISPOSED,
            CHANGE_OBJECT,
            CHANGE_TYPE_DOC,
            CHANGE_TYPE_PROTO,
            CHANGE_TYPE_PROJ,
            CHANGE_SENDER,
            ABORT_RECORD,
            ABORT_COUNTER_REPERTOIRE
        }

    /// <summary>
    /// This structure contains the data for the follow object
    /// </summary>
    public struct FollowDomainObject
    {
        public enum OperationFollow
        {
            AddDocFolder,
            RemoveDocFolder,
            AddFolder,
            RemoveFolder,
            AddDoc,
            RemoveDoc
        }

        public string IdProfile
        {
            get;
            set;
        }

        public string IdProject
        {
            get;
            set;
        }

        public string IdGroup
        {
            get;
            set;
        }

        public string IdPeople
        {
            get;
            set;
        }

        public string IdAmm
        {
            get;
            set;
        }

        public string App
        {
            get;
            set;
        }

        public OperationFollow Operation
        {
            get;
            set;
        }
    }

}
