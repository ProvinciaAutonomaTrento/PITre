// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Oracle
{
    public partial class OraclePi3DbContext 
    {
        public async override Task BookRegProto(long idRegistro)
        {
            await this.Database.ExecuteSqlInterpolatedAsync($"SELECT RP.NUM_RIF FROM DPA_REG_PROTO RP WHERE RP.ID_REGISTRO = {idRegistro} FOR UPDATE OF RP.ID_REGISTRO, RP.NUM_RIF");
        }

        public async override Task BookProgressivoFascicolo(long idTitolario, long? idRegistro = null) 
        {
            if (idRegistro.HasValue)
                await this.Database.ExecuteSqlInterpolatedAsync($"SELECT RF.NUM_RIF FROM DPA_REG_FASC RF WHERE RF.ID_REGISTRO = {idRegistro} AND ID_TITOLARIO = {idTitolario} FOR UPDATE OF RF.ID_REGISTRO, RF.ID_TITOLARIO, RF.NUM_RIF");
            else
                await this.Database.ExecuteSqlInterpolatedAsync($"SELECT RF.NUM_RIF FROM DPA_REG_FASC RF WHERE RF.ID_REGISTRO IS NULL AND ID_TITOLARIO = {idTitolario} FOR UPDATE OF RF.ID_REGISTRO, RF.ID_TITOLARIO, RF.NUM_RIF");
        }

        public async override Task<DateTime> GetSystemDateTime()
        {
            return await this.Database.SqlQuery<DateTime>($"SELECT SYSDATE as \"Value\" FROM DUAL").SingleAsync();
        }

        public async override Task<string> ConvertDegre()
        {
            return "' || chr(ASCII('°')) || '";
        }


        public override async Task<long> GetValCampoProfDocOrderWithNumberCast(long docNumber, long objId)
        {
            return await this.Database.SqlQuery<long>($"SELECT to_number(getValCampoProfDocOrder({docNumber}, {objId})) FROM DUAL").SingleAsync();
        }
        public override async Task<DateTime> GetValCampoProfDocWithDataCast(long docNumber, long objId)
        {
            return await this.Database.SqlQuery<DateTime>($"SELECT to_date(getValCampoProfDoc({docNumber}, {objId}), 'dd/mm/yyyy HH24:mi:ss') FROM DUAL").SingleAsync();
        }

        public async override Task BookContatoreRepertorio(long idContatore) {
            await this.Database.ExecuteSqlInterpolatedAsync($"SELECT CD.VALORE FROM  DPA_CONTATORI_DOC CD WHERE CD.SYSTEM_ID = {idContatore} FOR UPDATE OF CD.SYSTEM_ID, CD.VALORE");
        }

        public async override Task BookContatoreRepertorioComune(long idContatore)
        {
            await this.Database.ExecuteSqlInterpolatedAsync($"SELECT CD.VALORE FROM  DPA_CONTATORI_DOC CD WHERE CD.SYSTEM_ID = {idContatore} FOR UPDATE OF CD.SYSTEM_ID, CD.VALORE");
        }

        public async override Task BookContatoreRepertorioFasc(long idContatore)
        {
            await this.Database.ExecuteSqlInterpolatedAsync($"SELECT CD.VALORE FROM  DPA_CONTATORI_FASC CD WHERE CD.SYSTEM_ID = {idContatore} FOR UPDATE OF CD.SYSTEM_ID, CD.VALORE");
        }

        public override IQueryable<ProfileFullTextEntity> ProfileFullText(string text)
        {
            string[] specialChars = @"\ - ~ | & ( ) [ ] { } , = > * ; $ ! _".Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string wildcardChar = @"\";

            foreach (string item in specialChars)
                text = text.Replace(item, wildcardChar + item);

            var sql = FormattableStringFactory.Create($"SELECT p.SYSTEM_ID FROM PROFILE p WHERE CONTAINS(VAR_PROF_OGGETTO, '{text.Replace("'", "''")}') > 0 ");

            return this.Set<ProfileFullTextEntity>().FromSqlInterpolated(sql);
        }

        public override IQueryable<CorrGlobaliFullTextEntity> CorrGlobaliFullText(string text)
        {
            string[] specialChars = @"\ - ~ | & ( ) [ ] { } , = > * ; $ ! _".Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string wildcardChar = @"\";

            foreach (string item in specialChars)
                text = text.Replace(item, wildcardChar + item);

            var sql = FormattableStringFactory.Create($"SELECT c.SYSTEM_ID FROM DPA_CORR_GLOBALI c WHERE CONTAINS(c.VAR_DESC_CORR, '{text.Replace("'", "''")}') > 0 ");

            return this.Set<CorrGlobaliFullTextEntity>().FromSqlInterpolated(sql);
        }
    }
}
