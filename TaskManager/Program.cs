using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Repositories;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		//runn the app with dev power shell by "dotnet run seeddata" to seed data at your local db
		builder.Services.AddTransient<Seed>();
		//bringing in the automapper
		builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
		builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});
		builder.Services.AddSwaggerGen();

		//the seeder code
		var app = builder.Build();

		if (args.Length == 1 && args[0].ToLower() == "seeddata")
			SeedData(app);

		void SeedData(IHost app)
		{
			var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

			using (var scope = scopedFactory.CreateScope())
			{
				var service = scope.ServiceProvider.GetService<Seed>();
				service.SeedDataContext();
			}
		}

		// Configure the HTTP request pipeline.
		//if (app.Environment.IsDevelopment())
		//{
			app.UseSwagger();
			app.UseSwaggerUI();
		//}

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}