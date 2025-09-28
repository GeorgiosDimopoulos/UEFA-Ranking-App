using Core.Interfaces;
using Infrastructure.DataAccess;
using Infrastructure.Helpers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddOpenApi();
builder.Services.AddControllersWithViews();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

builder.Services.AddScoped<DatabaseFeeder>();
builder.Services.AddScoped<DatabaseInitializer>();

SQLitePCL.Batteries_V2.Init();

var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDir);

var dbPath = Path.Combine(dataDir, "uefa.db");
if (string.IsNullOrEmpty(dbPath))
{
    throw new InvalidOperationException("Connection string is not set.");
}

var connectionString = $"Data Source={dbPath}";
builder.Configuration["ConnectionStrings:Default"] = connectionString;

builder.Services.AddSwaggerGen(opt =>
{
    opt.EnableAnnotations();
    opt.SwaggerDoc("v1", new() { Title = "UEFA API Endpoints", Version = "v1" });
    opt.DocInclusionPredicate((docName, apiDesc) => true);

    opt.OrderActionsBy(apiDesc =>
    {
        var group = apiDesc.GroupName ?? apiDesc.ActionDescriptor.RouteValues["controller"];
        var groupPrefix = group switch
        {
            "Countries" => "01-",
            "Teams" => "02-",
            "Matches" => "03-",
            _ => "99-",
        };

        var methodPrefix = apiDesc.HttpMethod switch
        {
            "GET" => "01-",
            "POST" => "02-",
            "PUT" => "03-",
            "PATCH" => "04-",
            "DELETE" => "05-",
            _ => "99-",
        };
        return $"{groupPrefix}{methodPrefix}-{apiDesc.RelativePath}";
    });

});

var app = builder.Build();
app.Logger.LogInformation($"SQLite DB path is: {dbPath}");

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    var databaseFeeder = scope.ServiceProvider.GetRequiredService<DatabaseFeeder>();

    databaseInitializer.EnsureCountryTableExists(connectionString);
    var recordsExist = databaseFeeder.EnsureRecordsExist(connectionString);
    if (!recordsExist)
    {
        await databaseFeeder.SeedCountriesAndTeams();
    }
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.ConfigObject.AdditionalItems["operationsSorter"] = "(a,b)=>{const o={get:1,post:2,put:3,patch:4,delete:5};return o[a.get('method')]-o[b.get('method')];}";
        c.ConfigObject.AdditionalItems["tagsSorter"] = "(a,b)=>{const order=['Countries','Teams','Matches'];return order.indexOf(a)-order.indexOf(b);}";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();