--------------------------------------------------------
--  DDL for Table DPA_NOTE
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_NOTE" 
   (	"SYSTEM_ID" NUMBER(*,0), 
	"TESTO" NVARCHAR2(2000), 
	"DATACREAZIONE" DATE, 
	"IDUTENTECREATORE" NUMBER(*,0), 
	"IDRUOLOCREATORE" NUMBER(*,0), 
	"TIPOVISIBILITA" CHAR(1 BYTE), 
	"TIPOOGGETTOASSOCIATO" CHAR(1 CHAR), 
	"IDOGGETTOASSOCIATO" NUMBER(*,0), 
	"IDPEOPLEDELEGATO" NUMBER(*,0), 
	"IDRFASSOCIATO" NUMBER(*,0)
   ) ;
