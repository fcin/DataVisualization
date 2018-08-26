using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace DataVisualization.Models
{

    public class GridColumn
    {
        public string Name { get; set; }
        public string ColumnType { get; set; }
        public bool IsIgnored { get; set; }
        public Axes Axis { get; set; }

        public GridColumn(string name, string columnType, bool isIgnored, Axes axis)
        {
            Name = name;
            ColumnType = columnType;
            IsIgnored = isIgnored;
            Axis = axis;
        }

        public GridColumn() { }
    }

    public class DataGridColumnsModel : PropertyChangedBase
    {
        private ObservableCollection<GridColumn> _columns = new ObservableCollection<GridColumn>();
        public ObservableCollection<GridColumn> Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                NotifyOfPropertyChange(() => Columns);
            }
        }

        public int GetColumnIndex(string name)
        {
            int index;
            var found = false;
            for (index = 0; index < Columns.Count; index++)
            {
                var column = Columns[index];
                if (column.Name.Equals(name))
                {
                    found = true;
                    break;
                }
            }

            return found ? index : -1;
        }
    }
}
