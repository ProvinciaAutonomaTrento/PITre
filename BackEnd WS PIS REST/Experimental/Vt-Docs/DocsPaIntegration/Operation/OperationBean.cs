using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DocsPaIntegration.ObjectTypes.Attributes;

namespace DocsPaIntegration.Operation
{
    public abstract class OperationBean
    {
        public List<OperationParam> Params
        {
            //a partire dai fields e dalle properties che hanno un attributo di 
            //tipo IntegrationTypeAttribute, costruisce la lista di oggetti OperationParam
            get
            {
                List<OperationParam> res=new List<OperationParam>();
                foreach (FieldInfo field in Fields)
                {
                    IntegrationObjectTypeAttribute[] attrs = (IntegrationObjectTypeAttribute[])field.GetCustomAttributes(typeof(IntegrationObjectTypeAttribute), true);
                    if (attrs.Length == 1)
                    {
                        OperationParam temp = new OperationParam();
                        temp.Name = attrs[0].Name;
                        temp.Type = attrs[0].Type;
                        res.Add(temp);
                    }
                }
                foreach(PropertyInfo property in Properties){
                    IntegrationObjectTypeAttribute[] attrs = (IntegrationObjectTypeAttribute[])property.GetCustomAttributes(typeof(IntegrationObjectTypeAttribute), true);
                    if (attrs.Length == 1)
                    {
                        OperationParam temp = new OperationParam();
                        temp.Name = attrs[0].Name;
                        temp.Type = attrs[0].Type;
                        res.Add(temp);
                    }
                }
                return res;
            }
        }

        private FieldInfo[] Fields
        {
            get
            {
                return this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }
        }

        private PropertyInfo[] Properties
        {
            get
            {
                return this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }
        }

    }
}
