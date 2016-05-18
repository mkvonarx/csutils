/* 
 * =========================================================================================================
 * 3-clause BSD license for GridHelper.cs (see http://www.opensource.org/licenses/bsd-license.php)
 * -----------------------------------------------------------------------------------------------
 * Copyright (c) 2011-2014, Markus von Arx <mkvonarx@gmail.com>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * 3. Neither the name of Markus von Arx nor the names of any contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * =========================================================================================================
 * 
 * GridHelper.cs Homepage: https://sites.google.com/site/mkvonarxcode/code/gridhelper
 * GridHelper.cs Github: https://github.com/mkvonarx/mkvonarx-csutils/blob/master/Source/WPF/GridHelper.cs
 * 
 */

/* Release Notes:
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

/* TODO: improve GridHelper
 * - support "2*"
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Mkvonarx.Utils.Wpf
{
	/// <summary>
	/// Utility class that allows to set Grid layouts with simple(r) attached properties directly on the Grid XML element instead of using the child elements Grid.RowDefinitions and Grid.ColumnDefinitions. <br/>
	/// Format: <br/>
	///		- comma separated list of rows/columns:                GridHelper.Rows="ROW1,ROW2,ROW3,..." <br/>
	///		- possible values: "*", "Auto" or number of pixels:    GridHelper.Rows="*,Auto,200" <br/>
	///     - options can be provided in parenthesis:              GridHelper.Rows="ROW1(OPTIONS1),ROW2(OPTIONS2),ROW3(OPTIONS3),..." <br/>
	///     - possible options: min/max limits, shared size group: GridHelper.Rows="*(min=100,max=200),Auto(min=100),200,Auto(SharedSizeGroup=abc)" <br/>
	/// Samples: <br/>
	///		- utils:GridHelper.Rows="Auto,*,200" <br/>
	///		- utils:GridHelper.Columns="Auto(max=500),*(min=300),100" <br/>
	/// Documentation: <br/>
	///     - GridHelper.md <br/>
	///     - http://mkvonarx.blogspot.com/2011/05/gridhelper.html <br/>
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

			var rows = ParseXmlAttribute(e.NewValue as string);
			if (rows == null || rows.Length == 0)
			{
				return;
			}

			grid.RowDefinitions.Clear();
			foreach (var row in rows)
			{
				RowDefinition newRowDefinition;
				switch (row.Type)
				{
					case RowColumnType.Auto:
						newRowDefinition = new RowDefinition { Height = GridLength.Auto };
						break;
					case RowColumnType.Exact:
						newRowDefinition = new RowDefinition { Height = new GridLength(row.ExactValue.Value) };
						break;
					default:
						//case RowColumnType.Star:
						newRowDefinition = new RowDefinition();
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

			var columns = ParseXmlAttribute(e.NewValue as string);
			if (columns == null || columns.Length == 0)
			{
				return;
			}

			grid.ColumnDefinitions.Clear();
			foreach (var column in columns)
			{
				ColumnDefinition newColumnDefinition;
				switch (column.Type)
				{
					case RowColumnType.Auto:
						newColumnDefinition = new ColumnDefinition { Width = GridLength.Auto };
						break;
					case RowColumnType.Exact:
						newColumnDefinition = new ColumnDefinition { Width = new GridLength(column.ExactValue.Value) };
						break;
					default:
						//case RowColumnType.Star:
						newColumnDefinition = new ColumnDefinition();
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
			Exact
		}

		private class RowColumnDefinition
		{
			public RowColumnType Type { get; set; }
			public double? ExactValue { get; set; }
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

			var allSubValues = xmlAttribute.Split(',');
			foreach (var subValue in allSubValues)
			{
				var subValueCopy = subValue;

				// has options?
				string options = null;
				var optionsBegin = subValueCopy.IndexOf('(');
				var optionsEnd = subValueCopy.IndexOf(')');
				if ((optionsBegin != -1) && (optionsEnd != -1) && (optionsBegin < optionsEnd))
				{
					options = subValueCopy.Substring(optionsBegin + 1, optionsEnd - optionsBegin - 1);
					subValueCopy = subValueCopy.Substring(0, optionsBegin);
				}

				// parse value
				RowColumnDefinition rowColumnDefinition = null;
				if (subValueCopy == "*")
				{
					rowColumnDefinition = new RowColumnDefinition { Type = RowColumnType.Star };
				}
				else if (string.Compare(subValueCopy, "auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					rowColumnDefinition = new RowColumnDefinition { Type = RowColumnType.Auto };
				}
				else
				{
					double exactValue;
					if (double.TryParse(subValueCopy, out exactValue))
					{
						rowColumnDefinition = new RowColumnDefinition { Type = RowColumnType.Exact, ExactValue = exactValue };
					}
					// else: ignore (skip)
				}
				if (rowColumnDefinition == null)
				{
					continue;
				}

				// parse options
				if (options != null)
				{
					var allOptions = options.Split(',');
					foreach (var option in allOptions)
					{
						var keyValuePair = option.Split('=');
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
