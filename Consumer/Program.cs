using Consumer.RabbitMq.Consumer;
using Consumer.RabbitMq.ConsumerHost;
using Data;
using Rabbit;
using Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddData(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddScoped<IConsumerService, ConsumerService>();
builder.Services.AddScoped<ConsumerHostedService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();