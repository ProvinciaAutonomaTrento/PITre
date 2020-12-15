--------------------------------------------------------
--  Constraints for Table DPA_ASS_GRIDS
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."DPA_ASS_GRIDS" ADD CONSTRAINT "PK_DPA_ASS_GRIDS" PRIMARY KEY ("GRID_ID", "USER_ID", "ROLE_ID") ENABLE;
 
  ALTER TABLE "ITCOLL_6GIU12"."DPA_ASS_GRIDS" MODIFY ("GRID_ID" NOT NULL ENABLE);
