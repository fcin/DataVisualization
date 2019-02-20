using DataVisualization.Models.Transformations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DataVisualization.Core.Controls
{

    public partial class TransformList : UserControl
    {
        [Bindable(true)]
        public List<TransformationDefinition> Transformations
        {
            get => (List<TransformationDefinition>)GetValue(TransformationsProperty);
            set => SetValue(TransformationsProperty, value);
        }

        public static DependencyProperty TransformationsProperty = 
            DependencyProperty.Register(nameof(Transformations), typeof(List<TransformationDefinition>), typeof(TransformList)
                , new PropertyMetadata(new List<TransformationDefinition>(), new PropertyChangedCallback(OnTransformationsChanged)));

        public IEnumerable<string> AllTransformationDefinitionNames { get; set; }

        private static void OnTransformationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public TransformList()
        {
            AllTransformationDefinitionNames = TransformationDefinitionFactory.GetAllDefinitions().Select(d => d.Name);
            InitializeComponent();
        }
    }
}
