

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//! For the meantime, swagger is commented out because of mismatch version from .NET 8.0
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddSwaggerGen();

// Add the connection string to the services
builder.Services.AddTransient<UserService>(provider =>
    new UserService(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string for Postgresql is not found.")));


var app = builder.Build();


//! Uncomment the following lines to enable swagger
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


