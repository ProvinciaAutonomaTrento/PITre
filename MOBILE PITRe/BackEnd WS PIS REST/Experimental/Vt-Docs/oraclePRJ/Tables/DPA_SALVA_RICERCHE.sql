--------------------------------------------------------
--  DDL for Table DPA_SALVA_RICERCHE
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_SALVA_RICERCHE" 
   (	"SYSTEM_ID" NUMBER(10,0), 
	"VAR_DESCRIZIONE" VARCHAR2(64 BYTE), 
	"ID_PEOPLE" NUMBER(10,0), 
	"ID_GRUPPO" NUMBER(10,0), 
	"VAR_PAGINA_RIC" VARCHAR2(50 BYTE), 
	"VAR_FILTRI_RIC" CLOB, 
	"CHA_IN_ADL" CHAR(1 CHAR), 
	"TIPO" CHAR(1 BYTE), 
	"GRID_ID" NUMBER(*,0)
   ) ;
