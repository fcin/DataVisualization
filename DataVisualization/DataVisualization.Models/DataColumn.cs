namespace DataVisualization.Models
{
    public class DataColumn
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public ColumnTypeDef ColumnType { get; set; }
        public Axes Axis { get; set; }
    }
}
