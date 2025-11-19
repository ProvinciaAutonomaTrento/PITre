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
    public interface IRubricaComuneDbContext
    {
        DbSet<ElementoRubricaEntity> ElementiRubricaEntities { get; set; }

        DbSet<EmailEntity> EmailEntities { get; set; }

        DbSet<UtenteEntity> UtentiEntities { get; set; }
    }
}
