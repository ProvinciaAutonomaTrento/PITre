VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "FilenetFullText"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True


Public Function RicercaFullText(ByVal testo As String, ByVal LibraryName As String, _
    ByVal UserID As String, ByVal Pwd As String) As String()
    Dim conn As ADODB.Connection
    Dim rec As ADODB.Recordset
    Dim ConnectionString As String
    Dim sUserID As String
    Dim sPwd As String
    Dim Sql As String
    Dim retVal() As String
    
    Set conn = New ADODB.Connection
    Set rec = New ADODB.Recordset
    
    If UserID = "" Then
        sUserID = "Admin"
    Else
        sUserID = UserID
    End If
    
    If Pwd <> "" Then swd = Pwd
        
    ConnectionString = "provider=FnDBProvider;data source=" & _
        LibraryName & ";user id=" & sUserID & ";password=" & sPwd & ";SystemType=2;Prompt=4;"
    
    conn.ConnectionString = ConnectionString
    conn.Open
    
    rec.ActiveConnection = conn
    
    Sql = "SELECT idmId,idmName,idmDocType, idmDateAdded,idmAddedByUser FROM FnDocument " & _
            "CONTENTS HAS PHRASE = " & "'" & testo & "'" & " ORDER BY idmId"
    
    rec.Open Sql
    
    If rec.EOF Then GoTo Fine
               
    ReDim retVal(rec.RecordCount - 1) As String
    Dim i
    
    i = 0
    Do While Not rec.EOF
       retVal(i) = rec!idmId
       i = i + 1
       rec.MoveNext
    Loop
    
    RicercaFullText = retVal
    
Fine:
    rec.Close
    conn.Close
        
End Function



