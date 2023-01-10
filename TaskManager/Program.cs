using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Interfaces;
using TaskManager.Repositories;
using TaskManager.Services;

internal class Program
{
	private static string dbHost = Environment.GetEnvironmentVariable("DB_HOST");
	private static string dbName = Environment.GetEnvironmentVariable("DB_NAME");
	private static string Password = Environment.GetEnvironmentVariable("SA_PASSWORD");
	private static string connectionstring = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={Password};Encrypt=false";
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);


		builder.Services.AddControllers();

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddTransient<Seed>();
		builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
		builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
		builder.Services.AddScoped<ICheckProjectRepository, CheckProjectRepository>();
		builder.Services.AddScoped<ICheckTaskRepository, CheckTaskRepository>();
		builder.Services.AddScoped<IProjectValidationService, ProjectValidationService>();
		builder.Services.AddScoped<ITaskValidationService, TaskValidationService>();
		//Add DI for database context

		builder.Services.AddDbContext<ProjectDbContext>(options =>
				{
					options.UseSqlServer(connectionstring);
				});
		builder.Services.AddSwaggerGen();

		//the seeder code
		var app = builder.Build();

		//if (args.Length == 1 && args[0].ToLower() == "seeddata")
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