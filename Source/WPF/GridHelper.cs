using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

// Copyright (c) 2011-2020, Markus von Arx
// All rights reserved.
// This source code is licensed under the 3-clause BSD license found here https://opensource.org/licenses/BSD-3-Clause 

// GridHelper.cs Github: https://github.com/mkvonarx/mkvonarx-csutils/blob/master/Source/WPF/GridHelper.cs

/* Release Notes:
 *
 * v5 (10 March 2020)
 * - support multiple star values (e.g. 2*)
 *
 * v4 (18 May 2016):
 * - added SharedSizeGroup=aaa option
 * 
 * v3 (16 February 2012):
 * - fixed MaxValue->MaxHeight/MaxWidth assignement
 *
 * v2 (17 Mai 2011):
 * - refactored string parsing
 *
 * v1 (15 Mai 2011):
 * - initial version
 */

namespace Mkvonarx.Utils.Wpf
{
	/// <summary>
	/// Utility class that allows to set Grid layouts with simple(r) attached properties directly on the Grid XML element instead of using the child elements Grid.RowDefinitions and Grid.ColumnDefinitions. <br/>
	/// Format: <br/>
	///		- comma separated list of rows/columns:                                      GridHelper.Rows="ROW1,ROW2,ROW3,..." <br/>
	///		- supported size values:                  "*", "2*", "Auto" or "200":        GridHelper.Rows="*,Auto,200,2*" <br/>
	///     - options can be provided in parenthesis:                                    GridHelper.Rows="ROW1(OPTIONS1),ROW2(OPTIONS2),ROW3(OPTIONS3),..." <br/>
	///     - supported options:                      min/max limits, shared size group: GridHelper.Rows="*(min=100,max=200),Auto(min=100),200,Auto(SharedSizeGroup=abc)" <br/>
	/// Samples: <br/>
	///		- utils:GridHelper.Rows="Auto,*,200,2*" <br/>
	///		- utils:GridHelper.Columns="Auto(max=500),*(min=300),100" <br/>
	/// Documentation: <br/>
	///     - https://github.com/mkvonarx/mkvonarx-csutils/blob/master/Source/WPF/ <br/>
	/// </summary>
	public static class GridHelper
	{
		#region Rows

		public static string GetRows(DependencyObject obj)
		{
			return (string)obj.GetValue(RowsProperty);
		}

		public static void SetRows(DependencyObject obj, string value)
		{
			obj.SetValue(RowsProperty, value);
		}

		public static DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
			"Rows",
			typeof(string),
			typeof(GridHelper),
			new PropertyMetadata("", OnRowsPropertyChanged));

		private static void OnRowsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var grid = d as Grid;
			if (grid == null)
			{
				return;
			}

			RowColumnDefinition[] rows = ParseXmlAttribute(e.NewValue as string);
			if (rows == null || rows.Length == 0)
			{
				return;
			}

			grid.RowDefinitions.Clear();
			foreach (RowColumnDefinition row in rows)
			{
				RowDefinition newRowDefinition;
				switch (row.Type)
				{
					case RowColumnType.Auto:
						newRowDefinition = new RowDefinition { Height = GridLength.Auto };
						break;
					case RowColumnType.Pixel:
						newRowDefinition = new RowDefinition { Height = new GridLength(row.Value, GridUnitType.Pixel) };
						break;
					default:
						//case RowColumnType.Star:
						newRowDefinition = new RowDefinition { Height = new GridLength(row.Value, GridUnitType.Star) };
						break;
				}
				if (row.MinValue.HasValue)
				{
					newRowDefinition.MinHeight = row.MinValue.Value;
				}
				if (row.MaxValue.HasValue)
				{
					newRowDefinition.MaxHeight = row.MaxValue.Value;
				}
				if (!string.IsNullOrEmpty(row.SharedSizeGroup))
				{
					newRowDefinition.SharedSizeGroup = row.SharedSizeGroup;
				}
				grid.RowDefinitions.Add(newRowDefinition);
			}
		}

		#endregion Rows

		#region Columns

		public static string GetColumns(DependencyObject obj)
		{
			return (string)obj.GetValue(ColumnsProperty);
		}

		public static void SetColumns(DependencyObject obj, string value)
		{
			obj.SetValue(ColumnsProperty, value);
		}

		public static DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
			"Columns",
			typeof(string),
			typeof(GridHelper),
			new PropertyMetadata("", OnColumnsPropertyChanged));

		private static void OnColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var grid = d as Grid;
			if (grid == null)
			{
				return;
			}

			RowColumnDefinition[] columns = ParseXmlAttribute(e.NewValue as string);
			if (columns == null || columns.Length == 0)
			{
				return;
			}

			grid.ColumnDefinitions.Clear();
			foreach (RowColumnDefinition column in columns)
			{
				ColumnDefinition newColumnDefinition;
				switch (column.Type)
				{
					case RowColumnType.Auto:
						newColumnDefinition = new ColumnDefinition { Width = GridLength.Auto };
						break;
					case RowColumnType.Pixel:
						newColumnDefinition = new ColumnDefinition { Width = new GridLength(column.Value, GridUnitType.Pixel) };
						break;
					default:
						//case RowColumnType.Star:
						newColumnDefinition = new ColumnDefinition { Width = new GridLength(column.Value, GridUnitType.Star) };
						break;
				}
				if (column.MinValue.HasValue)
				{
					newColumnDefinition.MinWidth = column.MinValue.Value;
				}
				if (column.MaxValue.HasValue)
				{
					newColumnDefinition.MaxWidth = column.MaxValue.Value;
				}
				if (!string.IsNullOrEmpty(column.SharedSizeGroup))
				{
					newColumnDefinition.SharedSizeGroup = column.SharedSizeGroup;
				}
				grid.ColumnDefinitions.Add(newColumnDefinition);
			}
		}

		#endregion Columns

		#region Utils

		private enum RowColumnType
		{
			Star,
			Auto,
			Pixel
		}

		private class RowColumnDefinition
		{
			public RowColumnType Type { get; set; }
			public double Value { get; set; }
			public double? MinValue { get; set; }
			public double? MaxValue { get; set; }
			public string SharedSizeGroup { get; set; }
		}

		private static RowColumnDefinition[] ParseXmlAttribute(string xmlAttribute)
		{
			if (string.IsNullOrEmpty(xmlAttribute))
			{
				return null;
			}

			var result = new List<RowColumnDefinition>();

			string[] allSubValues = xmlAttribute.Split(',');
			foreach (string subValue in allSubValues)
			{
				string subValueCopy = subValue;

				// has options?
				string options = null;
				int optionsBegin = subValueCopy.IndexOf('(');
				int optionsEnd = subValueCopy.IndexOf(')');
				if ((optionsBegin != -1) && (optionsEnd != -1) && (optionsBegin < optionsEnd))
				{
					options = subValueCopy.Substring(optionsBegin + 1, optionsEnd - optionsBegin - 1);
					subValueCopy = subValueCopy.Substring(0, optionsBegin);
				}

				// parse value
				RowColumnDefinition rowColumnDefinition = null;
				if (subValueCopy == "*")
				{
					rowColumnDefinition = new RowColumnDefinition { Type = RowColumnType.Star, Value = 1.0 };
				}
				else if (subValueCopy.EndsWith("*") && double.TryParse(subValueCopy.Substring(0, subValueCopy.Length - 1), out double value1))
				{
					rowColumnDefinition = new RowColumnDefinition { Type = RowColumnType.Star, Value = value1 };
				}
				else if (string.Compare(subValueCopy, "auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					rowColumnDefinition = new RowColumnDefinition { Type = RowColumnType.Auto };
				}
				else if (double.TryParse(subValueCopy, out double value2))
				{
					rowColumnDefinition = new RowColumnDefinition { Type = RowColumnType.Pixel, Value = value2 };
				}
				// else: ignore (skip)
				if (rowColumnDefinition == null)
				{
					continue;
				}

				// parse options
				if (options != null)
				{
					string[] allOptions = options.Split(',');
					foreach (string option in allOptions)
					{
						string[] keyValuePair = option.Split('=');
						if (keyValuePair.Length == 2)
						{
							double v;
							if ((string.Compare(keyValuePair[0], "min", StringComparison.OrdinalIgnoreCase) == 0) && double.TryParse(keyValuePair[1], out v))
							{
								rowColumnDefinition.MinValue = v;
							}
							else if ((string.Compare(keyValuePair[0], "max", StringComparison.OrdinalIgnoreCase) == 0) && double.TryParse(keyValuePair[1], out v))
							{
								rowColumnDefinition.MaxValue = v;
							}
							else if ((string.Compare(keyValuePair[0], "SharedSizeGroup", StringComparison.OrdinalIgnoreCase) == 0) && !string.IsNullOrEmpty(keyValuePair[1]))
							{
								rowColumnDefinition.SharedSizeGroup = keyValuePair[1];
							}
							// else: ignore (skip)
						}
						// else: ignore (skip)
					}
				}

				result.Add(rowColumnDefinition);
			}

			return result.ToArray();
		}

		#endregion Utils
	}
}
