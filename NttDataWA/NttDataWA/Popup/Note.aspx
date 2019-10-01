<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="Note.aspx.cs" Inherits="NttDataWA.Popup.Note" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container {width: 95%; margin: 0 auto;}
    </style>
    <script type="text/javascript">
        function noteSelected(sender, e) {
            var value = e.get_value();
            if (!value) {
                if (e._item.parentElement && e._item.parentElement.tagName == "LI")
                    value = e._item.parentElement.attributes["_value"].value;
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")
                    value = e._item.parentElement.parentElement.attributes["_value"].value;
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI")
                    value = e._item.parentNode._value;
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")
                    value = e._item.parentNode.parentNode._value;
                else value = "";
            }

            var searchText = $get('<%=TxtNote.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceDescrizione = testo.lastIndexOf('[');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            document.getElementById("<%=this.TxtNote.ClientID%>").focus();
            document.getElementById("<%=TxtNote.ClientID%>").value = descrizione;
            document.getElementById("<%=txtNoteAutoComplete.ClientID %>").value = '';

            var tmp = document.getElementById("<%=TxtNote_chars.ClientID %>").getAttribute("rel");
            charsLeft(tmp.split('_')[0], tmp.split('_')[1], tmp.split('_')[2]);

            var codiceRF = testo.substring(testo.lastIndexOf(" ["));
            codiceRF = codiceRF.substring(2, codiceRF.lastIndexOf("]"))
            if (codiceRF == "TUTTI") {
                $('ul.RblTypeNote input')[$('ul.RblTypeNote input').length - 1].checked = true;
                __doPostBack('<%=this.rblTipiVisibilita.ClientID%>', '');
            }
            else {
                $('ul.rblTipiVisibilita input')[$('ul.rblTipiVisibilita input').length - 2].checked = true;
            }
        }
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <div class="row">
        <p align="center"><strong><asp:Literal ID="litTitle" runat="server" /></strong></p>
    </div>

    <asp:UpdatePanel ID="UpPnlGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row">
                 <p align="center"><asp:Literal ID="litMessage" runat="server" /></p>
            </div>

            <asp:GridView ID="grdNote" runat="server" Width="98%" AutoGenerateColumns="False" ClientIDMode="Static" CssClass="tbl"
                OnRowDataBound="grdNote_RowDataBound"
            >
                <RowStyle CssClass="NormalRow clickable" />
                <SelectedRowStyle CssClass="selectedrow clickable" />
                <AlternatingRowStyle CssClass="AltRow clickable" />
                <Columns>
                    <asp:BoundField DataField="Id" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden" />
                    <asp:BoundField DataField="Testo" ItemStyle-Width="45%" />
                    <asp:TemplateField ItemStyle-Width="26%">
                        <ItemTemplate>
                            <asp:Label ID="lblUtente" runat="server" Text='<%# this.GetDescrizioneUtenteCreatore((NttDataWA.DocsPaWR.InfoNota) Container.DataItem) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="DataCreazione" DataFormatString="{0:d}" ItemStyle-Width="12%" />
                    <asp:BoundField DataField="TipoVisibilita" ItemStyle-Width="12%" />
                    <asp:TemplateField ItemStyle-Width="5%">
                        <ItemTemplate>
                            <cc1:CustomImageButton runat="server" ID="btnDelete" ImageUrl="../Images/Icons/delete.png"
                                ImageAlign = "Middle" OnClientClick='<%# "$(\"#grid_rowindex\").val(\""+Container.DataItemIndex.ToString()+"\"); return confirm(\""+this.GetMessage("ConfirmNoteDelete")+"\");" %>'
                                OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/deletes_disabled.png" OnClick="btnDelete_Click"
                                Visible = '<%# this.CanDeleteNota((NttDataWA.DocsPaWR.InfoNota) Container.DataItem) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />

            <div class="row">
                <div class="col">
                    <asp:RadioButtonList ID="rblTipiVisibilita" runat="server" AutoPostBack="true" RepeatLayout="UnorderedList" CssClass="rblTipiVisibilita rblHorizontal" OnSelectedIndexChanged="rblTipiVisibilita_SelectedIndexChanged">
                        <asp:ListItem id="optPersonal" Value="Personale" />
                        <asp:ListItem id="optRole" Value="Ruolo" />
                        <asp:ListItem id="optRF" Value="RF" />
                        <asp:ListItem id="optAll" Value="Tutti" Selected="True" />
                    </asp:RadioButtonList>
                </div>
                <div class="col-right-no-margin-alLeft">
                    <asp:DropDownList Visible="false" ID="ddlNoteRF" runat="server" CssClass="chzn-select-deselect"
                        Width="150px" AutoPostBack="true" OnSelectedIndexChanged="ddlNoteRF_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <cc1:CustomTextArea ID="txtNoteAutoComplete" Visible="false" Width="100%" runat="server"
                    CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" />
                <asp:HiddenField ID="isTutti" runat="server" Value="" />
                 <uc1:AutoCompleteExtender runat="server" ID="autoComplete1" TargetControlID="txtNoteAutoComplete"
                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaNote"
                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                    OnClientItemSelected="noteSelected">
                </uc1:AutoCompleteExtender>
            </div>
            <div class="row">
                <cc1:CustomTextArea ID="TxtNote" runat="server" TextMode="MultiLine" ClientIDMode="static" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"></cc1:CustomTextArea>
                <span class="col-right"><asp:Literal ID="DocumentLitVisibleNotesChars" runat="server" />: <span id="TxtNote_chars" runat="server" clientidmode="Static"></span></span>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:Button ID="BtnSelect" runat="server" ClientIDMode="Static" CssClass="hidden" OnClick="BtnSelect_Click" />

    <asp:UpdatePanel ID="upPnlButtons" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnNew" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="BtnNew_Click" />
            <cc1:CustomButton ID="BtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="BtnSave_Click" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" 
                onclick="BtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
