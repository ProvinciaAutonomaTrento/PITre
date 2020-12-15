--------------------------------------------------------
--  Ref Constraints for Table ELEMENTIRUBRICA
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."ELEMENTIRUBRICA" ADD CONSTRAINT "ELEMENTIRUBRICA_R01" FOREIGN KEY ("IDAMMINISTRAZIONE")
	  REFERENCES "ITCOLL_6GIU12"."AMMINISTRAZIONI" ("ID") DISABLE;
