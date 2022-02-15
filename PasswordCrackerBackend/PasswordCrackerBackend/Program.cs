using PasswordCrackerBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PasswordHub>();
builder.Services.AddSignalR();

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

const string corsKey = "corsKey";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsKey,
        x => x.SetIsOriginAllowed(_ => true)
            //.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(corsKey);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapHub<PasswordHub>("/Cracker"));

app.UseMvc();
app.MapControllers();

app.Run();
