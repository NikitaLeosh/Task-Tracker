using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaskManager.Dto
{
    public class ProjectTaskDto
	{
		//This DTO is intended to avoid conflicts with existing fields while creating/editing tasks
		public string TaskName { get; set; }
		public Enum.DtoProjectTaskStatus TaskStatus { get; set; }
		public string TaskDescription { get; set; }
		public int Priority { get; set; }
	}
}
