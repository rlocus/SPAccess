using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SP2013Access.Extensions
{
    internal static class TreeViewExtensions
    {
        public static readonly DependencyProperty SelectItemOnRightClickProperty = DependencyProperty.RegisterAttached(
            "SelectItemOnRightClick",
            typeof(bool),
            typeof(TreeViewExtensions),
            new UIPropertyMetadata(false, OnSelectItemOnRightClickChanged));

        public static bool GetSelectItemOnRightClick(DependencyObject d)
        {
            return (bool)d.GetValue(SelectItemOnRightClickProperty);
        }

        public static void SetSelectItemOnRightClick(DependencyObject d, bool value)
        {
            d.SetValue(SelectItemOnRightClickProperty, value);
        }

        private static void OnSelectItemOnRightClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool selectItemOnRightClick = (bool)e.NewValue;

            TreeView treeView = d as TreeView;
            if (treeView != null)
            {
                if (selectItemOnRightClick)
                    treeView.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
                else
                    treeView.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
            }
        }

        private static void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        public static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        public static void ExpandCollapseAll<T>(this TreeViewItem treeViewItem, bool expand = true, Func<T, bool> selector = null)
        {
            if (treeViewItem == null)
            {
                return;
            }
            var items = (selector != null ? treeViewItem.Items.Cast<T>().Where(selector) : treeViewItem.Items.Cast<T>()).ToArray();
            foreach (T current in items)
            {
                var tvi = treeViewItem.ItemContainerGenerator.ContainerFromItem(current) as TreeViewItem;

                if (tvi != null)
                {
                    ExpandCollapseAll(tvi, expand, selector);
                }
            }
            treeViewItem.IsExpanded = expand;
        }

        public static void ExpandCollapseAll<T>(this TreeView treeView, bool expand = true, Func<T, bool> selector = null)
        {
            IEnumerable<T> items = selector != null ? treeView.Items.Cast<T>().Where(selector) : treeView.Items.Cast<T>();

            foreach (T current in items.ToArray())
            {
                var treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(current) as TreeViewItem;

                if (treeViewItem != null)
                {
                    ExpandCollapseAll(treeViewItem, expand, selector);
                }
            }
        }

        public static TreeViewItem GetTreeViewItemFromChild(this TreeView treeView, DependencyObject child)
        {
            if (child == null)
            {
                return null;
            }
            if (child is TreeViewItem)
            {
                return child as TreeViewItem;
            }
            if (child is TreeView)
            {
                return null;
            }
            return treeView.GetTreeViewItemFromChild(VisualTreeHelper.GetParent(child));
        }

        public static void ClearTreeViewSelection(this TreeView treeView)
        {
            if (treeView != null)
            {
                treeView.Items.ClearTreeViewItemsControlSelection(treeView.ItemContainerGenerator);
            }
        }

        private static void ClearTreeViewItemsControlSelection(this ItemCollection ic, ItemContainerGenerator icg)
        {
            if (ic != null && icg != null)
            {
                for (int i = 0; i < ic.Count; i++)
                {
                    var treeViewItem = icg.ContainerFromIndex(i) as TreeViewItem;

                    if (treeViewItem != null)
                    {
                        treeViewItem.Items.ClearTreeViewItemsControlSelection(treeViewItem.ItemContainerGenerator);
                        treeViewItem.IsSelected = false;
                    }
                }
            }
        }

        /// <summary>
        /// Tries its best to return the specified element's parent. It will
        /// try to find, in this order, the VisualParent, LogicalParent, LogicalTemplatedParent.
        /// It only works for Visual, FrameworkElement or FrameworkContentElement.
        /// </summary>
        /// <param name="element">The element to which to return the parent. It will only
        /// work if element is a Visual, a FrameworkElement or a FrameworkContentElement.</param>
        /// <remarks>If the logical parent is not found (Parent), we check the TemplatedParent
        /// (see FrameworkElement.Parent documentation). But, we never actually witnessed
        /// this situation.</remarks>
        public static DependencyObject GetParent(DependencyObject element)
        {
            Visual visual = element as Visual;
            DependencyObject parent = (visual == null) ? null : VisualTreeHelper.GetParent(visual);

            if (parent == null)
            {
                // No Visual parent. Check in the logical tree.
                var fe = element as FrameworkElement;

                if (fe != null)
                {
                    parent = fe.Parent ?? fe.TemplatedParent;
                }
                else
                {
                    var fce = element as FrameworkContentElement;

                    if (fce != null)
                    {
                        parent = fce.Parent ?? fce.TemplatedParent;
                    }
                }
            }

            return parent;
        }

        /// <summary>
        /// This will search for a parent of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="startingObject">The node where the search begins. This element is not checked.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        public static T FindParent<T>(DependencyObject startingObject) where T : DependencyObject
        {
            return FindParent<T>(startingObject, false, null);
        }

        /// <summary>
        /// This will search for a parent of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="startingObject">The node where the search begins.</param>
        /// <param name="checkStartingObject">Should the specified startingObject be checked first.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        public static T FindParent<T>(DependencyObject startingObject, bool checkStartingObject) where T : DependencyObject
        {
            return FindParent<T>(startingObject, checkStartingObject, null);
        }

        /// <summary>
        /// This will search for a parent of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="startingObject">The node where the search begins.</param>
        /// <param name="checkStartingObject">Should the specified startingObject be checked first.</param>
        /// <param name="additionalCheck">Provide a callback to check additional properties
        /// of the found elements. Can be left Null if no additional criteria are needed.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        /// <example>Button button = TreeHelper.FindParent&lt;Button&gt;( this, foundChild => foundChild.Focusable );</example>
        public static T FindParent<T>(DependencyObject startingObject, bool checkStartingObject, Func<T, bool> additionalCheck) where T : DependencyObject
        {
            DependencyObject parent = (checkStartingObject ? startingObject : GetParent(startingObject));

            while (parent != null)
            {
                var foundElement = parent as T;

                if (foundElement != null)
                {
                    if (additionalCheck == null)
                    {
                        return foundElement;
                    }
                    if (additionalCheck(foundElement))
                    {
                        return foundElement;
                    }
                }

                parent = GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// This will search for a child of the specified type. The search is performed
        /// hierarchically, breadth first (as opposed to depth first).
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="parent">The root of the tree to search for. This element itself is not checked.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            return FindChild<T>(parent, null);
        }

        /// <summary>
        /// This will search for a child of the specified type. The search is performed
        /// hierarchically, breadth first (as opposed to depth first).
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="parent">The root of the tree to search for. This element itself is not checked.</param>
        /// <param name="additionalCheck">Provide a callback to check additional properties
        /// of the found elements. Can be left Null if no additional criteria are needed.</param>
        /// <returns>Returns the found element. Null if nothing is found.</returns>
        /// <example>Button button = TreeHelper.FindChild&lt;Button&gt;( this, foundChild => foundChild.Focusable );</example>
        public static T FindChild<T>(DependencyObject parent, Func<T, bool> additionalCheck) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            T child;

            for (int index = 0; index < childrenCount; index++)
            {
                child = VisualTreeHelper.GetChild(parent, index) as T;

                if (child != null)
                {
                    if (additionalCheck == null)
                    {
                        return child;
                    }
                    if (additionalCheck(child))
                    {
                        return child;
                    }
                }
            }

            for (int index = 0; index < childrenCount; index++)
            {
                child = FindChild(VisualTreeHelper.GetChild(parent, index), additionalCheck);

                if (child != null)
                    return child;
            }

            return null;
        }

        /// <summary>
        /// Returns true if the specified element is a child of parent somewhere in the visual
        /// tree. This method will work for Visual, FrameworkElement and FrameworkContentElement.
        /// </summary>
        /// <param name="element">The element that is potentially a child of the specified parent.</param>
        /// <param name="parent">The element that is potentially a parent of the specified element.</param>
        public static bool IsDescendantOf(DependencyObject element, DependencyObject parent)
        {
            while (element != null)
            {
                if (Equals(element, parent))
                {
                    return true;
                }
                element = GetParent(element);
            }

            return false;
        }
    }
}