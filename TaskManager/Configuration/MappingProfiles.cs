using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaskManager.Dto;
using TaskManager.Models;
using AutoMapper.Extensions.EnumMapping;

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
			CreateMap<Models.Enum.ProjectStatus, Dto.Enum.DtoProjectStatus>()
				.ConvertUsingEnumMapping(opt => opt.MapByValue());
			CreateMap<Dto.Enum.DtoProjectStatus, Models.Enum.ProjectStatus>()
				.ConvertUsingEnumMapping(opt => opt.MapByValue());
			CreateMap<Models.Enum.ProjectTaskStatus, Dto.Enum.DtoProjectTaskStatus>()
				.ConvertUsingEnumMapping(opt => opt.MapByValue());
			CreateMap<Dto.Enum.DtoProjectTaskStatus, Models.Enum.ProjectTaskStatus>()
				.ConvertUsingEnumMapping(opt => opt.MapByValue());
		}
	}
}
