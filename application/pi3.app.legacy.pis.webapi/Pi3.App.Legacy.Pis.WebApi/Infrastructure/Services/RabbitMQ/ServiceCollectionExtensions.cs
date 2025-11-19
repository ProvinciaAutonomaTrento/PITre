// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pi3.Core.SeedWork;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ
{
    public class RabbitMQOptions
    {
        public bool? EnableSubscribers { get; set; } = true;
        public string HostName { get; set; } = null!;
        public ushort? HostPort { get; set; }
        public string? VirtualHost { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureMockRabbitMQ(this IServiceCollection services)
        {
            services.AddScoped<IRabbitMQService, MockRabbitMQService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var configName = typeof(RabbitMQOptions).Name;
            services.Configure<RabbitMQOptions>(configuration.GetSection(configName));

            var options = new RabbitMQOptions();
            configuration.GetSection(configName).Bind(options);

            services.AddInfrastructureRabbitMQ(cfg =>
            {
                cfg.HostName = options.HostName;
                cfg.HostPort = options.HostPort;
                cfg.VirtualHost = options.VirtualHost;
                cfg.Username = options.Username;
                cfg.Password = options.Password;
            });

            return services;
        }

        public static IServiceCollection AddInfrastructureRabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> config)
        {
            var options = new RabbitMQOptions();
            config.Invoke(options);

            var factory = new ConnectionFactory()
            {
                HostName = options.HostName,
                DispatchConsumersAsync = true
            };

            if (options.HostPort.HasValue)
                factory.Port = options.HostPort.Value;

            if (!string.IsNullOrWhiteSpace(options.VirtualHost))
                factory.VirtualHost = options.VirtualHost;

            if (!string.IsNullOrWhiteSpace(options.Username))
                factory.UserName = options.Username;

            if (!string.IsNullOrWhiteSpace(options.Password))
                factory.Password = options.Password;

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            services.AddSingleton(s => channel);
            services.AddSingleton<IRabbitMQService, RabbitMQService>();

            return services;
        }
    }
}
