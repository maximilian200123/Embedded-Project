namespace GarbageCollectionApp.Controllers
{
    using GarbageCollectionApp.Data;
    using GarbageCollectionApp.Models;
    using GarbageCollectionApp.Models.GarbageCollectionApp.Models;
    using GarbageCollectionApp.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class RouteController : Controller
    {
        private readonly MapboxService _mapbox;
        private readonly AppDbContext _db;

        public RouteController(MapboxService mapbox, AppDbContext db)
        {
            _mapbox = mapbox;
            _db = db;
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

            var points = await _db.GarbageCollections
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

            var allPoints = new List<Coordinate> { startPoint }.Concat(points).Append(finishPoint).ToList();

            var (distances, durations) = await _mapbox.GetMatrix(points);
            var optimizedOrder = OptimizeRoute(distances);

            return Json(new
            {
                points,
                optimizedOrder,
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
    }
}
