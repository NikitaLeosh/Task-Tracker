using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Data
{
	public class Seed
	{
		private readonly ApplicationDbContext _context;
		public Seed(ApplicationDbContext context)
		{
			_context = context;
		}
		public void SeedDataContext()
		{
			if (!_context.Projects.Any())
			{
				var projects = new List<Project>()
				{
					new Project()
					{
						ProjectName = "New fancy project",
						StartDate = new DateTime(2016,11,23),
						CompletionDate= new DateTime(2017,3,20),
						ProjectStatus = Enum.ProjectStatus.Active,
						Priority = 42,
						Tasks = new List<ProjectTask>()
						{
							new ProjectTask
							{
								TaskName = "First task",
								TaskStatus = Enum.ProjectTaskStatus.ToDo,
								Priority = 3,
								TaskDescription = "To perform the very first action"
							},
							new ProjectTask
							{
								TaskName = "Second task",
								TaskStatus = Enum.ProjectTaskStatus.InProgress,
								Priority = 33,
								TaskDescription = "To perform the second action"
							},
							new ProjectTask
							{
								TaskName = "Third task",
								TaskStatus = Enum.ProjectTaskStatus.Done,
								Priority = 20,
								TaskDescription = "To perform the third action"
							}
						}
					},
					new Project()
					{
						ProjectName = "Second fancy project",
						StartDate = new DateTime(2010,5,23),
						CompletionDate= new DateTime(2012,1,15),
						ProjectStatus = Enum.ProjectStatus.Active,
						Priority = 27,
						Tasks = new List<ProjectTask>()
						{
							new ProjectTask
							{
								TaskName = "First task",
								TaskStatus = Enum.ProjectTaskStatus.ToDo,
								Priority = 20,
								TaskDescription = "To perform the very first action"
							},
							new ProjectTask
							{
								TaskName = "Second task",
								TaskStatus = Enum.ProjectTaskStatus.InProgress,
								Priority = 43,
								TaskDescription = "To perform the second action"
							},
							new ProjectTask
							{
								TaskName = "Third task",
								TaskStatus = Enum.ProjectTaskStatus.Done,
								Priority = 33,
								TaskDescription = "To perform the third action"
							}
						}
					}
				};
				_context.Projects.AddRange(projects);
				_context.SaveChanges();
			}
		}
	}
}
