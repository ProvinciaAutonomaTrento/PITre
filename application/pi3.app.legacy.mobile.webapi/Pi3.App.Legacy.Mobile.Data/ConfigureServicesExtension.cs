// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.DependencyInjection;
using Pi3.App.Legacy.Mobile.Data.Repositories;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.App.Legacy.Mobile.Data.Services;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
using Pi3.Infrastructure.Legacy.EF;

namespace Pi3.App.Legacy.Mobile.Data;

public static class ConfigureServicesExtension
{
    public static IServiceCollection AddHandlerRepositories( this IServiceCollection services )
    {
        services.AddScoped<IPeopleRepository, PeopleRepository>();
        services.AddScoped<ITrasmissionRepository, TrasmissionRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ISecurityRepository, SecurityRepository>();
        services.AddScoped<ICorrGlobaliRepository, CorrGlobaliRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        services.AddScoped<IAdministrationRepository, AdministrationRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRegisterRepository, RegisterRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IPeopleService, PeopleService>();
        services.AddScoped<ITrasmissionService, TrasmissionService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDelegheService, DelegheService>();
        services.AddScoped<IFascicoloService, FascicoloService>();

        services.AddInfrastructureLegacyEFDelegaAggregate();


        return services;
    }
}
