﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MinimalVS
{
	public static class VisualTreeExtensions
	{
		public static DependencyObject GetVisualOrLogicalParent(this DependencyObject sourceElement)
		{
			if (sourceElement is Visual)
				return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
			return LogicalTreeHelper.GetParent(sourceElement);
		}

		public static IEnumerable<T> FindDescendants<T>(this DependencyObject obj) where T : class
		{
			List<T> descendants = new List<T>();
            obj.TraverseVisualTree<T>(child => descendants.Add(child));
			return descendants;
		}

		public static void TraverseVisualTree<T>(this DependencyObject obj, Action<T> action) where T : class
		{
			for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
				T obj1 = child as T;
				Action<T> action1 = action;
				child.TraverseVisualTreeReverse(action1);
				if ((object)obj1 != null)
					action(obj1);
			}
		}

		public static void TraverseVisualTreeReverse<T>(this DependencyObject obj, Action<T> action) where T : class
		{
			for (int childIndex = VisualTreeHelper.GetChildrenCount(obj) - 1; childIndex >= 0; --childIndex) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
				T obj1 = child as T;
				Action<T> action1 = action;
				child.TraverseVisualTreeReverse(action1);
				if (obj1 != null)
					action(obj1);
			}
		}

		public static FrameworkElement FindElement(this Visual v, string name)
		{
			if (v == null)
				return null;
			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(v); ++i)
			{
				var child = VisualTreeHelper.GetChild(v, i) as Visual;
				if (child != null)
				{
					var e = child as FrameworkElement;
					if (e != null && e.Name == name)
						return e;
				}
				var result = FindElement(child, name);
				if (result != null)
					return result;
			}
			return null;
		}
	}
}
