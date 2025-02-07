using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using UIFramework.PropertyGrid;
using System.Threading;

namespace SET_MAIN_DETAIL
{
    public partial class DisplayRebarCages : Form
    {

        #region Private Fields
        private Dictionary<string, RebarCages> _rebarCagesDict = new Dictionary<string, RebarCages>();
        private List<RebarCages> _rebarCagesList;
        private RebarCages _currentRebarCages;
        private TreeNode _currentTreeNode;
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
                treeNode.Text = $"{a.CageName}";
                treeNode.Tag = a;

                treeView.Nodes.Add(treeNode);
            });
            RefreshElements();
        }
        #endregion

        private void AfterSelect_Event(object sender, TreeViewEventArgs e)
        {
            propertyGrid.SelectedObject = e.Node.Tag;

            if(e.Node.Parent == null ) 
            {
                _currentRebarCages = _rebarCagesDict[e.Node.Text];
                _currentTreeNode = e.Node;
            }

            propertyGrid.Refresh();
        }

        private void SetMaineOneCage_button_Click(object sender, System.EventArgs e)
        {
            if (_currentTreeNode.Parent != null) return;
            this.Hide();
            _currentRebarCages = _setManeOneCage_EventHandler?.Raise(this, _currentRebarCages);

            int numb = 0;
            _currentTreeNode.Nodes.Clear();
            _currentRebarCages?.oneRebarCagesList.ForEach(a =>
            {
                numb ++;
                TreeNode treeNode = new TreeNode();
                treeNode.Text = $"Каркас {a.CageName} ({numb})";
                treeNode.Tag = a;

                _currentTreeNode.Nodes.Add(treeNode);
            });
            RefreshElements();
            RefreshElements();

        }

        public void RefreshElements()
        {
            treeView.Refresh();
            propertyGrid.Refresh();
        }
    }
}
