// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using iText.Kernel.Pdf;
using System.Collections.Concurrent;

namespace Signing.Api
{
    public interface IDocumentService
    {
        Task<DocumentModel> GetDocumentByIdAsync(int documentId);
        Task SavePreparedDocumentAsync(string sessionId, byte[] preparedDocument, string userId);
        Task<byte[]> GetPreparedDocumentAsync(string sessionId);
        Task SaveSignedDocumentAsync(int documentId, byte[] signedDocument);
        Task<bool> CanUserSignDocumentAsync(string userId, int documentId);
        Task AssociateSigningSessionWithUserAsync(string sessionId, string userId, int documentId);
        Task<bool> ValidateSigningSessionAsync(string sessionId, string userId);
        Task<int> GetDocumentIdForSessionAsync(string sessionId);
        Task<string> GetUserIdForSessionAsync(string sessionId);
    }

    public class DocumentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public byte[] SignedContent { get; set; } // Contenuto firmato
        public string AssignedUserId { get; set; } // ID dell'utente assegnato al documento
    }

    public record SigningSession(byte[] PreparedDocument, string UserId, int DocumentId);

    public class DocumentService : IDocumentService
    {
        private readonly ConcurrentDictionary<int, DocumentModel> _documents = new();
        private readonly ConcurrentDictionary<string, SigningSession> _signingSessions = new();

        public DocumentService() {
            var model = new DocumentModel() {
                Id = 1,
                Name = "test content",
                Content = GenerateTestPdf()
            };
            _documents[model.Id] = model;

        }

        public Task<DocumentModel> GetDocumentByIdAsync(int documentId)
        {
            if (_documents.TryGetValue(documentId, out var document))
            {
                return Task.FromResult(document);
            }

            throw new KeyNotFoundException($"Document with ID {documentId} not found.");
        }

        public Task SavePreparedDocumentAsync(string sessionId, byte[] preparedDocument, string userId)
        {
            // Controllo se la sessione esiste già
            if (_signingSessions.TryGetValue(sessionId, out var existingSession))
            {
                // Aggiorna la sessione esistente mantenendo l'ID documento
                _signingSessions[sessionId] = new SigningSession(preparedDocument, userId, existingSession.DocumentId);
            }
            else
            {
                // Crea una nuova sessione con documentId temporaneo (-1)
                // Il documentId reale verrà impostato successivamente con AssociateSigningSessionWithUserAsync
                _signingSessions[sessionId] = new SigningSession(preparedDocument, userId, -1);
            }
            return Task.CompletedTask;
        }

        public Task<byte[]> GetPreparedDocumentAsync(string sessionId)
        {
            if (_signingSessions.TryGetValue(sessionId, out var session))
            {
                return Task.FromResult(session.PreparedDocument);
            }

            throw new KeyNotFoundException($"Prepared document for session ID {sessionId} not found.");
        }

        public Task SaveSignedDocumentAsync(int documentId, byte[] signedDocument)
        {
            if (_documents.TryGetValue(documentId, out var document))
            {
                document.SignedContent = signedDocument;
                File.WriteAllBytes($"c:\\temp\\signed_document_{documentId}.pdf", signedDocument);

                return Task.CompletedTask;
            }

            throw new KeyNotFoundException($"Document with ID {documentId} not found.");
        }

        public Task<bool> CanUserSignDocumentAsync(string userId, int documentId)
        {
            // Logica per verificare se l'utente ha i permessi per firmare il documento
            // Per esempio, controlla se l'utente è assegnato al documento
            if (_documents.TryGetValue(documentId, out var document))
            {
                return Task.FromResult(document.AssignedUserId == userId);
            }

            return Task.FromResult(false);
        }

        public Task AssociateSigningSessionWithUserAsync(string sessionId, string userId, int documentId)
        {
            if (!_documents.ContainsKey(documentId))
            {
                throw new KeyNotFoundException($"Document with ID {documentId} not found.");
            }


            _signingSessions[sessionId] = new SigningSession(_signingSessions[sessionId].PreparedDocument, userId, documentId);
            return Task.CompletedTask;
        }

        public Task<bool> ValidateSigningSessionAsync(string sessionId, string userId)
        {
            if (_signingSessions.TryGetValue(sessionId, out var session))
            {
                return Task.FromResult(session.UserId == userId);
            }

            return Task.FromResult(false);
        }

        public Task<int> GetDocumentIdForSessionAsync(string sessionId)
        {
            if (_signingSessions.TryGetValue(sessionId, out var session))
            {
                return Task.FromResult(session.DocumentId);
            }

            throw new KeyNotFoundException($"Session ID {sessionId} not found.");
        }

        public Task<string> GetUserIdForSessionAsync(string sessionId)
        {
            if (_signingSessions.TryGetValue(sessionId, out var session))
            {
                return Task.FromResult(session.UserId);
            }

            throw new KeyNotFoundException($"Session ID {sessionId} not found.");
        }

        private byte[] GenerateTestPdf()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(memoryStream))
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new iText.Layout.Document(pdf);

                    // Add some content to the document
                    document.Add(new iText.Layout.Element.Paragraph("Test PDF Document for Remote Signing"));
                    document.Add(new iText.Layout.Element.Paragraph("This is a sample PDF created to test remote digital signing."));
                    document.Add(new iText.Layout.Element.Paragraph($"Created on {DateTime.Now:yyyy-MM-dd HH:mm:ss}"));

                    // Add a second page to demonstrate multi-page signing
                    document.Add(new iText.Layout.Element.AreaBreak());
                    document.Add(new iText.Layout.Element.Paragraph("Second page of the document"));
                    document.Add(new iText.Layout.Element.Paragraph("The signature should be applied to the first page."));

                    document.Close();
                }

                return memoryStream.ToArray();
            }
        }

    }
}
