﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataVisualization.Core.ViewModels;
using LiveCharts.Events;

namespace DataVisualization.Core.Views
{
    /// <summary>
    /// Interaction logic for VisualizerView.xaml
    /// </summary>
    public partial class VisualizerView : UserControl
    {
        public VisualizerView()
        {
            InitializeComponent();
        }

        private void Axis_OnPreviewRangeChanged(PreviewRangeChangedEventArgs eventargs)
        {
            ((VisualizerViewModel)DataContext)?.OnRangeChanged((long)eventargs.PreviewMinValue, (long)eventargs.PreviewMaxValue);
        }
    }
}
