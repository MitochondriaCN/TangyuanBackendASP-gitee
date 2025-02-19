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

//���������£������ڴ����ݿ����ڵ���
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<TangyuanDbContext>(
        options => options.UseInMemoryDatabase("tangyuan"));
}
//���������£�����MySQL
else
{
    builder.Services.AddDbContext<TangyuanDbContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("TangyuanDbContext")));
}


var app = builder.Build();

//ȷ�����ݿⱻ����
app.Services.CreateScope().ServiceProvider.GetRequiredService<TangyuanDbContext>().Database.EnsureCreated();

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
