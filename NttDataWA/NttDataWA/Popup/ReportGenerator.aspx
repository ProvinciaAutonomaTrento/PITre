<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="ReportGenerator.aspx.cs" Inherits="NttDataWA.Popup.ReportGenerator" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
        .watermark
        {
            border: 1px solid #cccccc;
            line-height: 18px;
            font-size: 13px;
            height: 18px;
            color: #333333;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
            font-style: italic;
            width: 87%;
        }
    </style>

</asp:Content>

<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="containerReportGenerator" style="margin: 0; padding:10px">
         <div class="riga" style="margin:0; padding-top:10px">
            <div class="colonnasx4">
                <asp:Label id="lblReportGenerator" runat="server"></asp:Label>
            </div>
            <div class="colonnadx4">
                <asp:DropDownList ID="DdlReportGenerator" runat="server" OnSelectedIndexChanged="ddlReport_SelectedIndexChanged" Width="300"
                 CssClass="chzn-select-deselect" AutoPostBack="true" DataTextField="ReportName" DataValueField="ReportKey">
                 </asp:DropDownList>
            </div>
        </div>
        <div class="riga" style="margin:0; padding-top:10px">
            <div class="colonnasx4">
                <asp:Label id="lblTitle" runat="server"></asp:Label>
            </div>
            <div class="colonnadx4">
                <cc1:CustomTextArea ID="txtReportTitle" runat="server" Width="450" CssClass="txt_objectLeft" AutoCompleteType="Disabled"></cc1:CustomTextArea>
                <cc2:TextBoxWatermarkExtender ID="tbwTitle" runat="server" WatermarkCssClass="watermark"
                    TargetControlID="txtReportTitle" WatermarkText="Titolo report">
                </cc2:TextBoxWatermarkExtender>
            </div>
        </div>
        <div class="riga" style="margin:0; padding-top:10px">
            <div class="colonnasx4">
                <asp:Label id="lblReportSubtitle" runat="server"></asp:Label>
            </div>
            <div class="colonnadx4">
                <cc1:CustomTextArea ID="txtReportSubtitle" runat="server" Width="450" CssClass="txt_objectLeft" AutoCompleteType="Disabled"></cc1:CustomTextArea>
                <cc2:TextBoxWatermarkExtender ID="tbwSubtitle" runat="server" WatermarkCssClass="watermark"
                    TargetControlID="txtReportSubtitle">
                </cc2:TextBoxWatermarkExtender>
            </div>
        </div>
         <div class="riga" style="margin:0; padding-top:10px">
            <div class="colonnasx4">
                <asp:Label id="lblExportFormat" runat="server"></asp:Label>
            </div>
            <div class="colonnadx4">
                <asp:DropDownList ID="ddlExportFormat" runat="server" CssClass="chzn-select-deselect" Width="200">
                    <asp:ListItem text="PDF" />
                    <asp:ListItem Text="Excel" />
                    <asp:ListItem Text="Open Office Calc" Value="ODS" />
            </asp:DropDownList>
            </div>
        </div>
        <div class="riga" style="margin:0; padding-top:30px">
            <div id="grdMail">
                <asp:Panel runat="server" ID="pnlFieldsSelection" Visible="false">
                    <div id="filterExport"  style="margin:0; padding-bottom:5px">
                        <span class="weight">
                            <asp:Label id="lblFilterExport" runat="server"></asp:Label>
                        </span>
                      </div>
                  <asp:GridView ID="GrdFields" runat="server" Width="100%" AutoGenerateColumns="false"
                              CssClass="tbl_rounded round_onlyextreme" BorderWidth="0"
                               style="cursor:pointer;">
                               <Columns>
                                   <asp:TemplateField HeaderText='<%$ localizeByText:ReportGeneratorFieldName%>'>
                                        <ItemTemplate>
                                            <asp:Literal ID="ltlFieldName" runat="server" Text="<%# ((NttDataWA.DocsPaWR.HeaderProperty)Container.DataItem).ColumnName %>" />
                                        </ItemTemplate>
                                   </asp:TemplateField>
                                   <asp:TemplateField HeaderText='<%$ localizeByText:ReportGeneratorExport%>'>
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfFieldName" runat="server" Value="<%# ((NttDataWA.DocsPaWR.HeaderProperty)Container.DataItem).OriginalName %>" />
                                            <asp:CheckBox OnCheckedChanged="chkSelected_CheckedChanged" ID="chkSelected" runat="server"
                                                Checked="<%# ((NttDataWA.DocsPaWR.HeaderProperty)Container.DataItem).Export %>" />
                                        </ItemTemplate>
                                   </asp:TemplateField>
                               </Columns>
                  </asp:GridView>
                  </asp:Panel>
              </div>
        </div>
        <asp:UpdatePanel ID="upPanelFrame" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <iframe runat="server" clientidmode="Static" id="reportContent" style="width: 0px; height: 0px;" frameborder="0" marginheight="0" marginwidth="0"/>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>


<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
<asp:UpdatePanel ID="UpdatePanelButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
    <ContentTemplate>
            <cc1:CustomButton ID="ReportGeneratorExport" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick=" return __doPostBack('UpdatePanelButtons');" OnClick="ReportGeneratorExport_Click" />
            <cc1:CustomButton ID="ReportGeneratorClose" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick=" return __doPostBack('UpdatePanelButtons');" OnClick="ReportGeneratorClose_Click" />
            </ContentTemplate>
</asp:UpdatePanel>

      <script type="text/javascript">          $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
          }); $(".chzn-select").chosen({
              no_results_text: "Nessun risultato trovato"
          }); </script>
</asp:Content>

