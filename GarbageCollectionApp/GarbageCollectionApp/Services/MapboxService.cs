namespace GarbageCollectionApp.Services
{
    using GarbageCollectionApp.Models;
    using System.Text.Json;

    public class MapboxService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private const int MaxPointsPerRequest = 25;

        public MapboxService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["Mapbox:Key"];
            Directory.CreateDirectory("App_Data");
        }

        public async Task<(double[][] distances, double[][] durations)> GetMatrix(List<Coordinate> points)
        {
            int count = points.Count;
            var distances = CreateEmptyMatrix(count);
            var durations = CreateEmptyMatrix(count);

            for (int i = 0; i < count; i++)
            {
                var source = points[i];

                var destinations = points
                    .Select((p, idx) => new { Point = p, Index = idx })
                    .Where(p => p.Index != i)
                    .Skip(i)
                    .Take(MaxPointsPerRequest)
                    .ToList();

                if (destinations.Count == 0)
                    continue;

                var batchPoints = new List<Coordinate> { source };
                batchPoints.AddRange(destinations.Select(d => d.Point));

                var (batchDistances, batchDurations) = await GetBatchMatrix(batchPoints);

                for (int j = 0; j < destinations.Count; j++)
                {
                    int destIndex = destinations[j].Index;
                    distances[i][destIndex] = batchDistances[0][j + 1];
                    durations[i][destIndex] = batchDurations[0][j + 1];
                }
            }

            return (distances, durations);
        }

        private async Task<(double[][] distances, double[][] durations)> GetBatchMatrix(List<Coordinate> batchPoints)
        {
            string coords = string.Join(";", batchPoints.Select(p => $"{p.Longitude},{p.Latitude}"));
            string url = $"https://api.mapbox.com/directions-matrix/v1/mapbox/driving/{coords}?" +
                        $"access_token={_apiKey}&annotations=distance,duration";

            var response = await _http.GetFromJsonAsync<JsonElement>(url);
            return (
                JsonSerializer.Deserialize<double[][]>(response.GetProperty("distances").ToString()),
                JsonSerializer.Deserialize<double[][]>(response.GetProperty("durations").ToString())
            );
        }

        private double[][] CreateEmptyMatrix(int size)
        {
            var matrix = new double[size][];
            for (int i = 0; i < size; i++)
            {
                matrix[i] = new double[size];
                matrix[i][i] = 0;
            }
            return matrix;
        }

        private record MatrixData(double[][] Distances, double[][] Durations);
    }
}
