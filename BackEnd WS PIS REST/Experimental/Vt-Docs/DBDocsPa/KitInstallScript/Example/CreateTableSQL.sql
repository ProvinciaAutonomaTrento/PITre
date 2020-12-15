IF NOT EXISTS (select * from sysobjects where name = 'DPA_ITEMS_CONSERVAZIONE' and type = 'U')
begin
CREATE TABLE [@db_user].[DPA_ITEMS_CONSERVAZIONE] (
  [SYSTEM_ID]           [int]    NOT NULL IDENTITY (1, 1),
  [ID_CONSERVAZIONE]    [int] NULL,
  [ID_PROFILE]          [int] NULL,
  [ID_PROJECT]          [int] NULL,
  [CHA_TIPO_DOC]        [char] NULL,
  [VAR_OGGETTO]         [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  [ID_REGISTRO]         [int] NULL,
  [DATA_INS]            [datetime] NULL,
  [CHA_STATO]           [char] NULL,
  [VAR_XML_METADATI]    [varchar] (1024) NULL,
  [SIZE_ITEM]           [int] NULL,
  [COD_FASC]            [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
  [DOCNUMBER]           [int] NULL,
  [VAR_TIPO_FILE]       [varchar] (32) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
  [NUMERO_ALLEGATI]     [int] NULL
)ON [PRIMARY]
end
GO