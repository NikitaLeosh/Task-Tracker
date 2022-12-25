using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaskManager.Dto;
using TaskManager.Models;

namespace TaskManager.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Project, ProjectNoTasksDto>();
			CreateMap<ProjectNoTasksDto, Project>();
			CreateMap<ProjectDto, Project>();
			CreateMap<Project, ProjectDto>();
			CreateMap<ProjectTaskDto, ProjectTask>();
			CreateMap<ProjectTask,ProjectTaskDto>();
			CreateMap<TaskNoNavigationPropsDto, ProjectTask>();
			CreateMap<ProjectTask, TaskNoNavigationPropsDto>();
		}
	}
}
