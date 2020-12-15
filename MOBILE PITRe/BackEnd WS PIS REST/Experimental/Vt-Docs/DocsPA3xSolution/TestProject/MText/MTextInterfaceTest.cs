using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MText;
using MText.DomainObjects;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for MTextInterfaceTest and is intended
    ///to contain all MTextInterfaceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MTextInterfaceTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        // Istanza della classe di interfaccia verso mtext
        private MTextModelProvider mTextInterface;

        // Nome del documento
        private String documentName;

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestInitialize()]
        public void MTextInterfaceInitialize()
        {
            // Instanziazione  interfaccia
            this.mTextInterface = new MTextModelProvider("http://10.166.20.239:8080/mtextIntegrationAdapterWS/IntegrationAdapter?wsdl");
            
        }

        /// <summary>
        /// Test della funzionalità di recupero della lista dei data bindings
        /// </summary>
        public void GetModelsTest()
        {
            List<ModelInfo> docTypes = this.mTextInterface.GetModels();

            // docTypes deve contenere Comunicazione_Pin
            Assert.IsTrue(docTypes.Where(e => e.Name.Contains("Comunicazione_Pin_Test")).Count() > 0);
        }

        /// <summary>
        /// Test della funziona di salvataggio del documento
        /// </summary>
        public void CreateDocumentTest()
        {
            // Nome documento
            this.documentName = "Documento_di_test";

            // Lista dei campi profilati M/Text
            List<LabelValuePair> customObjects = new List<LabelValuePair>();

            #region Aggiunta dei campi

            customObjects.Add(new LabelValuePair()
                {
                    Label = "Data lettera",
                    Value = DateTime.Now.ToString("dd/MM/yyyy")
                });

            customObjects.Add(new LabelValuePair()
                {
                    Label = "Titolo",
                    Value = "Signor"
                });

            customObjects.Add(new LabelValuePair()
                {
                    Label = "Nome e cognome",
                    Value = "Samuele Furnari"
                });

            customObjects.Add(new LabelValuePair()
                {
                    Label = "Indirizzo",
                    Value = "Via Pluto, 190"
                });

            customObjects.Add(new LabelValuePair()
                {
                    Label = "Codice Avviamento Postale",
                    Value = "00456"
                });

            customObjects.Add(new LabelValuePair()
                {
                    Label = "Citta",
                    Value = "Roma"
                });

            customObjects.Add(new LabelValuePair()
                {
                    Label = "Provincia",
                    Value = "RM"
                });

            customObjects.Add(new LabelValuePair()
                {
                    Label = "Data richiesta",
                    Value = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy")
                });

            customObjects.Add(new LabelValuePair()
                {
                    Label = "PIN",
                    Value = Guid.NewGuid().ToString()
                });
            
            #endregion

            String response = this.mTextInterface.CreateDocument(
                Path.Combine(@"\TEST\Test", documentName),
                customObjects,
                @"\\Comunicazione_Pin_Test\Document_Templates\Comunicazione_Pin_Test.dataBinding");

            Assert.IsTrue(!String.IsNullOrEmpty(response));

        }

        /// <summary>
        /// Test della funzionalità di reperimento url del documento
        /// </summary>
        public void GetDocumentEditUrlTest()
        {
            String url = this.mTextInterface.GetDocumentEditUrl(@"\TEST\Test\Documento_di_test");

            Assert.IsTrue(!String.IsNullOrEmpty(url));

        }

        /// <summary>
        /// Test funzionalità esportazione in pdf
        /// </summary>
        public void ExportDocumentTest()
        {
            Byte[] document = this.mTextInterface.ExportDocument(@"\TEST\Test\Documento_di_test", "application/pdf");

            Assert.IsNotNull(document);
 
        }

        /// <summary>
        /// Test cancellazione documento
        /// </summary>
        public void DeleteDocumentTest()
        {
            this.mTextInterface.DeleteDocument(@"\TEST\Test\Documento_di_test");
        }
        public void SaveMTextFQNTest()
        {
            DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService();
            ws.SetMTextFullQualifiedName(new DocsPaWS.MTextDocumentInfo()
                {
                    DocumentDocNumber = "1",
                    DocumentVersionId = "1",
                    FullQualifiedName = @"\TESTODIPROVA"
                });

        }
        public void LoadMTextFQNTest()
        {
            DocsPaWS.MTextDocumentInfo docInfo = new DocsPaWS.MTextDocumentInfo()
                {
                    DocumentDocNumber = "1",
                    DocumentVersionId = "1",
                    FullQualifiedName = String.Empty
                };


            DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService();
            DocsPaWS.MTextDocumentInfo doc = ws.GetMTextFullQualifiedName(docInfo);

            Assert.IsTrue(!String.IsNullOrEmpty(doc.FullQualifiedName) && doc.FullQualifiedName == @"\TESTODIPROVA");

        }

    }
}
