using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;

namespace DataVisualization.Models
{
    public class DataGridColumnsModel : PropertyChangedBase
    {
        private ObservableCollection<Tuple<string, string, bool>> _columns = new ObservableCollection<Tuple<string, string, bool>>();
        public ObservableCollection<Tuple<string, string, bool>> Columns
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
                if (column.Item1.Equals(name))
                {
                    found = true;
                    break;
                }
            }

            return found ? index : -1;
        }
    }
}
