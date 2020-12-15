--------------------------------------------------------
--  Ref Constraints for Table EMAILS
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."EMAILS" ADD CONSTRAINT "EMAILS_R01" FOREIGN KEY ("IDELEMENTORUBRICA")
	  REFERENCES "ITCOLL_6GIU12"."ELEMENTIRUBRICA" ("ID") DISABLE;
