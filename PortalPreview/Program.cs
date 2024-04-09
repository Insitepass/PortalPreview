using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using Microsoft.Extensions.FileProviders;
using PortalPreview.Models;
using static PortalPreview.Models.DatabaseEditDashboardStorage;

var builder = WebApplication.CreateBuilder(args);


IFileProvider? fileProvider = builder.Environment.ContentRootFileProvider;
IConfiguration? configuration = builder.Configuration;


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDevExpressControls();
builder.Services.AddMvc();



// save dashaboards to sql database.
/* builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) =>
{

    DashboardConfigurator configurator = new DashboardConfigurator();
    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

    var dataBaseDashboardStoreage = new DataBaseEditaleDashboardStorage(
    configuration.GetConnectionString("Multicorp_Source"));

    configurator.SetDashboardStorage(dataBaseDashboardStoreage);


    DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
    DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source", typeof(Dashboard));

    objDataSource.DataMember = "GetDashboards";

    dataSourceStorage.RegisterDataSource("objectDataSource", objDataSource.SaveToXml());
    configurator.SetDataSourceStorage(dataSourceStorage);

    return configurator;


});
*/



//***in file storage configuration ***

builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) =>
{
   
    DashboardConfigurator configurator = new DashboardConfigurator();

    // Create and configure a dashboard storage for excel.
    DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath);
    
   // Create and configure a data source storage for excel.
    DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
   ExcelDataSourceConfigurator.ConfigureDataSource(configurator, dataSourceStorage, fileProvider);
//    //dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml());

    // Register the storage for the Web Dashboard.
    configurator.SetDataSourceStorage(dataSourceStorage);


    //save and open as xml
    configurator.SetDashboardStorage(new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));
  configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));
  return configurator;

    
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseDevExpressControls();
EndpointRouteBuilderExtension.MapDashboardRoute(app, "api/dashboard", "DefaultDashboard");


app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
