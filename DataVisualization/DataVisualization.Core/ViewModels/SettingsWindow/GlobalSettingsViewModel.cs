using Caliburn.Micro;
using DataVisualization.Core.Translations;
using DataVisualization.Services;
using System;
using System.Collections.Generic;

namespace DataVisualization.Core.ViewModels.SettingsWindow
{
    public class GlobalSettingsViewModel : Conductor<GlobalSettingsViewModelBase>
    {
        public IEnumerable<TreeNode> Nodes { get; set; }

        public GlobalSettingsViewModel(GlobalSettings globalSettings, IEventAggregator eventAggregator)
        {
            Nodes = new List<TreeNode> {
                new TreeNode
                {
                    Name = Translation.Settings,
                    Openable = false,
                    Children = new List<TreeNode>
                    {
                        new TreeNode
                        {
                            Name = Translation.General,
                            Openable = true,
                            NodeView = new Lazy<GlobalSettingsViewModelBase>(() => new GeneralGlobalSettingsViewModel(globalSettings, eventAggregator)) },
                        new TreeNode
                        {
                            Name = Translation.Advanced,
                            Openable = true,
                            NodeView = new Lazy<GlobalSettingsViewModelBase>(() => new AdvancedGlobalSettingsViewModel(globalSettings))
                        }
                    }
                }
            };
        }

        public void NodeSelected(TreeNode selectedNode)
        {
            if (!selectedNode.Openable)
                return;

            ActivateItem(selectedNode.NodeView.Value);
        }
    }

    public class TreeNode
    {
        public string Name { get; set; }
        public bool Openable { get; set; }
        public Lazy<GlobalSettingsViewModelBase> NodeView { get; set; }
        public IEnumerable<TreeNode> Children { get; set; }
    }
}
