using System.Windows.Input;

namespace SP2013Access.Controls.PropertyGrid.Commands
{
    public static class PropertyItemCommands
    {
        private static readonly RoutedCommand ResetValueCommand = new RoutedCommand();

        public static RoutedCommand ResetValue
        {
            get
            {
                return ResetValueCommand;
            }
        }
    }
}