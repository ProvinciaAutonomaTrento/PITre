	--------------------------------------------------------
	--  DDL for Table DPA_STATI
	--------------------------------------------------------

	  CREATE TABLE "@db_user"."DPA_STATI" 
	   (	"SYSTEM_ID" NUMBER(*,0), 
		"ID_DIAGRAMMA" NUMBER(*,0), 
		"VAR_DESCRIZIONE" VARCHAR2(255 BYTE), 
		"STATO_INIZIALE" NUMBER(*,0), 
		"STATO_FINALE" NUMBER(*,0), 
		"CONV_PDF" NUMBER(*,0), 
		"STATO_CONSOLIDAMENTO" CHAR(1 BYTE), 
		"NON_RICERCABILE" NUMBER(*,0),
		"CHA_STATO_SISTEMA" CHAR(1 CHAR) DEFAULT 0
	   ) ;
