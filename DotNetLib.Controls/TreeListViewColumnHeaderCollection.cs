/***************************************************************************\
|  Author:  Josh Carlson                                                    |
|                                                                           |
|  This work builds on code posted to CodeProject                           |
|  Jon Rista http://codeproject.com/cs/miscctrl/extendedlistviews.asp       |
|  and also updates by                                                      |
|  Bill Seddon http://codeproject.com/cs/miscctrl/Extended_List_View_2.asp  |
|                                                                           |
|  This code is provided "as is" and no warranty about its fitness for any  |
|  specific task is expressed or implied.  If you choose to use this code,  |
|  you do so at your own risk.                                              |
\***************************************************************************/

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace DotNetLib.Controls
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="TreeListViewColumnHeader"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>TreeListViewColumnHeaderCollection</b> provides an <see cref="ArrayList"/> 
	/// that is strongly typed for <see cref="TreeListViewColumnHeader"/> elements.
	/// </remarks>    
	public sealed class TreeListViewColumnHeaderCollection : IList, ICollection
	{
		#region Variables

		private TreeListView _listView;
		private ArrayList _physicalData = new ArrayList(); // actual ordering of columns
		private ArrayList _logicalData = new ArrayList(); // display ordering of columns

		#endregion

		#region Constructors

		internal TreeListViewColumnHeaderCollection(TreeListView listView)
		{
			_listView = listView;
            _listView.Resize += OnTreeListViewResized;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the <see cref="TreeListViewColumnHeader"/> at the specified index.
		/// In C#, this property is the indexer for the <b>TreeListViewColumnHeaderCollection</b> class.
		/// </summary>
		/// <value>The <see cref="TreeListViewColumnHeader"/> at the specified index.</value>
		public TreeListViewColumnHeader this[int index]
		{
			get
			{ 
				return _physicalData[index] as TreeListViewColumnHeader;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "TreeListView cannot contain null TreeListViewColumnHeaders");

				if(value == _physicalData[index])
					return;

				TreeListViewColumnHeader currentColumn = this[index];

				if(value.Collection == this) // incoming column is part of this collection already
				{
					// just swap them, display index will stay the same
					_physicalData[value.Index] = currentColumn;
					_physicalData[index] = value;
				}
				else
				{
					// let the current column know they they don't belong to us anymore
					currentColumn.Collection = null;

					if(value.Collection != null) // incoming column is part of a different collection, remove it from that collection
						value.Collection.Remove(value);

					_physicalData[index] = value;
					_logicalData[index] = value;

					value.Collection = this;
				}
			}
		}

		/// <summary>
		/// Gets the number of <see cref="TreeListViewColumnHeader"/> in this collection.
		/// </summary>
		public int Count
		{
			get
			{
				return _physicalData.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="TreeListViewColumnHeaderCollection"/> has a fixed size.
		/// </summary>
		/// <value>This property is always <b>false</b>.</value>
		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="TreeListViewColumnHeaderCollection"/> is read-only.
		/// </summary>
		/// <value>This property is always <b>false</b>.</value>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="TreeListViewColumnHeaderCollection"/>.
		/// </summary>
		/// <value>An object that can be used to synchronize access to the <see cref="TreeListViewColumnHeaderCollection"/>.</value>
		public object SyncRoot
		{
			get
			{
				return _physicalData.SyncRoot;
			}
		}

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="TreeListViewColumnHeaderCollection"/> is synchronized.
		/// </summary>
		/// <value>This property is always <b>false</b>.</value>
		public bool IsSynchronized
		{
			get
			{
				return _physicalData.IsSynchronized;
			}
		}

		/// <summary>
		/// Gets the <see cref="TreeListView"/> that this collection is attached to.
		/// </summary>
		public TreeListView ListView
		{
			get
			{
				return _listView;
			}
		}

		#endregion

		#region Add/AddRange

		/// <summary>
		/// Adds an existing <see cref="TreeListViewColumnHeader"/> object to the collection.
		/// </summary>
		/// <param name="column">The <b>TreeListViewColumnHeader</b> object to add to the collection.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		public int Add(TreeListViewColumnHeader column)
		{
			int index = _physicalData.Count;

			Insert(index, column);

			return index;
		}

		/// <summary>
		/// Adds a column to the collection with the specified text.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was added to the collection.</returns>
		public TreeListViewColumnHeader Add(string text)
		{
			return Insert(_physicalData.Count, text);
		}

		/// <summary>
		/// Adds a column to the collection with the specified text and width.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was added to the collection.</returns>
		public TreeListViewColumnHeader Add(string text, int width)
		{
			return Insert(_physicalData.Count, text, width);
		}

		// Aulofee customization - start. Method added (can directly specify the sort type)
		/// <summary>
		/// Adds a column to the collection with the specified text and width.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="sortDataType">The data type to use to sort the column.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was added to the collection.</returns>
		public TreeListViewColumnHeader Add(string text, int width, SortDataType sortDataType)
		{
			return Insert(_physicalData.Count, text, width, sortDataType);
		}
		// Aulofee customization - end
		
		/// <summary>
		/// Adds a column to the collection with the specified properties.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="horizontalAlign">The horizontal alignment, will default vertical alignment to middle.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was added to the collection.</returns>
		public TreeListViewColumnHeader Add(string text, int width, HorizontalAlignment horizontalAlign)
		{
			return Insert(_physicalData.Count, text, width, horizontalAlign);
		}

		/// <summary>
		/// Adds a column to the collection with the specified properties.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="contentAlign">The content alignment.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was added to the collection.</returns>
		public TreeListViewColumnHeader Add(string text, int width, ContentAlignment contentAlign)
		{
			return Insert(_physicalData.Count, text, width, contentAlign);
		}

		/// <summary>
		/// Adds an array of <see cref="TreeListViewColumnHeader"/> objects to the collection.
		/// </summary>
		/// <param name="columns">An array of <see cref="TreeListViewColumnHeader"/> objects to add to the collection.</param>
		public void AddRange(TreeListViewColumnHeader[] columns)
		{
            if (columns == null)
                return;

			_listView.BeginUpdate();

			lock(_physicalData.SyncRoot)
				for(int index = 0; index < columns.Length; ++index)
					Add(columns[index]);

			_listView.EndUpdate();
		}

		#endregion

		#region Insert

		/// <summary>
		/// Inserts an existing <see cref="TreeListViewColumnHeader"/> into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="column">The <see cref="TreeListViewColumnHeader"/> that represents the column to insert.</param>
		public void Insert(int index, TreeListViewColumnHeader column)
		{
            if (column == null)
                throw new ArgumentNullException("column");

			lock(_physicalData.SyncRoot)
			{
				_physicalData.Insert(index, column);
				_logicalData.Add(column);
				column.Collection = this;
			}

            if (ListView != null)
            {
                //create a new subitem
                foreach (TreeListViewItem item in ListView.Items)
                    item.SubItems.InternalInsert(index, new TreeListViewSubItem(index));
                //recalculate layout and redraw
                ListView.ColumnInvalidated(column, true, true);
            }
		}

		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was inserted into the collection.</returns>
		public TreeListViewColumnHeader Insert(int index, string text)
		{
			TreeListViewColumnHeader column = new TreeListViewColumnHeader(text);

			Insert(index, column);

			return column;
		}

		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was inserted into the collection.</returns>
		public TreeListViewColumnHeader Insert(int index, string text, int width)
		{
			TreeListViewColumnHeader column = new TreeListViewColumnHeader(text, width);

			Insert(index, column);

			return column;
		}

		// Aulofee customization - start. Method added (can directly specify the sort type)
		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="sortDataType">The data type to use to sort the column.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was inserted into the collection.</returns>
		public TreeListViewColumnHeader Insert(int index, string text, int width, SortDataType sortDataType)
		{
			TreeListViewColumnHeader column = new TreeListViewColumnHeader(text, width);
			column.SortDataType = sortDataType;

			Insert(index, column);

			return column;
		}
		// Aulofee customization - end

		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="horizontalAlign">The horizontal alignment of the text, will default vertical alignment to middle.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was inserted into the collection.</returns>
		public TreeListViewColumnHeader Insert(int index, string text, int width, HorizontalAlignment horizontalAlign)
		{
			TreeListViewColumnHeader column = new TreeListViewColumnHeader(text, width, horizontalAlign);

			Insert(index, column);

			return column;
		}

		/// <summary>
		/// Creates a new header and inserts it into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the column is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="width">The starting width.</param>
		/// <param name="contentAlign">The content alignment.</param>
		/// <returns>The <see cref="TreeListViewColumnHeader"/> that was inserted into the collection.</returns>
		public TreeListViewColumnHeader Insert(int index, string text, int width, ContentAlignment contentAlign)
		{
			TreeListViewColumnHeader column = new TreeListViewColumnHeader(text, width, contentAlign);

			Insert(index, column);

			return column;
		}

		#endregion

		#region Remove/RemoveAt

		/// <summary>
		/// Removes the specified <see cref="TreeListViewColumnHeader"/> from the collection.
		/// </summary>
		/// <param name="column">A <see cref="TreeListViewColumnHeader"/> to remove from the collection.</param>
		public void Remove(TreeListViewColumnHeader column)
		{
			Remove(column, false);
		}

		/// <summary>
		/// Removes the specified <see cref="TreeListViewColumnHeader"/> from the collection.
		/// </summary>
		/// <param name="column">A <see cref="TreeListViewColumnHeader"/> to remove from the collection.</param>
		/// <param name="removeSubItems"><b>True</b> if you want to remove the subitems, <b>false</b> otherwise.</param>
		public void Remove(TreeListViewColumnHeader column, bool removeSubItems)
		{
            if (column == null)
                return;

			lock(_physicalData.SyncRoot)
			{
                if (_listView != null)
                {
                    if (removeSubItems)
                    {
                        //remove subitems
                        foreach (TreeListViewItem item in _listView.Items)
                            item.RemoveSubItem(column.Index);
                    }
                    else
                    {
                        //hide subitem controls
                        foreach (TreeListViewItem item in _listView.Items)
                        {
                            Control subItemControl = item.SubItems[column.Index].ItemControl;
                            if (subItemControl != null)
                                subItemControl.Visible = false;
                        }
                    }
                }
				_physicalData.Remove(column);
				_logicalData.Remove(column);

				column.Collection = null;
			}

            if (ListView != null)
                ListView.ColumnInvalidated(column, true, true);
		}

		/// <summary>
		/// Removes the column at the specified location.
		/// </summary>
		/// <param name="index">The zero-based index of the column you want to remove.</param>
		public void RemoveAt(int index)
		{
			RemoveAt(index, false);
		}

		/// <summary>
		/// Removes the column at the specified location.
		/// </summary>
		/// <param name="index">The zero-based index of the column you want to remove.</param>
		/// <param name="removeSubItems"><b>True</b> if you want to remove the subitems, <b>false</b> otherwise.</param>
		public void RemoveAt(int index, bool removeSubItems)
		{
			Remove(this[index], removeSubItems);
		}

		#endregion

		#region Others

        private void OnTreeListViewResized(object sender, EventArgs e)
        {
            TreeListViewColumnHeader headerAutoSize = null;

            foreach (TreeListViewColumnHeader header in _physicalData)
            {
                if (header.WidthBehavior == ColumnWidthBehavior.Fill)
                {
                    headerAutoSize = header;
                    break;
                }
            }

            if (headerAutoSize != null)
            {
                int cumulativeColumnWidth = 0;
                for (int x = 0; x < _physicalData.Count; ++x)
                {
                    if (_physicalData[x] != headerAutoSize)
                        cumulativeColumnWidth += (_physicalData[x] as TreeListViewColumnHeader).Width;
                }

                int autoSizeWidth = _listView.Width - cumulativeColumnWidth - 2;
                headerAutoSize.Width = autoSizeWidth > headerAutoSize.MinimumWidth ? autoSizeWidth : headerAutoSize.MinimumWidth;
            }
        }

        /// <summary>
        /// Returns the index within the collection of the specified column.
        /// </summary>
        /// <param name="column">A <see cref="TreeListViewColumnHeader"/> representing the column to locate in the collection.</param>
        /// <returns>The zero-based index of the column's location in the collection.  If the column is not located in the collection the return value is negative one (-1).</returns>
        public int IndexOf(TreeListViewColumnHeader column)
		{
			return _physicalData.IndexOf(column);
		}

		/// <summary>
		/// Returns the display index of the specified column.
		/// </summary>
		/// <param name="column">A <see cref="TreeListViewHeader"/> representing the column to locate.</param>
		/// <returns>The zero-based display index of the column's location in the collection.  If the column is not located in the collection the return value is negative one (-1).</returns>
		public int DisplayIndexOf(TreeListViewColumnHeader column)
		{
			return _logicalData.IndexOf(column);
		}

		/// <summary>
		/// Determines whether the specified column is located in the collection.
		/// </summary>
		/// <param name="column">A <see cref="TreeListViewColumnHeader"/> representing the column to locate in the collection.</param>
		/// <returns><b>true</b> if the column is contained in the collection; otherwise, <b>false</b>.</returns>
		public bool Contains(TreeListViewColumnHeader column)
		{
			return _physicalData.Contains(column);
		}

		/// <summary>
		/// Removes all columns from the collection.
		/// </summary>
		public void Clear()
		{
			_listView.BeginUpdate();

			lock(_physicalData.SyncRoot)
			{
				for(int index = 0; index < _physicalData.Count; ++index)
					this[index].Collection = null;

				_physicalData.Clear();
				_logicalData.Clear();
			}

			_listView.EndUpdate();
		}

		/// <summary>
		/// Copies the entire collection into an existing array at a specified location within the array.
		/// </summary>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The zero-based relative index in <em>array</em> at which copying begins.</param>
		public void CopyTo(TreeListViewItem[] array, int arrayIndex)
		{
			_physicalData.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator"/> for the collection.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> for the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return _physicalData.GetEnumerator();
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator"/> for the collection ordered by the display index.
		/// </summary>
		/// <returns>An <see cref="IEnumerator"/> for the collection ordered by the display index.</returns>
		public IEnumerable GetDisplayOrderEnumerator()
		{
			return _logicalData;
		}

		internal void SetDisplayIndex(TreeListViewColumnHeader column, int newDisplayIndex)
		{
			if(!Contains(column))
				return;

			if(newDisplayIndex >= Count)
				newDisplayIndex = Count - 1;

			int curDisplayIndex = column.DisplayIndex;

			if(curDisplayIndex == newDisplayIndex)
				return;

			_logicalData.RemoveAt(curDisplayIndex);
			_logicalData.Insert(newDisplayIndex, column);

			if(_listView != null)
				_listView.ColumnInvalidated(null, false, true);
		}

		#endregion

		#region Explicit interface implementations

		int IList.Add(object value)
		{
			return this.Add(value as TreeListViewColumnHeader);
		}

		bool IList.Contains(object value)
		{
			return this.Contains(value as TreeListViewColumnHeader);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf(value as TreeListViewColumnHeader);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, value as TreeListViewColumnHeader);
		}

		void IList.Remove(object value)
		{
			this.Remove(value as TreeListViewColumnHeader);
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = value as TreeListViewColumnHeader;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo((TreeListViewItem[])array, index);
		}

		#endregion
	}
}
