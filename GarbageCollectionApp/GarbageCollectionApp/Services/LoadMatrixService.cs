using System.Data;
using System.Globalization;
using ExcelDataReader;

namespace GarbageCollectionApp.Services
{
    public class LoadMatrixService
    {
        private readonly double[][] distanceMatrix;

        public LoadMatrixService(IWebHostEnvironment env)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var path = Path.Combine(env.ContentRootPath, "App_Data/distance_matrix.xlsx");
            using var stream = File.OpenRead(path);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var table = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            }).Tables[0];

            int size = table.Rows.Count;
            distanceMatrix = new double[size][];

            for (int i = 0; i < size; i++)
            {
                distanceMatrix[i] = new double[size];
                for (int j = 0; j < size; j++)
                {
                    // skip first column containing row names
                    string raw = table.Rows[i][j + 1].ToString().Replace(",", ".");
                    distanceMatrix[i][j] = double.Parse(raw, CultureInfo.InvariantCulture);
                }
            }

        }

        public double[][] GetDistanceMatrix() => distanceMatrix;
    }
}
