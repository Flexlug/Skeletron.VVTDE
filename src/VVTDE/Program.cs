using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;


var builder = WebApplication.CreateBuilder(args);   

builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddMvcCore();

var app = builder.Build();

app.MapControllers();
app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
    string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));

app.Run();