using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace USBTerminal.Core.Utils
{
    public static class VisualTreeUtils
    {

        public static bool HasVisualParent<T>(this IInputElement child) where T : DependencyObject
        {
            var hasParent = false;
            DependencyObject currParent;

            currParent = VisualTreeHelper.GetParent(child as DependencyObject);
            while (currParent != null)
            {
                if (currParent is T)
                {
                    hasParent = true;
                    break;
                }

                // find the next parent
                currParent = VisualTreeHelper.GetParent(currParent);
            }

            return hasParent;
        }


        /// <summary>
        /// Finds the parent element of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the parent element to return.</typeparam>
        /// <param name="child">The child to find the element for.</param>
        /// <returns>The parent element or null.</returns>
        public static T GetVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            T parent = null;
            DependencyObject currParent;

            currParent = VisualTreeHelper.GetParent(child);
            while (currParent != null)
            {
                if (currParent is T)
                {
                    parent = (T)currParent;
                    break;
                }

                // find the next parent
                currParent = VisualTreeHelper.GetParent(currParent);
            }

            return (parent);
        }

        /// <summary>
        /// Finds the child element of the specified type and name.
        /// If no name is given, the first child of the specified type will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the child element to return.</typeparam>
        /// <param name="parent">The parent to find the child within.</param>
        /// <param name="name">An optional name of the child element.</param>
        /// <returns>The child element or null.</returns>
        public static T GetVisualChild<T>(DependencyObject parent,
                                          string name = null) where T : DependencyObject
        {
            T result;
            int count;
            DependencyObject currChild;
            FrameworkElement currChildElement;

            count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                currChild = VisualTreeHelper.GetChild(parent, i);
                currChildElement = currChild as FrameworkElement;

                if (currChild is T)
                {
                    if (string.IsNullOrEmpty(name) == false)
                    {
                        // A name was given so the child must match it
                        if ((currChildElement != null) &&
                            (currChildElement.Name == name))
                        {
                            return ((T)currChild);
                        }
                    }
                    else
                    {
                        // No name was given but the type matches
                        return ((T)currChild);
                    }
                }

                // Recursively search children
                result = GetVisualChild<T>(currChild, name);
                if (result != null)
                {
                    return (result);
                }
            }

            return (null);
        }
    }
}
