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

namespace DotNetLib.Controls
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="TreeListViewItem"/> elements
	/// that are currently selected in a <see cref="TreeListView"/>.
	/// </summary>
	/// <remarks>
	/// <b>TreeListViewSelectedItemCollection</b> provides an <see cref="ArrayList"/>
	/// that is strongly typed for <see cref="TreeListViewItem"/> elements. The items
	/// always are maintained sorted by the item's position in the list.
	/// </remarks>
	public sealed class TreeListViewSelectedItemCollection : IList, ICollection, System.Collections.Generic.IList<TreeListViewItem>, System.Collections.Generic.ICollection<TreeListViewItem>
	{
		#region Variables

		private TreeListView _listView;
		private ArrayList _data = new ArrayList();

		#endregion

		#region Constructors

		internal TreeListViewSelectedItemCollection(TreeListView listView)
		{
			_listView = listView;
		}

		#endregion

		/// <summary>
		/// Indicates the <see cref="TreeListViewItem"/> at the specified indexed
		/// location in the collection.  In C#, this property is the indexer for the
		/// <b>TreeListViewSelectedItemCollection</b> class.
		/// </summary>
		public TreeListViewItem this[int index]
		{
			get
			{
				return _data[index] as TreeListViewItem;
			}
            set
            {
                this.Insert(index, value);
            }
		}

		/// <summary>
		/// Selects existing <see cref="TreeListViewItem"/> object to the list.
		/// </summary>
		/// <param name="item">The <b>TreeListViewItem</b> object to select.</param>
		public int Add(TreeListViewItem item)
		{
            if (item == null)
                throw new ArgumentNullException("item");

			if(item.ListView != _listView)
				throw new ArgumentException("Cannot select an item that isn't part of this TreeListView", "item");

			return _data.Add(item);
		}

		/// <summary>
		/// Always throws <see cref="NotSupportedException"/>, use the Add method instead.
		/// </summary>
		public void Insert(int index, TreeListViewItem item)
		{
			throw new NotSupportedException("Cannot insert an item into the selected collection at an arbitrary location.");
		}

		/// <summary>
		/// Removes a <see cref="TreeListViewItem"/> object from the selected list.
		/// </summary>
		/// <param name="item">The <b>TreeListViewItem</b> object you want to remove from being selected.</param>
		public void Remove(TreeListViewItem item)
		{
			_data.Remove(item);
		}

		/// <summary>
		/// Adds an array of <see cref="TreeListViewItem"/> objects to the selected item collection.
		/// </summary>
		/// <param name="items">An array of <see cref="TreeListViewItem"/> objects to add to the collection.</param>
		public void AddRange(TreeListViewItem[] items)
        {
            if (items == null)
                return;

            _listView.BeginUpdate();

			lock(_data.SyncRoot)
			{
				for(int index = 0; index < items.Length; ++index)
					_data.Add(items[index]);
            }

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
		/// De-selects all the items.
		/// </summary>
		public void Clear()
		{
			_listView.ClearSelectedItems(true);
		}

		/// <summary>
		/// Gets the number of <see cref="TreeListViewItem"/> elements in this collection.
		/// </summary>
		public int Count
		{
			get
			{
				return _data.Count;
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

		internal void InternalClear()
		{
			_data.Clear();
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
				throw new NotSupportedException("Cannot insert an item at an arbitrary position.  Use Add instead.");
			}
		}

		#endregion

		#region ICollection

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			this.CopyTo((TreeListViewItem[])array, arrayIndex);
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


        #region IList<TreeListViewItem> Members of


        public void RemoveAt(int index)
        {
            _data.RemoveAt(index);
        }

        #endregion

        #region ICollection<TreeListViewItem> Members of


        public bool IsReadOnly
        {
            get { return _data.IsReadOnly; }
        }

        bool System.Collections.Generic.ICollection<TreeListViewItem>.Remove(TreeListViewItem item)
        {
            this.Remove(item);
            return true;
        }

        #endregion

        #region IEnumerable<TreeListViewItem> Members of

        void System.Collections.Generic.ICollection<TreeListViewItem>.Add(TreeListViewItem item)
        {
            this.Add(item);
        }

        public System.Collections.Generic.IEnumerator<TreeListViewItem> GetEnumerator()
        {
            for (int i = 0; i < _data.Count; i++)
                yield return _data[i] as TreeListViewItem;
        }

        #endregion
    }

}
