using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SET_MAIN_DETAIL
{
    public partial class DisplayRebarCages : Form
    {

        private List<RebarCages> _rebarCagesList;

        public DisplayRebarCages(Dictionary<string, RebarCages> rebarCagesDict)
        {
            _rebarCagesList = rebarCagesDict.Values.ToList();
            InitializeComponent();
            InitTreeViewNodes();

            propertyGrid.PropertySort = PropertySort.NoSort;
            propertyGrid.ToolbarVisible = false;
            propertyGrid.HelpVisible = false;

            //treeView.DrawNode += TreeView_DrawNode;
            treeView.AfterSelect += AfterSelect_Event;
        }

        private void InitTreeViewNodes()
        {
            //treeView.DrawMode = TreeViewDrawMode.OwnerDrawText;

            _rebarCagesList.ForEach(a =>
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Text = $"Изделие {a.CageName}";
                treeNode.Tag = a;

                a.SimilarRebarsDict.Values.ToList().ForEach(b =>
                {
                    TreeNode node = new TreeNode();
                    node.Text = $"{b.RebarName})";
                    node.Tag = b;

                    b.RebarInstanceList.ForEach(c =>
                    {
                        TreeNode instanceNode = new TreeNode();
                        instanceNode.Text = $"{c.GetRebarName()})";
                        instanceNode.Tag = c;
                        node.Nodes.Add(instanceNode);
                    });

                    treeNode.Nodes.Add(node);
                });

                treeView.Nodes.Add(treeNode);
            });
            treeView.Refresh();
        }

        private void TreeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            //Color textColor = Color.Green;

            //if (e.Node.Text.Contains(" (Failure)"))
            //    textColor = Color.Red;
            //e.Graphics.DrawString(e.Node.Text, e.Node.NodeFont ?? ((TreeView)sender).Font, new SolidBrush(textColor), e.Bounds.Location);

            e.DrawDefault = false;
        }

        private void AfterSelect_Event(object sender, TreeViewEventArgs e)
        {
            propertyGrid.SelectedObject = e.Node.Tag;
            propertyGrid.Refresh();

            //richTextBox.Text = "";
            //if (e.Node.Tag is TestMethodResult testMethodResult)
            //    if (testMethodResult.Cause != null)
            //        richTextBox.Text = $"{testMethodResult.Cause}";
            //richTextBox.Refresh();
        }
    }
}
