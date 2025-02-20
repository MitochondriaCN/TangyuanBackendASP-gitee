/*
 * �ڿ�ѧ��û��ƽ̹�Ĵ����
 * ֻ�в�η�Ϳ����Ŷ���ɽ·�ʵǵ��ˣ�
 * ����ϣ���ﵽ��ԵĶ��㡣
 * �������������˼
 */

using Microsoft.EntityFrameworkCore;
using TangyuanBackendASP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//���������£�����Զ��MySQL
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<TangyuanDbContext>(
        options => options.UseMySQL(builder.Configuration.GetConnectionString("Remote")));
}
//���������£����ӱ���MySQL
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

//��ԭ�����޵�

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
