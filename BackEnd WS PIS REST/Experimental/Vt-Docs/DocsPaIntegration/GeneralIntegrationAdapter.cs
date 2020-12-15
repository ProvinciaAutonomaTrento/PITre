using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DocsPaIntegration.Config;
using DocsPaIntegration.Search;
using DocsPaIntegration.ObjectTypes;
using DocsPaIntegration.Attributes;
using System.Drawing;
using DocsPaIntegration.ObjectTypes.Attributes;
using log4net;
using DocsPaIntegration.Operation;

namespace DocsPaIntegration
{
    public abstract class GeneralIntegrationAdapter : IIntegrationAdapter
    {
        private static ILog logger = LogManager.GetLogger(typeof(GeneralIntegrationAdapter));
        [IntegrationStringType("Etich. codice",true)]
        protected string _idLabel;

        [IntegrationStringType("Etich. descrizione", true)]
        protected string _descriptionLabel;

        public void Init(ConfigurationInfo configurationInfo)
        {
            List<ValidationError> errors = new List<ValidationError>();
            foreach (ConfigurationParam confParam in configurationInfo.ConfigurationParams)
            {
                SetConfigurationField(confParam, errors);
            }
            if (errors.Count > 0)
            {
                throw new ConfigurationException(ConfigurationExceptionCode.VALIDATION_ERROR, null, errors);
            }
            InitParticular();
        }

        private FieldInfo[] Fields
        {
            get
            {
                return this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }

        private PropertyInfo[] Properties
        {
            get
            {
                return this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }

        private MethodInfo[] Methods
        {
            get
            {
                return this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        private IntegrationAdapterAttribute IntegrationAdapterAttribute
        {
            get
            {
                return ((IntegrationAdapterAttribute[]) this.GetType().GetCustomAttributes(typeof(IntegrationAdapterAttribute), true))[0];
            }
        }

        private void FillConfigurationInfo(MemberInfo memberInfo, ConfigurationInfo res)
        {
            IntegrationObjectTypeAttribute[] attrs = (IntegrationObjectTypeAttribute[])memberInfo.GetCustomAttributes(typeof(IntegrationObjectTypeAttribute), true);
            foreach (IntegrationObjectTypeAttribute temp in attrs)
            {
                ConfigurationParam tempParam = new ConfigurationParam();
                tempParam.Name = temp.Name;
                tempParam.Type = temp.Type;
                tempParam.Mandatory = temp.Mandatory;
                res.ConfigurationParams.Add(tempParam);
            }
        }

        public ConfigurationInfo ConfigurationInfo
        {
            get
            {
                ConfigurationInfo res = new ConfigurationInfo(IntegrationAdapterAttribute.UniqueId, IntegrationAdapterAttribute.Version);
                foreach (FieldInfo field in Fields)
                {
                    FillConfigurationInfo(field, res);
                }
                foreach (PropertyInfo property in Properties)
                {
                    FillConfigurationInfo(property, res);
                }
                return res;
            }
        }

        public IntegrationAdapterInfo AdapterInfo
        {
            get
            {
                return new IntegrationAdapterInfo(IntegrationAdapterAttribute.UniqueId,IntegrationAdapterAttribute.Name,IntegrationAdapterAttribute.Description,IntegrationAdapterAttribute.Version,GetType(),IntegrationAdapterAttribute.HasIcon);
            }
        }

        private void SetConfigurationField(ConfigurationParam confParam,List<ValidationError> errors)
        {
            foreach(FieldInfo field in Fields){
                IntegrationObjectTypeAttribute[] attrs = (IntegrationObjectTypeAttribute[])field.GetCustomAttributes(typeof(IntegrationObjectTypeAttribute), true);
                foreach (IntegrationObjectTypeAttribute temp in attrs)
                {
                    if (temp.Name.Equals(confParam.Name))
                    {
                        logger.Debug("setting "+temp.Name+"...");
                        if (temp.Mandatory && string.IsNullOrEmpty(confParam.Value))
                        {
                            logger.Debug("mandatory field " + temp.Name + " not set");
                            errors.Add(new ValidationError(confParam, ValidationErrorCode.MANDATORY_VALUE));
                            return;
                        }
                        if (string.IsNullOrEmpty(confParam.Value)) return;
                        try
                        {
                            field.SetValue(this, temp.GetValue(confParam.Value));
                        }
                        catch (ObjectValidationException ove)
                        {
                            errors.Add(new ValidationError(confParam, ValidationErrorCode.NOT_VALID_VALUE, ove.ErrorMessage));
                        }
                        catch (Exception e)
                        {
                            logger.Error("exception in setting " + temp.Name + ": "+e);
                            errors.Add(new ValidationError(confParam, ValidationErrorCode.NOT_VALID_VALUE));
                        }
                        return;
                    }
                }
            }
            foreach (PropertyInfo property in Properties)
            {
                IntegrationObjectTypeAttribute[] attrs = (IntegrationObjectTypeAttribute[])property.GetCustomAttributes(typeof(IntegrationObjectTypeAttribute), true);
                foreach (IntegrationObjectTypeAttribute temp in attrs)
                {
                    if (temp.Name.Equals(confParam.Name))
                    {
                        logger.Debug("setting " + temp.Name + "...");
                        if (temp.Mandatory && string.IsNullOrEmpty(confParam.Value))
                        {
                            logger.Debug("mandatory field " + temp.Name + " not set");
                            errors.Add(new ValidationError(confParam, ValidationErrorCode.MANDATORY_VALUE));
                            return;
                        }
                        if (string.IsNullOrEmpty(confParam.Value)) return;
                        try
                        {
                            property.SetValue(this, temp.GetValue(confParam.Value), null);
                        }
                        catch (ObjectValidationException ove)
                        {
                            errors.Add(new ValidationError(confParam, ValidationErrorCode.NOT_VALID_VALUE, ove.ErrorMessage));
                        }
                        catch (Exception e)
                        {
                            logger.Error("exception in setting " + temp.Name + ": " + e);
                            errors.Add(new ValidationError(confParam, ValidationErrorCode.NOT_VALID_VALUE));
                        }
                        return;
                    }
                }
            }
        }

        public List<OperationInfo> Operations
        {
            get
            {
                List<OperationInfo> res = new List<OperationInfo>();
                foreach (MethodInfo method in Methods)
                {
                    IntegrationAdapterOperationAttribute[] attrs = (IntegrationAdapterOperationAttribute[])method.GetCustomAttributes(typeof(IntegrationAdapterOperationAttribute), true);
                    if (attrs.Length > 0)
                    {
                        OperationInfo temp = new OperationInfo();
                        temp.Name = attrs[0].Name;
                        List<ParameterInfo> pars = method.GetParameters().Where(e => e.ParameterType.BaseType == typeof(OperationBean)).ToList();
                        //esame input
                        //ci aspettiamo che il numero di parametri di un metodo segnato come operation sia 0 o 1
                        //in caso contrario si potrebbe sollevare una eccezione
                        if(pars.Count>0)
                        {
                            ParameterInfo tempParam = pars[0];
                            temp.Input=((OperationBean)Activator.CreateInstance(tempParam.ParameterType)).Params;
                        }
                        //esame output
                        temp.Output = ((OperationBean)Activator.CreateInstance(method.ReturnType)).Params;
                        res.Add(temp);
                    }
                }
                return res;
            }
        }

        public string IdLabel
        {
            get
            {
                return _idLabel;
            }
        }

        public string DescriptionLabel
        {
            get
            {
                return _descriptionLabel;
            }
        }

        protected abstract void InitParticular();

        public abstract SearchOutputRow PuntualSearch(PuntualSearchInfo puntualSearchInfo);

        public abstract SearchOutput Search(SearchInfo searchInfo);


        public abstract Bitmap Icon
        {
            get;
        }

        public abstract bool HasIcon
        {
            get;
        }
    }
}
