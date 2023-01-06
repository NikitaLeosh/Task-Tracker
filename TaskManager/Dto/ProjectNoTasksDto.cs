using System.ComponentModel.DataAnnotations;


namespace TaskManager.Dto
{
	public class ProjectNoTasksDto
	{
		//This Dto is created to show the list of projects without tasks
		public Guid Id { get; set; }
		public string ProjectName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime CompletionDate { get; set; }
		public Enum.DtoProjectStatus ProjectStatus { get; set; }
		public int Priority { get; set; }
	}
}
