<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="GridPersonalization.aspx.cs" Inherits="NttDataWA.Popup.GridPersonalization" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container
        {
            width: 98%;
            margin: 0 auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <fieldset class="basic">
            <div class="riga">
                <div class="colonna">
                    <asp:Label runat="server" ID="lblOrdina" CssClass="margin_top" />
                    <asp:DropDownList ID="ddlOrder" CssClass="chzn-select-deselect" CssClassReadOnly="txt_addressBookLeft_disabled"
                        runat="server" DataTextField="Text" DataValueField="Id" AppendDataBoundItems="true"
                        Width="70%" OnSelectedIndexChanged="ChangeFieldOrder" AutoPostBack="true">
                        <asp:ListItem />
                    </asp:DropDownList>
                </div>
                <div class="colonna2">
                    <asp:DropDownList ID="ddlAscDesc" runat="server" CssClass="chzn-select-deselect"
                        CssClassReadOnly="txt_addressBookLeft_disabled" OnSelectedIndexChanged="ChangeAscDescOrder"
                        AutoPostBack="true" Width="100%">
                        <asp:ListItem Text="Crescente" Value="Asc" />
                        <asp:ListItem Text="Descrescente" Value="Desc" />
                    </asp:DropDownList>
                </div>
            </div>
        </fieldset>
        <fieldset class="basic">
            <asp:UpdatePanel runat="server" ID="UpPnlDettaglio" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="riga">
                        <p class="weight">
                            <asp:Label ID="lblTitle" runat="server"></asp:Label></p>
                        <asp:HiddenField ID="hfFieldId" runat="server" />
                        <div class="colonna2dx">
                            <asp:Label ID="lblEtichetta" runat="server" Enabled="false"></asp:Label>
                            <cc1:CustomTextArea ID="txtLabel" runat="server" CssClass="txt_Left" CssClassReadOnly="txt_Left_disabled"
                                Enabled="false"></cc1:CustomTextArea>
                        </div>
                        <div class="colonna2dx">
                            <asp:Label ID="lblLarghezza" runat="server" Enabled="false"></asp:Label>
                            <asp:DropDownList ID="ddlWidth" runat="server" Enabled="false" AutoPostBack="true" OnSelectedIndexChanged="ddlWidth_SelectedIndexChanged"
                                CssClass="chzn-select-deselect" CssClassReadOnly="txt_addressBookLeft_disabled"
                                Width="200">
                                 <asp:ListItem>20</asp:ListItem>
                                <asp:ListItem>50</asp:ListItem>
                                <asp:ListItem Selected="True">100</asp:ListItem>
                                <asp:ListItem>200</asp:ListItem>
                                <asp:ListItem>300</asp:ListItem>
                                <asp:ListItem>400</asp:ListItem>
                                <asp:ListItem>600</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </fieldset>
        <fieldset class="basic">
            <div id="contenuto">
                <div id="row nowrap">
                    <div class="riga">
                        <div class="colonnaImgSx">
                        </div>
                        <div class="colonna2dx">
                            <p class="weight">
                                <asp:Label ID="Lblfieldview" runat="server"></asp:Label></p>
                        </div>
                        <div class="colonna2dx">
                            <p class="weight">
                                <asp:Label ID="lbl_type_template" runat="server"></asp:Label></p>
                        </div>
                    </div>
                    <div class="riga">
                        <div class="colonnaImgSx">
                            <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="col">
                                        <cc1:CustomImageButton ID="btn_up_field" runat="server" OnClick="UpField" ImageUrl="~/Images/Icons/up_arrow2.png"
                                            OnMouseOverImage="../Images/Icons/up_arrow2_hover.png" OnMouseOutImage="../Images/Icons/up_arrow2.png"
                                            ImageUrlDisabled="../Images/Icons/up_arrow2_disabled.png" CssClass="clickableRight"
                                            Enabled="false" Width="30" />
                                    </div>
                                    <cc1:CustomImageButton ID="btn_down_field" ImageUrl="../Images/Icons/down_arrow2.png"
                                        runat="server" OnMouseOverImage="../Images/Icons/down_arrow2_hover.png" OnMouseOutImage="../Images/Icons/down_arrow2.png"
                                        ImageUrlDisabled="../Images/Icons/down_arrow2_disabled.png" CssClass="clickableRight"
                                        Enabled="false" OnClick="DownField" Width="30" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="colonna2dx">
                            <asp:UpdatePanel ID="UpnlGrigliaPersonalizzata" runat="server" 
                                UpdateMode="Conditional" ClientIDMode="Static">
                                <ContentTemplate>
                                    <asp:HiddenField ID="selectedFieldPosition" runat="server" />
                                    <asp:GridView runat="server" ID="GridPersonalizzata" AllowPaging="false" AutoGenerateColumns="False"
                                        HorizontalAlign="Center" HeaderStyle-CssClass="tableHeaderPopup" CssClass="tabPopup tbl"
                                        Width="100%" Visible="true" OnSelectedIndexChanging="GridPersonalizzata_SelectedIndexChanging"
                                        OnPageIndexChanging="GridPersonalizzata_PageIndexChanging" OnRowCreated="GridPersonalizzata_RowCreated"
                                        OnRowDataBound="GridPersonalizzata_RowDataBound">
                                        <SelectedRowStyle CssClass="selectedrow" />
                                        <SelectedRowStyle BackColor="Yellow" />
                                        <HeaderStyle CssClass="tableHeaderPopup" />
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="cb_selectall" runat="server" onclick="javascript:HeaderClick(this);" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="checkDocumento" runat="server" Checked='<%# this.GetChecked((NttDataWA.DocsPaWR.Field)Container.DataItem) %>'
                                                        Enabled='<%# this.GetEnable((NttDataWA.DocsPaWR.Field)Container.DataItem) %>' />
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Text='<%# GetText((NttDataWA.DocsPaWR.Field)Container.DataItem) %>'
                                                        ToolTip='<%# this.GetText((NttDataWA.DocsPaWR.Field)Container.DataItem) %>' ID="linkField"></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle Wrap="False" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetFieldID((NttDataWA.DocsPaWR.Field)Container.DataItem) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="colonna2dx">
                            <asp:UpdatePanel ID="UpnlGrigliaTemplate" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gridTemplates" runat="server" AllowPaging="false" AutoGenerateColumns="False"
                                        HorizontalAlign="Center" HeaderStyle-CssClass="tableHeaderPopup" CssClass="tabPopup tbl"
                                        Width="100%" Visible="true" CellPadding="0" CellSpacing="0" OnSelectedIndexChanging="gridTemplates_SelectedIndexChanging"
                                        OnRowDataBound="gridTemplates_RowDataBound">
                                        <SelectedRowStyle CssClass="selectedrow" />
                                        <SelectedRowStyle BackColor="Yellow" />
                                        <HeaderStyle CssClass="tableHeaderPopup" />
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <Columns>
                                            <asp:TemplateField ItemStyle-Width="5%" ItemStyle-HorizontalAlign="center" HeaderStyle-Height="15">
                                                <ItemTemplate>
                                                    <div align="center">
                                                        <asp:CheckBox ID="chkSelectedTemplate" runat="server" Checked='<%# this.GetTemplateVisible((NttDataWA.DocsPaWR.Templates)Container.DataItem) %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="95%" ItemStyle-CssClass="link_field" ItemStyle-HorizontalAlign="left"
                                                HeaderStyle-Height="15">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%# this.GetNameTemplate((NttDataWA.DocsPaWR.Templates)Container.DataItem) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="SYSTEM_ID_TEMPLATE" runat="server" Text='<%# this.GetTemplateID((NttDataWA.DocsPaWR.Templates)Container.DataItem) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:HiddenField ID="gridTemplate_index" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="GridPersonalizationBtnInserisci" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2')"
                OnClick="GridPersonalizationBtnInserisci_Click" />
            <cc1:CustomButton ID="GridPersonalizationBtnClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="GridPersonalizationBtnClose_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });

        function HeaderClick(CheckBox) {

            var TargetBaseControl = document.getElementById('<%= this.GridPersonalizzata.ClientID %>');
            var TargetChildControl = "checkDocumento";
            var Inputs = TargetBaseControl.getElementsByTagName("input");

            for (var n = 0; n < Inputs.length; ++n)
                if (Inputs[n].type == 'checkbox' &&
                Inputs[n].id.indexOf(TargetChildControl, 0) >= 0)
                    Inputs[n].checked = CheckBox.checked;

        }

        function ChildClick(CheckBox, HCheckBox) {
            var HeaderCheckBox = document.getElementById(HCheckBox);

            var TargetBaseControl = document.getElementById('<%= this.GridPersonalizzata.ClientID %>');
            var TargetChildControl = "checkDocumento";
            var Inputs = TargetBaseControl.getElementsByTagName("input");
            HeaderCheckBox.checked = true;

            //verifica se almeno 1 non è checked
            for (var n = 0; n < Inputs.length; ++n)
                if (Inputs[n].type == 'checkbox' &&
                Inputs[n].id.indexOf(TargetChildControl, 0) >= 0)
                    if (Inputs[n].checked == false) {
                        HeaderCheckBox.checked = false;
                        break;
                    }
        }

    </script>
</asp:Content>
