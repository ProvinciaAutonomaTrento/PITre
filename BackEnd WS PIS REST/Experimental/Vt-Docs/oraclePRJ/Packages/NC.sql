--------------------------------------------------------
--  DDL for Package NC
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE "ITCOLL_6GIU12"."NC" AS 
  
  TYPE T_CURSOR IS REF Cursor; 
  
  -- Procedura per l'inserimento di un nuovo canale
  Procedure InsertChannel(p_Label NVarChar2, p_Description VarChar2);
  
  -- Procedura per il reperimento dei canali
  Procedure LoadChannels(channels Out t_Cursor);
  
  -- Procedura per il reperimento di un canale a partire dalla sua etichetta
  Procedure LoadChannelByLabel(p_LabelToSearch NVarChar2, p_Id Out Integer, p_Label Out NVarChar2, p_Description Out NVarChar2);
  
  -- Procedura per l'inserimento di una nuova istanza
  Procedure InsertInstance(p_Description NVarChar2);
  
  -- Procedura per il recupero delle istanze
  Procedure LoadInstances(Inst Out t_Cursor);
  
  -- Procedura per il recupero di istanze a partire dal nome
  Procedure SearchInstancesByName(p_Name NVarChar2, Inst Out t_Cursor);
  
  -- Procedura per il salvataggio delle impostazioni relative ad un canale
  Procedure AssociateChannelToInstance(p_ChannelId Integer, p_InstanceId Integer);
  
  -- Caricamento delle impostazioni relative ad un canale
  Procedure LoadChannelsRelatedToInstance(p_InstanceId Number, Channels Out t_Cursor);
  
  -- Inserimento di un item nella lista degli item
  Procedure InsertItem(p_Author NVarchar2, p_Title NVarChar2, p_Text NVarChar2, p_ChannelId Number, p_MessageId Number, p_MessageNumber Integer, p_ItemId Out Integer);
  
  -- Associazione di un item ad una categoria
  Procedure AssociateItemToChannel(p_ItemId Number, p_ChannelId Integer);
  
  -- Recupero delle informazioni su di un item
  Procedure LoadItem(p_ItemId Integer, p_Author Out NVarchar2, p_Title Out NVarChar2, p_Text Out NVarChar2, p_LastUpdate Out Date, p_PublishDate Out Date, p_MessageId Out Number, p_MessageNumber Out Number);
  
  -- Recupero dei canali associati ad un item
  Procedure LoadChannelsRelatedToItem(p_ItemId Number, Channel Out T_Cursor);
  
  -- Salvatggio del riferimento ad un id di utente che deve visualizzare una notifica
  Procedure InsertUser(p_UserId Number, p_ItemId Number, p_InstanceId Number);
  
  -- Impostazione della data di visualizzazione di una notifica relativa ad uno specifico utente
  Procedure SetItemViewed(p_ItemId Integer, p_UserId Integer, p_InstanceId Integer);
  
  -- Ricerca di item per Canale
  Procedure SearchItemByChannelId(p_ChannelId Number, p_InstanceId Number, Items Out T_Cursor);
  
  -- Ricerca di item per intervallo di date di ricezione
  Procedure SearchItemByDateRange(p_LowDate Date, p_HightDate Date, p_InstanceId Integer, Items Out T_Cursor);
  
  -- Ricerca di item per intervallo di id
  Procedure SearchItemByMessageIdRange(p_LowMessageId Integer, p_HightMessageId Integer, p_InstanceId Number, Items Out T_Cursor);
  
  -- Ricerca degli item che devono ancora essere visualizzati da un utente relativamente ad una specifica categoria
  Procedure SearchItemsNotViewedByUser(p_UserId Integer, p_ChannelId Integer, p_PageSize Integer, p_PageNumber Integer, p_Count Out Integer, p_InstanceId Number, Item Out T_Cursor);
  
  -- Conteggio degli item che devono ancora essere visualizzati da un utente
  Procedure CountNotViewedItems(p_UserId Integer, p_InstanceId Integer, p_Count Out Integer);
  
  -- Ricerca di item con filtri su data, id messaggio, testo contenuto nella notifica
  Procedure SearchItem(p_UserId Integer, p_SearchForMessageNumber Integer, p_LowMessageNumber Integer, p_HightMessageNumber Integer, 
    p_SearchForDate Number, p_LowDate Date, p_HightDate Date, 
    p_SearchForTitle Integer, p_ItemText VarChar2, p_InstanceId Integer,
    Items Out T_Cursor);
  
END NC;

/

--------------------------------------------------------
--  DDL for Package Body NC
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY "ITCOLL_6GIU12"."NC" AS

-- Procedura per l'inserimento di un nuovo canale
  Procedure InsertChannel(p_Label NVarChar2, p_Description VarChar2) Is Begin
    
    INSERT
    INTO NOTIFICATIONCHANNEL
      (
        ID,
        LABEL,
        DESCRIPTION
      )
      VALUES
      (
        SEQ_NOTIFICATIONCHANNEL.nextVal,
        p_Label,
        p_Description
      );

  End InsertChannel;

  -- Procedura per il reperimento dei canali
  Procedure LoadChannels(channels Out t_Cursor) Is Begin
    Open channels For 'Select * From NotificationChannel';
  
  End LoadChannels;
  
  
  -- Procedura per il reperimento di un canale a partire dalla sua etichetta
  Procedure LoadChannelByLabel(p_LabelToSearch NVarChar2, p_Id Out Integer, p_Label Out NVarChar2, p_Description Out NVarChar2) Is Begin
    Select Id, Label, Description Into p_Id, p_Label, p_Description From NotificationChannel Where Label = p_LabelToSearch;
  End LoadChannelByLabel;
  
  -- Procedura per l'inserimento di una nuova istanza
  Procedure InsertInstance(p_Description NVarChar2) Is Begin
    INSERT INTO NOTIFICATIONINSTANCE
    (ID, DESCRIPTION
    ) VALUES
    (
      SEQ_NOTIFICATIONINSTANCES.Nextval, 
      p_Description
    );
    
  End InsertInstance;
  
  -- Procedura per il recupero delle istanze
  Procedure LoadInstances(Inst Out t_Cursor) Is Begin
    Open Inst For Select * From NotificationInstance;
    
  End LoadInstances;
  
  -- Procedura per il recupero di istanze a partire dal nome
  Procedure SearchInstancesByName(p_Name NVarChar2, Inst Out t_Cursor) Is Begin
    Open Inst For Select * From NotificationInstance Where Upper(Description) = Upper(p_Name);
  
  End SearchInstancesByName;
  
  -- Procedura per l'associazione di un canale ad una istanza
  Procedure AssociateChannelToInstance(p_ChannelId Integer, p_InstanceId Integer) Is Begin
    Declare exist Number;
    
    Begin
  
    -- L'associazione fra canale e istanza viene fatta solo se non esiste gia
    Select Count(*) Into exist From NotificationInstanceChannels Where InstanceId = p_InstanceId And ChannelId = p_ChannelId;
    
    If exist = 0 Then
      INSERT INTO NotificationInstanceChannels
      (INSTANCEID, CHANNELID
      ) VALUES
      (
        p_InstanceId,
        p_ChannelId
      );
    End If;
    End;
  End AssociateChannelToInstance;
  
  -- Caricamento dei canali associati ad una istanza
  Procedure LoadChannelsRelatedToInstance(p_InstanceId Number, Channels Out t_Cursor) Is Begin
  
    Open Channels For Select * From NotificationChannel Where Exists (Select 'x' From NotificationInstanceChannels Where InstanceId = p_InstanceId);
  End LoadChannelsRelatedToInstance;

  -- Inserimento di un item nella lista degli item
  Procedure InsertItem(p_Author NVarchar2, p_Title NVarChar2, p_Text NVarChar2, p_ChannelId Number, p_MessageId Number, p_MessageNumber Integer, p_ItemId Out Integer) Is Begin
  
    Begin
    
    INSERT
    INTO NOTIFICATIONITEM
      (
        ID,
        AUTHOR,
        TITLE,
        TEXT,
        LASTUPDATE,
        PUBLISHDATE,
        MESSAGEID,
        MessageNumber
      )
      VALUES
      (
        SEQ_NOTIFICATIONITEM.nextval,
        p_Author,
        p_Title,
        p_Text,
        sysdate,
        sysdate,
        p_MessageId,
        p_MessageNumber
      );
      
      Select Max(Id) Into p_itemId From NotificationItem;
    
      INSERT INTO NOTIFICATIONITEMCATEGORIES
      (ITEMID, CATEGORYID
      ) VALUES
      (
        p_itemId,
        p_ChannelId
      );
      
      End;
      
  End InsertItem;
  
  -- Associazione di un item ad un canale
  Procedure AssociateItemToChannel(p_ItemId Number, p_ChannelId Integer) Is Begin
    INSERT INTO NOTIFICATIONITEMCATEGORIES
    (ITEMID, CATEGORYID
    ) VALUES
    (
      p_ItemId,
      p_ChannelId
    );

  End AssociateItemToChannel;
  
  -- Recupero delle informazioni su di un item
  Procedure LoadItem(p_ItemId Integer, p_Author Out NVarchar2, p_Title Out NVarChar2, p_Text Out NVarChar2, p_LastUpdate Out Date, p_PublishDate Out Date, p_MessageId Out Number, p_MessageNumber Out Number) Is Begin
    SELECT
      AUTHOR,
      TITLE,
      TEXT,
      LASTUPDATE,
      PUBLISHDATE,
      MESSAGEID,
      MessageNumber Into
      p_Author,
      p_Title,
      p_Text,
      p_LastUpdate,
      p_PublishDate,
      p_MessageId,
      p_MessageNumber
    FROM NOTIFICATIONITEM ;

  End LoadItem;
  
  Procedure LoadChannelsRelatedToItem(p_ItemId Number, Channel Out T_Cursor) Is Begin
    Open Channel For Select * From NotificationChannel Where Exists (Select 'x' From NotificationItemCategories Where NotificationChannel.Id = NotificationItemCategories.CategoryId And NotificationItemCategories.ItemId = p_ItemId);
    
  End LoadChannelsRelatedToItem;
  
  -- Salvatggio del riferimento ad un id di utente di una data istanza che deve visualizzare una notifica
  Procedure InsertUser(p_UserId Number, p_ItemId Number, p_InstanceId Number) Is Begin
    INSERT INTO NOTIFICATIONUSER
    (ITEMID, USERID, InstanceId
    ) VALUES
    (
      p_ItemId,
      p_UserId,
      p_InstanceId
    );

  End InsertUser;
  
  -- Impostazione della data di visualizzazione di una notifica relativa ad uno specifico utente
  Procedure SetItemViewed(p_ItemId Integer, p_UserId Integer, p_InstanceId Integer) Is Begin
    Update NOTIFICATIONUSER
    Set ViewDate = sysdate
    Where ItemId = p_ItemId And UserId = p_UserId And InstanceId = p_InstanceId; 
  End SetItemViewed;
  
  -- Ricerca di item per Canale e id istanza
  Procedure SearchItemByChannelId(p_ChannelId Number, p_InstanceId Number, Items Out T_Cursor) Is Begin

    Open Items For Select *
                    From NotificationItem ni
                    Where Exists
                      (Select 'x'
                      From NotificationItemCategories nic
                      Where nic.CategoryId = p_ChannelId
                      And nic.ItemId       = ni.Id
                      And Exists
                        (Select 'x'
                        From NotificationInstanceChannels nic1
                        Where nic1.InstanceId  = p_InstanceId
                        And  nic1.ChannelId = nic.CategoryId
                        )
                      );

  End SearchItemByChannelId;
  
  -- Ricerca di item per intervallo di date di ricezione per una data istanza
  Procedure SearchItemByDateRange(p_LowDate Date, p_HightDate Date, p_InstanceId Integer, Items Out T_Cursor) Is Begin
    Open Items For Select * 
        From NotificationItem
        Inner Join NotificationItemCategories
        On Id = ItemId
        Inner Join NotificationInstanceChannels
        On ChannelId = CategoryId
        Where PublishDate >= p_LowDate 
        And PublishDate <= p_HightDate
        And InstanceId = p_InstanceId;
    
  End SearchItemByDateRange;

  -- Ricerca di item per intervallo di id
  Procedure SearchItemByMessageIdRange(p_LowMessageId Integer, p_HightMessageId Integer, p_InstanceId Number, Items Out T_Cursor) Is Begin
    Open Items For Select * 
      From NotificationItem 
      Inner Join NotificationItemCategories
      On Id = ItemId
      Inner Join NotificationInstanceChannels
      On ChannelId = CategoryId
      Where MessageId >= p_LowMessageId And MessageId <= p_HightMessageId And InstanceId = p_InstanceId;

    
  End SearchItemByMessageIdRange;

  -- Ricerca degli item che devono ancora essere visualizzati da un utente relativamente ad una specifica categoria
  Procedure SearchItemsNotViewedByUser(p_UserId Integer, p_ChannelId Integer, p_PageSize Integer, p_PageNumber Integer, p_Count Out Integer, p_InstanceId Number, Item Out T_Cursor) Is Begin
    -- Calcolo degli indici minimo e massimo degli elementi da visualizzare
    Declare lowRowNum Integer;
    hightRowNum Integer;
    
    Begin
      hightRowNum := (p_PageNumber * p_PageSize);
      lowRowNum := (hightRowNum - p_PageSize) + 1;
      
      -- Calcolo dl numero di item totali
      Select count(*) Into p_Count
      From NotificationItem ni
      Inner Join NotificationItemCategories nic
      On ni.Id = nic.ItemId
      Inner Join NotificationUser nu
      On nu.ItemId = ni.Id
      Where nu.ViewDate Is Null And nic.CategoryId =  p_ChannelId And nu.UserId = p_UserId And nu.InstanceId = p_InstanceId;
      
      Open Item For   Select * 
                      From ( Select /*+ FIRST_ROWS(n) */ 
                      a.*, ROWNUM rnum 
                      From ( Select * 
                      From NotificationItem ni
                      Inner Join NotificationItemCategories nic
                      On ni.Id = nic.ItemId
                      Inner Join NotificationUser nu
                      On nu.ItemId = ni.Id
                      Where nu.ViewDate Is Null And nic.CategoryId = p_ChannelId And nu.UserId = p_UserId
                      And nu.InstanceId = p_InstanceId ) a 
                      Where ROWNUM <= hightRowNum )
                      Where rnum  >= lowRowNum;
    End;    
  End SearchItemsNotViewedByUser;
  
    -- Conteggio degli item che devono ancora essere visualizzati da un utente
  Procedure CountNotViewedItems(p_UserId Integer, p_InstanceId Integer, p_Count Out Integer) Is Begin
    Select count(*) Into p_Count
      From NotificationItem ni
      Inner Join NotificationItemCategories nic
      On ni.Id = nic.ItemId
      Inner Join NotificationUser nu
      On nu.ItemId = ni.Id
      Where nu.ViewDate Is Null And nu.UserId = p_UserId And nu.InstanceId = p_InstanceId;
  End CountNotViewedItems;
  
  -- Ricerca di item con filtri su data, id messaggio, testo contenuto nella notifica
  Procedure SearchItem(p_UserId Integer, p_SearchForMessageNumber Integer, p_LowMessageNumber Integer, p_HightMessageNumber Integer, 
    p_SearchForDate Number, p_LowDate Date, p_HightDate Date, 
    p_SearchForTitle Integer, p_ItemText VarChar2, p_InstanceId Integer,
    Items Out T_Cursor) Is Begin
    -- Query da eseguire per effettuare la ricerca 
    Declare queryToExecute VarChar2 (2000) := 'Select ni.* From NotificationItem ni Inner Join NotificationUser nu On ni.Id = nu.ItemId Where nu.UserId = ' || p_UserId || ' And nu.InstanceId = ' || p_InstanceId;
    
    Begin    
    -- Inserimento condizione sull'id del messaggio
    If p_SearchForMessageNumber = 1 Then
      queryToExecute := queryToExecute || ' And MessageNumber >= ' || p_LowMessageNumber;
      
      -- Inserimento condizione sul massimo id se specificato e maggiore del minimo
      If p_HightMessageNumber >= p_LowMessageNumber Then
        queryToExecute := queryToExecute || ' And MessageNumber <= ' || p_HightMessageNumber;
      End If;
      
    End If;
    
    -- Inserimento della condizione per data
    If p_SearchForDate = 1 Then
      queryToExecute := queryToExecute || ' And PublishDate >= to_date(''' || to_char(p_LowDate, 'dd/mm/yyyy') || ' 00:00:00'', ''dd/mm/yyyy HH24:mi:ss'')';
      
      -- Inserimento condizione sul massimo id se specificato e maggiore del minimo
      If p_HightDate >= p_LowDate Then
        queryToExecute := queryToExecute || ' And PublishDate <= to_date(''' || to_char(p_HightDate, 'dd/mm/yyyy') || ' 23:59:59'', ''dd/mm/yyyy HH24:mi:ss'')';
      End If;
      
    End If;
    
    -- Inserimento della condizione sul contenuto dell'item
    If p_SearchForTitle = 1 Then
      queryToExecute := queryToExecute || ' And upper(Title) Like upper(''%' || p_ItemText || '%'')';
    End If;
    
    Open Items For queryToExecute;
    
    End;
  End SearchItem;
  
End NC;

/
