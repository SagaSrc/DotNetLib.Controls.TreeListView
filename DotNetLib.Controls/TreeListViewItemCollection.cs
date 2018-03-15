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
using System.Collections;
using System.Windows.Forms;

namespace DotNetLib.Controls
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="TreeListViewItem"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>TreeListViewItemCollection</b> provides an <see cref="ArrayList"/>
	/// that is strongly typed for <see cref="TreeListViewItem"/> elements.
	/// </remarks>
	public sealed class TreeListViewItemCollection : IList, ICollection
	{
		#region Variables

		private TreeListView _listView;
		private TreeListViewItem _owningItem;
		private ArrayList _data = new ArrayList();

		#endregion

		#region Constructors

		internal TreeListViewItemCollection(TreeListViewItem owningItem)
		{
			_owningItem = owningItem;
		}

		#endregion

		/// <summary>
		/// Indicates the <see cref="TreeListViewItem"/> at the specified indexed
		/// location in the collection.  In C#, this property is the indexer for the
		/// <b>TreeListViewItemCollection</b> class.
		/// </summary>
		public TreeListViewItem this[int index]
		{
			get
			{
				return _data[index] as TreeListViewItem;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "TreeListView cannot contain null TreeListViewItems");

				if(value != _data[index])
				{
					// remove the existing item
					TreeListViewItem item = this[index];
					item.OwnerListView = null;

					// and add the new item in place
					_data[index] = value;
					value.OwnerListView = _listView;
				}
			}
		}

		#region Add

		/// <summary>
		/// Adds an existing <see cref="TreeListViewItem"/> object to the collection.
		/// </summary>
		/// <param name="item">The <b>TreeListViewItem</b> object to add to the collection.</param>
		/// <returns>The position into which the new element was inserted.</returns>
		public int Add(TreeListViewItem item)
		{
			int index = _data.Count;

			Insert(index, item);

			return index;
		}

		/// <summary>
		/// Adds an item to the collection with the specified text.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <returns>The <see cref="TreeListViewItem"/> that was added to the collection.</returns>
		public TreeListViewItem Add(string text)
		{
			return Insert(_data.Count, text);
		}

		/// <summary>
		/// Adds an item to the collection with the specified text and image index.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="imageIndex">The index of the image to display.</param>
		/// <returns>The <see cref="TreeListViewItem"/> that was added to the collection.</returns>
		public TreeListViewItem Add(string text, int imageIndex)
		{
			return Insert(_data.Count, text, imageIndex);
		}

		/// <summary>
		/// Adds an item to the collection with the specified properties.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="imageIndex">The index of the image to display.</param>
		/// <param name="selectedImageIndex">The index of the image to display when the item is selected.</param>
		/// <returns>The <see cref="TreeListViewItem"/> that was added to the collection.</returns>
		public TreeListViewItem Add(string text, int imageIndex, int selectedImageIndex)
		{
			return Insert(_data.Count, text, imageIndex, selectedImageIndex);
		}

		#endregion

		#region Insert

		/// <summary>
		/// Inserts an existing <see cref="TreeListViewItem"/> object to the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the item is inserted.</param>
		/// <param name="item">The <b>TreeListViewItem</b> object to add to the collection.</param>
		public void Insert(int index, TreeListViewItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (_data.Count != 0 && index > 0)
            {
                TreeListViewItem previousItem = _data[index - 1] as TreeListViewItem;
                item.InternalPreviousItem = previousItem;
                previousItem.InternalNextItem = item;
            }
            if (_data.Count - 1 >= index)
            {
                TreeListViewItem nextItem = _data[index] as TreeListViewItem;
                item.InternalNextItem = nextItem;
            }

            lock (_data.SyncRoot)
                _data.Insert(index, item);

            item.InternalParentItem = _owningItem;
            SetTreeListViewOwnerRecursive(item, _listView);
		}

		/// <summary>
		/// Inserts an item to the collection with the specified text at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the item is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <returns>The <see cref="TreeListViewItem"/> that was added to the collection.</returns>
		public TreeListViewItem Insert(int index, string text)
		{
			TreeListViewItem item = new TreeListViewItem(text);

			Insert(index, item);

			return item;
		}

		/// <summary>
		/// Inserts an item to the collection with the specified text and image index at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the item is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="imageIndex">The index of the image to display.</param>
		/// <returns>The <see cref="TreeListViewItem"/> that was added to the collection.</returns>
		public TreeListViewItem Insert(int index, string text, int imageIndex)
		{
			TreeListViewItem item = new TreeListViewItem(text, imageIndex);

			Insert(index, item);

			return item;
		}

		/// <summary>
		/// Inserts an item to the collection with the specified properties at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index location where the item is inserted.</param>
		/// <param name="text">The text to display.</param>
		/// <param name="imageIndex">The index of the image to display.</param>
		/// <param name="selectedImageIndex">The index of the image to display when the item is selected.</param>
		/// <returns>The <see cref="TreeListViewItem"/> that was added to the collection.</returns>
		public TreeListViewItem Insert(int index, string text, int imageIndex, int selectedImageIndex)
		{
			TreeListViewItem item = new TreeListViewItem(text, imageIndex, selectedImageIndex);

			Insert(index, item);

			return item;
		}

		#endregion

		/// <summary>
		/// Removes the specified <see cref="TreeListViewItem"/> from the collection.
		/// </summary>
		/// <param name="item">A <see cref="TreeListViewItem"/> to remove from the collection.</param>
		/// <custom type="modified">Fixed bugs (the method was throwing exceptions) and improved the method.</custom>
        public void Remove(TreeListViewItem item)
        {
			// Aulofee customization - start. Bug fixed + improvments (initial method is at the end)
			if (item == null)
				return;

        	// Aulofee customization - start. Line below added.
        	_listView.BeginUpdate();

            TreeListViewItem recalculateFrom = RemoveInternal(item);

            _listView.RecalculateItemPositions(recalculateFrom);

            // Aulofee customization - start. Line below added.
        	_listView.EndUpdate();
        }

        /// <summary>
        /// Removes the specified collection of items from the collection.
        /// </summary>
        /// <param name="item">A <see cref="TreeListViewItemCollection"/> to remove from the collection.</param>
        public void RemoveRange(TreeListViewItemCollection items)
        {
            if (items == null)
                return;

        	// Aulofee customization - start. Line below added.
        	_listView.BeginUpdate();

            TreeListViewItem recalculateFrom = null;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                TreeListViewItem newRecalculateFrom = RemoveInternal(items[i]);
                if (recalculateFrom == null || newRecalculateFrom.Y < recalculateFrom.Y)
                    recalculateFrom = newRecalculateFrom;
            }

            _listView.RecalculateItemPositions(recalculateFrom);

        	// Aulofee customization - start. Line below added.
        	_listView.EndUpdate();
        }

        /// <summary>
        /// Removes the specified collection of items from the collection.
        /// </summary>
        /// <param name="item">A list of items to remove from the collection.</param>
        public void RemoveRange(System.Collections.Generic.IList<TreeListViewItem> items)
        {
            if (items == null)
                return;

        	// Aulofee customization - start. Line below added.
        	_listView.BeginUpdate();

            TreeListViewItem recalculateFrom = null;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                TreeListViewItem newRecalculateFrom = RemoveInternal(items[i]);
                if (recalculateFrom == null || newRecalculateFrom.Y < recalculateFrom.Y)
                    recalculateFrom = newRecalculateFrom;
            }

            _listView.RecalculateItemPositions(recalculateFrom);

        	// Aulofee customization - start. Line below added.
        	_listView.EndUpdate();
        }

        internal TreeListViewItem RemoveInternal(TreeListViewItem item)
        {
            TreeListViewItem recalculateFrom = null;
            if (item.InternalPreviousItem != null && item.InternalNextItem != null)
            {
                item.InternalPreviousItem.InternalNextItem = item.InternalNextItem;
                item.InternalNextItem.InternalPreviousItem = item.InternalPreviousItem;
                recalculateFrom = item.InternalPreviousItem;
            }
            else if (item.InternalPreviousItem != null)
            {
                item.InternalPreviousItem.InternalNextItem = null;
                recalculateFrom = item.InternalPreviousItem;
            }
            else if (item.InternalNextItem != null)
            {
                item.InternalNextItem.InternalPreviousItem = null;
            }

            lock (_data.SyncRoot)
                _data.Remove(item);

            item.Selected = false;
            item.Focused = false;
            item.OwnerListView = null;
            item.InternalParentItem = null;

            foreach (TreeListViewSubItem si in item.SubItems)
                if (si.ItemControl != null)
                    si.ItemControl.Visible = false;

            return recalculateFrom;
        }

		/// <summary>
		/// Adds an array of <see cref="TreeListViewItem"/> objects to the collection.
		/// </summary>
		/// <param name="items">An array of <see cref="TreeListViewItem"/> objects to add to the collection.</param>
		public void AddRange(TreeListViewItem[] items)
		{
            if (items == null)
                return;

			_listView.BeginUpdate();

			lock(_data.SyncRoot)
				for(int index = 0; index < items.Length; ++index)
					Add(items[index]);

			_listView.EndUpdate();
		}

		/// <summary>
		/// Returns the index within the collection of the specified item.
		/// </summary>
		/// <param name="item">A <see cref="TreeListViewItem"/> representing the item to locate in the collection.</param>
		/// <returns>The zero-based index of the item's location in the collection.  If the item is not located in the collection the return value is negative one (-1).</returns>
		public int IndexOf(TreeListViewItem item)
		{
			return _data.IndexOf(item);
		}

		/// <summary>
		/// Determines whether the specified item is located in the collection.
		/// </summary>
		/// <param name="item">A <see cref="TreeListViewItem"/> representing the item to locate in the collection.</param>
		/// <returns><b>true</b> if the column is contained in the collection; otherwise, <b>false</b>.</returns>
		public bool Contains(TreeListViewItem item)
		{
			return _data.Contains(item);
		}

		/// <summary>
		/// Removes all items from the collection.
		/// </summary>
		public void Clear()
		{
			_listView.BeginUpdate();

            //avoid firing one SelectedItemsChanged event for every selected item
            if (_listView.SelectedItems.Count > 0)
                _listView.SelectedItems.Clear();

			for(int index = 0; index < _data.Count; ++index)
			{
				TreeListViewItem item = this[index];

				item.Selected = false;
				item.Focused = false;
				item.OwnerListView = null;

				for(int subIndex = 0; subIndex < item.SubItems.Count; ++subIndex)
				{
					if (item.SubItems[subIndex].ItemControl != null)
					{
						item.SubItems[subIndex].ItemControl.Parent = null;
						item.SubItems[subIndex].ItemControl.Visible = false;
						item.SubItems[subIndex].ItemControl = null;
					}
				}
			}
			_data.Clear();

			_listView.EndUpdate();
		}

		/// <summary>
		/// Sorts the elements using the specified comparer.
		/// </summary>
		/// <param name="comparer">The <see cref="IComparer"/> to use when comparing elements.</param>
		/// <param name="recursiveSort">Whether to sort these items child items as well.</param>
		public void Sort(IComparer comparer, bool recursiveSort)
		{
			try
			{
				_data.Sort(comparer);

				TreeListViewItem lastItem = null;
				TreeListViewItem curItem = null;

				for(int index = 0; index < _data.Count; ++index)
				{
					curItem = this[index];

					curItem.InternalPreviousItem = lastItem;
					if(lastItem != null)
						lastItem.InternalNextItem = curItem;

					lastItem = curItem;

					if(recursiveSort && curItem.HasChildren)
						curItem.Items.Sort(comparer, recursiveSort);
				}

				if(curItem != null)
					curItem.InternalNextItem = null;
			}
			catch
			{
				// TODO: should likely refine this and determine the cause of the error and handle appropriately.
			}
		}

		/// <summary>
		/// Copies the entire collection into an existing array at a specified location within the array.
		/// </summary>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The zero-based relative index in <em>array</em> at which copying begins.</param>
		public void CopyTo(TreeListViewItem[] array, int arrayIndex)
		{
			_data.CopyTo(array, arrayIndex);
		}

        internal void SetTreeListViewOwnerRecursive(TreeListViewItem item, TreeListView treeListView)
        {
            if (item.Items.Count > 0)
            {
                foreach (TreeListViewItem childItem in item.Items)
                    SetTreeListViewOwnerRecursive(childItem, treeListView);
            }

            item.OwnerListView = treeListView;
        }

		/// <summary>
		/// Gets the number of items in this collection
		/// </summary>
		public int Count
		{
			get
			{
				return _data.Count;
			}
		}

		#region IList

		int IList.Add(object value)
		{
			return this.Add(value as TreeListViewItem);
		}

		bool IList.Contains(object value)
		{
			return this.Contains(value as TreeListViewItem);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf(value as TreeListViewItem);
		}

		void IList.Insert(int index, object value)
		{
			this.Insert(index, value as TreeListViewItem);
		}

		void IList.Remove(object value)
		{
			this.Remove(value as TreeListViewItem);
		}

		void IList.RemoveAt(int index)
		{
			_data.RemoveAt(index);
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = value as TreeListViewItem;
			}
		}

		#endregion

		#region ICollection

		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyTo((TreeListViewItem[])array, index);
		}

		object ICollection.SyncRoot
		{
			get
			{
				return _data.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return _data.IsSynchronized;
			}
		}

		#endregion

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		#endregion

		internal TreeListView InternalListView
		{
			set
			{
				_listView = value;
			}
		}
	}
}
