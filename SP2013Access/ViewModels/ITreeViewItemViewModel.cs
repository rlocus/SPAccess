using SP2013Access.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SP2013Access.ViewModels
{
    internal interface ITreeViewItemViewModel : INotifyPropertyChanged
    {
        ObservableCollection<TreeViewItemViewModel> Children { get; }

        bool HasDummyChild { get; }

        bool HasChildren { get; }

        bool IsExpanded { get; set; }

        bool IsSelected { get; set; }

        TreeViewItemViewModel Parent { get; }

        ObservableCollection<CommandEntity> Commands { get; }
    }
}