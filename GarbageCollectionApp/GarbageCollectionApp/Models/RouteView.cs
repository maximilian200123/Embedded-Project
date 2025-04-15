namespace GarbageCollectionApp.Models
{
    namespace GarbageCollectionApp.Models
    {
        public class RouteViewModel
        {
            public List<Coordinate> Points { get; set; }
            public List<int> OriginalOrder { get; set; }
            public List<int> OptimizedOrder { get; set; }
            public double[][] Distances { get; set; }
            public double[][] Durations { get; set; }
            public DateTime SelectedDate { get; set; }
        }
    }

}
