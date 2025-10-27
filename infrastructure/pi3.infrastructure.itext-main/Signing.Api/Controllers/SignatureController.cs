// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Signing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private readonly IHubContext<SignatureHub> _hubContext;
        private readonly IUserConnectionManager _connectionManager;
        private readonly IDocumentService _documentService;
        private readonly IPdfSigningService _pdfSigningService;

        public SignatureController(
            IHubContext<SignatureHub> hubContext,
            IUserConnectionManager connectionManager,
            IDocumentService documentService,
            IPdfSigningService pdfSigningService)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _documentService = documentService;
            _pdfSigningService = pdfSigningService;
        }

        [HttpPost("start/{documentId}")]
        public async Task<IActionResult> StartSignatureProcess(int documentId)
        {
            // Identifica l'utente corrente
            string userId = "user@example.com"; // User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            try
            {
                // Recupera il documento
                var document = await _documentService.GetDocumentByIdAsync(documentId);

                // Genera il PDF e calcola l'hash
                var (preparedPdf, hash) = GenerateDocumentHash(document.Content);

                // Crea un ID di sessione per questa firma
                string signatureSessionId = Guid.NewGuid().ToString("N");

                // Salva il PDF preparato associato alla sessione
                await _documentService.SavePreparedDocumentAsync(signatureSessionId, preparedPdf, userId);
                await _documentService.AssociateSigningSessionWithUserAsync(signatureSessionId, userId, documentId);
                // Recupera le connessioni SignalR dell'utente
                var connections = _connectionManager.GetConnections(userId);

                if (!connections.Any())
                {
                    // L'utente non ha connessioni attive
                    return BadRequest("No active client connection found for the user");
                }

                // Invia la notifica a tutte le connessioni dell'utente
                foreach (var connectionId in connections)
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("HashToSign", new
                    {
                        SessionId = signatureSessionId,
                        Hash = Convert.ToBase64String(hash),
                        DocumentName = document.Name
                    });
                }

                return Ok(new { SessionId = signatureSessionId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing signature request: {ex.Message}");
            }
        }

        // Altri metodi del controller...

        private (byte[] PreparedPdf, byte[] Hash) GenerateDocumentHash(byte[] pdfContent)
        {
            return _pdfSigningService.PrepareDocumentForSigning(pdfContent, "signature", DateTime.Now);
        }
    }
}
