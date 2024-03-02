using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ToDoApi;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        }));

builder.Services.AddDbContext<ToDoDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("MyPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");


app.MapGet("/tasks", [Authorize] (HttpContext http, ToDoDbContext context) =>
{
    var jwt = http.User.FindFirst("id");
    if (jwt is null) return null;
    var id = int.Parse(jwt.Value);
    return context.Items.Where(i => i.UserId == id);
});

app.MapPost("/tasks", [Authorize] async (HttpContext http, ToDoDbContext context, [FromBody] Item item) =>
{
    if (item is not null)
    {
        var jwt = http.User.FindFirst("id");
        if (jwt is null) return null;
        var id = int.Parse(jwt.Value);
        item.UserId = id;
        context.Items.Add(item);
        await context.SaveChangesAsync();
        return Results.Ok(item);
    }
    return Results.Ok();
});

app.MapPut("/tasks/{id}", [Authorize] async (ToDoDbContext context, int id, [FromBody] Item item) =>
{
    var task = await context.Items.FindAsync(id);
    if (task is null)
        return Results.NotFound();
    task.IsCompelte = item.IsCompelte;
    await context.SaveChangesAsync();
    return Results.Ok(task);
});

app.MapDelete("/tasks/{id}", [Authorize] async (ToDoDbContext context, int id) =>
{
    var task = await context.Items.FindAsync(id);
    if (task is null)
        return Results.NotFound();
    context.Items.Remove(task);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPost("error", (string error) =>
{
    using (var file = new StreamWriter("log.txt", true))
    {
        file.WriteLineAsync(Guid.NewGuid().ToString());
        file.WriteLineAsync(error);
    }

});

app.MapPost("/login", (ToDoDbContext context, User user) =>
{
    var existUser = context.Users.Where(u => u.UserName == user.UserName && u.Password == user.Password).FirstOrDefault();
    if (existUser is null)
        return Results.StatusCode(401);
    var claims = new List<Claim>()
            {
                new Claim("name", existUser.UserName!),
                new Claim("id", existUser.Id.ToString()),
            };

    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:Key")));
    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    var tokeOptions = new JwtSecurityToken(
        issuer: builder.Configuration.GetValue<string>("JWT:Issuer"),
        audience: builder.Configuration.GetValue<string>("JWT:Audience"),
        claims: claims,
        expires: DateTime.Now.AddMinutes(6),
        signingCredentials: signinCredentials
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    return Results.Ok(new { Token = tokenString });
});

app.MapPost("/logup", async (ToDoDbContext context, User user) =>
{
    Console.WriteLine("I enter to this fucntion");
    var existUserName = context.Users.Where(u => u.UserName == user.UserName).FirstOrDefault();
    if (existUserName is not null)
        return Results.StatusCode(403);
    context.Users.Add(user);
    await context.SaveChangesAsync();
    var existUser = context.Users.Where(u => u.UserName == user.UserName && u.Password == user.Password).FirstOrDefault();
    var claims = new List<Claim>()
            {
                new Claim("name", user.UserName!),
                new Claim("id", existUser!.Id.ToString()),

            };

    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:Key")));
    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    var tokeOptions = new JwtSecurityToken(
        issuer: builder.Configuration.GetValue<string>("JWT:Issuer"),
        audience: builder.Configuration.GetValue<string>("JWT:Audience"),
        claims: claims,
        expires: DateTime.Now.AddMinutes(6),
        signingCredentials: signinCredentials
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    return Results.Ok(new { Token = tokenString });

});

app.Run();
