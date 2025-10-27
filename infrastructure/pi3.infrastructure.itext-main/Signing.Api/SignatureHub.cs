// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Cms;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace Signing.Api;

//[Authorize] // Richiede che l'utente sia autenticato
public class SignatureHub : Hub
{
    private readonly IDocumentService _documentService;
    private readonly IPdfSigningService _signingService;
    private readonly IUserConnectionManager _connectionManager;

    private record UserMetadata(string Name, string Email, string Role, string CustomData );

    public SignatureHub(IDocumentService documentService, IPdfSigningService signingService,
        IUserConnectionManager connectionManager)
    {
        _documentService = documentService;
        _signingService = signingService;
        _connectionManager = connectionManager;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            // Ottiene l'ID utente dal contesto
            //string userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Extract the user metadata from query string
            string userMetadata = Context.GetHttpContext()?.Request.Query["userMetadata"];
            UserMetadata metadataObj = JsonSerializer.Deserialize<UserMetadata>(userMetadata);
            if (!string.IsNullOrEmpty(metadataObj.Email))
            {
                // Memorizza la mappatura tra utente e ID connessione
                _connectionManager.AddConnection(metadataObj.Email, Context.ConnectionId);
                _connectionManager.StoreConnectionMetadata(Context.ConnectionId, metadataObj);

                // Registra l'evento di connessione
                await Clients.Caller.SendAsync("ConnectionEstablished", new
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = metadataObj.Email,
                    Message = "Connected to signature service"
                });

                Console.WriteLine($"User {metadataObj.Email} connected with connection ID: {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine("Connected user has no identity");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnConnectedAsync: {ex.Message}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
            //string userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var connMetadata = _connectionManager.GetConnectionMetadata< UserMetadata>(Context.ConnectionId);
            _connectionManager.RemoveConnection(Context.ConnectionId);


            if (!string.IsNullOrEmpty(connMetadata.Email))
            {
                Console.WriteLine($"User {connMetadata.Email} disconnected");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnDisconnectedAsync: {ex.Message}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    private UserMetadata UserData => _connectionManager.GetConnectionMetadata<UserMetadata>(Context.ConnectionId);

    // Metodo chiamato dal browser per iniziare la procedura di firma
    public async Task<string> RequestDocumentSignature(int documentId)
    {
        //string userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //if (string.IsNullOrEmpty(userId))
        //{
        //    throw new HubException("User not authenticated");
        //}

        try
        {
            var connMetadata = _connectionManager.GetConnectionMetadata<UserMetadata>(Context.ConnectionId);
            // Verifica che l'utente abbia i permessi per firmare questo documento
            bool canSign = await _documentService.CanUserSignDocumentAsync(connMetadata.Email, documentId);

            if (!canSign)
            {
                throw new HubException("User not authorized to sign this document");
            }

            // Recupera il documento
            var document = await _documentService.GetDocumentByIdAsync(documentId);

            // Genera un ID di sessione per questa procedura di firma
            string signatureSessionId = Guid.NewGuid().ToString("N");

            // Prepara il PDF e calcola l'hash (usa la tua logica esistente)
            var (preparedPdf, hash) = _signingService.PrepareDocumentForSigning(
                document.Content,
                $"Signature_{signatureSessionId}",
                DateTime.Now);

            // Salva il PDF preparato in storage temporaneo
            await _documentService.SavePreparedDocumentAsync(signatureSessionId, preparedPdf, connMetadata.Email);

            // Salva anche l'associazione tra sessione di firma e utente
            await _documentService.AssociateSigningSessionWithUserAsync(signatureSessionId, connMetadata.Email, documentId);

            // Notifica l'app client che è disponibile un documento da firmare
            var connections = _connectionManager.GetConnections(connMetadata.Email);
            foreach (var connectionId in connections)
            {
                await Clients.Client(connectionId).SendAsync("DocumentReadyForSigning", new
                {
                    SessionId = signatureSessionId,
                    DocumentId = documentId,
                    DocumentName = document.Name,
                    Hash = Convert.ToBase64String(hash)
                });
            }

            return signatureSessionId;
        }
        catch (HubException)
        {
            throw; // Rilancia le HubException perché contengono già informazioni sull'errore
        }
        catch (Exception ex)
        {
            throw new HubException($"Error requesting document signature: {ex.Message}");
        }
    }

    // Metodo chiamato dal client WinForms dopo che l'utente ha firmato l'hash
    public async Task<bool> CompleteSignature(string sessionId, string base64Signature)
    {
        //string userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //if (string.IsNullOrEmpty(userId))
        //{
        //    throw new HubException("User not authenticated");
        //}

        try
        {
            // Verifica che questa sessione di firma appartenga all'utente corrente
            bool isValidSession = await _documentService.ValidateSigningSessionAsync(sessionId, UserData.Email);

            if (!isValidSession)
            {
                throw new HubException("Invalid or expired signing session");
            }

            // Recupera il documento preparato
            byte[] preparedPdf = await _documentService.GetPreparedDocumentAsync(sessionId);

            // Converti la firma da base64
            byte[] signature = Convert.FromBase64String(base64Signature);

            // Applica la firma al documento preparato
            byte[] signedPdf = _signingService.ApplySignatureToDocument(preparedPdf, signature, "signature");

            // Salva il documento firmato
            int documentId = await _documentService.GetDocumentIdForSessionAsync(sessionId);
            await _documentService.SaveSignedDocumentAsync(documentId, signedPdf);

            // Notifica il risultato al browser
            string browserUserId = await _documentService.GetUserIdForSessionAsync(sessionId);
            if (!string.IsNullOrEmpty(browserUserId))
            {
                // Get all connections for this user from the connection manager
                var connections = _connectionManager.GetConnections(browserUserId);

                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("SignatureCompleted", new
                    {
                        SessionId = sessionId,
                        DocumentId = documentId,
                        Success = true,
                        Message = "Document successfully signed"
                    });
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            // Notifica l'errore al browser
            string browserUserId = await _documentService.GetUserIdForSessionAsync(sessionId);
            if (!string.IsNullOrEmpty(browserUserId))
            {
                // Get all connections for this user from the connection manager
                var connections = _connectionManager.GetConnections(browserUserId);

                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("SignatureError", new
                    {
                        SessionId = sessionId,
                        Error = ex.Message
                    });
                }
            }

            throw new HubException($"Error completing signature: {ex.Message}");
        }
    }

    // Metodo per ottenere l'ID di connessione corrente (utile per debug)
    public string GetConnectionId() => Context.ConnectionId;
}
