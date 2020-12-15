--------------------------------------------------------
--  DDL for Table DPA_LDAP_SYNC_HISTORY
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_LDAP_SYNC_HISTORY" 
   (	"SYSTEM_ID" NUMBER(*,0), 
	"ID_AMM" NUMBER(*,0), 
	"USER_ID" VARCHAR2(50 BYTE), 
	"SYNC_DATE" DATE, 
	"ITEMS_SYNCRONIZED" NUMBER(*,0), 
	"ERROR_DETAILS" VARCHAR2(2000 BYTE)
   ) ;
