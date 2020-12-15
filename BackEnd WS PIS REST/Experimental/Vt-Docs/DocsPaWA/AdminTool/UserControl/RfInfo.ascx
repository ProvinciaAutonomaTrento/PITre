<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RfInfo.ascx.cs" Inherits="DocsPAWA.AdminTool.UserControl.RfInfo" %>
<div style="width: 99%;">
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            Indirizzo:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtIndirizzo" runat="server" CssClass="testo" Width="92%" />
        </div>
    </div>
    <div style="clear: both;" />
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            Città:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtCitta" runat="server" CssClass="testo" Width="92%" />
        </div>
    </div>
    <div style="clear: both;" />
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            &nbsp;Provincia:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtProvincia" runat="server" CssClass="testo" Width="92%" />
        </div>
    </div>
    <div style="clear: both;" />
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            &nbsp;CAP:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtCap" runat="server" CssClass="testo" Width="92%" />
        </div>
    </div>
    <div style="clear: both;" />
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            &nbsp;Nazione:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtNazione" runat="server" CssClass="testo" Width="92%" />
        </div>
    </div>
    <div style="clear: both;" />
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            &nbsp;Telefono:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtTelefono" runat="server" CssClass="testo" Width="92%" />
        </div>
    </div>
    <div style="clear: both;" />
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            &nbsp;Fax:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtFax" runat="server" CssClass="testo" Width="92%" />
        </div>
    </div>
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            &nbsp;Codice Fiscale:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtCodiceFiscale" runat="server" CssClass="testo" Width="92%" MaxLength="16" />
        </div>
    </div>
    <div style="height: 20px;">
        <div class="testo_grigio_scuro" style="float: left; width: 25%;">
            &nbsp;Partita Iva:&nbsp;
        </div>
        <div style="float: left; width: 74%; margin-left: 2px;">
            <asp:TextBox ID="txtPartitaIva" runat="server" CssClass="testo" Width="92%" MaxLength="11"/>
        </div>
    </div>
</div>
