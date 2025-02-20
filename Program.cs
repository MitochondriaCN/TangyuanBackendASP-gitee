/*
 * 在科学上没有平坦的大道，
 * 只有不畏劳苦沿着陡峭山路攀登的人，
 * 才有希望达到光辉的顶点。
 * ——卡尔·马克思
 */

using Microsoft.EntityFrameworkCore;
using TangyuanBackendASP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//糖原天下无敌

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
