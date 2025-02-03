using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using UIFramework.PropertyGrid;

namespace SET_MAIN_DETAIL
{
    public partial class DisplayRebarCages : Form
    {

        #region Private Fields
        private Dictionary<string, RebarCages> _rebarCagesDict = new Dictionary<string, RebarCages>();
        private List<RebarCages> _rebarCagesList;
        private SetManeOneCage_EventHandler _setManeOneCage_EventHandler;
        #endregion

        #region Constructor
        public DisplayRebarCages(Dictionary<string, RebarCages> rebarCagesDict, SetManeOneCage_EventHandler setManeOneCage)
        {
            this.TopMost = true;

            _setManeOneCage_EventHandler = setManeOneCage;
            _rebarCagesDict = rebarCagesDict;
            _rebarCagesList = rebarCagesDict.Values.ToList();
            InitializeComponent();
            InitTreeViewNodes();

            propertyGrid.PropertySort = PropertySort.NoSort;
            propertyGrid.ToolbarVisible = false;
            propertyGrid.HelpVisible = false;

            treeView.AfterSelect += AfterSelect_Event;
        }

        private void InitTreeViewNodes()
        {
            _rebarCagesList.ForEach(a =>
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Text = $"Изделие {a.CageName}";
                treeNode.Tag = a;

                treeView.Nodes.Add(treeNode);
            });
            RefreshElements();
        }
        #endregion

        private void AfterSelect_Event(object sender, TreeViewEventArgs e)
        {
            propertyGrid.SelectedObject = e.Node.Tag;
            propertyGrid.Refresh();
        }

        private void SetManeOneCage_button_Click(object sender, System.EventArgs e)
        {
            this.Hide();
            _setManeOneCage_EventHandler?.Raise(this, _rebarCagesDict);
            
        }

        public void RefreshElements()
        {
            treeView.Refresh();
            propertyGrid.Refresh();
        }
    }
}
