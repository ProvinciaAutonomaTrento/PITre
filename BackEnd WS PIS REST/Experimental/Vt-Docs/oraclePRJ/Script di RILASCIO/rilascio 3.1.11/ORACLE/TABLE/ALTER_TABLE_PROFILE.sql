ALTER TABLE 
   PROFILE
ADD
   (EXT  VARCHAR(256 BYTE));


CREATE INDEX INDX_EXT_PROFILE ON PROFILE (EXT)