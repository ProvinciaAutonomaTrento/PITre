<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="ClassificationScheme.aspx.cs" Inherits="NttDataWA.Popup.ClassificationScheme" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="ctw" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .tbl td {background: #fff; cursor: pointer;}
        .tbl tr.selectedrow td {background-color: #FCD85C;}
        .tbl tr.AltRow td {background-color: #e1e9f0;}
        .tbl tr.NormalRow:hover td, .tbl tr.AltRow:hover td {background-color: #b2d7f8;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">

    <div class="contenuto">
        <div class="contenutoPopUpSX">
       
            <ctw:CustomTreeView ID="TreeTitolario" OnTreeNodeExpanded="ExpandeTreeView" runat="server"
                OnSelectedNodeChanged="TreeTitolario_SelectedNodeChanged">
                <SelectedNodeStyle CssClass="treeViewSelected" />
                        <SelectedNodeStyle BackColor="#FCD85C" />
            </ctw:CustomTreeView>
        </div>
        <div class="contenutoPopUpDX">
        
            <div class="riga">
                <div class="colonnasx">
                    <asp:Label runat="server" ID="ClassificationSchemelabelRegistro" Visible="false"></asp:Label>
                </div>
                <div class="colonnadx">
                    <asp:DropDownList runat="server" ID="ddlRegistri" Visible="false" Width="300"
                        AutoPostBack="true" onselectedindexchanged="ddlRegistri_SelectedIndexChanged" CssClass="chzn-select-deselect" />
                </div>
            </div>
            <div class="riga">
                <div class="colonnasx">
                    <asp:Label runat="server" ID="ClassificationSchemelabelCodice"></asp:Label>
                </div>
                <div class="colonnadx">
                    <cc1:CustomTextArea ID="ClassificationSchemeTxtCodice" runat="server" CssClass="txt_addressBookLeft defaultAction"></cc1:CustomTextArea>
                </div>
            </div>
            <div class="riga">
                <div class="colonnasx">
                    <asp:Label runat="server" ID="ClassificationSchemelabelDescrizione"></asp:Label>
                </div>
                <div class="colonnadx">
                    <cc1:CustomTextArea ID="ClassificationSchemeTxtDescrizione" runat="server" CssClass="txt_addressBookLeft defaultAction"></cc1:CustomTextArea>
                </div>
            </div>
            <div class="riga" runat="server" ID="divIndiceSistematico">
                <div class="colonnasx">
                    <asp:Label runat="server" ID="ClassificationSchemelabelIndiceSistematico"></asp:Label>
                </div>
                <div class="colonna">
                    <cc1:CustomTextArea ID="ClassificationSchemeTxtIndiceSistematico" runat="server"
                        CssClass="txt_addressBookLeft defaultAction"></cc1:CustomTextArea>                        
                </div>
                <div class="colonnaImg">
                 <cc1:CustomImageButton ID="CustomImageIndiceSistematico" ImageUrl="../Images/Icons/export excel-pdf.png"
                        runat="server" OnMouseOverImage="../Images/Icons/export-excel-pdf_hover.png" OnMouseOutImage="../Images/Icons/export excel-pdf.png" onclick="CustomImageIndiceSistematico_Click" CssClass="clickableLeftN"/>
                
                 <asp:Panel runat="server" ID="pnlapplet" Visible="false"> 
                    <applet id='appletFso' 
                             code = 'com.nttdata.fsoApplet' 
                             codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>';
                             archive="FsoApplet.jar"
		                     width = '10'   height = '9'>
                    </applet>  
                </asp:Panel>
                </div>
            </div>
            <div class="riga">
                <div class="colonnasx">
                    <asp:Label runat="server" ID="ClassificationSchemelabelNote"></asp:Label>
                </div>
                <div class="colonnadx">
                    <cc1:CustomTextArea ID="ClassificationSchemeTxtNote" runat="server" CssClass="txt_addressBookLeft defaultAction"></cc1:CustomTextArea>
                </div>
            </div>
        </div>
        <%--     --%>
        <asp:UpdatePanel ID="UpdatePanelGridSearchClassificaction" runat="server">
            <ContentTemplate>
                <div id="divRisultati" runat="server" class="contenutoPopUpDX">
                    <asp:GridView ID="GridSearchClassificationScheme" runat="server" AllowPaging="True"
                        AutoGenerateColumns="False" HorizontalAlign="Center" HeaderStyle-CssClass="tableHeaderPopup"
                        CssClass="tabPopup tbl" Width="100%" OnPageIndexChanging="gridSearchObject_PageIndexChanging"
                        OnSelectedIndexChanging="GridSearchClassificationScheme_SelectedIndexChanging"
                        OnRowDataBound="GridSearchClassificationScheme_RowDataBound"
                        PageSize="8" ClientIDMode="Static">
                        <SelectedRowStyle CssClass="selectedrow" />
                        <SelectedRowStyle BackColor="Yellow" />
                        <HeaderStyle CssClass="tableHeaderPopup" />
                        <RowStyle CssClass="NormalRow" />
                        <AlternatingRowStyle CssClass="AltRow" />
                        <Columns>
                            <asp:TemplateField HeaderText="Codice">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodice" runat="server" Text='<%# Bind("CODICE") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Descrizione">
                                <ItemTemplate>
                                    <asp:Label ID="lblDescrizione" runat="server" Text='<%# Bind("DESCRIZIONE") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Livello" ItemStyle-Width="90px">
                                <ItemTemplate>
                                    <asp:Label ID="lblLivello" runat="server" Text='<%# Bind("LIVELLO") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Livello" ItemStyle-Width="90px" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdparent" runat="server" Text='<%# Bind("PARENT") %>'></asp:Label>
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                </ItemTemplate>
                                
                                <HeaderStyle Wrap="False" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
        <ContentTemplate>
        <div style="float: left;">
            <cc1:CustomButton ID="ClassificationSchemaBtnSearch" runat="server" OnClick="ClassificationSchemaBtnSearch_Click"
                CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" />
        </div>
        <div style="float: left;">
            <cc1:CustomButton ID="ClassificationSchemaBtnOk" runat="server" OnClick="ClassificationSchemaBtnOk_Click"
                CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" 
                Enabled="false" />
        </div>
        <div style="float: left;">
            <cc1:CustomButton ID="ClassificationSchemaBtnClose" runat="server" OnClick="ClassificationSchemaBtnClose_Click"
                CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
        </div>
        </ContentTemplate>
    </asp:UpdatePanel>
      <script type="text/javascript">
          $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
          $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });

          function generateRendomExportFileName() {
              var text = "_";
              var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

              for (var i = 0; i < 5; i++)
                  text += possible.charAt(Math.floor(Math.random() * possible.length));

              return text;
          }
  
			function OpenFile()
			{
				var filePath;
				var exportUrl;
				var http;
				var applName;				
				var fso;                
                var folder;
                var path;
                try {
				    fso = appletFso();
                    // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
				    folder = fso.GetSpecialFolder(2);
				    alert('folder:' + folder);
				    path = folder.Path;
				    alert('path:' + path);
				    filePath = path + "\\export" + generateRendomExportFileName() + ".xls";
					applName = "Microsoft Excel";	
					//exportUrl= "..\\exportDati\\exportDatiPage.aspx";				
					http = CreateObject("MSXML2.XMLHTTP");
					http.Open("POST",exportUrl,false);
					http.send();


					var content = http.responseBody;
					alert('content:' + content);
					if (content!=null)
					{
						AdoStreamWrapper_SaveBinaryData(filePath,content);
						
						ShellWrappers_Execute(filePath);
					}								 					
				}
				catch (ex)
				{			
				    alert(ex.message.toString());
				}						
			}


			$(function () {
			    $('.defaultAction').keypress(function (e) {
			        if (e.which == 13) {
			            e.preventDefault();
			            $('#ClassificationSchemaBtnSearch').click();
			        }
			    });
			});
    
    </script>
</asp:Content>
