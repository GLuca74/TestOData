using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.IO;
using System;
using TestODataModels;
using Microsoft.AspNetCore.OData.Batch;

namespace TestODataServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var batchHandler = new DefaultODataBatchHandler();

            builder.Services.AddControllers().AddOData(opt => opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null)
                                                                  .AddRouteComponents("", GetEdmModel(), batchHandler));



            var app = builder.Build();

            app.UseODataBatching();
            app.UseRouting();


            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<City>("Cities");
            return builder.GetEdmModel();
        }

    }
}
