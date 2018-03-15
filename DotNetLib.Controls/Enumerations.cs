/***************************************************************************\
|  Author:  Josh Carlson                                                    |
|                                                                           |
|  This work builds on code posted to CodeProject                           |
|  Jon Rista http://codeproject.com/cs/miscctrl/extendedlistviews.asp       |
|  and also updates by                                                      |
|  Bill Seddon http://codeproject.com/cs/miscctrl/Extended_List_View_2.asp  |
|                                                                           |
|  This code is provided "as is" and no warranty about                      |
|  it fitness for any specific task is expressed or                         |
|  implied.  If you choose to use this code, you do so                      |
|  at your own risk.                                                        |
\***************************************************************************/

using System;

namespace DotNetLib.Controls
{
	#region SortDataType

	/// <summary>
	/// Spefifies the data type to use for sorting the column
	/// </summary>
	public enum SortDataType
	{
		/// <summary>
		/// No sorting.
		/// </summary>
		None,

		/// <summary>
		/// Sorts values as strings.
		/// </summary>
		String,

		/// <summary>
		/// Sorts values as integers.
		/// </summary>
		Integer,

		/// <summary>
		/// Sorts values as doubles.
		/// </summary>
		Double,

		/// <summary>
		/// Sorts values as DateTime.
		/// </summary>
		/// <remarks>
		/// Uses DateTime.Parse to convert the text into a DateTime.  If it fails
		/// it will revert back to string comparisons.  This will be very slow
		/// sort due to the large number of exceptions being thrown.  If you expect
		/// to have a mixture of dates and strings, write your own custom sorter.
		/// </remarks>
		Date,

		/// <summary>
		/// Indicates that the sort algorithm should use a user defined IComparer.
		/// See the <b>CustomSortComparer</b> property. Handle exceptions yourself
		/// as this control will not. Your IComparer will receive the two
		/// <see cref="TreeListViewSubItem"/>s that need to be compared.
		/// </summary>
		/// <remarks>
		/// Do not worry yourself with <b>SortOrder</b>, that will be handled by
		/// the control automatically.  Your job is to simply sort them in order.
		/// </remarks>
		Custom,
	}

	#endregion

	#region GridLines

	/// <summary>
	/// Specifies the grid line styles for controls displaying items in a table structure
	/// </summary>
	[Flags()]
	public enum GridLines
	{
		/// <summary>
		/// No grid lines
		/// </summary>
		None,

		/// <summary>
		/// Horizontal grid lines
		/// </summary>
		Horizontal,

		/// <summary>
		/// Vertical grid lines
		/// </summary>
		Vertical,

		/// <summary>
		/// Both horizontal and vertical grid lines rendered.
		/// </summary>
		Both,
	}

	#endregion

	#region NullableBoolean

	/// <summary>
	/// A boolean value that can also be defined as "not-set"
	/// </summary>
	public enum NullableBoolean
	{
		/// <summary>
		/// This value hasn't been set, and is neither true nor false
		/// </summary>
		NotSet,

		/// <summary>
		/// True value
		/// </summary>
		True,

		/// <summary>
		/// False value
		/// </summary>
		False,
	}

	#endregion

	#region ColumnWidthBehavior

	/// <summary>
	/// Specifies how the <see cref="TreeListViewColumnHeader"/> width should behave when other columns or the list changes size.
	/// </summary>
	public enum ColumnWidthBehavior
	{
		/// <summary>
		/// Will not respond to other columns being resized.
		/// </summary>
		Normal,

		/// <summary>
		/// A percentage will be caclulated when this or the width property is set and that percentage will be maintained.
		/// </summary>
		MaintainPercent,

		/// <summary>
		/// Will grow/shrink as needed to keep the list filled (will share excess evenly with other columns that have this behavior).
		/// A filled list is one that has no dead space to the right of the items and there is no horizontal scroll bar.
		/// </summary>
		Fill,
	}

	#endregion

	#region ControlResizeBehavior

	/// <summary>
	/// Specifies how the control should resize as part of a <see cref="TreeListViewSubItem" />.
	/// </summary>
	public enum ControlResizeBehavior
	{
		/// <summary>
		/// Width or height will not be altered
		/// </summary>
		None,

		/// <summary>
		/// Height will be set to fill sub item height; width won't be changed
		/// </summary>
		HeightFit,

		/// <summary>
		/// Height will be set to fill sub item height; aspect ratio will be maintained
		/// </summary>
		HeightFitMaintainRatio,

		/// <summary>
		/// Width will be set to fill sub item width; height won't be changed
		/// </summary>
		WidthFit,

		/// <summary>
		/// Width will be set to fill sub item width; aspect ratio will be maintained
		/// </summary>
		WidthFitMaintainRatio,

		/// <summary>
		/// Both height and width will be set to fill entire sub item
		/// </summary>
		BothFit,
	}

	#endregion
}