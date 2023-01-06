using System.ComponentModel.DataAnnotations;

namespace TaskManager.Dto
{
	public class TaskNoNavigationPropsDto
	{
		//This DTO is created to show tasks without navigational properties but with IDs
		public Guid Id { get; set; }
		public string TaskName { get; set; }
		public Enum.DtoProjectTaskStatus TaskStatus { get; set; }
		public string TaskDescription { get; set; }
		public int Priority { get; set; }
	}
}
