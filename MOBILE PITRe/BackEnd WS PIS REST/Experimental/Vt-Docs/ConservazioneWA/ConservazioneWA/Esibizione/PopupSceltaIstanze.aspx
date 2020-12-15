<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PopupSceltaIstanze.aspx.cs" Inherits="ConservazioneWA.Esibizione.PopupSceltaIstanze" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head" runat="server">
    <title>Seleziona un'istanza</title>
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script type="text/javascript" language="javascript">
        function close_and_save(ret) {
            window.returnValue = ret;
            window.close();
        }

        function findSelection() {
            
            var radios = document.getElementsByName('rbl_pref');
            
            for (var i = 0, length = radios.length; i < length; i++) {
                if (radios[i].checked) {
                    // do whatever you want with the checked radio
                    //alert(radios[i].value);

                    // only one radio can be logically checked, don't check the rest
                    break;
                }
            }
        }
    </script>
    <style type="text/css">
        body
        {
            font-family: Verdana;
        }
        #container
        {
            float: left;
            width: 100%;
            height: 360px;
        }
        #content
        {
            border: 1px solid #cccccc;
            text-align: center;
            width: 98%;
            float: left;
            margin-left: 6px;
            margin-top: 5px;
        }
        .title
        {
            margin: 0px;
            padding: 0px;
            font-size: 11px;
            font-weight: bold;
            text-align: center;
            width: 100%;
            float: left;
            padding-top: 4px;
            padding-bottom: 4px;
            background-color: #19475e;
            color: #ffffff;
        }
        #content_field
        {
            width: 100%;
            float: left;
            background-color: #fbfbfb;
            height: 305px;
            padding-top: 10px;
            overflow-y: scroll;
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 10px;
        }
        .contenitore_box fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 0px;
        }
        .contenitore_box legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
        }
        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 10px;
        }
        .contenitore_box_due fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 5px;
        }
        .contenitore_box_due legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 5px;
        }

        .titolo_scheda
        {
            font-size: 11px;
            color: #666666;
            font-weight: bold;
        }
        .testo_grigio
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
        }
        .tab_format
        {
            border-collapse: collapse;
            margin: 0px;
            padding: 0px;
            width: 95%;
        }
        .tab_format td
        {
            margin: 0px;
            padding: 0px;
            padding: 2px;
            font-size: 10px;
            border: 1px solid #cccccc;
        }
        .head_tab
        {
            background-color: #19475e;
            color: #ffffff;
            font-size: 12px;
            text-align: center;
        }
        .tab_sx
        {
            color: #333333;
            text-align: left;
        }
        .tab_sx2
        {
            color: #333333;
            text-align: center;
        }
        #button
        {
            margin-top: 5px;
        }
        
                .cbtn
        {
            background-image: url('../Img/bg_button.jpg');
        }
        
        .cbtnHover
        {
            background-image: url('../Img/bg_button_hover.jpg');
        }
        
        .tab_istanze_header
        {
            background-image: url('../Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }
        
        #content
        {
            background-image: url('../Img/bg_content.jpg');
        }
        
        .menu_pager_grigio
        {
            background-image: url('../Img/bg_pager_table.jpg');
            background-repeat: repeat-x;
        }
    </style>
</head>
<body>
    <form id="GridSave" method="post" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server" />
    <%--<asp:HiddenField ID = "hf_rb_ist_selected" runat="server" />--%>
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <div id="container">
        <div id="content">
            <asp:Label runat="server" ID="titlePage" Text="Seleziona un'istanza"
                CssClass="title"></asp:Label>
            <div id="content_field">
                <div align="center">
                    <%--<asp:DataGrid ID="grvFileType" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                        AllowPaging="False" CssClass="tab_format" OnItemDataBound="CheckRadio"
                        >--%>
                        <asp:DataGrid ID="grvFileType" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                        AllowPaging="False" CssClass="tab_format"
                        >
                        <AlternatingItemStyle BackColor="#fefefe" />
                        <ItemStyle BackColor="#f0f0f0" ForeColor="#333333" />
                        <Columns>
                            <asp:TemplateColumn ItemStyle-Width="8%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab">
                                <%--<ItemTemplate>
                                    <input id="rb_ist" name="rbl_pref" type="radio" class="testo_grigio_scuro" 
                                    value='<%# this.GetIDIstanza((ConservazioneWA.WSConservazioneLocale.InfoConservazione)Container.DataItem) %>' 
                                    onclick="findSelection()"
                                    />--%>
                                    <ItemTemplate>
                                    <input name="rbl_pref" type="radio" class="testo_grigio_scuro" 
                                    value='<%# this.GetIDIstanza((ConservazioneWA.WSConservazioneLocale.InfoConservazione)Container.DataItem) %>' 
                                    />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="true" HeaderText="ID ISTANZA" ItemStyle-CssClass="tab_sx2"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="lblSystemId" runat="server" Text='<%# this.GetIDIstanza((ConservazioneWA.WSConservazioneLocale.InfoConservazione)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="30%" HeaderText="DESCRIZIONE" ItemStyle-CssClass="tab_sx2"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="lblDescrzione" runat="server" Text='<%# this.GetDescrizioneIstanza((ConservazioneWA.WSConservazioneLocale.InfoConservazione)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="20%" HeaderText="DATA APERTURA" ItemStyle-CssClass="tab_sx"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="lblDataApertura" runat="server" Text='<%# this.GetDataAperturaIstanza((ConservazioneWA.WSConservazioneLocale.InfoConservazione)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="20%" HeaderText="DATA INVIO" ItemStyle-CssClass="tab_sx"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="lblDataInvio" runat="server" Text='<%# this.GetDataInvioIstanza((ConservazioneWA.WSConservazioneLocale.InfoConservazione)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn ItemStyle-Width="20%" HeaderText="DATA CHIUSURA" ItemStyle-CssClass="tab_sx"
                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                <ItemTemplate>
                                    <asp:Label ID="lblDataChiusura" runat="server" Text='<%# this.GetDataConservazioneIstanza((ConservazioneWA.WSConservazioneLocale.InfoConservazione)Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
            </div>
            <asp:UpdatePanel ID="UpdatePanelConfirm" runat="server" UpdateMode="Conditional">
                <contenttemplate>
                <table id="Table3" align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_salva" runat="server" CssClass="cbtn" Text="Seleziona" OnClick="BtnSaveDocumentFormat_Click"></asp:Button>
                        </td>
                        <td>
                            <asp:Button ID="btn_annulla" runat="server" CssClass="cbtn" Text="Chiudi"
                                OnClientClick="window.close();"></asp:Button>
                        </td>
                    </tr>
                </table>
                   </contenttemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    </form>
</body>
</html>
