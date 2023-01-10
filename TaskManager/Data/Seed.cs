using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Enum;

namespace TaskManager.Data
{
	public class Seed
	{
		private readonly ProjectDbContext _context;
		public Seed(ProjectDbContext context)
		{
			_context = context;
		}
		public void SeedDataContext()
		{
			if (!_context.Projects.Any())
			{
				var projects = new List<Project>();
				projects.Add(
					new Project
					{
						Id = Guid.Parse("879addc4-a54f-42e2-9dd2-3c0831faad95"),
						ProjectName = $"project 1",
						StartDate = new DateTime(2020, 2, 2),
						CompletionDate = new DateTime(2021, 2, 1),
						ProjectStatus = (ProjectStatus)(0),
						Priority = 1,
						Tasks = new List<ProjectTask>
						{
							new ProjectTask
							{
								Id = Guid.Parse("5d08d5fe-0ff9-4108-84e6-2a82a922e36d"),
								TaskName = $"Task number 1 of project 1",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(1),
								Priority = 1
							},
							new ProjectTask
							{
								Id = Guid.NewGuid(),
								TaskName = $"Task number 2 of project 1",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(1),
								Priority = 1
							}
						}
					});
				for (int i = 1; i < 10; i++)
				{
					projects.Add(
					new Project
					{
						ProjectName = $"project {i + 1}",
						StartDate = new DateTime(2020, 2, 2),
						CompletionDate = new DateTime(2021, 2, 1),
						ProjectStatus = (ProjectStatus)(i % 2),
						Priority = (i % 3) + 1,
						Tasks = new List<ProjectTask>
						{
							new ProjectTask
							{
								TaskName = $"Task number 1 of project {i+1}",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(i % 2),
								Priority = (i % 3) + 1
							},
							new ProjectTask
							{
								TaskName = $"Task number 2 of project {i+1}",
								TaskDescription = "blablabla",
								TaskStatus = (ProjectTaskStatus)(i % 2),
								Priority = (i % 3) + 1
							}
						}
					});
				}
				//{

				//	new Project()
				//	{
				//		ProjectName = "New fancy project",
				//		StartDate = new DateTime(2016,11,23),
				//		CompletionDate= new DateTime(2017,3,20),
				//		ProjectStatus = ProjectStatus.Active,
				//		Priority = 4,
				//		Tasks = new List<ProjectTask>()
				//		{
				//			new ProjectTask
				//			{
				//				TaskName = "First task",
				//				TaskStatus = ProjectTaskStatus.ToDo,
				//				Priority = 3,
				//				TaskDescription = "To perform the very first action"
				//			},
				//			new ProjectTask
				//			{
				//				TaskName = "Second task",
				//				TaskStatus = ProjectTaskStatus.InProgress,
				//				Priority = 2,
				//				TaskDescription = "To perform the second action"
				//			},
				//			new ProjectTask
				//			{
				//				TaskName = "Third task",
				//				TaskStatus = ProjectTaskStatus.Done,
				//				Priority = 5,
				//				TaskDescription = "To perform the third action"
				//			}
				//		}
				//	},
				//	new Project()
				//	{
				//		ProjectName = "Second fancy project",
				//		StartDate = new DateTime(2010,5,23),
				//		CompletionDate= new DateTime(2012,1,15),
				//		ProjectStatus = ProjectStatus.Active,
				//		Priority = 2,
				//		Tasks = new List<ProjectTask>()
				//		{
				//			new ProjectTask
				//			{
				//				TaskName = "First task",
				//				TaskStatus = ProjectTaskStatus.ToDo,
				//				Priority = 2,
				//				TaskDescription = "To perform the very first action"
				//			},
				//			new ProjectTask
				//			{
				//				TaskName = "Second task",
				//				TaskStatus = ProjectTaskStatus.InProgress,
				//				Priority = 1,
				//				TaskDescription = "To perform the second action"
				//			},
				//			new ProjectTask
				//			{
				//				TaskName = "Third task",
				//				TaskStatus = ProjectTaskStatus.Done,
				//				Priority = 3,
				//				TaskDescription = "To perform the third action"
				//			}
				//		}
				//	}
				//};
				_context.Projects.AddRange(projects);
				_context.SaveChanges();
			}
		}
	}
}
