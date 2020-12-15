-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 04/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- insert_dpa_elenco_note
-- =============================================

-- CREATE INDEX IX_ELENCO_NOTE_K2 ON DPA_ELENCO_NOTE (ID_REG_RF)  

EXECUTE DOCSADM.utl_add_index '3.24','DOCSADM','DPA_ELENCO_NOTE','IX_ELENCO_NOTE_K2',null,'ID_REG_RF',null,null,null,'NORMAL', null,null,null 

DELETE 
FROM DOCSADM.Dpa_Elenco_Note 
WHERE COD_REG_RF = 'TUTTI' 
	AND ID_REG_RF = 0
	AND VAR_DESC_NOTA IN ('N.B. Irregolarit nel documento principale','N.B. Irregolarit negli allegati','N.B. Irregolarit nel documento principale e negli allegati');

-- need this dummy record if table is empty
INSERT INTO DOCSADM.DPA_ELENCO_NOTE 
(
	--System_Id
	 Id_Reg_Rf
	, Var_Desc_Nota
	, Cod_Reg_Rf
)
VALUES 
(
	--1       
	0         
	,'dummy note'  
	,'TUTTI'
) 

INSERT INTO DOCSADM.DPA_ELENCO_NOTE 
(
	--System_Id
	 Id_Reg_Rf
	, Var_Desc_Nota
	, Cod_Reg_Rf
)
VALUES
(
	0 
	, 'N.B. Irregolarità nel documento principale'
	, 'TUTTI' 
)


INSERT INTO DOCSADM.DPA_ELENCO_NOTE
(	
	--System_Id, 
	Id_Reg_Rf
	, Var_Desc_Nota
	, Cod_Reg_Rf
)
VALUES
(
	0 
	, 'N.B. Irregolarità negli allegati'
	, 'TUTTI' 
)

INSERT INTO DOCSADM.DPA_ELENCO_NOTE 
(
	--System_Id, 
	Id_Reg_Rf
	, Var_Desc_Nota
	, Cod_Reg_Rf
)
VALUES
(
	0 
	, 'N.B. Irregolarità nel documento principale e negli allegati'
	, 'TUTTI' 
)

DELETE 
FROM DOCSADM.DPA_ELENCO_NOTE 
WHERE COD_REG_RF = 'TUTTI' 
	AND ID_REG_RF = 0
	AND VAR_DESC_NOTA = 'dummy note'

