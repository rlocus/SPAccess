using System.Windows.Input;

namespace SP2013Access.Controls.PropertyGrid.Commands
{
    public class PropertyGridCommands
    {
        private static readonly RoutedCommand ClearFilterCommand = new RoutedCommand();

        public static RoutedCommand ClearFilter
        {
            get
            {
                return ClearFilterCommand;
            }
        }
    }
}