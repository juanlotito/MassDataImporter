using importacionmasiva.api.net.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;
using System.Text;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependency(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.OrderActionsBy((apiDesc) =>
    {
        string methodOrder = apiDesc.HttpMethod switch
        {
            "GET" => "1",
            "PUT" => "2",
            "POST" => "3",
            "DELETE" => "4",
            _ => "5"
        };
        return $"{methodOrder}-{apiDesc.RelativePath}";
    });

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Importación Masiva API", Version = "v1.0.0" });
});

builder.Services.AddHealthChecks();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToList();

        var result = new
        {
            Error = true,
            Status = (int)HttpStatusCode.BadRequest,
            Errors = errors
        };

        return new BadRequestObjectResult(result);
    };

}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder
                //.WithOrigins("http://localhost:62416", "http://localhost:57877", "https://192.168.0.8:45457", "https://192.168.0.7:45457", "http://localhost:54246")
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Importación Masiva API");
});

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();