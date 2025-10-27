// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.Oracle
{
    public class OracleRubricaComuneDbContext : RubricaComuneDbContext
    {
        protected readonly ILogger<OracleRubricaComuneDbContext> _logger;
        protected readonly IConfiguration _configuration;
        protected readonly IOptions<OracleRubricaComuneDbContextOptions> _options;

        public OracleRubricaComuneDbContext(ILogger<OracleRubricaComuneDbContext> logger, IConfiguration configuration, IOptions<OracleRubricaComuneDbContextOptions> options)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseOracle(
                this._options.Value.ConnectionString,
                opt =>
                {
                    if (!string.IsNullOrWhiteSpace(this._options.Value.UseOracleSQLCompatibility))
                        opt.UseOracleSQLCompatibility(this._options.Value.UseOracleSQLCompatibility);
                });

            if (this._options.Value.EnableLogging ?? false)
                optionsBuilder.LogTo(a => this._logger.LogDebug(a));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasSequence<long>("SEQ_ER");
            modelBuilder.HasSequence<long>("SEQ_UT");
            modelBuilder.HasSequence<long>("SEQ_AMMINISTRAZIONI");

            modelBuilder.Entity<ElementoRubricaEntity>().ToTable("ELEMENTIRUBRICA");
            modelBuilder.Entity<ElementoRubricaEntity>().HasKey(p => p.ID);
            modelBuilder.Entity<ElementoRubricaEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ_ER");

            modelBuilder.Entity<EmailEntity>().ToTable("EMAILS");
            modelBuilder.Entity<EmailEntity>().HasKey(p => new { p.IDELEMENTORUBRICA, p.EMAIL });

            modelBuilder.Entity<UtenteEntity>().ToTable("UTENTI");
            modelBuilder.Entity<UtenteEntity>().HasKey(p => p.ID);
            modelBuilder.Entity<UtenteEntity>().Property(p => p.ID).ValueGeneratedOnAdd().UseHiLo("SEQ_UT");
        }
    }
}
