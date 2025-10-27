// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Infrastructure.Legacy.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pi3.Infrastructure.Legacy.EF.Oracle
{
    public partial class OraclePi3DbContext : Pi3DbContext 
    {
        protected readonly ILogger<OraclePi3DbContext> _logger;
        protected readonly IOptions<OraclePi3DbContextOptions> _options;

        public OraclePi3DbContext(ILogger<OraclePi3DbContext> logger, IOptions<OraclePi3DbContextOptions> options)
        {
            this._logger = logger;
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
    }
}