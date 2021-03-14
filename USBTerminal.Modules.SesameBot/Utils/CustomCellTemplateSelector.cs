using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using USBTerminal.Services.SeasameService.GridViewCells;

namespace USBTerminal.Modules.SesameBot.Utils
{
    public class CustomCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ComboboxTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ComboboxField comboboxField)
            {
                return ComboboxTemplate;
            }
            if (item is TextField textField || item is GridViewField field)
            {
                return TextTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
