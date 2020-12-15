using System;
using System.Linq;
using System.Xml.Linq;

namespace ReflectionExcelReportEngine
{
    /// <summary>
    /// Creates an Xml document with the format that generates an Excel document.
    /// </summary>
    public class XmlExcelHelper
    {
        #region - Protected Constants -
        protected const string DEFAULT_FILE_NAME = @"c:\GeneratedExcel.xml";
        protected const string DEFAULT_FONT_NAME = "Arial";
        protected const int DEFAULT_FONT_SIZE = 10;
        protected const string XML_DECLARATION_STRING = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>";
        #endregion

        #region - Enumerations -
        public enum CellType
        {
            String,
            Number
        }

        /// <summary>
        /// Styles included by default for rows and cells.
        /// </summary>
        /// <remarks>
        /// BoldColumn sets the text as Bold using the default Font and Font Size.
        /// StringLiteral sets the text as the default Font and Font Size.
        /// DateLiteral sets the date format to mm/dd/yyyy
        /// </remarks>
        public enum DefaultStyles
        {
            BoldColumn,
            StringLiteral,
            DecimalNone,
            DecimalOne,
            DecimalTwo,
            DecimalFour,
            DecimalEight,
            Integer,
            DateLiteral
        }
        #endregion

        #region - Private Attributes -
        private readonly XDocument _XExport;
        private XElement _Workbook;
        private readonly XNamespace _MainNamespace = XNamespace.Get("urn:schemas-microsoft-com:office:spreadsheet");
        private readonly XNamespace _o = XNamespace.Get("urn:schemas-microsoft-com:office:office");
        private readonly XNamespace _x = XNamespace.Get("urn:schemas-microsoft-com:office:excel");
        private readonly XNamespace _ss = XNamespace.Get("urn:schemas-microsoft-com:office:spreadsheet");
        private readonly XNamespace _html = XNamespace.Get("http://www.w3.org/TR/REC-html40");
        private string _ExcelFileXml;
        #endregion

        #region - Properties -
        /// <summary>
        /// Gets or sets the name in which the file will be saved.
        /// </summary>
        /// <remarks>If the path is relative the file will be saved where the application is running.</remarks>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the Font Name that is used by default on the Excel document.
        /// </summary>
        public string DefaultFontName { get; private set; }

        /// <summary>
        /// Gets the Font Size that is used by default on the Excel document.
        /// </summary>
        public int DefaultFontSize { get; private set; }

        /// <summary>
        /// Gets the generated xml string to create an Excel document.
        /// </summary>
        public string ExcelFileXml
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                _ExcelFileXml = _XExport.ToString();

                //During testing, even though during the creation of the _XExport document the Declaration element
                //was added, it seems that the ToString method of the XElement object removes it so this code was 
                //added to ensure that the declaration element always exists in the returned xml string.
                //If the declaration is missing the Excel document will not be properly rendered.
                if (!_ExcelFileXml.Contains(XML_DECLARATION_STRING))
                {
                    sb.Append(XML_DECLARATION_STRING);
                    sb.Append(_ExcelFileXml);
                    _ExcelFileXml = sb.ToString();
                }

                return _ExcelFileXml;
            }
            private set
            {
                _ExcelFileXml = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file's author. This value will be included in the Excel file's properties.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the name of the file's last author. This value will be included in the Excel file's properties.
        /// </summary>
        public string LastAuthor { get; set; }
        #endregion

        #region - Constructors -
        /// <summary>
        /// Creates a new instance of the XmlExcelHelper class.
        /// </summary>
        /// <remarks>The FileName property is set to the default value "c:\GeneratedExcel.xml"</remarks>
        public XmlExcelHelper()
            : this(DEFAULT_FILE_NAME, DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE)
        {
        }

        /// <summary>
        /// Creates a new instance of the XmlExcelHelper class.
        /// </summary>
        /// <param name="fileName">A string that sets the name in which the file will be saved.</param>
        public XmlExcelHelper(string fileName)
            : this(fileName, DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE)
        {
        }

        /// <summary>
        /// Creates a new instance of the XmlExcelHelper class.
        /// </summary>
        /// <param name="fileName">A string that sets the name in which the file will be saved.</param>
        /// <param name="fontName">A string that sets the Font Name that will be used as default for the Excel document.</param>
        /// <param name="fontSize">An integer that sets the Font Size that will be used as default for the Excel document.</param>
        public XmlExcelHelper(string fileName, string fontName, int fontSize)
        {
            FileName = fileName;
            DefaultFontName = fontName;
            DefaultFontSize = fontSize;

            _XExport = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), null);
            _XExport.Add(new XProcessingInstruction("mso-application", "progid=\"Excel.Sheet\""));
            CreateExcelHeader();
            _XExport.Add(_Workbook);
            ExcelFileXml = _XExport.ToString(); //Xml for an empty excel document.
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Saves the document to disk using the value of the FileName property.
        /// </summary>
        public void SaveDocument()
        {
            _XExport.Save(FileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void SaveDocument(System.IO.Stream stream)
        {
            System.IO.TextWriter w = new System.IO.StreamWriter(stream);
            _XExport.Save(w);
        }

        /// <summary>
        /// Adds a new worksheet to the Excel document.
        /// </summary>
        /// <param name="sheetName">A string with the name for the new worksheet.</param>
        public void CreateSheet(string sheetName)
        {
            var worksheet = (new XElement(_MainNamespace + "Worksheet",
                       new XAttribute(_ss + "Name", sheetName),
                       new XElement(_MainNamespace + "Table")));

            _Workbook.Add(worksheet);

            ExcelFileXml = _XExport.ToString();
        }

        /// <summary>
        /// Adds a new row to the current worksheet with the Default style.
        /// </summary>
        public void AddRow()
        {
            AddRow("Default");
        }

        /// <summary>
        /// Adds a new row to the current worksheet with the Default style.
        /// </summary>
        /// <param name="style">A DefaultStyle value that sets the style of the new row.</param>
        public void AddRow(DefaultStyles style)
        {
            AddRow(style.ToString());
        }

        /// <summary>
        /// Adds a new row to the current worksheet with the received style.
        /// </summary>
        /// <param name="style">A string with a style that will be applied to the new row.</param>
        /// <remarks>The style string must exist in the styles collection of the xml document. Custom styles
        /// can be added by using the AddStringStyle method. A style only needs to be added once. When a style 
        /// has been added it can be used repeatedly for both rows and cells.
        /// If the received style does not exist Excel will not be able to open the document. 
        /// See the AddStringStyle method for details.</remarks>
        public void AddRow(string style)
        {
            var table = _Workbook.Elements().Where(w => w.Name == _MainNamespace + "Worksheet").LastOrDefault()
                .Elements().Where(e => e.Name == _MainNamespace + "Table").LastOrDefault();

            var row = new XElement(_MainNamespace + "Row",
                                     new XAttribute(_ss + "StyleID", style));

            table.Add(row);

            ExcelFileXml = _XExport.ToString();
        }

        /// <summary>
        /// Adds a cell to the current row in the current worksheet.
        /// </summary>
        /// <param name="type">A CellType enumeration. The values can be String and Number.</param>
        /// <param name="value">A string with the contents for the cell.</param>
        public void AddCell(CellType type, string value)
        {
            AddCell(type, string.Empty, value, 0);
        }

        /// <summary>
        /// Adds a cell to the current row in the current worksheet.
        /// </summary>
        /// <param name="type">A CellType enumeration. The values can be String and Number.</param>
        /// <param name="value">A string with the contents for the cell.</param>
        /// <param name="mergeAcrossCells"></param>
        public void AddCell(CellType type, string value, int mergeAcrossCells)
        {
            AddCell(type, string.Empty, value, mergeAcrossCells);
        }

        /// <summary>
        /// Adds a cell to the current row in the current worksheet.
        /// </summary>
        /// <param name="type">A CellType enumeration. The values can be String and Number.</param>
        /// <param name="style">A DefaultStyle value that sets the style of the new row.</param>
        /// <param name="value">A string with the contents for the cell.</param>
        public void AddCell(CellType type, DefaultStyles style, string value)
        {
            AddCell(type, style.ToString(), value, 0);
        }

        /// <summary>
        /// Adds a cell to the current row in the current worksheet.
        /// </summary>
        /// <param name="type">A CellType enumeration. The values can be String and Number.</param>
        /// <param name="style">A DefaultStyle value that sets the style of the new row.</param>
        /// <param name="value">A string with the contents for the cell.</param>
        /// <param name="mergeAcrossCells"></param>
        public void AddCell(CellType type, DefaultStyles style, string value, int mergeAcrossCells)
        {
            AddCell(type, style.ToString(), value, mergeAcrossCells);
        }

        /// <summary>
        /// Adds a cell to the current row in the current worksheet.
        /// </summary>
        /// <param name="type">A CellType enumeration. The values can be String and Number.</param>
        /// <param name="style">A DefaultStyle value that sets the style of the new row.</param>
        /// <param name="value">A string with the contents for the cell.</param>
        public void AddCell(CellType type, string style, string value)
        {
            AddCell(type, style, value, 0);
        }

        /// <summary>
        /// Adds a cell to the current row in the current worksheet.
        /// </summary>
        /// <param name="type">A CellType enumeration. The values can be String and Number.</param>
        /// <param name="style">A string with a style that will be applied to the new cell.</param>
        /// <param name="value">A string with the contents for the cell.</param>
        /// <param name="mergeAcrossCells"></param>
        /// <remarks>The style string must exist in the styles collection of the xml document. Custom styles
        /// can be added by using the AddStringStyle method. A style only needs to be added once. When a style 
        /// has been added it can be used repeatedly for both rows and cells.
        /// If the received style does not exist Excel will not be able to open the document. 
        /// See the AddStringStyle method for details.</remarks>
        public void AddCell(CellType type, string style, string value, int mergeAcrossCells)
        {
            var row = _Workbook.Elements().Where(w => w.Name == _MainNamespace + "Worksheet").LastOrDefault()
                .Elements().Where(t => t.Name == _MainNamespace + "Table").LastOrDefault()
                .Elements().Where(r => r.Name == _MainNamespace + "Row").LastOrDefault();

            XElement cell;

            cell = new XElement(_MainNamespace + "Cell",
                                        new XElement(_MainNamespace + "Data",
                                                     new XAttribute(_ss + "Type", type.ToString()),
                                                     value)
                        );

            if (mergeAcrossCells != 0)
            {
                XAttribute mergeAttribute = new XAttribute(_ss + "MergeAcross", mergeAcrossCells);
                cell.Add(mergeAttribute);
            }

            if (!string.IsNullOrEmpty(style))
            {
                XAttribute styleAttribute = new XAttribute(_ss + "StyleID", style);
                cell.Add(styleAttribute);
            }

            row.Add(cell);

            ExcelFileXml = _XExport.ToString();
        }

        /// <summary>
        /// Creates a new style for rows or cells with text content.
        /// </summary>
        /// <param name="styleId">A string with a unique id from which the style will be referenced in order for it to be applied.</param>
        /// <param name="fontName">A string with the name of the font to be used in the style.</param>
        /// <param name="size">An integer with the size for the font to be used in the style.</param>
        /// <param name="color">A string with the hexadecimal value that represents the color in which the text will be displayed.
        /// The value must be preceeded by the pound symbol i.e. "#000000" for black.</param>
        /// <param name="bold">A boolean to indicate if the text should be bold.</param>
        /// <remarks>Once this method is executed, a new "style" element will be added to the "Styles" node in the xml
        /// document and will be identified by the styleId parameter. 
        /// The styleId is the string that must be used in the AddRow and AddCell "style" parameter</remarks>
        public void AddStringStyle(string styleId, string fontName, int size, string color, bool bold)
        {
            AddStringStyle(styleId, fontName, "Swiss", size, color, string.Empty, bold);
        }

        /// <summary>
        /// Creates a new style for rows or cells with text content.
        /// </summary>
        /// <param name="styleId">A string with a unique id from which the style will be referenced in order for it to be applied.</param>
        /// <param name="fontName">A string with the name of the font to be used in the style.</param>
        /// <param name="size">An integer with the size for the font to be used in the style.</param>
        /// <param name="color">A string with the hexadecimal value that represents the color in which the text will be displayed.
        /// The value must be preceeded by the pound symbol i.e. "#000000" for black.</param>
        /// <param name="backgroundColor">A string with the hexadecimal value that represents the color in which the cell will be displayed.
        /// The value must be preceeded by the pound symbol i.e. "#000000" for black.</param>
        /// <param name="bold">A boolean to indicate if the text should be bold.</param>
        /// <remarks>Once this method is executed, a new "style" element will be added to the "Styles" node in the xml
        /// document and will be identified by the styleId parameter. 
        /// The styleId is the string that must be used in the AddRow and AddCell "style" parameter</remarks>
        public void AddStringStyle(string styleId, string fontName, int size, string color, string backgroundColor, bool bold)
        {
            AddStringStyle(styleId, fontName, "Swiss", size, color, backgroundColor, bold);
        }

        //PERSONALIZZATO
        public void AddStringStyle(string styleId, string fontName, string fontFamily, int size, string color, string backgroundColor, bool bold, bool bottom, bool left, bool right, bool top, int weight, string align)
        {
            AddStringStyle(styleId, fontName, fontFamily, size, color, backgroundColor, bold, bottom, left, right, top, weight, align);
        }

        /// <summary>
        /// Creates a new style for rows or cells with text content.
        /// </summary>
        /// <param name="styleId">A string with a unique id from which the style will be referenced in order for it to be applied.</param>
        /// <param name="fontName">A string with the name of the font to be used in the style.</param>
        /// <param name="fontFamily">A string with the name of the font family to be used in the style.</param>
        /// <param name="size">An integer with the size for the font to be used in the style.</param>
        /// <param name="color">A string with the hexadecimal value that represents the color in which the text will be displayed.
        /// The value must be preceeded by the pound symbol i.e. "#000000" for black.</param>
        /// <param name="backgroundColor">A string with the hexadecimal value that represents the color in which the cell will be displayed.
        /// The value must be preceeded by the pound symbol i.e. "#000000" for black.</param>
        /// <param name="bold">A boolean to indicate if the text should be bold.</param>
        /// <remarks>Once this method is executed, a new "style" element will be added to the "Styles" node in the xml
        /// document and will be identified by the styleId parameter. 
        /// The styleId is the string that must be used in the AddRow and AddCell "style" parameter</remarks>
        public void AddStringStyle(string styleId, string fontName, string fontFamily, int size, string color, string backgroundColor, bool bold)
        {
            var styles = _Workbook.Elements().Where(s => s.Name == _MainNamespace + "Styles").FirstOrDefault();
            XElement interior;

            if (!string.IsNullOrEmpty(backgroundColor))
            {
                interior = new XElement(_MainNamespace + "Interior",
                    new XAttribute(_ss + "Color", backgroundColor),
                    new XAttribute(_ss + "Pattern", "Solid"));
            }
            else
            {
                interior = new XElement(_MainNamespace + "Interior");
            }

            var style = new XElement(_MainNamespace + "Style",
                                     new XAttribute(_ss + "ID", styleId),
                                     new XElement(_MainNamespace + "Font",
                                                  new XAttribute(_ss + "FontName", fontName),
                                                  new XAttribute(_x + "Family", fontFamily),
                                                  new XAttribute(_ss + "Size", size.ToString()),
                                                  new XAttribute(_ss + "Color", color),
                                                  new XAttribute(_ss + "Bold", Convert.ToInt16(bold).ToString())
                                         ),
                                     interior
                );

            styles.Add(style);

            ExcelFileXml = _XExport.ToString();
        }

        //PERONALIZZATO

        public void AddStringStyleBorder(string styleId, string fontName, int size, string color, string backgroundColor, bool bold, bool border_bottom)
        {
            var styles = _Workbook.Elements().Where(s => s.Name == _MainNamespace + "Styles").FirstOrDefault();
            XElement interior;
            XElement border;

            if (!string.IsNullOrEmpty(backgroundColor))
            {
                interior = new XElement(_MainNamespace + "Interior",
                    new XAttribute(_ss + "Color", backgroundColor),
                    new XAttribute(_ss + "Pattern", "Solid"));
            }
            else
            {
                interior = new XElement(_MainNamespace + "Interior");
            }


            if (border_bottom)
            {
                border = new XElement(_MainNamespace + "Borders",
                            new XElement(_MainNamespace + "Border",
                                    new XAttribute(_ss + "Position", "Bottom"),
                                    new XAttribute(_ss + "LineStyle", "Continuous"),
                                    new XAttribute(_ss + "Weight", "1")
                                    )
               );
            }
            else
            {
                border = new XElement(_MainNamespace + "Borders");
            }

            var style = new XElement(_MainNamespace + "Style",
                                     new XAttribute(_ss + "ID", styleId),
                                     new XElement(_MainNamespace + "Font",
                                                  new XAttribute(_ss + "FontName", fontName),
                                                  new XAttribute(_ss + "Size", size.ToString()),
                                                  new XAttribute(_ss + "Color", color),
                                                  new XAttribute(_ss + "Bold", Convert.ToInt16(bold).ToString())
  
                                         ),
                                     interior,
                                     border
                );

            styles.Add(style);

            ExcelFileXml = _XExport.ToString();
        }

        public void AddStringStyleBorderRight(string styleId, string fontName, int size, string color, string backgroundColor, bool bold, bool border_bottom)
        {
            var styles = _Workbook.Elements().Where(s => s.Name == _MainNamespace + "Styles").FirstOrDefault();
            XElement interior;
            XElement border;

            if (!string.IsNullOrEmpty(backgroundColor))
            {
                interior = new XElement(_MainNamespace + "Interior",
                    new XAttribute(_ss + "Color", backgroundColor),
                    new XAttribute(_ss + "Pattern", "Solid"));
            }
            else
            {
                interior = new XElement(_MainNamespace + "Interior");
            }


            if (border_bottom)
            {
                border = new XElement(_MainNamespace + "Borders",
                            new XElement(_MainNamespace + "Border",
                                    new XAttribute(_ss + "Position", "Right"),
                                    new XAttribute(_ss + "LineStyle", "Continuous"),
                                    new XAttribute(_ss + "Weight", "1")
                                    )
               );
            }
            else
            {
                border = new XElement(_MainNamespace + "Borders");
            }

            var style = new XElement(_MainNamespace + "Style",
                                     new XAttribute(_ss + "ID", styleId),
                                     new XElement(_MainNamespace + "Font",
                                                  new XAttribute(_ss + "FontName", fontName),
                                                  new XAttribute(_ss + "Size", size.ToString()),
                                                  new XAttribute(_ss + "Color", color),
                                                  new XAttribute(_ss + "Bold", Convert.ToInt16(bold).ToString())

                                         ),
                                     interior,
                                     border
                );

            styles.Add(style);

            ExcelFileXml = _XExport.ToString();
        }

        public void AddStringStyleBorderItalic(string styleId, string fontName, int size, string color, string backgroundColor, bool bold, bool border_bottom)
        {
            var styles = _Workbook.Elements().Where(s => s.Name == _MainNamespace + "Styles").FirstOrDefault();
            XElement interior;
            XElement border;

            if (!string.IsNullOrEmpty(backgroundColor))
            {
                interior = new XElement(_MainNamespace + "Interior",
                    new XAttribute(_ss + "Color", backgroundColor),
                    new XAttribute(_ss + "Pattern", "Solid"));
            }
            else
            {
                interior = new XElement(_MainNamespace + "Interior");
            }


            if (border_bottom)
            {
                border = new XElement(_MainNamespace + "Borders",
                            new XElement(_MainNamespace + "Border",
                                    new XAttribute(_ss + "Position", "Bottom"),
                                    new XAttribute(_ss + "LineStyle", "Continuous"),
                                    new XAttribute(_ss + "Weight", "1")
                                    )
               );
            }
            else
            {
                border = new XElement(_MainNamespace + "Borders");
            }

            var style = new XElement(_MainNamespace + "Style",
                                     new XAttribute(_ss + "ID", styleId),
                                     new XElement(_MainNamespace + "Font",
                                                  new XAttribute(_ss + "FontName", fontName),
                                                  new XAttribute(_ss + "Size", size.ToString()),
                                                  new XAttribute(_ss + "Color", color),
                                                  new XAttribute(_ss + "Bold", Convert.ToInt16(bold).ToString()),
                                                  new XAttribute(_ss + "Italic", "1")

                                         ),
                                     interior,
                                     border
                );

            styles.Add(style);

            ExcelFileXml = _XExport.ToString();
        }

        #endregion

        #region - Excel File Header Methods -
        /// <summary>
        /// Generates the Xml header with the necessary namespaces and schema for an Excel document.
        /// </summary>
        /// <remarks>Within the Xml header created in this method a Workbook element is created.
        /// All subsequent elements will be created as children of the Workbook one.</remarks>
        private void CreateExcelHeader()
        {
            _Workbook = new XElement(_MainNamespace + "Workbook",
                    new XAttribute(XNamespace.Xmlns + "html", _html),
                    CreateNamespaceAttribute(XName.Get("ss", "http://www.w3.org/2000/xmlns/"), _ss),
                    CreateNamespaceAttribute(XName.Get("o", "http://www.w3.org/2000/xmlns/"), _o),
                    CreateNamespaceAttribute(XName.Get("x", "http://www.w3.org/2000/xmlns/"), _x),
                    CreateNamespaceAttribute(_MainNamespace),
                    new XElement(_o + "DocumentProperties",
                        CreateNamespaceAttribute(_o),
                        new XElement(_o + "Author", Author),
                        new XElement(_o + "LastAuthor", LastAuthor),
                        new XElement(_o + "Created", DateTime.Now.ToString())
                    ), //end document properties)
                    new XElement(_x + "ExcelWorkbook",
                        CreateNamespaceAttribute(_x),
                        new XElement(_x + "WindowHeight", 12750),
                        new XElement(_x + "WindowWidth", 24855),
                        new XElement(_x + "WindowTopX", 240),
                        new XElement(_x + "WindowTopY", 75),
                        new XElement(_x + "ProtectStructure", "False"),
                        new XElement(_x + "ProtectWindows", "False")
                    ), //end ExcelWorkbook
                    new XElement(_MainNamespace + "Styles",
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "Default"),
                            new XAttribute(_ss + "Name", "Normal"),
                            new XElement(_MainNamespace + "Alignment",
                                new XAttribute(_ss + "Vertical", "Bottom")
                            ),
                            new XElement(_MainNamespace + "Borders"),
                            new XElement(_MainNamespace + "Font",
                                new XAttribute(_ss + "FontName", DefaultFontName),
                                new XAttribute(_x + "Family", "Swiss"),
                                new XAttribute(_ss + "Size", DefaultFontSize.ToString()),
                                new XAttribute(_ss + "Color", "#000000")
                            ),
                            new XElement(_MainNamespace + "Interior"),
                            new XElement(_MainNamespace + "NumberFormat"),
                            new XElement(_MainNamespace + "Protection")
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "BoldColumn"),
                            new XElement(_MainNamespace + "Font",
                                new XAttribute(_ss + "FontName", DefaultFontName),
                                new XAttribute(_x + "Family", "Swiss"),
                                new XAttribute(_ss + "Size", DefaultFontSize),
                                new XAttribute(_ss + "Color", "#000000"),
                                new XAttribute(_ss + "Bold", "1")
                            )
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "StringLiteral"),
                            new XElement(_MainNamespace + "NumberFormat",
                                new XAttribute(_ss + "Format", "@")
                            )
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "DecimalNone"),
                            new XElement(_MainNamespace + "NumberFormat",
                                new XAttribute(_ss + "Format", "#,##0")
                            )
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "DecimalOne"),
                            new XElement(_MainNamespace + "NumberFormat",
                                new XAttribute(_ss + "Format", "#,##0.0")
                            )
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "DecimalTwo"),
                            new XElement(_MainNamespace + "NumberFormat",
                                new XAttribute(_ss + "Format", "#,##0.00")
                            )
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "DecimalFour"),
                            new XElement(_MainNamespace + "NumberFormat",
                                new XAttribute(_ss + "Format", "#,##0.0000")
                            )
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "DecimalEight"),
                            new XElement(_MainNamespace + "NumberFormat",
                                new XAttribute(_ss + "Format", "#,##0.00000000")
                            )
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "Integer"),
                            new XElement(_MainNamespace + "NumberFormat",
                                new XAttribute(_ss + "Format", "0")
                            )
                        ),
                        new XElement(_MainNamespace + "Style",
                            new XAttribute(_ss + "ID", "DateLiteral"),
                            new XElement(_MainNamespace + "NumberFormat",
                                new XAttribute(_ss + "Format", "mm/dd/yyyy;@")
                            )
                        )
                    ) // close styles
              );
        }

        private static XAttribute CreateNamespaceAttribute(XNamespace ns)
        {
            return CreateNamespaceAttribute(XName.Get("xmlns", ""), ns);
        }

        private static XAttribute CreateNamespaceAttribute(XName name, XNamespace ns)
        {
            var ssAtt = new XAttribute(name, ns.NamespaceName);
            ssAtt.AddAnnotation(ns);
            return ssAtt;
        }
        #endregion
    }
}
