using System;
using InformaticaTrentinaPCL.iOS.Helper;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.EmbedComponent
{

    public class StructCellUtility
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:InformaticaTrentinaPCL.iOS.EmbedComponent.StructCellUtility"/> class.
        /// </summary>
        /// <param name="labelDefault">Label default.</param>
        /// <param name="imageLeft">Image left. prende anche valori a null</param>
        public StructCellUtility(String labelDefault,UIImageView imageLeft = null)
        {
            this.labelDefault = labelDefault;
            if (imageLeft != null)
            this.imageLeft = imageLeft;
        }

        public String labelDefault;
        public UIImageView imageLeft;
    }


    public class CellUtility
    {

        /// <summary>
        /// Creates the cell. https://developer.xamarin.com/recipes/ios/content_controls/tables/specify_the_cell_type/
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="style">Style.</param>
        /// <param name="tableView">Table view.</param>
        /// <param name="model">Model.</param>
        public static UITableViewCell CreateCell(UITableViewCellStyle style,UITableView tableView,StructCellUtility model)
        {
            // request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell("cellIdentifier");
            // if there are no cells to reuse, create a new one
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "cellIdentifier");
                //cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellIdentifier);
                //cell = new UITableViewCell (UITableViewCellStyle.Value1, cellIdentifier);
                //cell = new UITableViewCell (UITableViewCellStyle.Value2, cellIdentifier);
            }

            cell.TextLabel.Text = model.labelDefault;
            cell.TextLabel.Lines = 0;  // multiline 

            if (model.imageLeft != null)
            cell.ImageView.Image = model.imageLeft.Image;
            
            // optionally set the other text and image properties here
            return cell;
        }

        /// <summary>
        /// Creates the cell. https://developer.xamarin.com/recipes/ios/content_controls/tables/specify_the_cell_type/
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="style">Style.</param>
        /// <param name="tableView">Table view.</param>
        /// <param name="model">Model.</param>
        public static UITableViewCell CreateCellStyleLabel(UITableView tableView, String title)
        {
            // request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell("cellIdentifier");
            // if there are no cells to reuse, create a new one
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "cellIdentifier");
            }

            cell.TextLabel.Text = title;
            cell.TextLabel.Lines = 0;  // multiline 
            Font.SetCustomStyleFont(cell.TextLabel, Font.LABEL);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
       
            // optionally set the other text and image properties here
            return cell;
        }

    }
}
