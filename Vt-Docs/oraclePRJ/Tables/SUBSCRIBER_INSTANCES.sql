--------------------------------------------------------
--  DDL for Table SUBSCRIBER_INSTANCES
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."SUBSCRIBER_INSTANCES" 
   (	"ID" NUMBER(10,0), 
	"NAME" NVARCHAR2(50), 
	"DESCRIPTION" NVARCHAR2(255), 
	"SMTPHOST" NVARCHAR2(50), 
	"SMTPPORT" NUMBER(*,0), 
	"SMTPSSL" CHAR(1 BYTE), 
	"SMTPUSERNAME" NVARCHAR2(50), 
	"SMTPPASSWORD" NVARCHAR2(50), 
	"SMTPMAIL" NVARCHAR2(255)
   ) ;
