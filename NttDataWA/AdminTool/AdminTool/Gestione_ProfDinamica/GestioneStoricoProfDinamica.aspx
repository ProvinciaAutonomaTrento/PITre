<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneStoricoProfDinamica.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamica.GestioneStoricoProfDinamica" %>

<%@ Register src="../../UserControls/AjaxMessageBox.ascx" tagname="AjaxMessageBox" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Abilitazione storico campi</title>
        <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <script type="text/javascript" language="javascript">

        // Evento scatenato al click sul checkbox 'Seleziona tutti'
        // Seleziona o deseleziona tutti i checkbox della pagina
        function chkSelDeselAll_onClick(chkSelDeselAll) {
            // Prelevamento di tutti gli elementi input presenti nella pagina
            var inputElems = document.getElementsByTagName('input');

            // Ricerca e selezione / deselezione di tutti gli input di tipo 'checkbox'
            for (i = 0; i < inputElems.length; i++) {
                if (inputElems[i].type == 'checkbox')
                    inputElems[i].checked = chkSelDeselAll.checked;
            }
        }

        // Evento scatenato al click su una checkbox di selezione singola
        // Se a seguito del click sul flag che ha scatenato l'evento, tutti i
        // flag sono checked, viene flaggato anche 'Seleziona tutti' altrimenti
        // quest'ultimo viene deflaggato
        function chkEnabled_onClick(chkEnabled) {
            // Recupero di tutti gli elementi di tipo input
            var inputElems = document.getElementsByTagName('input');

            // Flag utilizzato per indicare se bisogna flaggare il seleziona tutti
            var exit = false;
            for (i = 0; i < inputElems.length; i++) {
                // Se l'input in posizione X è una checkbox, non checked e non è chkSelectDeselectAll,
                // significa che non tutte le checkbox sono checked
                if (inputElems[i].type === 'checkbox' && !inputElems[i].checked && inputElems[i].name != 'chkSelectDeselectAll')
                    exit = true;
            }

            if (!exit)
                document.getElementById("chkSelectDeselectAll").checked = 'checked';
            else
                document.getElementById("chkSelectDeselectAll").checked = '';

        }

    </script>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
    <div>
        <asp:CheckBox ID="chkSelectDeselectAll" CssClass="testo_grigio_scuro" runat="server" onclick="chkSelDeselAll_onClick(this);" Text="Seleziona tutti" />
    </div>
    <div align="center" >
        <asp:DataGrid ID="dgFields" runat="server" AllowCustomPaging="false" Width="100%" AutoGenerateColumns="false">
         <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
			        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
					<ItemStyle CssClass="bg_grigioN"></ItemStyle>
					<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
            <Columns>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:HiddenField ID="hfObjectId" runat="server" Value="<%# ((SAAdminTool.DocsPaWR.CustomObjHistoryState)Container.DataItem).FieldId %>"  />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="Description" />
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkEnabled" runat="server" Checked="<%# ((SAAdminTool.DocsPaWR.CustomObjHistoryState)Container.DataItem).Enabled %>" onclick="chkEnabled_onClick(this);" />
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
    <div style="text-align:center" >
        <asp:UpdatePanel ID="pblButtons" runat="server">
            <ContentTemplate>
                <asp:Button Text="Conferma" ID="btnConfirm" ToolTip="Conferma" CssClass="testo_btn_p" runat="server" 
                    onclick="btnConfirm_Click" />&nbsp;
                <input type="button"   title="Annulla" value="Annulla" class="testo_btn_p" onclick="window.close();" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <uc1:AjaxMessageBox ID="AjaxMessageBox" runat="server" />
    </form>
</body>
</html>
