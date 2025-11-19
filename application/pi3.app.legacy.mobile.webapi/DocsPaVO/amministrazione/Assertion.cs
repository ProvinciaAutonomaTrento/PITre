// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// This class implements the statement, which was introduced with the center of notifications
    /// </summary>
    [DataContract]
    public class Assertion
    {
        #region private fiels

        private long _systemId;
        private long _idTypeEvent;
        private string _descTypeEvent;
        private long _idAur;
        private string _descAur;
        private string _typeAur;
        private char _typeNotify;
        private bool _isExercise;
        private long _idAmm;
        private string _configTypeEvent;

        #endregion

        #region Property

        /// <summary>
        /// ID of the assertion
        /// </summary>
        [DataMember]
        public long SYSTEM_ID
        {
            get 
            {
                return _systemId;
            }

            set
            {
                _systemId = value;
            }
        }

        /// <summary>
        /// Event type object identifier of the assertion
        /// </summary>
        [DataMember]
        public long ID_TYPE_EVENT
        {
            get
            {
                return _idTypeEvent;
            }

            set
            {
                _idTypeEvent = value;
            }
        }

        /// <summary>
        /// Description of the event type
        /// </summary>
        [DataMember]
        public string DESC_TYPE_EVENT
        {
            get
            {
                return _descTypeEvent;
            }

            set
            {
                _descTypeEvent = value;
            }
        }

        /// <summary>
        /// ID aggregator of assertion
        /// </summary>
        [DataMember]
        public long ID_AUR
        {
            get
            {
                return _idAur;
            }

            set
            {
                _idAur = value;
            }
        }

        /// <summary>
        /// Description of the assertion of the aggregator
        /// </summary>
        [DataMember]
        public string DESC_AUR
        {
            get
            {
                return _descAur;
            }

            set
            {
                _descAur = value;
            }
        }

        /// <summary>
        /// Type of aggregator (administration, RF, role type, role, OU)
        /// </summary>
        [DataMember]
        public string TYPE_AUR
        {
            get
            {
                return _typeAur;
            }

            set
            {
                _typeAur = value;
            }
        }

        /// <summary>
        /// Type of notification (Operational Information)
        /// </summary>
        [DataMember]
        public char TYPE_NOTIFY
        {
            get
            {
                return _typeNotify;
            }

            set
            {
                _typeNotify = value;
            }
        }

        /// <summary>
        /// Indicates whether the assertion is in operation or not.
        /// </summary>
        [DataMember]
        public bool IS_EXERCISE
        {
            get
            {
                return _isExercise;
            }

            set
            {
                _isExercise = value;
            }
        }

        /// <summary>
        /// Administration as defined in the assertion
        /// </summary>
        [DataMember]
        public long ID_AMM
        {
            get
            {
                return _idAmm;
            }

            set
            {
                _idAmm = value;
            }
        }

        /// <summary>
        /// Configuration of the event type
        /// </summary>
        [DataMember]
        public string CONFIG_TYPE_EVENT
        {
            get
            {
                return _configTypeEvent;
            }

            set
            {
                _configTypeEvent = value;
            }
        }

        #endregion
    }
}
