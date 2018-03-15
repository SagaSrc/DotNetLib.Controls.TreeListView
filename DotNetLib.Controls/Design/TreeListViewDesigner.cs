/***************************************************************************\
|  Author:  Josh Carlson                                                    |
|                                                                           |
|  This code is provided "as is" and no warranty about                      |
|  it fitness for any specific task is expressed or                         |
|  implied.  If you choose to use this code, you do so                      |
|  at your own risk.                                                        |
\***************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace DotNetLib.Controls.Design
{
	/// <summary>
	/// Extends design-time behavior for a <see cref="TreeListView"/> control.
	/// </summary>
	public class TreeListViewDesigner : ControlDesigner
	{
		private TreeListView ListView
		{
			get
			{
				return Control as TreeListView;
			}
		}

		/// <summary>
		/// Gets the collection of components associated with the component managed by the designer.
		/// </summary>
		public override System.Collections.ICollection AssociatedComponents
		{
			get
			{
				return ListView.Columns;
			}
		}

		/// <summary>
		/// Gets the design-time verbs supported by the component that is associated with the designer.
		/// </summary>
		public override DesignerVerbCollection Verbs
		{
			get
			{
				DesignerVerb defaultListView = new DesignerVerb("ListView Default", new EventHandler(DefaultListView));
				DesignerVerb defaultTreeView = new DesignerVerb("TreeView Default", new EventHandler(DefaultTreeView));

				DesignerVerbCollection vbc = new DesignerVerbCollection();

				vbc.Add(defaultListView);
				vbc.Add(defaultTreeView);

				return vbc;
			}
		}

		/// <summary>
		/// Removes the 'Text' property from being exposed.
		/// </summary>
		/// <param name="properties"></param>
		protected override void PostFilterProperties(System.Collections.IDictionary properties)
		{
			properties.Remove("Text");

			base.PostFilterProperties(properties);
		}

		private void DefaultListView(object sender, EventArgs e)
		{
			SetTreeProperties(false);
		}

		private void DefaultTreeView(object sender, EventArgs e)
		{
			SetTreeProperties(true);
		}

		private void SetTreeProperties(bool value)
		{
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(host != null)
			{
				DesignerTransaction trans = host.CreateTransaction("Set Default Properties");

				PropertyDescriptor showPlusMinusDescriptor = TypeDescriptor.GetProperties(Component)["ShowPlusMinus"];
				PropertyDescriptor showTreeLinesDescriptor = TypeDescriptor.GetProperties(Component)["ShowTreeLines"];
				PropertyDescriptor showRootTreeLinesDescriptor = TypeDescriptor.GetProperties(Component)["ShowRootTreeLines"];

				base.RaiseComponentChanging(showPlusMinusDescriptor);
				base.RaiseComponentChanging(showTreeLinesDescriptor);
				base.RaiseComponentChanging(showRootTreeLinesDescriptor);

				ListView.ShowPlusMinus = value;
				ListView.ShowTreeLines = value;
				ListView.ShowRootTreeLines = value;

				base.RaiseComponentChanged(showPlusMinusDescriptor, null, null);
				base.RaiseComponentChanged(showTreeLinesDescriptor, null, null);
				base.RaiseComponentChanged(showRootTreeLinesDescriptor, null, null);

				trans.Commit();
			}
		}
	}
}
