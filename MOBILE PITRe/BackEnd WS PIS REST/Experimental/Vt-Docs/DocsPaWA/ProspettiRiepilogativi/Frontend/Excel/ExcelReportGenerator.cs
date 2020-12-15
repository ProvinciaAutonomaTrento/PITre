using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ReflectionExcelReportEngine
{
    /// <summary>
    /// Writes data to an XML file that is rendered as an Excel Workbook. The data is taken from a 
    /// custom object. The class uses a Dictionary with the list of properties that should be read
    /// from the object and Reflection is used to get the specified properties.
    /// </summary>
    /// <remarks>This class inherits from the XmlExcelHelper class.</remarks>
    public class ExcelReportGenerator : XmlExcelHelper
    {
        #region - Properties -
        /// <summary>
        /// Gets or sets a string value with a format for DateTime properties.
        /// </summary>
        /// <remarks>The format is used in the ToString method of received DateTime properties. 
        /// Default value is "d".</remarks>
        public string CustomDateFormat { get; set; }
        #endregion

        #region - Constructors -
        /// <summary>
        /// Creates a new instance of the DumperFromDtoToExcel class.
        /// </summary>
        /// /// <remarks>The FileName property is set to the default value "c:\GeneratedExcel.xml"</remarks>
        public ExcelReportGenerator() : this(DEFAULT_FILE_NAME, DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE)
        {
        }

        /// <summary>
        /// Creates a new instance of the DumperFromDtoToExcel class.
        /// </summary>
        /// <param name="fileName">A string that sets the name in which the file will be saved.</param>
        public ExcelReportGenerator(string fileName) : this(fileName, DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE)
        {   
        }

        /// <summary>
        /// Creates a new instance of the DumperFromDtoToExcel class.
        /// </summary>
        /// <param name="fileName">A string that sets the name in which the file will be saved.</param>
        /// <param name="fontName">A string that sets the Font Name that will be used as default for the Excel document.</param>
        /// <param name="fontSize">An integer that sets the Font Size that will be used as default for the Excel document.</param>
        public ExcelReportGenerator(string fileName, string fontName, int fontSize) : base(fileName, fontName, fontSize)
        {
            CustomDateFormat = "d";
        }
        #endregion

        #region - Methods that generate the report -
        /// <summary>
        /// Creates a table in the last worksheet added to the Excel file.
        /// </summary>
        /// <param name="dataObject">A custom object that contains the data to be added to the Excel worksheet.</param>
        /// <param name="header">A Dictionary(Tkey, Tvalue) object with the list of properties that should be read
        /// from the received object.</param>
        /// <param name="headerStyle">A string with the name of the style that will be applied to the header.
        /// 
        /// The name sent on this parameter needs to have been added previously using the AddStringStyle method. If
        /// the style does not exist, the Excel file will not be rendered.
        /// </param>
        /// <remarks>The key of the Dictionary must be the name of the property (case sensitive) that is wanted to
        /// be displayed on the report. The value is the string as it is wanted to be displayed in the report. 
        /// i.e. To display the property CustomerFullName of the Customer object as "Customer Name" you would have
        /// to add the entry in a Dictionary called header like this:  header.Add("CustomerFullName", "Customer Name");
        /// 
        /// If the property that you want displayed is a property of a nested object, the key should be the full 
        /// qualified name of the property starting with the object that is being received. i.e.
        /// Consider a Company object with an Employee object property, Employee has an Address object property, and
        /// Address has a Telephone object property with the properties CountryCode, AreaCode, and TelephoneNumber.
        /// 
        /// If the received object is Company and we want to display the employees' telephones, the key in the 
        /// Dictionary should be "Employee.Address.Telephone.TelephoneNumber".
        /// </remarks>
        public void CreateReportTable(object dataObject, Dictionary<string, string> header, string headerStyle)
        {
            AddRow();
            AddHeader(header, headerStyle);

            /* If the received dataObject implements the IEnumerable interface then each of the objects in the
             collection is parsed to add the selected properties into the table. */
            if (dataObject is IEnumerable)
            {
                IList collection = (IList)dataObject;

                foreach (object currentObject in collection)
                {
                    AddPropertyValuesToRow(currentObject, header);
                }
            }
            else
            {
                AddPropertyValuesToRow(dataObject, header);
            }
        }

        /// <summary>
        /// Adds the header to the table.
        /// </summary>
        /// <param name="propertyNames">A Dictionary(Tkey, Tvalue) object with the list of properties that should be read
        /// from the received object.</param>
        /// <param name="headerStyle">A string with the name of the style that will be applied to the header.</param>
        private void AddHeader(Dictionary<string, string> propertyNames, string headerStyle)
        {
            foreach (KeyValuePair<string, string> label in propertyNames)
            {
                string headerLabel;

                headerLabel = label.Value;
                AddCell(CellType.String, headerStyle, headerLabel);
            }
        }

        /// <summary>
        /// Adds the values of the properties' values specified in the propertyNames Dictionary to a new row.
        /// </summary>
        /// <param name="dataObject">A custom object that contains the data to be added to the Excel worksheet.</param>
        /// <param name="propertyNames">A Dictionary(Tkey, Tvalue) object with the list of properties that should be read
        /// from the received object.</param>
        private void AddPropertyValuesToRow(object dataObject, Dictionary<string, string> propertyNames)
        {
            AddRow();
            foreach (KeyValuePair<string, string> label in propertyNames)
            {
                PropertyInfo singleProperty;
                object internalObject;
                bool isEmptyCellNeeded;

                GetObjectProperty(label.Key, dataObject, out singleProperty, out internalObject, out isEmptyCellNeeded);

                if (isEmptyCellNeeded)
                {
                    AddCell(CellType.String, string.Empty);
                }
                else
                {
                    AddValueToCell(singleProperty, internalObject);
                }
            }
        }

        /// <summary>
        /// Gets the property for which the value is needed to add to the report.
        /// </summary>
        /// <param name="propertyName">A string with the name of the property to be displayed. If a property of a nested object is
        /// needed, then the property name should be fully qualified. i.e. "Employee.Address.Telephone.TelephoneNumber"</param>
        /// <param name="dataObject">The object from which the properties are being read. Considering the previous example 
        /// it would be the Company object that contains the Employee property.</param>
        /// <param name="property">A PropertyInfo output parameter with the property for which the value is needed to add to the report.</param>
        /// <param name="internalObject">An object output parameter. In the case of nested properties is the next object in the 
        /// hierarchy.</param>
        /// <param name="isEmptyCellNeeded">A boolean that is set to true if the received dataObject is null so no information
        /// regarding its properties can be accessed so an empty cell can be added to the report.</param>
        /// <remarks>Considering the Company object example, the calls would be as follows:
        /// 
        /// In the first call propertyName = "Employee.Address.Telephone.TelephoneNumber", dataObject = Company. 
        /// In the second call propertyName = "Address.Telephone.TelephoneNumber", dataObject = Employee.
        /// In the third call propertyName = "Telephone.TelephoneNumber", dataObject = Address.
        /// The fourth call is the last one in which propertyName = "TelephoneNumber", dataObject = Telephone. These are the
        /// values returned all the way to the caller of the method to retrieve the value of the TelephoneNumber property.
        /// </remarks>
        private static void GetObjectProperty(string propertyName, object dataObject, out PropertyInfo property, out object internalObject, out bool isEmptyCellNeeded)
        {
            Type dataObjectType;
            isEmptyCellNeeded = false;

            /* If the received dataObject is null it may be that there are no values on the database or that the application didn't
             * set it for some reason. This should not interrupt the creation of the report so everything is set so that an empty
             * string is added to its corresponding cell.
             */
            if(dataObject == null)
            {
                property = null;
                internalObject = null;
                isEmptyCellNeeded = true;

                return;
            }

            if (propertyName.Contains("."))
            {
                dataObjectType = dataObject.GetType();
                PropertyInfo innerProperty = dataObjectType.GetProperty(propertyName.Substring(0, propertyName.IndexOf(".")));

                if(innerProperty == null)
                {
                    throw new ArgumentException("Object of type " + dataObjectType.FullName + " does not contain a definition for property " + propertyName);
                }
                internalObject = innerProperty.GetValue(dataObject, null);

                /* The first object is removed from the propertyName since this is the condition for the recursive method to stop. */
                string newLabel = propertyName.Substring(propertyName.IndexOf(".") + 1);

                /* Recursion is used to get properties in nested objects. 
                 
                 * If the property that you want displayed is a property of a nested object, the propertyName is the full 
                 * qualified name of the property starting with the dataObject that is being received. i.e.
                 * Consider a Company object with an Employee object property, Employee has an Address object property, and
                 * Address has a Telephone object property with the properties CountryCode, AreaCode, and TelephoneNumber.
                 *    
                 * If the received dataObject is Company and we want to display the employees' telephones, the propertyName in the 
                 * Dictionary should be "Employee.Address.Telephone.TelephoneNumber" so this function is being called until
                 * the TelephoneNumber property is reached and the property output parameter is set to TelephoneNumber and 
                 * the internalObject output parameter is set to Telephone so its value can be retrieved.
                 */
                GetObjectProperty(newLabel, internalObject, out property, out internalObject, out isEmptyCellNeeded);
            }
            else
            {
                //Internal object is assigned to the dataObject since it's the output parameter.
                internalObject = dataObject;
                dataObjectType = dataObject.GetType();
                property = dataObjectType.GetProperty(propertyName);

                if (property == null)
                {
                    throw new ArgumentException("Object of type " + dataObjectType.FullName + " does not contain a definition for property " + propertyName);
                }
            }
        }

        /// <summary>
        /// Adds the property value to a new cell with the apropriate formatting according to the property type.
        /// </summary>
        /// <param name="singleProperty">A PropertyInfo object with the value to add to the new cell.</param>
        /// <param name="dataObject">An object that contains the property to be added to the new cell.</param>
        private void AddValueToCell(PropertyInfo singleProperty, object dataObject)
        {
            object propertyData;
            string valueToDisplay;

            propertyData = singleProperty.GetValue(dataObject, null);

            if(propertyData == null)
            {
                if (singleProperty.PropertyType == typeof(DateTime) || singleProperty.PropertyType == typeof(DateTime?))
                {
                    valueToDisplay = string.Empty;
                }
                else if (singleProperty.PropertyType == typeof(Decimal) || singleProperty.PropertyType == typeof(decimal) ||
                    singleProperty.PropertyType == typeof(Decimal?) || singleProperty.PropertyType == typeof(decimal?)
                    || singleProperty.PropertyType == typeof(int) || singleProperty.PropertyType == typeof(Int16) || singleProperty.PropertyType == typeof(Int32) || singleProperty.PropertyType == typeof(Int64) ||
                    singleProperty.PropertyType == typeof(int?) || singleProperty.PropertyType == typeof(Int16?) || singleProperty.PropertyType == typeof(Int32?) || singleProperty.PropertyType == typeof(Int64?))
                {
                    valueToDisplay = 0.ToString();
                }
                else
                {
                    valueToDisplay = string.Empty;
                }
                AddCell(CellType.String, valueToDisplay);
            }
            else if (singleProperty.PropertyType == typeof(DateTime) || singleProperty.PropertyType == typeof(DateTime?))
            {
                valueToDisplay = Convert.ToDateTime(propertyData).ToString(CustomDateFormat);
                AddCell(CellType.String, valueToDisplay);
            }
            else if (singleProperty.PropertyType == typeof(Decimal) || singleProperty.PropertyType == typeof(decimal) ||
                singleProperty.PropertyType == typeof(Decimal?) || singleProperty.PropertyType == typeof(decimal?))
            {
                DefaultStyles decimals;
                
                valueToDisplay = propertyData.ToString();

                /* Determines how many decimal positions to use to display the decimal. If the string does not contain a dot (.) 
                 * it means the number should not display any decimal positions. */
                int numberOfDecimals = valueToDisplay.IndexOf(".") == -1 ? -1 : valueToDisplay.Substring(valueToDisplay.IndexOf(".") + 1).Length;

                if(numberOfDecimals <= 0)
                {
                    decimals = DefaultStyles.DecimalNone;
                }
                else if(numberOfDecimals == 1)
                {
                    decimals = DefaultStyles.DecimalOne;
                }
                else if(numberOfDecimals < 4)
                {
                    decimals = DefaultStyles.DecimalTwo;
                }
                else if(numberOfDecimals < 8)
                {
                    decimals = DefaultStyles.DecimalFour;
                }
                else
                {
                    decimals = DefaultStyles.DecimalEight;
                }
                AddCell(CellType.Number, decimals, valueToDisplay);
            }
            else if (singleProperty.PropertyType == typeof(int) || singleProperty.PropertyType == typeof(Int16) || singleProperty.PropertyType == typeof(Int32) || singleProperty.PropertyType == typeof(Int64) ||
                singleProperty.PropertyType == typeof(int?) || singleProperty.PropertyType == typeof(Int16?) || singleProperty.PropertyType == typeof(Int32?) || singleProperty.PropertyType == typeof(Int64?))
            {
                valueToDisplay = propertyData.ToString();
                AddCell(CellType.Number, DefaultStyles.Integer, valueToDisplay);
            }
            else
            {
                valueToDisplay = propertyData.ToString();
                AddCell(CellType.String, valueToDisplay);
            }
        }
        #endregion
    }
}
