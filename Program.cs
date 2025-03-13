/*
 * 在科学上没有平坦的大道，
 * 只有不畏劳苦沿着陡峭山路攀登的人，
 * 才有希望达到光辉的顶点。
 * ——卡尔·马克思
 */

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TangyuanBackendASP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<AuthService>();

//开发环境下，连接远端MySQL
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<TangyuanDbContext>(
        options => options.UseMySQL(builder.Configuration.GetConnectionString("Remote")));
}
//生产环境下，连接本地MySQL
else
{
    builder.Services.AddDbContext<TangyuanDbContext>(
        options => options.UseMySQL(builder.Configuration.GetConnectionString("Local")));
}


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TangyuanDbContext>();
    dbContext.Database.EnsureCreated(); // Alternatively, use this to ensure the database is created
}

app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

//糖原天下无敌

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//如果根目录不存在，创建
if (!Directory.Exists("wwwroot"))
{
    Directory.CreateDirectory("wwwroot");
}
//如果images文件夹不存在，创建
if (!Directory.Exists("wwwroot/images"))
{
    Directory.CreateDirectory("wwwroot/images");
}

app.Run();
