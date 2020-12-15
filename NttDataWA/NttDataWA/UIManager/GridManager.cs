using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System.Data;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI;

namespace NttDataWA.UIManager
{
    public class GridManager
    {        
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        private const string GRIGLIAPERSONALIZZATA = "GRID_PERSONALIZATION";

        /// <summary>
        /// verifica se il ruolo è abilitato a utilizzare la griglia personalizzata
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public static bool EnableCustomGrid(Ruolo ruolo)
        {
            bool result = false;
            try
            {

                result = ruolo.funzioni.Where(e => e.codice.Equals(GRIGLIAPERSONALIZZATA)).Count() > 0;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public static DataTable InitializeDataSet(Grid selectedGrid, Templates templateTemp, bool showGridPersonalization)
        {
            try
            {
                // Il dataset da restituire
                DataSet toReturn;

                // La tabella da aggiungere al dataset
                DataTable dataTable;

                // La colonna da aggiungere alla tabella
                DataColumn dataColumn;

                // Inizializzazione del dataset
                toReturn = new DataSet();
                dataTable = new DataTable();
                toReturn.Tables.Add(dataTable);

                List<Field> fields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();

                dataTable.Columns.Add("IdProfile", typeof(String));
                dataTable.Columns.Add("FileExtension", typeof(String));
                dataTable.Columns.Add("IsInStorageArea", typeof(Boolean));
                dataTable.Columns.Add("IsInWorkingArea", typeof(Boolean));
                dataTable.Columns.Add("IsInWorkingAreaRole", typeof(Boolean));
                dataTable.Columns.Add("IsSigned", typeof(Boolean));
                dataTable.Columns.Add("ProtoType", typeof(String));
                dataTable.Columns.Add("InLibroFirma", typeof(Boolean));

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    dataColumn = new DataColumn();
                    dataColumn.DataType = typeof(string);
                    dataColumn.ColumnName = field.FieldId;
                    dataTable.Columns.Add(dataColumn);
                }

                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();
                    if (customObjectTemp != null)
                    {
                        dataTable.Columns.Add("CONTATORE", typeof(String));
                    }
                }


                DataRow drow = dataTable.NewRow();
                dataTable.Rows.Add(drow);
                return dataTable;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public static GridView HeaderGridView(Grid selectedGrid, Templates templateTemp, bool showGridPersonalization, GridView grid)
        {
            try
            {
                List<Field> fields = selectedGrid.Fields.Where(e => e.Visible).OrderBy(f => f.Position).ToList();
                OggettoCustom customObjectTemp = new OggettoCustom();

                if (templateTemp != null && !showGridPersonalization)
                {
                    customObjectTemp = templateTemp.ELENCO_OGGETTI.Where(
                         e => e.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && e.DA_VISUALIZZARE_RICERCA == "1").FirstOrDefault();

                    Field d = new Field();

                    if (customObjectTemp != null)
                    {
                        d.AssociatedTemplateName = templateTemp.DESCRIZIONE;
                        d.CustomObjectId = customObjectTemp.SYSTEM_ID;
                        d.FieldId = "CONTATORE";
                        d.IsNumber = true;
                        d.Label = customObjectTemp.DESCRIZIONE;
                        d.OriginalLabel = customObjectTemp.DESCRIZIONE;
                        d.OracleDbColumnName = "to_number(getcontatoredocordinamento (a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "'))";
                        d.SqlServerDbColumnName = "@dbUser@.getContatoreDocOrdinamento(a.system_id, '" + customObjectTemp.TIPO_CONTATORE + "')";
                        fields.Insert(2, d);
                    }
                    else
                        fields.Remove(d);
                }

                grid.Columns.Clear();

                // Creazione delle colonne
                foreach (Field field in fields)
                {
                    BoundField column = null;
                    ButtonField columnHL = null;
                    TemplateField columnCKB = null;
                    if (field.OriginalLabel.ToUpper().Equals("DOCUMENTO"))
                        columnHL = GetLinkColumn(field.Label,
                            field.FieldId,
                            field.Width);
                    else
                    {

                        if (field is SpecialField)
                        {
                            switch (((SpecialField)field).FieldType)
                            {
                                case SpecialFieldsEnum.Icons:
                                    columnCKB = GetBoundColumnIcon(field.Label, field.Width, field.FieldId);
                                    break;
                                case SpecialFieldsEnum.CheckBox:
                                    {
                                        columnCKB = GetBoundColumnCheckBox(field.Label, field.Width, field.FieldId);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (field.FieldId)
                            {
                                case "CONTATORE":
                                    {
                                        column = GetBoundColumn(
                                            field.Label,
                                            field.OriginalLabel,
                                            100,
                                            field.FieldId);
                                        break;
                                    }

                                default:
                                    {
                                        column = GetBoundColumn(
                                         field.Label,
                                         field.OriginalLabel,
                                         field.Width,
                                         field.FieldId);
                                        break;
                                    }
                            }
                        }
                    }

                    if (columnCKB != null)
                        grid.Columns.Add(columnCKB);
                    else
                        if (column != null)
                            grid.Columns.Add(column);
                        else
                            grid.Columns.Add(columnHL);
                }
                grid.Columns.Add(GetBoundColumnNascosta("IdProfile", "IdProfile"));
                grid.Columns.Add(GetBoundColumnNascosta("FileExtension", "FileExtension"));
                grid.Columns.Add(GetBoundColumnNascosta("IsInStorageArea", "IsInStorageArea"));
                grid.Columns.Add(GetBoundColumnNascosta("IsInWorkingArea", "IsInWorkingArea"));
                grid.Columns.Add(GetBoundColumnNascosta("IsInWorkingAreaRole", "IsInWorkingAreaRole"));
                grid.Columns.Add(GetBoundColumnNascosta("ProtoType", "ProtoType"));
                grid.Columns.Add(GetBoundColumnNascosta("IsSigned", "IsSigned"));
                return grid;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ButtonField GetLinkColumn(String label, String boundColumnName, int width)
        {
            try
            {
                // La colonna da restituire
                ButtonField toReturn = new ButtonField();
                toReturn.HeaderText = label;
                toReturn.Visible = true;
                toReturn.ItemStyle.Width = new Unit(width, UnitType.Pixel);
                toReturn.DataTextField = boundColumnName;
                toReturn.SortExpression = boundColumnName;
                toReturn.ItemStyle.CssClass = "noLink jstree-unamovable";
                toReturn.ItemStyle.Wrap = true;
                toReturn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                toReturn.ItemStyle.VerticalAlign = VerticalAlign.Middle;
                toReturn.CommandName = "viewDetails";
                toReturn.ButtonType = ButtonType.Link;
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static BoundField GetBoundColumn(String label, String originalLabel, int width, String boundColumnName)
        {
            try
            {
                // La colonna da restituire
                BoundField toReturn = new BoundField();
                toReturn.HeaderText = label;
                toReturn.Visible = true;
                toReturn.DataField = boundColumnName;
                toReturn.HtmlEncode = false;
                toReturn.ItemStyle.Width = new Unit(width, UnitType.Pixel);
                toReturn.ItemStyle.Wrap = true;
                toReturn.SortExpression = boundColumnName;
                toReturn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                toReturn.ItemStyle.VerticalAlign = VerticalAlign.Middle;
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static BoundField GetBoundColumnNascosta(String label, String boundColumnName)
        {
            try
            {
                // La colonna da restituire
                BoundField toReturn = new BoundField();
                toReturn.Visible = false;
                toReturn.DataField = boundColumnName;
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TemplateField GetBoundColumnCheckBox(string label, int width, string idField)
        {
            try
            {
                // La colonna da restituire
                TemplateField toReturn = new TemplateField();
                toReturn.Visible = true;
                toReturn.ItemStyle.Width = new Unit(width, UnitType.Pixel);
                toReturn.ItemStyle.Wrap = true;
                toReturn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                toReturn.ItemStyle.VerticalAlign = VerticalAlign.Middle;
                toReturn.HeaderTemplate = new NttDatalLibrary.CustomColumnCheckBox(DataControlRowType.Header, label);
                toReturn.ItemTemplate = new NttDatalLibrary.CustomColumnCheckBox(DataControlRowType.DataRow, label);
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static TemplateField GetBoundColumnIcon(string label, int width, string idField)
        {
            try
            {
                // La colonna da restituire
                TemplateField toReturn = new TemplateField();
                toReturn.Visible = true;
                toReturn.ItemStyle.Width = new Unit(width, UnitType.Pixel);
                toReturn.ItemStyle.Wrap = true;
                toReturn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                toReturn.ItemStyle.VerticalAlign = VerticalAlign.Middle;
                toReturn.HeaderTemplate = new NttDatalLibrary.CustomColumnIcon(DataControlRowType.Header, label);
                toReturn.ItemTemplate = new NttDatalLibrary.CustomColumnIcon(DataControlRowType.DataRow, label);
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per il caricamento della griglia standard
        /// </summary>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Griglia standard</returns>
        public static Grid GetStandardGridForUser(
            GridTypeEnumeration gridType, InfoUtente infoutente)
        {
            Grid griglia = null;
            string language = UIManager.UserManager.GetUserLanguage();

            try
            {
                griglia = docsPaWS.GetStandardGridForUser(
                    infoutente,
                    gridType);

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            if (griglia != null && language.Trim().ToUpper() != "ITALIAN" && (!string.IsNullOrEmpty(language)))
            {
                griglia = EnglishGrid(griglia, language);
            }

            return griglia;
        }

        


        /// <summary>
        /// restituisce un folder da un idFascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="Fasc"></param>
        /// <returns></returns>
        public static DocsPaWR.Folder GetFolderByIdFasc(InfoUtente infoUtente, Fascicolo Fasc)
        {
            Folder result = null;
            try
            {
                result = docsPaWS.FascicolazioneGetFolder(infoUtente.idPeople, infoUtente.idGruppo, Fasc);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static FiltroRicerca[][] GetFiltriOrderRicerca(Grid SelectedGrid)
        {
            try
            {
                // Creazione oggetti filtro
                FiltroRicerca[][] qV = new FiltroRicerca[1][];
                qV = new FiltroRicerca[1][];
                qV[0] = new FiltroRicerca[1];
                FiltroRicerca[] fVList = new FiltroRicerca[0];

                List<FiltroRicerca> filterList = GetOrderFilterForDocumentInProject(SelectedGrid);

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                {
                    foreach (FiltroRicerca filter in filterList)
                        fVList = AddToArrayFiltroRicerca(fVList, filter);

                    qV[0] = fVList;
                }
                return qV;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<FiltroRicerca> GetOrderFilter(Grid SelGrid)
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            field = SelGrid.Fields.Where(e => e.FieldId == SelGrid.FieldForOrder.FieldId).FirstOrDefault();

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString(),
                        valore = field.OracleDbColumnName,
                        nomeCampo = field.FieldId,
                    });

                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.SQL_FIELD_FOR_ORDER.ToString(),
                        valore = field.SqlServerDbColumnName,
                        nomeCampo = field.FieldId
                    });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString(),
                        valore = field.CustomObjectId.ToString(),
                        nomeCampo = field.FieldId
                    });
                }
            }


            string tempValue = string.Empty;

            if (field != null)
            {
                tempValue = field.FieldId;
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
            {
                argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                nomeCampo = tempValue,
                valore = SelGrid.OrderDirection.ToString()
            });


            // Restituzione dei filtri creati
            return toReturn;
        }

        public static List<FiltroRicerca> GetOrderFilter()
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            field = SelectedGrid.Fields.Where(e => e.FieldId == GridManager.SelectedGrid.FieldForOrder.FieldId).FirstOrDefault();

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString(),
                        valore = field.OracleDbColumnName,
                        nomeCampo = field.FieldId,
                    });

                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.SQL_FIELD_FOR_ORDER.ToString(),
                        valore = field.SqlServerDbColumnName,
                        nomeCampo = field.FieldId
                    });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString(),
                        valore = field.CustomObjectId.ToString(),
                        nomeCampo = field.FieldId
                    });
                }
            }


            string tempValue = string.Empty;

            if (field != null)
            {
                tempValue = field.FieldId;
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
            {
                argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                nomeCampo = tempValue,
                valore = GridManager.SelectedGrid.OrderDirection.ToString()
            });


            // Restituzione dei filtri creati
            return toReturn;
        }

        public static List<FiltroRicerca> GetOrderFilterForDocumentInProject(Grid SelectedGrid)
        {
            try
            {
                // Lista dei fitri da restituire
                List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

                // Campo da utilizzare per l'ordinamento
                Field field = null;

                field = SelectedGrid.Fields.Where(e => e.FieldId == SelectedGrid.FieldForOrder.FieldId).FirstOrDefault();

                // Se il campo è valorizzato, vengono creati i filtri
                if (field != null)
                {
                    // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                    if (field.CustomObjectId == 0)
                    {
                        toReturn.Add(new FiltroRicerca()
                        {
                            argomento = FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString(),
                            valore = field.OracleDbColumnName,
                            nomeCampo = field.FieldId,
                        });

                        toReturn.Add(new FiltroRicerca()
                        {
                            argomento = FiltriDocumento.SQL_FIELD_FOR_ORDER.ToString(),
                            valore = field.SqlServerDbColumnName,
                            nomeCampo = field.FieldId
                        });
                    }
                    else
                    {
                        // Creazione di un filtro per la profilazione
                        toReturn.Add(new FiltroRicerca()
                        {
                            argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString(),
                            valore = field.CustomObjectId.ToString(),
                            nomeCampo = field.FieldId
                        });
                    }
                }

                string tempValue = string.Empty;

                if (field != null)
                {
                    tempValue = field.FieldId;
                }

                // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
                toReturn.Add(new FiltroRicerca()
                {
                    argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                    nomeCampo = tempValue,
                    valore = SelectedGrid.OrderDirection.ToString()
                });


                // Restituzione dei filtri creati
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static FiltroRicerca[] AddToArrayFiltroRicerca(FiltroRicerca[] array, FiltroRicerca nuovoElemento)
        {
            try
            {
                FiltroRicerca[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new FiltroRicerca[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new FiltroRicerca[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per il popolamento della Drop Down List con i campi relativi all'ordinamento
        /// e per la selezione del filtro attualmente impostato
        /// </summary>
        /// <param name="grid">Griglia su cui basare la creazione e la selezione del filtro di ricerca</param>
        /// <param name="ddlOrder">Drop down in cui inserire i possibile filtri da utilizzare per l'ordinamento</param>
        /// <param name="ddlOrderDirection">Drop down in cui inserire la direzione dell'ordinamento</param>
        public static void CompileDdlOrderAndSetOrderFilterProjects(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection, bool IsRoleEnabledToUseGrids)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                // Cancellazione degli item delle due drop down list
                ddlOrder.Items.Clear();
                ddlOrderDirection.Items.Clear();

                // Recupero dei campi su cui è possibile ordinare
                List<Field> fields = new List<Field>();

                if (IsRoleEnabledToUseGrids)
                {
                    fields = grid.Fields.Where(e => e.Visible && !e.Locked && !(e.FieldId.Equals("P20"))).OrderBy(e => e.Position).ToList();
                }
                else
                {
                    fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.SqlServerDbColumnName)).OrderBy(e => e.Position).ToList();
                }

                // Inserimento del campo di ricerca standard
                if (!string.IsNullOrEmpty(language) && language.ToUpper() != "ITALIANO")
                    ddlOrder.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("P20", language), "P20"));
                else
                    ddlOrder.Items.Add(new ListItem("Data creazione", "P20"));

                // Per ogni campo
                foreach (Field field in fields)
                    // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                    ddlOrder.Items.Add(new ListItem(field.Label, field.FieldId));

                // Inserimento delle due direzioni di ordinamento nella drop down della direzione
                if (!string.IsNullOrEmpty(language) && language.ToUpper() != "ITALIANO")
                {
                    ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ASC", language), "ASC"));
                    ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("DESC", language), "DESC"));
                }
                else
                {
                    ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
                    ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));
                }

                // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
                ddlOrder.SelectedIndex = 0;
                ddlOrderDirection.SelectedIndex = 1;

                // Selezione del filtro di ricerca documenti se selezionato
                if (grid.FieldForOrder != null)
                {
                    ddlOrder.ClearSelection();
                    ddlOrderDirection.ClearSelection();

                    for (int i = 0; i < ddlOrder.Items.Count; i++)
                        if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                            ddlOrder.SelectedIndex = i;
                }

                // Selezione della direzione dell'ordinamento
                if (grid.OrderDirection == OrderDirectionEnum.Asc)
                    ddlOrderDirection.SelectedIndex = 0;
                else
                    ddlOrderDirection.SelectedIndex = 1;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void CompileDdlOrderAndSetOrderFilterDocumentsWithCount(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();
                // Recupero dei campi su cui è possibile ordinare
                List<Field> fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.SqlServerDbColumnName)).OrderBy(e => e.Position).ToList();

                // Cancellazione degli item delle due drop down list
                ddlOrder.Items.Clear();
                ddlOrderDirection.Items.Clear();

                // Inserimento del campo di ricerca standard
                if (language != null && language.ToUpper() != "ITALIANO")
                    ddlOrder.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("D9", language), "D9"));
                else
                    ddlOrder.Items.Add(new ListItem("Data protocollazione / Creazione", "D9"));

                // Per ogni campo
                foreach (Field field in fields)
                    // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                    ddlOrder.Items.Add(new ListItem(field.Label, field.OriginalLabel));

                // Inserimento delle due direzioni di ordinamento nella drop down della direzione
                if (!string.IsNullOrEmpty(language) && language.ToUpper() != "ITALIANO")
                {
                    ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ASC", language), "ASC"));
                    ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("DESC", language), "DESC"));
                }
                else
                {
                    ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
                    ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));
                }

                // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
                ddlOrder.SelectedIndex = 0;
                ddlOrderDirection.SelectedIndex = 1;

                // Selezione del filtro di ricerca documenti se selezionato
                if (grid.FieldForOrder != null)
                {
                    ddlOrder.ClearSelection();
                    ddlOrderDirection.ClearSelection();

                    for (int i = 0; i < ddlOrder.Items.Count; i++)
                        if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                            ddlOrder.SelectedIndex = i;
                }

                // Selezione della direzione dell'ordinamento
                if (grid.OrderDirection == OrderDirectionEnum.Asc)
                    ddlOrderDirection.SelectedIndex = 0;
                else
                    ddlOrderDirection.SelectedIndex = 1;

                ddlOrder.Items.Add(new ListItem("Contatore", "-2"));
                ddlOrder.SelectedValue = "-2";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Funzione per il popolamento della Drop Down List con i campi relativi all'ordinamento
        /// e per la selezione del filtro attualmente impostato
        /// </summary>
        /// <param name="grid">Griglia su cui basare la creazione e la selezione del filtro di ricerca</param>
        /// <param name="ddlOrder">Drop down in cui inserire i possibile filtri da utilizzare per l'ordinamento</param>
        /// <param name="ddlOrderDirection">Drop down in cui inserire la direzione dell'ordinamento</param>
        public static void CompileDdlOrderAndSetOrderFilterDocuments(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection, bool IsRoleEnabledToUseGrids)
        {
            try
            {
                // Cancellazione degli item delle due drop down list
                string language = UIManager.UserManager.GetUserLanguage();
                ddlOrder.Items.Clear();
                ddlOrderDirection.Items.Clear();

                // Recupero dei campi su cui è possibile ordinare
                List<Field> fields = new List<Field>();

                if (IsRoleEnabledToUseGrids)

                    fields = grid.Fields.Where(e => e.Visible && !e.Locked && !(e.FieldId.Equals("D9"))).OrderBy(e => e.Position).ToList();
                else
                    fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.SqlServerDbColumnName)).OrderBy(e => e.Position).ToList();

                // Inserimento del campo di ricerca standard
                if (!string.IsNullOrEmpty(language) && language.ToUpper() != "ITALIANO")
                    ddlOrder.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("D9", language), "D9"));
                else
                    ddlOrder.Items.Add(new ListItem("Data protocollazione / Creazione", "D9"));

                // Per ogni campo
                foreach (Field field in fields)
                    // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                    ddlOrder.Items.Add(new ListItem(field.Label, field.FieldId));

                // Inserimento delle due direzioni di ordinamento nella drop down della direzione
                if (!string.IsNullOrEmpty(language) && language.ToUpper() != "ITALIANO")
                {
                    ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ASC", language), "ASC"));
                    ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("DESC", language), "DESC"));
                }
                else
                {
                    ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
                    ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));
                }

                // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
                ddlOrder.SelectedIndex = 0;
                ddlOrderDirection.SelectedIndex = 1;

                // Selezione del filtro di ricerca documenti se selezionato
                if (grid.FieldForOrder != null)
                {
                    ddlOrder.ClearSelection();
                    ddlOrderDirection.ClearSelection();

                    for (int i = 0; i < ddlOrder.Items.Count; i++)
                        if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                            ddlOrder.SelectedIndex = i;
                }

                // Selezione della direzione dell'ordinamento
                if (grid.OrderDirection == OrderDirectionEnum.Asc)
                    ddlOrderDirection.SelectedIndex = 0;
                else
                    ddlOrderDirection.SelectedIndex = 1;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        //public static bool IsRoleEnabledToUseGrids(Ruolo ruolo)
        //{
        //   bool result = false;
        //    try
        //    {
        //       result =  ruolo.funzioni.Where(e => e.codice.Equals("GRID_PERSONALIZATION")).Count() > 0;                
        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return false;
        //    }
        //    return result;
        //}

        /// <summary>
        /// Funzione per il caricamento della griglia standard
        /// </summary>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Griglia standard</returns>
        public static Grid getUserGrid(
            GridTypeEnumeration gridType, InfoUtente infoutente, Ruolo ruolo)
        {
            Grid griglia = null;
            string language = UIManager.UserManager.GetUserLanguage();

            try
            {
                if (UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"))
              
                    griglia= docsPaWS.GetUserGridCustom(
                        infoutente,
                        gridType);
                else
                    griglia = docsPaWS.GetStandardGridForUser(
                      infoutente,
                       gridType);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            if (griglia != null && language.Trim().ToUpper() != "ITALIAN" && (!string.IsNullOrEmpty(language)))
            {
                griglia = EnglishGrid(griglia, language);
            }

            return griglia;
        }

        /// <summary>
        /// Funzione per la clonazione di una griglia
        /// </summary>
        /// <param name="gridToClone">Griglia da clonare</param>
        /// <returns>Griglia clonata</returns>
        public static Grid CloneGrid(Grid gridToClone)
        {
            try
            {
                // Griglia clonata da restituire
                Grid toReturn = new Grid();

                // Impostazione delle proprietà della griglia da restituire
                toReturn.GridId = gridToClone.GridId;
                toReturn.GridType = gridToClone.GridType;
                toReturn.RapidSearchId = gridToClone.RapidSearchId;
                toReturn.ColorForFieldWithotTemplate = gridToClone.ColorForFieldWithotTemplate;

                // Clonaggio dei system id dei template
                List<String> templatesId = new List<string>();
                if (gridToClone.TemplatesId != null)
                    templatesId.AddRange(gridToClone.TemplatesId);

                toReturn.TemplatesId = templatesId.ToArray();

                // Clonaggio dei campi
                List<Field> fields = new List<Field>();
                foreach (Field field in gridToClone.Fields)
                    if (field is SpecialField)
                        fields.Add(CloneSpecialFiels((SpecialField)field));
                    else
                        fields.Add(CloneField(field));
                toReturn.Fields = fields.ToArray();

                // Clonaggio della direzione dell'ordinamento e per l'impostazione del
                // campo da utilizzare per l'ordinamento
                toReturn.OrderDirection = gridToClone.OrderDirection;
                if (gridToClone.FieldForOrder != null)
                    toReturn.FieldForOrder = toReturn.Fields.Where(e => e.FieldId == gridToClone.FieldForOrder.FieldId).FirstOrDefault();

                // Restituzione della griglia clonata
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per la clonazione di un campo speciale
        /// </summary>
        /// <param name="field">Campo speciale da clonare</param>
        /// <returns>Campo clonato</returns>
        private static Field CloneSpecialFiels(SpecialField field)
        {
            try
            {
                // Creazione dell'oggetto clocanto da restituire
                SpecialField toReturn = new SpecialField();

                // Copia delle proprietà
                // Le colonne speciali devono essere sempre visibili
                toReturn.Visible = true;
                toReturn.Position = field.Position;
                toReturn.Locked = field.Locked;
                toReturn.FieldType = field.FieldType;
                toReturn.OriginalLabel = field.OriginalLabel;
                toReturn.Label = field.Label;
                toReturn.FieldId = field.FieldId;
                toReturn.Width = field.Width;
                toReturn.MaxLength = field.MaxLength;

                // Restituzione del campo clonato
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per il clonaggio di un campo della griglia
        /// </summary>
        /// <param name="field">Il campo da clonare</param>
        /// <returns>Campo clonato</returns>
        private static Field CloneField(Field field)
        {
            try
            {
                // Il campo da restituire
                Field toReturn = new Field();

                // Copia delle proprietà
                toReturn.CanAssumeMultiValues = field.CanAssumeMultiValues;
                toReturn.CustomObjectId = field.CustomObjectId;
                toReturn.FieldId = field.FieldId;
                toReturn.IsTruncable = field.IsTruncable;
                toReturn.Label = field.Label;
                toReturn.MaxLength = field.MaxLength;
                toReturn.OriginalLabel = field.OriginalLabel;
                toReturn.Position = field.Position;
                toReturn.Visible = field.Visible;
                toReturn.Width = field.Width;
                toReturn.OracleDbColumnName = field.OracleDbColumnName;
                toReturn.SqlServerDbColumnName = field.SqlServerDbColumnName;
                toReturn.AssociatedTemplateName = field.AssociatedTemplateName;
                toReturn.Locked = field.Locked;
                toReturn.IsCommonField = field.IsCommonField;
                toReturn.AssociatedTemplateId = field.AssociatedTemplateId;
                toReturn.IsNumber = field.IsNumber;
                toReturn.Values = field.Values;
                // Restituzione del campo clonato
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per il salvataggio della griglia
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        public static void SaveNewGrid(Grid grid, InfoUtente userInfo, String gridName, String visibility, Boolean isPreferred)
        {
            try
            {
                grid.GridId = docsPaWS.SaveNewGrid(grid, userInfo, gridName, visibility, isPreferred);
              
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Funzione per il salvataggio della griglia
        /// </summary>
        /// <param name="grid">Griglia da salvare</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        public static void ModifyGrid(Grid grid, InfoUtente userInfo, String visibility, Boolean isPreferred)
        {
            try
            {               
                if (isPreferred)
                {
                    grid.GridId = docsPaWS.ModifyGrid(grid, userInfo, visibility, isPreferred);
                }
                else
                {
                    docsPaWS.ModifyGrid(grid, userInfo, visibility, isPreferred);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Funzione per il caricamento delle informazioni di base relative alle griglie salvate per un dato utente
        /// appartenente ad un dato ruolo definito per una certa amministrazione e per un particolare tipo di ricerca
        /// </summary>
        /// <param name="userId">Identificativo dell'utente</param>
        /// <param name="roleId">Identificativo del ruolo</param>
        /// <param name="administrationId">Identificativo dell'amministrazione</param>
        /// <param name="gridType">Tipo di griglia</param>
        ///<returns>Lista di oggetti con le informazioni di base sulle griglie  definite da un utente</returns>
        public static GridBaseInfo[] GetGridsBaseInfo(InfoUtente infoUtente, GridTypeEnumeration gridType, bool allGrids)
        {
            GridBaseInfo[] result = null;
            try
            {
                result= docsPaWS.GetGridsBaseInfo(infoUtente, gridType, allGrids);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;

        }

        /// <summary>
        /// Funzione per l'inizializzazione del data set in base ai campi definiti nella 
        /// griglia
        /// </summary>
        /// <param name="selectedGrid">La griglia su cui basare la creazione del dataset</param>
        /// <returns></returns>
        public static DataSet InitializeDataSet(Grid selectedGrid)
        {
            // Il dataset da restituire
            DataSet toReturn;

            // La tabella da aggiungere al dataset
            DataTable dataTable;

            // La colonna da aggiungere alla tabella
            DataColumn dataColumn;

            // Inizializzazione del dataset
            toReturn = new DataSet();
            dataTable = new DataTable();
            toReturn.Tables.Add(dataTable);

            List<Field> fields = selectedGrid.Fields.Where(e => e.GetType().Equals(typeof(Field))).ToList();

            // Creazione delle colonne
            foreach (Field field in fields)
            {
                dataColumn = new DataColumn(field.FieldId, typeof(String));
                dataTable.Columns.Add(dataColumn);
            }

            // Colonna con il codice HEX da associare allo sfondo di una cella di template
            // che non appartiene ad un documento
            dataColumn = new DataColumn();
            dataTable.Columns.Add(dataColumn);

            // Restituzione del dataset
            return toReturn;

        }

        public static void AddPreferredGrid(string gridId, InfoUtente infoUser, GridTypeEnumeration gridType)
        {
            try
            {
                docsPaWS.AddPreferredGrid(gridId, infoUser, gridType);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Rimuove la griglia preferita
        /// </summary>
        /// <param name="type">Tipologia della griglia</param>
        /// <param name="userInfo">Informazioni sull'utente proprietario della griglia</param>
        public static void RemovePreferredTypeGrid(InfoUtente infoUser, GridTypeEnumeration gridType)
        {
            try
            {               
                docsPaWS.RemovePreferredTypeGrid(infoUser, gridType);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
       }

        public static bool RemoveGrid(GridBaseInfo gridBase, InfoUtente infoutente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RemoveGrid(gridBase, infoutente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;
        }

        /// <summary>
        /// Funzione per il caricamento della griglia standard
        /// </summary>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Griglia standard</returns>
        public static Grid getUserGrid(
            GridTypeEnumeration gridType)
        {
            Grid tempGrid = null;
            string language = UIManager.UserManager.GetUserLanguage();

            try
            {
                InfoUtente userInfo = UIManager.UserManager.GetInfoUser();

                if (UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"))
                {

                    tempGrid = docsPaWS.GetUserGridCustom(
                        userInfo,
                        gridType);
                }
                else
                {
                    tempGrid = docsPaWS.GetStandardGridForUser(
                      userInfo,
                       gridType);
                }

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                tempGrid = null;
            }

            if (tempGrid != null && language.Trim().ToUpper() != "ITALIAN" && (!string.IsNullOrEmpty(language)))
            {
                tempGrid = EnglishGrid(tempGrid, language);
            }

            return tempGrid;
        }

        private static Grid EnglishGrid(Grid tempGrid, string language)
        {
            foreach (Field colonna in tempGrid.Fields)
            {
                if (colonna.FieldId == "C1") colonna.Label = Utils.Languages.GetLabelFromCode("C1", language);
                if (colonna.FieldId == "D1") colonna.Label = Utils.Languages.GetLabelFromCode("D1", language);
                if (colonna.FieldId == "D2") colonna.Label = Utils.Languages.GetLabelFromCode("D2", language);
                if (colonna.FieldId == "D3") colonna.Label = Utils.Languages.GetLabelFromCode("D3", language);
                if (colonna.FieldId == "D4") colonna.Label = Utils.Languages.GetLabelFromCode("D4", language);
                if (colonna.FieldId == "D5") colonna.Label = Utils.Languages.GetLabelFromCode("D5", language);
                if (colonna.FieldId == "D6") colonna.Label = Utils.Languages.GetLabelFromCode("D6", language);
                if (colonna.FieldId == "D7") colonna.Label = Utils.Languages.GetLabelFromCode("D7", language);
                if (colonna.FieldId == "D8") colonna.Label = Utils.Languages.GetLabelFromCode("D8", language);
                if (colonna.FieldId == "D9") colonna.Label = Utils.Languages.GetLabelFromCode("D9", language);
                if (colonna.FieldId == "D10") colonna.Label = Utils.Languages.GetLabelFromCode("D10", language);
                if (colonna.FieldId == "D11") colonna.Label = Utils.Languages.GetLabelFromCode("D11", language);
                if (colonna.FieldId == "D12") colonna.Label = Utils.Languages.GetLabelFromCode("D12", language);
                if (colonna.FieldId == "D13") colonna.Label = Utils.Languages.GetLabelFromCode("D13", language);
                if (colonna.FieldId == "D14") colonna.Label = Utils.Languages.GetLabelFromCode("D14", language);
                if (colonna.FieldId == "D15") colonna.Label = Utils.Languages.GetLabelFromCode("D15", language);
                if (colonna.FieldId == "D16") colonna.Label = Utils.Languages.GetLabelFromCode("D16", language);
                if (colonna.FieldId == "D17") colonna.Label = Utils.Languages.GetLabelFromCode("D17", language);
                if (colonna.FieldId == "D18") colonna.Label = Utils.Languages.GetLabelFromCode("D18", language);
                if (colonna.FieldId == "D19") colonna.Label = Utils.Languages.GetLabelFromCode("D19", language);
                if (colonna.FieldId == "D20") colonna.Label = Utils.Languages.GetLabelFromCode("D20", language);
                if (colonna.FieldId == "D21") colonna.Label = Utils.Languages.GetLabelFromCode("D21", language);
                if (colonna.FieldId == "D22") colonna.Label = Utils.Languages.GetLabelFromCode("D22", language);
                if (colonna.FieldId == "D23") colonna.Label = Utils.Languages.GetLabelFromCode("D23", language);
                if (colonna.FieldId == "U1") colonna.Label = Utils.Languages.GetLabelFromCode("U1", language);
                if (colonna.FieldId == "IMPRONTA") colonna.Label = Utils.Languages.GetLabelFromCode("IMPRONTA", language);
                if (colonna.FieldId == "COD_EXT_APP") colonna.Label = Utils.Languages.GetLabelFromCode("COD_EXT_APP", language);
                if (colonna.FieldId == "NOME_ORIGINALE") colonna.Label = Utils.Languages.GetLabelFromCode("NOME_ORIGINALE", language);
                if (colonna.FieldId == "DTA_ADL") colonna.Label = Utils.Languages.GetLabelFromCode("DTA_ADL", language);
                if (colonna.FieldId == "esito_spedizione") colonna.Label = Utils.Languages.GetLabelFromCode("esito_spedizione", language);
                if (colonna.FieldId == "count_ric_interop") colonna.Label = Utils.Languages.GetLabelFromCode("count_ric_interop", language);
                if (colonna.FieldId == "stato_conservazione") colonna.Label = Utils.Languages.GetLabelFromCode("stato_conservazione", language);
                if (colonna.FieldId == "C2") colonna.Label = Utils.Languages.GetLabelFromCode("C2", language);

                if (colonna.FieldId == "P1") colonna.Label = Utils.Languages.GetLabelFromCode("P1", language);
                if (colonna.FieldId == "P2") colonna.Label = Utils.Languages.GetLabelFromCode("P2", language);
                if (colonna.FieldId == "P3") colonna.Label = Utils.Languages.GetLabelFromCode("P3", language);
                if (colonna.FieldId == "P4") colonna.Label = Utils.Languages.GetLabelFromCode("P4", language);
                if (colonna.FieldId == "P5") colonna.Label = Utils.Languages.GetLabelFromCode("P5", language);
                if (colonna.FieldId == "P6") colonna.Label = Utils.Languages.GetLabelFromCode("P6", language);
                if (colonna.FieldId == "P7") colonna.Label = Utils.Languages.GetLabelFromCode("P7", language);
                if (colonna.FieldId == "P8") colonna.Label = Utils.Languages.GetLabelFromCode("P8", language);
                if (colonna.FieldId == "P9") colonna.Label = Utils.Languages.GetLabelFromCode("P9", language);
                if (colonna.FieldId == "P10") colonna.Label = Utils.Languages.GetLabelFromCode("P10", language);
                if (colonna.FieldId == "P11") colonna.Label = Utils.Languages.GetLabelFromCode("P11", language);
                if (colonna.FieldId == "P12") colonna.Label = Utils.Languages.GetLabelFromCode("P12", language);
                if (colonna.FieldId == "P13") colonna.Label = Utils.Languages.GetLabelFromCode("P13", language);
                if (colonna.FieldId == "P14") colonna.Label = Utils.Languages.GetLabelFromCode("P14", language);
                if (colonna.FieldId == "P15") colonna.Label = Utils.Languages.GetLabelFromCode("P15", language);
                if (colonna.FieldId == "P16") colonna.Label = Utils.Languages.GetLabelFromCode("P16", language);
                if (colonna.FieldId == "P17") colonna.Label = Utils.Languages.GetLabelFromCode("P17", language);
                if (colonna.FieldId == "P18") colonna.Label = Utils.Languages.GetLabelFromCode("P18", language);
                if (colonna.FieldId == "P19") colonna.Label = Utils.Languages.GetLabelFromCode("P19", language);
                if (colonna.FieldId == "P20") colonna.Label = Utils.Languages.GetLabelFromCode("P20", language);
                if (colonna.FieldId == "P22") colonna.Label = Utils.Languages.GetLabelFromCode("P22", language);

                if (colonna.FieldId == "T7") colonna.Label = Utils.Languages.GetLabelFromCode("T7", language);

                if (colonna.FieldId == "CODICE_POLICY") colonna.Label = Utils.Languages.GetLabelFromCode("policy_code", language);
                if (colonna.FieldId == "CONTATORE_POLICY") colonna.Label = Utils.Languages.GetLabelFromCode("num_exec_policy", language);
                if (colonna.FieldId == "DATA_ESECUZIONE_POLICY") colonna.Label = Utils.Languages.GetLabelFromCode("date_exec_policy", language);
                if (colonna.FieldId == "CHA_TASK_STATUS") colonna.Label = Utils.Languages.GetLabelFromCode("TaskStatus", language);
            }

            return tempGrid;
        }

        /// <summary>
        /// Griglia di ricerca da utilizzare per disegnare una griglia di ricerca
        /// </summary>
        public static Grid SelectedGrid
        {
            get
            {
                Grid result = null;
                if (HttpContext.Current.Session["selectedGrid"] != null)
                {
                    result = HttpContext.Current.Session["selectedGrid"] as Grid;
                }
                return result;
            }

            set
            {
                HttpContext.Current.Session["selectedGrid"] = value;
            }
        }


        protected int MaxLenghtObject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["maxLenghtObject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["maxLenghtObject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["maxLenghtObject"] = value;
            }
        }

        /// <summary>
        /// Funzione per il popolamento della Drop Down List con i campi relativi all'ordinamento
        /// e per la selezione del filtro attualmente impostato
        /// </summary>
        /// <param name="grid">Griglia su cui basare la creazione e la selezione del filtro di ricerca</param>
        /// <param name="ddlOrder">Drop down in cui inserire i possibile filtri da utilizzare per l'ordinamento</param>
        /// <param name="ddlOrderDirection">Drop down in cui inserire la direzione dell'ordinamento</param>
        public static void CompileDdlOrderAndSetOrderFilterDocuments(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            // Cancellazione degli item delle due drop down list
            ddlOrder.Items.Clear();
            ddlOrderDirection.Items.Clear();

            // Recupero dei campi su cui è possibile ordinare
            List<Field> fields = new List<Field>();

            if (IsRoleEnabledToUseGrids())
            {
                fields = grid.Fields.Where(e => e.Visible && !e.Locked && !(e.FieldId.Equals("D9"))).OrderBy(e => e.Position).ToList();
            }
            else
            {
                fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.SqlServerDbColumnName)).OrderBy(e => e.Position).ToList();
            }

            // Inserimento del campo di ricerca standard
            if(language!=null && language.ToUpper() != "ITALIANO")
                ddlOrder.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("D9", language), "D9"));
            else
                ddlOrder.Items.Add(new ListItem("Data protocollazione / Creazione", "D9"));


            // Per ogni campo
            foreach (Field field in fields)
                // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                ddlOrder.Items.Add(new ListItem(field.Label, field.FieldId));

            // Inserimento delle due direzioni di ordinamento nella drop down della direzione
            if (!string.IsNullOrEmpty(language) && language.ToUpper() != "ITALIANO")
            {
                ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ASC", language), "ASC"));
                ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("DESC", language), "DESC"));
            }
            else
            {
                ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
                ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));
            }

            // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
            ddlOrder.SelectedIndex = 0;
            ddlOrderDirection.SelectedIndex = 1;

            // Selezione del filtro di ricerca documenti se selezionato
            if (grid.FieldForOrder != null)
            {
                ddlOrder.ClearSelection();
                ddlOrderDirection.ClearSelection();

                for (int i = 0; i < ddlOrder.Items.Count; i++)
                    if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                        ddlOrder.SelectedIndex = i;
            }

            // Selezione della direzione dell'ordinamento
            if (grid.OrderDirection == OrderDirectionEnum.Asc)
                ddlOrderDirection.SelectedIndex = 0;
            else
                ddlOrderDirection.SelectedIndex = 1;
        }

        public static bool IsRoleEnabledToUseGrids()
        {
            try
            {
                // Reperimento della lista di funzioni associate al ruolo
                Funzione[] functions = RoleManager.GetRoleInSession().funzioni;

                return functions.Where(e => e.codice.Equals("GRID_PERSONALIZATION")).Count() > 0;
            }
            catch
            {

                return false;
            }
        }

        /// <summary>
        /// Funzione per il caricamento di una griglia salvata per una determinata
        /// ricerca rapida. Nel caso in cui non sia stata salvata alcuna griglia, viene
        /// restituita quella standard
        /// </summary>
        /// <param name="searchId">System id della ricerca rapida di cui caricarene la griglia associata</param>
        /// <param name="gridType">Tipo di griglia da caricare</param>
        /// <returns>Griglia relativa alla ricerca rapida segnalata</returns>
        public static Grid GetGridFromSearchId(
            string searchId,
            GridTypeEnumeration gridType)
        {
            try
            {
                InfoUtente userInfo = UserManager.GetInfoUser();

                return docsPaWS.GetGridFromSearchId(
                    userInfo,
                    searchId,
                    gridType);


            }
            catch (Exception e)
            {
                throw new Exception("Errore durante il reperimento della griglia per la ricerca rapida.", e);
            }
        }

        /// <summary>
        /// Funzione per la creazione di un filtro di ricerca da utilizzare per l'ordinamento
        /// dei risultati restituiti dalla ricerca documenti
        /// </summary>
        /// <param name="fieldName">Nome del campo da utilizzare per l'ordinamento</param>
        /// <param name="orderDirection">Direzione da adottare per l'ordinamento</param>
        /// <returns>Lista dei filtri di ricerca da utilizzare per la ricerca</returns>
        public static List<FiltroRicerca> GetOrderFilterForDocument(String fieldId, String orderDirection)
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            // Recupero delle informazioni sul campo
            if (!String.IsNullOrEmpty(fieldId))
            {
                field = SelectedGrid.Fields.Where(e => e.FieldId == fieldId).FirstOrDefault();
            }

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString(),
                        valore = field.OracleDbColumnName,
                        nomeCampo = field.FieldId,
                    });

                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.SQL_FIELD_FOR_ORDER.ToString(),
                        valore = field.SqlServerDbColumnName,
                        nomeCampo = field.FieldId
                    });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString(),
                        valore = field.CustomObjectId.ToString(),
                        nomeCampo = field.FieldId
                    });
                }
            }


            string tempValue = string.Empty;

            if (field != null)
            {
                tempValue = field.FieldId;
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
            {
                argomento = FiltriDocumento.ORDER_DIRECTION.ToString(),
                nomeCampo = tempValue,
                valore = orderDirection
            });


            // Restituzione dei filtri creati
            return toReturn;
        }

        public static void CompileDdlOrderAndSetOrderFilterTransmission(Grid grid, DropDownList ddlOrder, DropDownList ddlOrderDirection)
        {
            // Recupero dei campi su cui è possibile ordinare
            List<Field> fields = grid.Fields.Where(e => e.Visible && !String.IsNullOrEmpty(e.OracleDbColumnName)).OrderBy(e => e.Position).ToList();
            string language = UIManager.UserManager.GetUserLanguage();

            // Cancellazione degli item delle due drop down list
            ddlOrder.Items.Clear();
            ddlOrderDirection.Items.Clear();

            // Inserimento del campo di ricerca standard
            ddlOrder.Items.Add(new ListItem("Data invio / creazione"));

            // Per ogni campo
            foreach (Field field in fields)
                // Inserimento delle informazioni sul campo nella ddl dei campi su cui è possibile ordinare
                ddlOrder.Items.Add(new ListItem(field.Label, field.OriginalLabel));

            // Inserimento delle due direzioni di ordinamento nella drop down della direzione
            if (!string.IsNullOrEmpty(language) && language.ToUpper() != "ITALIANO")
            {
                ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("ASC", language), "ASC"));
                ddlOrderDirection.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("DESC", language), "DESC"));
            }
            else
            {
                ddlOrderDirection.Items.Add(new ListItem("Crescente", "ASC"));
                ddlOrderDirection.Items.Add(new ListItem("Decrescente", "DESC"));
            }

            // Per default l'ordinamento è per "Data protocollazione / Creazione", "Crescente"
            ddlOrder.SelectedIndex = 0;
            ddlOrderDirection.SelectedIndex = 1;

            // Selezione del filtro di ricerca documenti se selezionato
            if (grid.FieldForOrder != null)
            {
                ddlOrder.ClearSelection();
                ddlOrderDirection.ClearSelection();

                for (int i = 0; i < ddlOrder.Items.Count; i++)
                    if (ddlOrder.Items[i].Text.Equals(grid.FieldForOrder.Label))
                        ddlOrder.SelectedIndex = i;

                // Selezione della direzione dell'ordinamento
                if (grid.OrderDirection == OrderDirectionEnum.Asc)
                    ddlOrderDirection.SelectedIndex = 0;
                else
                    ddlOrderDirection.SelectedIndex = 1;

            }

        }

        /// <summary>
        /// Funzione per la restituzione del filtro da utilizzare per l'ordinamento della
        /// ricerca trasmissioni
        /// </summary>
        /// <param name="fieldName">Nome del campo per cui ordinare</param>
        /// <param name="orderDirection">Direzione di ordinamento</param>
        /// <returns>Lista dei filtri da utilizzare per la ricerca</returns>
        public static List<FiltroRicerca> GetOrderFilterForTransmission(String fieldName, String orderDirection)
        {
            // Lista dei fitri da restituire
            List<FiltroRicerca> toReturn = new List<FiltroRicerca>();

            // Campo da utilizzare per l'ordinamento
            Field field = null;

            // Recupero delle informazioni sul campo
            if (!String.IsNullOrEmpty(fieldName))
                field = SelectedGrid.Fields.Where(e => e.OriginalLabel == fieldName).FirstOrDefault();

            // Se il campo è valorizzato, vengono creati i filtri
            if (field != null)
            {
                // Se il campo è standard, vengono creati i due filtri di ricerca per SQL e per ORACLE
                if (field.CustomObjectId == 0)
                {
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriTrasmissione.ORACLE_FIELD_FOR_ORDER.ToString(),
                        valore = field.OracleDbColumnName
                    });

                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriTrasmissione.SQL_FIELD_FOR_ORDER.ToString(),
                        valore = field.SqlServerDbColumnName
                    });
                }
                else
                {
                    // Creazione di un filtro per la profilazione
                    toReturn.Add(new FiltroRicerca()
                    {
                        argomento = FiltriTrasmissione.PROFILATION_FIELD_FOR_ORDER.ToString(),
                        valore = field.CustomObjectId.ToString()
                    });
                }
            }

            // Aggiunta del filtro con indicazioni sulla direzione dell'ordinamento
            toReturn.Add(new FiltroRicerca()
            {
                argomento = FiltriTrasmissione.ORDER_DIRECTION.ToString(),
                valore = orderDirection
            });

            // Restituzione dei filtri creati
            return toReturn;

        }

    }
}
