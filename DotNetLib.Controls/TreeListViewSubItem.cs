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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DotNetLib.Controls
{
	/// <summary>
	/// Represents a subitem of a <see cref="TreeListViewItem"/>.
	/// </summary>
	[DesignTimeVisible(false), TypeConverter("DotNetLib.Controls.TreeListViewSubItemConverter")]
	public class TreeListViewSubItem : ICloneable
	{
		#region Variables

		private TreeListViewSubItemCollection _collection;
        private int _columnIndex;

		private object _object = null;
        private string _toolTip = string.Empty;
		private Font _font;

		private Color _backColor = Color.Empty;
		private Color _foreColor = Color.Empty;

		private Size _childControlInitialSize = Size.Empty;
		private Control _childControl;
		private ControlResizeBehavior _controlResizeBehavior = ControlResizeBehavior.None;

        private Rectangle _bounds;

		private object _tag;

		#endregion

		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeListViewSubItem"/> class.
        /// </summary>
        public TreeListViewSubItem(int index)
        {
            this._columnIndex = index;
            this._object = string.Empty;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListViewSubItem"/> class.
		/// </summary>
		public TreeListViewSubItem(int index, object obj)
		{
            this._columnIndex = index;
            this._object = obj;
		}

		#endregion

		#region Properties

		#region Appearance

		/// <summary>
		/// Gets or sets the background color of the subitem.
		/// </summary>
		[
		Category("Appearance"),
		Description("The color to use to paint the back color of the sub item."),
		DefaultValue(typeof(Color), "Empty")
		]
		public Color BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				if(_backColor != value)
				{
					_backColor = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the subitem's text.
		/// </summary>
		[
		Category("Appearance"),
		Description("The color to use to paint the text of the item."),
		DefaultValue(typeof(Color), "Empty")
		]
		public Color ForeColor
		{
			get
			{
				return _foreColor;
			}
			set
			{
				if(_foreColor != value)
				{
					_foreColor = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the font of the text displayed by the subitem.
		/// </summary>
		[
		Category("Appearance"),
		Description("The font to use to draw the text of the column header."),
		AmbientValue(null)
		]
		public Font Font
		{
			get
			{
				if(_font != null)
					return _font;

				if(Item == null)
					return Control.DefaultFont;
				else
					return Item.Font;
			}
			set
			{
				if(_font != value)
				{
					_font = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the text of the subitem.  If <B>ItemControl</B> is set, this value is ignored.
		/// </summary>
		[
		Category("Appearance"),
		Description("The text of the subitem."),
		DefaultValue("")
		]
		public string Text
		{
			get
			{
				return _object.ToString();
			}
		}

        /// <summary>
        /// Gets or sets the tooltip text of the subitem.
        /// </summary>
        [
        Category("Appearance"),
        Description("The tooltip text of the subitem."),
        DefaultValue("")
        ]
        public string ToolTip
        {
            get
            {
                return _toolTip;
            }
            set
            {
                if (_toolTip != value)
                {
                    _toolTip = value + string.Empty;
                    Refresh();
                }
            }
        }

		#endregion

		#region Behavior

		/// <summary>
		/// Gets or sets the contained Control that will be drawn in this subitem
		/// </summary>
		[
		Category("Behavior"),
		Description("The control to embed in the subitem."),
		DefaultValue(null)
		]
		public Control ItemControl
		{
			get
			{
				return _childControl;
			}
			set
			{
				if(_childControl != value)
				{
					// remove current if needed
					if(_childControl != null)
					{
						_childControl.MouseDown -= new MouseEventHandler(ItemControl_MouseDown);
						_childControl.Parent = null;
					}

					// save the new control
					_childControl = value;

					if(_childControl != null)
					{
						_childControl.Visible = false;
						_childControl.MouseDown += new MouseEventHandler(ItemControl_MouseDown);
						_childControlInitialSize = _childControl.ClientSize;
					}

					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets how the sub control should resize in response to the size of its containing cell
		/// </summary>
		[
		Category("Behavior"),
		Description("The control to embed in the subitem."),
		DefaultValue(ControlResizeBehavior.BothFit)
		]
		public ControlResizeBehavior ControlResizeBehavior
		{
			get
			{
				return _controlResizeBehavior;
			}
			set
			{
				if(_controlResizeBehavior != value)
				{
					_controlResizeBehavior = value;
					Refresh();
				}

			}
		}

		#endregion

		#region Data

        /// <summary>
        /// Gets or sets the underlying object
        /// </summary>
        [Browsable(false)]
        public object Object
        {
            get { return _object; }
            set { _object = value; }
        }

        public int ColumnIndex
        {
            get { return _columnIndex; }
        }

		/// <summary>
		/// Gets or sets the user-defined data to associate with this item.
		/// </summary>
		[
		Category("Data"),
		Description("User defined data associated with the subitem.")
		]
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		#endregion

		/// <summary>
		/// Gets the <see cref="TreeListViewItem"/> this SubItem belongs to
		/// </summary>
		[
		Browsable(false)
		]
		public TreeListViewItem Item
		{
			get
			{
				if(_collection == null)
					return null;
				else
					return _collection.OwningItem;
			}
		}

		internal TreeListViewSubItemCollection Collection
		{
			get
			{
				return _collection;
			}
			set
			{
				_collection = value;
			}
		}

		internal Size ItemControlInitialSize
		{
			get
			{
				return _childControlInitialSize;
			}
		}

        internal Rectangle Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = value;
            }
        }

		#endregion

		/// <summary>
		/// Creates a close clone of this sub-item that is unattached to any item.
		/// Since a control cannot exist in two places at once, it does not copy
		/// the ItemControl property.
		/// </summary>
		/// <returns></returns>
		public TreeListViewSubItem Clone()
		{
			TreeListViewSubItem slvi = new TreeListViewSubItem(this._columnIndex);

			slvi._backColor = _backColor;
			slvi._childControlInitialSize = _childControlInitialSize;
			slvi._controlResizeBehavior = _controlResizeBehavior;
			slvi._font = _font;
			slvi._foreColor = _foreColor;
			slvi._tag = _tag;
			slvi._object = _object;
            slvi._toolTip = _toolTip;

			return slvi;
		}

		/// <summary>
		/// Forces a repaint of this subitem.
		/// </summary>
		public void Refresh()
		{
			if(Item != null)
				Item.Refresh(this);
		}

		/// <summary>
		/// Returns the value of the <b>Text</b> property or the <b>Text</b> property of the control returned by the <B>ItemControl</B> property.
		/// </summary>
		/// <returns>The text of the subitem</returns>
		public override string ToString()
		{
			return (_childControl == null ? _object.ToString() : _childControl.Text);
		}

		private void ItemControl_MouseDown(object sender, MouseEventArgs e)
		{
			if(Item != null && Item.ListView != null)
				Item.ListView.SubItemItemControlMouseDown(this);
		}

		#region ICloneable

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion

	}
}
