using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SendingEmailWithASPNETCore;
using SendingEmailWithASPNETCore.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddTransient<IAPIMailService, APIMailService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Example API",
        Version = "v1",
        Description = "An example off an ASP .NET Core Web API",
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Email = "example@example.com",
            Url = new Uri("https://example.com/contact"),
        },

    });

});


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddHttpClient("MailTrapApiClient", (services, client) => 
{
    var mailSettings = services.GetRequiredService<IOptions<MailSettings>>().Value;
    client.BaseAddress = new Uri(mailSettings.ApiBaseUrl);
    client.DefaultRequestHeaders.Add("Api-Token", mailSettings.ApiToken);
});

var app = builder.Build();


app.UseCors();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
