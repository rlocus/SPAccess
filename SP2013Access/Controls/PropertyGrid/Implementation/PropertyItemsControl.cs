using System.Windows;
using System.Windows.Controls;

namespace SP2013Access.Controls.PropertyGrid
{
    /// <summary>
    /// This Control is intended to be used in the template of the
    /// PropertyItemBase and PropertyGrid classes to contain the
    /// sub-children properties.
    /// </summary>
public class PropertyItemsControl : ItemsControl
	{
		internal static readonly RoutedEvent PreparePropertyItemEvent = EventManager.RegisterRoutedEvent("PreparePropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyItemsControl));
		internal static readonly RoutedEvent ClearPropertyItemEvent = EventManager.RegisterRoutedEvent("ClearPropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyItemsControl));
		internal event PropertyItemEventHandler PreparePropertyItem
		{
			add
			{
				base.AddHandler(PropertyItemsControl.PreparePropertyItemEvent, value);
			}
			remove
			{
				base.RemoveHandler(PropertyItemsControl.PreparePropertyItemEvent, value);
			}
		}
		internal event PropertyItemEventHandler ClearPropertyItem
		{
			add
			{
				base.AddHandler(PropertyItemsControl.ClearPropertyItemEvent, value);
			}
			remove
			{
				base.RemoveHandler(PropertyItemsControl.ClearPropertyItemEvent, value);
			}
		}
		private void RaisePreparePropertyItemEvent(PropertyItemBase propertyItem, object item)
		{
			base.RaiseEvent(new PropertyItemEventArgs(PropertyItemsControl.PreparePropertyItemEvent, this, propertyItem, item));
		}
		private void RaiseClearPropertyItemEvent(PropertyItemBase propertyItem, object item)
		{
			base.RaiseEvent(new PropertyItemEventArgs(PropertyItemsControl.ClearPropertyItemEvent, this, propertyItem, item));
		}
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PropertyItemBase;
		}
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ListPropertyItem();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			this.RaisePreparePropertyItemEvent((PropertyItemBase)element, item);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			this.RaiseClearPropertyItemEvent((PropertyItemBase)element, item);
			base.ClearContainerForItemOverride(element, item);
		}
	}
}