// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic.ApplicationServices;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private HubConnection _connection;
        private string _userId;
        private string _currentSessionId;
        private byte[] _currentHash;
        private AppConfig _config;

        private class ConnectionEstablishedParams { 
            
        }

        public Form1(string userId = null)
        {
            InitializeComponent();

            if (notifyIcon.Icon == null)
            {
                // Usa l'icona del form o un'icona predefinita
                notifyIcon.Icon = this.Icon != null ? this.Icon : SystemIcons.Application;
            }

            // Imposta la visibilit� dell'icona in tray
            notifyIcon.Visible = true;

            // Carica la configurazione
            _config = AppConfig.Load();

            _userId = userId;

            // Inizializza l'interfaccia
            lblStatus.Text = "In attesa di connessione...";

            // Verifica se deve partire minimizzata
            if (_config.StartMinimized)
            {
                WindowState = FormWindowState.Minimized;
                Hide();
                ShowInTaskbar = false;
            }

            // Avvia la connessione SignalR
            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            try
            {
                // Create user metadata to pass to the hub
                string userMetadata = JsonSerializer.Serialize(new
                {
                    Name = _config.UserName,
                    Email = _config.UserEmail,
                    Role = _config.UserRole,
                    CustomData = _config.CustomData
                });

                // URL encode the metadata
                string encodedMetadata = Uri.EscapeDataString(userMetadata);

                // Crea la connessione SignalR con il token di autenticazione
                _connection = new HubConnectionBuilder() // https://localhost:7064
                    .WithUrl($"{_config.ServerUrl}/signatureHub?userMetadata={encodedMetadata}", options =>
                    {
                        // Usa solo WebSockets come protocollo di trasporto
                        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;

                        // Salta la fase di negoziazione
                        options.SkipNegotiation = true;
                    })
                    .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5) })
                    .Build();

                // Gestisci l'evento di connessione stabilita
                _connection.On<JsonElement>("ConnectionEstablished", data =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblStatus.Text = "Connesso al servizio di firma";
                        string connectionId = data.GetProperty("connectionId").GetString();
                        txtConnectionInfo.Text = $"Connection ID: {connectionId}";
                        notifyIcon.ShowBalloonTip(3000, "Signature Client", "Connesso al servizio di firma", ToolTipIcon.Info);

                    });
                });

                // Gestisci l'evento di documento pronto per la firma
                _connection.On<JsonElement>("HashToSign", data =>
                {
                    try
                    {
                        _currentSessionId = data.GetProperty("sessionId").GetString();
                        string documentName = data.GetProperty("documentName").GetString();
                        string base64Hash = data.GetProperty("hash").GetString();

                        _currentHash = Convert.FromBase64String(base64Hash);

                        this.Invoke((MethodInvoker)delegate
                        {
                            lblStatus.Text = $"Richiesta firma per: {documentName}";
                            btnSign.Enabled = true;
                            signMenuItem.Enabled = true;

                            // Mostra una notifica all'utente
                            notifyIcon.ShowBalloonTip(5000, "Richiesta di firma",
                                $"� richiesta la tua firma per il documento: {documentName}",
                                ToolTipIcon.Info);

                            // Se l'app � minimizzata, fai lampeggiare l'icona
                            if (WindowState == FormWindowState.Minimized)
                            {
                                notifyIcon.BalloonTipClicked += (s, e) => {
                                    ShowForm();
                                };
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Errore nella richiesta di firma: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });

                // Gestisci l'evento di documento pronto per la firma
                _connection.On<object>("DocumentReadyForSigning", data =>
                {
                    try
                    {
                        _currentSessionId = data.GetType().GetProperty("SessionId").GetValue(data).ToString();
                        string documentName = data.GetType().GetProperty("DocumentName").GetValue(data).ToString();
                        string base64Hash = data.GetType().GetProperty("Hash").GetValue(data).ToString();

                        _currentHash = Convert.FromBase64String(base64Hash);

                        this.Invoke((MethodInvoker)delegate
                        {
                            lblStatus.Text = $"Richiesta firma per: {documentName}";
                            btnSign.Enabled = true;
                            signMenuItem.Enabled = true;

                            // Mostra una notifica all'utente
                            notifyIcon.ShowBalloonTip(5000, "Richiesta di firma",
                                $"� richiesta la tua firma per il documento: {documentName}",
                                ToolTipIcon.Info);

                            // Se l'app � minimizzata, fai lampeggiare l'icona
                            if (WindowState == FormWindowState.Minimized)
                            {
                                notifyIcon.BalloonTipClicked += (s, e) => {
                                    ShowForm();
                                };
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Errore nella richiesta di firma: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });

                // Gestisci l'evento di firma completata
                _connection.On<JsonElement>("SignatureCompleted", data =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        string sessionId = data.GetProperty("sessionId").GetString();
                        int documentId = data.GetProperty("documentId").GetInt32();

                        lblStatus.Text = "Firma completata con successo";
                        //MessageBox.Show("Il documento � stato firmato correttamente", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        notifyIcon.ShowBalloonTip(3000, "Firma completata", "Il documento � stato firmato correttamente", ToolTipIcon.Info);

                        // Reset dello stato
                        _currentSessionId = null;
                        _currentHash = null;
                        btnSign.Enabled = false;
                        signMenuItem.Enabled = false;
                    });
                });

                // Gestisci l'evento di errore nella firma
                _connection.On<object>("SignatureError", data =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        string error = data.GetType().GetProperty("Error").GetValue(data).ToString();
                        lblStatus.Text = "Errore nella firma";
                        MessageBox.Show($"Errore durante la firma: {error}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        notifyIcon.ShowBalloonTip(3000, "Errore di firma", error, ToolTipIcon.Error);


                        // Reset parziale dello stato
                        btnSign.Enabled = true;
                        signMenuItem.Enabled = true;
                    });
                });

                // Avvia la connessione
                await _connection.StartAsync();
                lblStatus.Text = "Connessione al servizio di firma in corso...";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Errore di connessione";
                MessageBox.Show($"Impossibile connettersi al server: {ex.Message}", "Errore di connessione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                notifyIcon.ShowBalloonTip(3000, "Errore di connessione", "Impossibile connettersi al server", ToolTipIcon.Error);

            }
        }

        private async void SignDocument()
        {
            if (_currentHash == null || string.IsNullOrEmpty(_currentSessionId))
            {
                MessageBox.Show("Nessun documento da firmare", "Informazione", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                btnSign.Enabled = false;
                signMenuItem.Enabled = false;
                lblStatus.Text = "Firma in corso...";

                // Mostra il selettore di certificati
                X509Certificate2 certificate = SelectCertificate();
                if (certificate == null)
                {
                    lblStatus.Text = "Firma annullata";
                    btnSign.Enabled = true;
                    signMenuItem.Enabled = true;
                    return;
                }

                // Firma l'hash con il certificato selezionato
                byte[] signature = await SignHashWithCertificate(_currentHash, certificate);

                //X509Certificate2 certificate = CreateTestCertificate();
                //byte[] signature = SignExactHash(_currentHash, certificate);

                // Invia la firma al server
                await _connection.InvokeAsync<bool>("CompleteSignature",
                    _currentSessionId,
                    Convert.ToBase64String(signature));

                lblStatus.Text = "Firma inviata al server, elaborazione in corso...";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Errore durante la firma";
                MessageBox.Show($"Si � verificato un errore: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSign.Enabled = true;
                signMenuItem.Enabled = true;
            }

        }

        private async void btnSign_Click(object sender, EventArgs e)
        {
            SignDocument();
        }

        private X509Certificate2 SelectCertificate()
        {
            // Apri il Windows Certificate Store
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                // Filtra i certificati per firma digitale
                var certs = store.Certificates.Find(
                    X509FindType.FindByKeyUsage,
                    X509KeyUsageFlags.DigitalSignature,
                    true);

                if (certs.Count == 0)
                {
                    MessageBox.Show("Nessun certificato di firma trovato", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                // Mostra il selettore di certificati
                X509Certificate2Collection selection = X509Certificate2UI.SelectFromCollection(
                    certs,
                    "Seleziona certificato",
                    "Seleziona il certificato da utilizzare per la firma",
                    X509SelectionFlag.SingleSelection);

                return selection.Count > 0 ? selection[0] : null;
            }
            finally
            {
                store.Close();
            }
        }

        private Task<byte[]> SignHashWithCertificate(byte[] hash, X509Certificate2 certificate)
        {
            return Task.Run(() =>
            {
                // Crea una firma CMS/PKCS#7 per l'hash
                ContentInfo contentInfo = new ContentInfo(hash);
                SignedCms signedCms = new SignedCms(contentInfo, true); // detached = true

                // Crea il firmatario con il certificato selezionato
                CmsSigner signer = new CmsSigner(certificate);

                // Aggiungi attributi necessari per una firma valida
                signer.SignedAttributes.Add(new Pkcs9AttributeObject(
                    new Oid("1.2.840.113549.1.9.3"), // Content Type
                    new byte[] { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x07, 0x01 } // id-data
                ));

                //signer.SignedAttributes.Add(new Pkcs9AttributeObject(
                //    new Oid("1.2.840.113549.1.9.5"), // Signing Time
                //    new Pkcs9SigningTime(DateTime.Now)
                //));
                signer.SignedAttributes.Add(new Pkcs9AttributeObject(
                    new Pkcs9SigningTime(DateTime.Now)
                ));


                // Calcola la firma
                signedCms.ComputeSignature(signer);

                // Restituisci la firma codificata
                return signedCms.Encode();
            });
        }

        private byte[] SignExactHash(byte[] hash, X509Certificate2 certificate)
        {
            // Convert .NET certificate to BouncyCastle format
            Org.BouncyCastle.X509.X509Certificate bcCert;
            using (var certStream = new MemoryStream(certificate.RawData))
            {
                var parser = new X509CertificateParser();
                bcCert = parser.ReadCertificate(certStream);
            }

            // Extract private key
            AsymmetricKeyParameter privateKey;
            using (var rsa = certificate.GetRSAPrivateKey())
            {
                var rsaParams = rsa.ExportParameters(true);
                privateKey = new RsaPrivateCrtKeyParameters(
                    new BigInteger(1, rsaParams.Modulus),
                    new BigInteger(1, rsaParams.Exponent),
                    new BigInteger(1, rsaParams.D),
                    new BigInteger(1, rsaParams.P),
                    new BigInteger(1, rsaParams.Q),
                    new BigInteger(1, rsaParams.DP),
                    new BigInteger(1, rsaParams.DQ),
                    new BigInteger(1, rsaParams.InverseQ)
                );
            }

            // Create CMS signature
            CmsSignedDataGenerator generator = new CmsSignedDataGenerator();
            generator.AddCertificate(bcCert);
            generator.AddSigner(privateKey, bcCert, CmsSignedGenerator.DigestSha256);
            CmsProcessableByteArray content = new CmsProcessableByteArray(hash);
            CmsSignedData signedData = generator.Generate(content, true);

            return signedData.GetEncoded();
        }

        /// <summary>
        /// Creates a test X.509 certificate that can be used for signing
        /// </summary>
        private X509Certificate2 CreateTestCertificate()
        {
            // Create an RSA certificate (better supported for PDF signing than ECDSA)
            using (var rsa = System.Security.Cryptography.RSA.Create(2048))
            {
                var distinguishedName = new X500DistinguishedName("CN=Remote Signing Test Certificate");
                var request = new CertificateRequest(
                    distinguishedName,
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                // Add enhanced key usage for document signing
                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection { new Oid("1.3.6.1.4.1.311.10.3.12") }, // Document Signing
                        false));

                // Add basic key usage
                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(
                        X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation,
                        false));

                // Create self-signed certificate valid for 1 year
                var certificate = request.CreateSelfSigned(
                    DateTimeOffset.Now.AddDays(-1),
                    DateTimeOffset.Now.AddYears(1));

                // Ensure the certificate has a private key for our test scenario
                // In a real remote signing scenario, the private key would be on the remote server
                return certificate;
            }
        }


        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Chiudi pulitamente la connessione SignalR
            if (_connection != null)
            {
                try
                {
                    await _connection.StopAsync();
                }
                catch
                {
                    // Ignora eventuali errori durante la disconnessione
                }
            }

        }

        // Gestisci l'evento di ridimensionamento della form
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                // Assicurati che l'icona sia visibile prima di nascondere il form
                notifyIcon.Visible = true;

                Hide();
                ShowInTaskbar = false;

                // Mostra una notifica all'utente
                notifyIcon.ShowBalloonTip(2000, "Signature Client",
                    "L'applicazione � attiva nell'area di notifica",
                    ToolTipIcon.Info);
            }
        }

        // Mostra la form quando l'icona nella tray viene cliccata due volte
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        // Mostra il form dal suo stato minimizzato
        private void ShowForm()
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Activate();
        }

        // Gestione dei click sulle voci del menu contestuale
        private void signMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm();
            SignDocument();
        }

        private void configMenuItem_Click(object sender, EventArgs e)
        {
            var configForm = new ConfigForm(_config);
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                // Riconnettiti se le impostazioni sono cambiate
                if (_connection != null)
                {
                    try
                    {
                        _connection.StopAsync().Wait();
                    }
                    catch
                    {
                        // Ignora eventuali errori durante la disconnessione
                    }
                }

                InitializeSignalR();
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void WndProc(ref Message m)
        {
            // Gestisci i messaggi nativi di Windows
            if (m.Msg == Program.NativeMethods.WM_SHOWME)
            {
                // Mostra il form quando riceve il messaggio
                ShowForm();
            }

            base.WndProc(ref m);
        }
    }
}
