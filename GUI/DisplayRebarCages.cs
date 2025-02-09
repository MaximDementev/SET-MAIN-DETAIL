using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using UIFramework.PropertyGrid;
using System.ComponentModel;
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
        private System.Windows.Forms.Timer _timer;
        private BackgroundWorker _backgroundWorker;
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
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 100;
            _timer.Tick += (arg, ev) =>
            {
                TaskProgressLabel.Text = $"Выполнено задач {CountOfComplete} из {TaskCount}";
                if (CountOfComplete == TaskCount)
                {
                    _timer.Stop();
                    TaskCount = -1;
                    CountOfComplete = 0;
                    TaskProgressLabel.Text = "";
                }
                TaskProgressLabel.Refresh();
            };

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += (sender, e) =>
            {
                CountOfComplete = 0;
                TaskCount = 500;
                for(int i = 0; i < 500; i++)
                {
                    Thread.Sleep(300);
                    CountOfComplete++;
                }
            };
            _backgroundWorker.ReportProgress(0);
            _backgroundWorker.ProgressChanged += (sender, e) =>
            {

            };
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

        public int CountOfComplete { get; set; }
        public int TaskCount { get; set; } = -1;

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

            _setManeOneCage_EventHandler?.Raise(this, _rebarCagesDict);
            while (TaskCount == -1)
            {}
            _timer.Start();

            if (_currentTreeNode.Parent != null) return;
            this.Hide();
            _currentTreeNode.Nodes.Clear();
            _currentRebarCages = _setManeOneCage_EventHandler?.Raise(this, _currentRebarCages);

            int numb = 0;
            _currentTreeNode.Nodes.Clear();
            _currentRebarCages?.OneRebarCagesList.ForEach(a =>
            {
                numb ++;
                TreeNode treeNode = new TreeNode();
                treeNode.Text = $"Каркас {a.CageName} ({numb})";
                treeNode.Tag = a;

                _currentTreeNode.Nodes.Add(treeNode);
            });
            RefreshElements();

        }

        public void RefreshElements()
        {
            treeView.Refresh();
            propertyGrid.Refresh();
        }

        private void DisplayRebarCages_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            _backgroundWorker.RunWorkerAsync();
            _timer.Start();
        }
    }
}
