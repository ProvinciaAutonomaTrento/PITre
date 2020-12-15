--------------------------------------------------------
--  DDL for Table PUBLISHER_ERRORS
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."PUBLISHER_ERRORS" 
   (	"ID" NUMBER, 
	"PUBLISHINSTANCEID" NUMBER, 
	"ERRORCODE" NVARCHAR2(255), 
	"ERRORDESCRIPTION" NVARCHAR2(2000), 
	"ERRORSTACK" NVARCHAR2(2000), 
	"ERRORDATE" DATE
   ) ;
