namespace DataVisualization.Models
{
    public class PullingMethodProperties
    {
        public PullingMethods Method { get; set; }
        public string EndpointUrl { get; set; }
    }

    public enum PullingMethods
    {
        LocalFile, HttpJson
    }
}
