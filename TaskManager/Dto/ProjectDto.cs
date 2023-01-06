using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto
{
    public class ProjectDto
	{
		//This DTO is created to allow to avoid key property conflicts during creating and editing the projects
		public string ProjectName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime CompletionDate { get; set; }
		public Enum.DtoProjectStatus ProjectStatus { get; set; }
		public int Priority { get; set; }

	}
}
