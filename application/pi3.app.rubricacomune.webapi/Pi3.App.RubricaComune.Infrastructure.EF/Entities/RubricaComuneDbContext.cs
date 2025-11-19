// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.Entities
{
    public class RubricaComuneDbContext : DbContext, IRubricaComuneDbContext
    {
        public DbSet<ElementoRubricaEntity> ElementiRubricaEntities { get; set; }

        public DbSet<EmailEntity> EmailEntities { get; set; }

        public DbSet<UtenteEntity> UtentiEntities { get; set; }
    }
}
