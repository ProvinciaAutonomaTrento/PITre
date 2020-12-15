using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Services.Protocols;
using System.Threading;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for SecurityWSTest and is intended
    ///to contain all SecurityWSTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SecurityWSTest
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

        // User id da utilizzare per il test
        //private String userId = "PR28911";
        private String userId = "YTURELLA";

        /// <summary>
        ///A test for SecurityWS Constructor
        ///</summary>
        [TestMethod()]
        public void SecurityWSConstructorTest()
        {
            SecurityWS target = new SecurityWS();
            
        }

        #region Test impossibilità generazione token per utenti non censiti o che già possiedono un token valido

        /// <summary>
        /// Generazione di un token one time e sua successiva rigenerazione.
        /// Attesa eccezione in quanto non è possibile richiedere un token per un utente
        /// se questi ha già un token valido.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SoapException))]
        public void GenerateOneTimeTokenAndRigenerate()
        {
            SecurityWS target = new SecurityWS();
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            Exception ex = null;

            // Generazione del token
            token = target.GenerateOneTimeToken(userId, keyLength, initializationVectorLength);

            // Salvataggio del token nel context
            this.TestContext.Properties["token"] = token;

            // Richiesta di generazione di un ulteriore token per lo stesso utente
            try
            {
                token = target.GenerateOneTimeToken(userId, keyLength, initializationVectorLength);
            }
            catch (Exception e)
            {
                ex = e;
            }

            //this.RemoveTokenTest();

            if (ex != null)
                throw ex;

        }

        /// <summary>
        /// Generazione di un token per un utente non censito.
        /// Attesa eccezione perché non è possibile generare token per utenti non censiti
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SoapException))]
        public void GenerateOneTimeTokenForUnknowUser()
        {
            SecurityWS target = new SecurityWS();
            String userId = "UNKNOW";
            KeyLengthEnum keyLength = KeyLengthEnum.ThirtyTwo;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;

            // Generazione del token
            token = target.GenerateOneTimeToken(userId, keyLength, initializationVectorLength);

        }

        #endregion

        #region Test su generazione token one time

        /// <summary>
        /// Generazione di un token di tipo one time e suo ripristino
        /// </summary>
        [TestMethod()]
        public void GenerateOneTimeTokenAndRestoreTokenTest()
        {
            SecurityWS target = new SecurityWS();
            target.Timeout = System.Threading.Timeout.Infinite;
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            String restoredToken = String.Empty;

            // Generazione del token
            token = target.GenerateOneTimeToken(userId, keyLength, initializationVectorLength);

            // Richiesta di ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            Assert.IsFalse(String.IsNullOrEmpty(restoredToken));

        }

        /// <summary>
        /// Generazione di un token one time, ripristino e riripristino.
        /// Attesa eccezione perché un token one time può essere ripristinato
        /// più di una volta
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SoapException))]
        public void GenerateOneTimeTokenRestoreTokenAndRerestorTokenTest()
        {
            SecurityWS target = new SecurityWS();
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            String restoredToken = String.Empty;

            // Generazione del token
            token = target.GenerateOneTimeToken(userId, keyLength, initializationVectorLength);

            // Richiesta di ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            // Ririprino del token
            restoredToken = target.RestoreToken(userId, token);

        }

        #endregion

        #region Test su generazione token temporale

        /// <summary>
        /// Generazione di un token di tipo temporal e suo ripristino
        /// </summary>
        [TestMethod()]
        public void GenerateTemporalTokenAndRestoreTokenTest()
        {
            SecurityWS target = new SecurityWS();
            double milliseconds = 3600;
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            String restoredToken = String.Empty;

            // Generazione del token
            token = target.GenerateTemporalToken(userId, milliseconds, keyLength, initializationVectorLength);

            // Riposo di un secondo
            Thread.Sleep(1000);

            // Salvataggio del token nel context
            this.TestContext.Properties["token"] = token;

            // Richiesta di ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            Assert.IsFalse(String.IsNullOrEmpty(restoredToken));

            //this.RemoveTokenTest();
 
        }

        /// <summary>
        /// Generazione di un token temporal, ripristino e riripristino richiesto all'interno del periodo
        /// di validità del token.
        /// </summary>
        [TestMethod()]
        public void GenerateTemporalTokenRestoreTokenAndRerestorTokenTest()
        {
            SecurityWS target = new SecurityWS();
            double milliseconds = 8000;
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            String restoredToken = String.Empty;

            // Generazione del token
            token = target.GenerateTemporalToken(userId, milliseconds, keyLength, initializationVectorLength);

            // Salvataggio del token nel contesto
            this.TestContext.Properties["token"] = token;

            // Sleep per 1000 millisecondi
            Thread.Sleep(1000);

            // Ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            // Sleep per 3200 millisecondi
            Thread.Sleep(3200);

            // Richiesta di ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            Assert.IsFalse(String.IsNullOrEmpty(restoredToken));
        }

        /// <summary>
        /// Generazione di un token temporal, ripristino e riripristino richiesto fuori del periodo
        /// di validità del token.
        /// Attesa eccezione in quanto il token è scaduto
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SoapException))]
        public void GenerateTemporalTokenRestoreTokenAndRerestorToken2Test()
        {
            SecurityWS target = new SecurityWS();
            double milliseconds = 3600;
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            String restoredToken = String.Empty;

            // Generazione del token
            token = target.GenerateTemporalToken(userId, milliseconds, keyLength, initializationVectorLength);

            // Sleep per 100 millisecondi
            Thread.Sleep(100);

            // Ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            // Sleep per 3600 millisecondi
            Thread.Sleep(3600);

            // Richiesta di ripristino del token
            restoredToken = target.RestoreToken(userId, token);

        }

        #endregion

        #region Test su generazione token a distruzione esplicita

        /// <summary>
        /// Generazione di un token di tipo esplicito e suo ripristino
        /// </summary>
        [TestMethod()]
        public void GenerateExplicitTokenAndRestoreTokenTest()
        {
            SecurityWS target = new SecurityWS();
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            String restoredToken = String.Empty;

            // Generazione del token
            token = target.GenerateExplicitToken(userId, keyLength, initializationVectorLength);

            // Richiesta di ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            // Salvataggio del token nel context
            this.TestContext.Properties["token"] = token;

            Assert.IsFalse(String.IsNullOrEmpty(restoredToken));

            //this.RemoveTokenTest();
        }

        /// <summary>
        /// Generazione di un token esplicit, ripristino e riripristino
        /// </summary>
        [TestMethod()]
        public void GenerateExplicitTokenRestoreTokenAndRerestorTokenTest()
        {
            SecurityWS target = new SecurityWS();
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            String restoredToken = String.Empty;

            // Generazione del token
            token = target.GenerateExplicitToken(userId, keyLength, initializationVectorLength);

            // Sleep per 100 millisecondi
            Thread.Sleep(100);

            // Ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            // Sleep per 5000 millisecondi
            Thread.Sleep(5000);

            // Richiesta di ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            // Salvataggio del token nel context
            this.TestContext.Properties["token"] = token;

            Assert.IsFalse(String.IsNullOrEmpty(restoredToken));

            //this.RemoveTokenTest();
        }

        /// <summary>
        /// Generazione di un token explicit, ripristino invalidazione e riripristino del token.
        /// Attesa eccezione in quanto il token è stato rimosso.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SoapException))]
        public void GenerateExplicitTokenRestoreTokenAndRerestorToken2Test()
        {
            SecurityWS target = new SecurityWS();
            KeyLengthEnum keyLength = KeyLengthEnum.Sixteen;
            KeyLengthEnum initializationVectorLength = KeyLengthEnum.ThirtyTwo;
            String token = String.Empty;
            String restoredToken = String.Empty;

            // Generazione del token
            token = target.GenerateExplicitToken(userId, keyLength, initializationVectorLength);

            // Sleep per 100 millisecondi
            Thread.Sleep(100);

            // Ripristino del token
            restoredToken = target.RestoreToken(userId, token);

            // Invalidazione del token
            target.RemoveToken(userId, token);

            // Richiesta di ripristino del token
            restoredToken = target.RestoreToken(userId, token);

        }

        #endregion

        #region Test su rimozione e restore di token

        /// <summary>
        /// Test di rimozione di un token non esistente.
        /// Attesa eccezione
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SoapException))]
        public void RemoveUnknowTokenTest()
        {
            SecurityWS target = new SecurityWS();
            String token = "NotValidToken";

            target.RemoveToken(userId, token);

        }

        /// <summary>
        /// Test su ripristino di un token non esistente
        /// Attesa eccezione perché il token non esiste
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SoapException))]
        public void RestoreTokenTest()
        {
            SecurityWS target = new SecurityWS();
            String token = "UNKNOW";

            target.RestoreToken(userId, token);

        }

        #endregion

        #region Test su verifica che una stringa sia un token

        /// <summary>
        /// Test per verificare che una stringa sia un token
        /// </summary>
        [TestMethod()]
        public void StringIsAuthTokenTest()
        {
            SecurityWS target = new SecurityWS();
            String token = "SSO=thisIsAToken";
            Boolean result = false;

            result = target.IsAuthToken(token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test per verificare che una stringa non è un token
        /// </summary>
        [TestMethod()]
        public void StringIsNotATokenTest()
        {
            SecurityWS target = new SecurityWS();
            String token = "test://thisIsNotAToken";
            Boolean result = false;

            result = target.IsAuthToken(token);

            Assert.IsFalse(result);
        }

        #endregion

        #region Cleanup del test

        /// <summary>
        /// Test di rimozione di un token esistente.
        /// </summary>
        [TestCleanup()]
        public void ContextCleanUp()
        {
            // Se non c'è la proprietà "token" non bisogna fare il cleanup
            if (!this.TestContext.Properties.Contains("token"))
                return;

            SecurityWS target = new SecurityWS();
            target.Timeout = System.Threading.Timeout.Infinite;

            String token = this.TestContext.Properties["token"].ToString();

            target.RemoveToken(userId, token);

        }

        #endregion

    }
}
