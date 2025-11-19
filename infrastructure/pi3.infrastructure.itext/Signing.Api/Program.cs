// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Signing.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registra i servizi necessari
            builder.Services.AddSingleton<IDocumentService, DocumentService>();
            builder.Services.AddSingleton<IPdfSigningService, PdfSigningService>();
            builder.Services.AddSingleton<IUserConnectionManager, UserConnectionManager>();

            // Configura SignalR
            builder.Services.AddSignalR(options =>
            {
                // Configurazioni opzionali per SignalR
                options.EnableDetailedErrors = true; // Utile per il debug
                options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 MB per messaggi pi� grandi
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignatureHub>("/signatureHub");
            });

            app.Run();
        }
    }
}
