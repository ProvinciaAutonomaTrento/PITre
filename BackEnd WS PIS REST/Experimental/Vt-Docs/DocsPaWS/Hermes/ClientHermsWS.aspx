<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientHermsWS.aspx.cs" Inherits="DocsPaWS.Hermes.ClientHermsWS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
<%--        <div>
        <asp:Label ID="l_Mandante" runat="server" Text="Mandante"></asp:Label><asp:TextBox ID="txt_Mandante" runat="server"></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_numerogara" Text="Numero Gara d'Appalto *" runat="server"></asp:Label><asp:TextBox ID="Txt_numerogara" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_OrganizzazioneAcquisti" Text="Organizzazione Acquisti" runat="server"></asp:Label><asp:TextBox ID="TextOrganizzazioneAcquisti" runat="server"></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_gruppoAquisti" Text="Gruppo Acquisti" runat="server"></asp:Label><asp:TextBox ID="TextgruppoAquisti" runat="server"></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_descrizionegara" Text="Descrizione della Gara" runat="server"></asp:Label>
            <asp:TextBox ID="Textdescrizionegara" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_dataScadenzagara" Text="Data scadenza gara" runat="server"></asp:Label><asp:TextBox ID="TextdataScadenzagara" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_dataemissionegara" Text="Data emissione Gara" runat="server"></asp:Label><asp:TextBox ID="Textdataemissionegara" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="l_DataAperturabustetecniche" Text="Data apertura buste tecniche" runat="server"></asp:Label><asp:TextBox ID="TextDataAperturabustetecniche" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_dataricezionerelazionetecnica" Text="data ricezione relazione tecnica" runat="server"></asp:Label><asp:TextBox ID="Textdataricezionerelazionetecnica" runat="server"></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_dataaperturabusteeconomiche" Text="data apertura buste economiche" runat="server"></asp:Label><asp:TextBox ID="Textdataaperturabusteeconomiche" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_datapropostaaggiudicazionetrattativa" Text="Data proposta aggiudicazione trattativa" runat="server"></asp:Label><asp:TextBox ID="Textdatapropostaaggiudicazionetrattativa" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_dataaggiudicazione" Text="Data aggiudicazione" runat="server"></asp:Label><asp:TextBox ID="Textdataaggiudicazione" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="L_dataannullamnetogara" Text="Data annullamento gara" runat="server"></asp:Label><asp:TextBox ID="Textdataannullamnetogara" runat="server" ></asp:TextBox>
        </div>
        <div>
        <asp:Label ID="l_valoregara" Text="Valore Gara" runat="server"></asp:Label><asp:TextBox ID="Textvaloregara" runat="server" ></asp:TextBox>
        </div>--%>
        <br />
         <div>
        <asp:Label ID="Label1" Text="User" runat="server"></asp:Label><asp:TextBox ID="TextUser" runat="server" Text="pr41683"></asp:TextBox>
        </div>
         <div>
        <asp:Label ID="Label2" Text="Password" runat="server"></asp:Label><asp:TextBox ID="TextPassword" runat="server" Text="password1"></asp:TextBox>
        </div>
         <div>
        <asp:Label ID="Label3" Text="Tipologia" runat="server"></asp:Label><asp:TextBox ID="TextTipologia" runat="server" Text="Hermes Bandi"></asp:TextBox>
        </div>
         <div>
        <asp:Label ID="Label4" Text="Nodo Titolario" runat="server"></asp:Label><asp:TextBox ID="TextNodo" runat="server" Text="1.1"></asp:TextBox>
        </div>
    </div>
    <div>
        <asp:Button  ID="invio" runat="server" Text="inserisci dati" 
            onclick="invio_Click"/>
    </div>

    <div>
    <asp:Label ID="risultato" runat="server" Text="Risposta: "></asp:Label> <asp:Label ID="l_risposta" runat="server" ></asp:Label>
    </div>
    </form>
</body>
</html>
