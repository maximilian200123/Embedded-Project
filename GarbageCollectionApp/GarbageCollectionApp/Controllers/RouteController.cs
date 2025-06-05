namespace GarbageCollectionApp.Controllers
{
    using GarbageCollectionApp.Data;
    using GarbageCollectionApp.Models;
    using GarbageCollectionApp.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class RouteController : Controller
    {
        private readonly AppDbContext db;
        private readonly LoadMatrixService matrixService;

        public RouteController(AppDbContext db, LoadMatrixService matrixService)
        {
            this.db = db;
            this.matrixService = matrixService;
        }

        public async Task<IActionResult> Optimize(DateTime date)
        {
            var startPoint = new Coordinate
            {
                Latitude = 45.7315361,
                Longitude = 24.1779393,
                Address = "SOMA HQ, Strada Șelimbărului 90, Cisnădie, Romania",
                CollectionTime = date
            };

            var finishPoint = new Coordinate
            {
                Latitude = 45.7877059,
                Longitude = 24.0247875,
                Address = "Groapa de gunoi Cristian, DN1 FN, Cristian 557085",
                CollectionTime = date
            };

            var points = await db.GarbageCollections
                .Where(g => g.CollectionTime.Date == date.Date)
                .OrderBy(g => g.CollectionTime)
                .Select(g => new Coordinate
                {
                    Latitude = g.Latitude,
                    Longitude = g.Longitude,
                    Address = g.Address,
                    CollectionTime = g.CollectionTime
                })
                .ToListAsync();

            var allPoints = new List<Coordinate> { startPoint }
                .Concat(points)
                .Append(finishPoint)
                .ToList();
            // uses LoadMatrixService to get the distance matrix
            var distances = matrixService.GetDistanceMatrix();

            // statistics will be returned as json
            var optimizedOrder = OptimizeRoute(distances);
            var originalOrder = Enumerable.Range(0, distances.Length).ToList();
            
            double originalDistance = RouteDistance(originalOrder, distances);
            double optimizedDistance = RouteDistance(optimizedOrder, distances);

            double originalTimeMin = EstimateRouteTimeMinutes(originalOrder, distances);
            double optimizedTimeMin = EstimateRouteTimeMinutes(optimizedOrder, distances);

            return Json(new
            {
                allPoints,
                optimizedOrder,
                originalDistance = originalDistance / 1000.0,
                totalDistance = optimizedDistance / 1000.0,
                originalTimeMinutes = Math.Round(originalTimeMin),
                estimatedTimeMinutes = Math.Round(optimizedTimeMin)
            });
        }

        private List<int> OptimizeRoute(double[][] distances)
        {
            var route = new List<int> { 0 };
            var unvisited = Enumerable.Range(1, distances.Length - 1).ToList();

            while (unvisited.Count > 0)
            {
                var last = route.Last();
                var next = unvisited.OrderBy(i => distances[last][i]).First();
                route.Add(next);
                unvisited.Remove(next);
            }

            return route;
        }
        private double RouteDistance(List<int> route, double[][] dist)
        {
            double sum = 0;
            for (int i = 0; i < route.Count - 1; i++)
            {
                sum += dist[route[i]][route[i + 1]];
            }
            return sum;
        }
        //method to estimate the time it takes to run the route, considering an average speed of 20 km/h
        private double EstimateRouteTimeMinutes(List<int> routeOrder, double[][] distanceMatrix, double averageSpeedKmh = 20.0)
        {
            double totalDistance = 0;
            for (int i = 0; i < routeOrder.Count - 1; i++)
            {
                totalDistance += distanceMatrix[routeOrder[i]][routeOrder[i + 1]];// add distance between consecutive points
            }

            double driveTimeMin = (totalDistance / 1000.0) / averageSpeedKmh * 60;
            // assumes 1 minute stop time at each point except the start and stop
            double collectionTime = Math.Max(0, routeOrder.Count - 2) * 1;
            return driveTimeMin + collectionTime;
        }

    }
}
