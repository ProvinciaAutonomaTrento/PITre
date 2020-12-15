USE [GFD_29b_07_2010]
GO

/****** Object:  Trigger [DOCSADM].[TR_INSERT_DPA_TODOLIST]    Script Date: 09/06/2010 07:39:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE      TRIGGER [DOCSADM].[TR_INSERT_DPA_TODOLIST] ON [DOCSADM].[DPA_TRASMISSIONE]
AFTER UPDATE
AS
IF UPDATE(DTA_INVIO)
BEGIN
INSERT INTO DPA_TODOLIST
SELECT DT.system_id, dtu.id_trasm_singola, dtu.system_id,
DT.dta_invio, DT.id_people, DT.id_ruolo_in_uo,
dtu.id_people,dts.id_ragione,DT.var_note_generali,
dts.var_note_sing,dts.dta_scadenza, DT.id_profile,
DT.id_project,
CONVERT (INT,docsadm.vardescribe(dts.id_corr_globale,'ID_GRUPPO')) AS id_ruolo_dest,
CONVERT (INT,docsadm.vardescribe(DT.id_profile,'PROF_IDREG')) AS id_registro,
dts.CHA_TIPO_TRASM,
(case when dtu.dta_vista is null then convert (datetime, '17530101', 121) else dtu.DTA_VISTA end) as dta_vista,
DT.ID_PEOPLE_DELEGATO
FROM INSERTED DT, DPA_TRASM_SINGOLA dts,DPA_TRASM_UTENTE dtu
WHERE dtu.id_trasm_singola = dts.system_id AND dts.id_trasmissione = DT.system_id AND dtu.cha_in_todolist = 1
END

GO

