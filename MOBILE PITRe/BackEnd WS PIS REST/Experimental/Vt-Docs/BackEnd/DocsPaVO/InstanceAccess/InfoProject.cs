using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.InstanceAccess
{
    public class InfoProject
    {
        #region private fields

        private string _idProject;
        private string _descriptionProject;
        private string _idParent;
        private string _idFascicolo;
        private string _codeProject;
        private string _codeClassification;

        #endregion


        #region public property

        public string ID_PROJECT
        {
            get
            {
                return _idProject;
            }

            set
            {
                _idProject = value;
            }
        }
        public string DESCRIPTION_PROJECT
        {
            get
            {
                return _descriptionProject;
            }

            set
            {
                _descriptionProject = value;
            }
        }
        public string ID_PARENT
        {
            get
            {
                return _idParent;
            }

            set
            {
                _idParent = value;
            }
        }

        public string ID_FASCICOLO
        {
            get
            {
                return _idFascicolo;
            }

            set
            {
                _idFascicolo = value;
            }
        }

        public string CODE_PROJECT
        {
            get
            {
                return _codeProject;
            }

            set
            {
                _codeProject = value;
            }
        }

        public string CODE_CLASSIFICATION
        {
            get
            {
                return _codeClassification;
            }

            set
            {
                _codeClassification = value;
            }
        }

        #endregion
    }
}
