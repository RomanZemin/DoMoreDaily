using AutoMapper;
using DMD.Domain.Entities;
using DMD.Application.DTOs;

namespace DMD.Application.Mappings
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<TodoTask, TaskDto>()
                .ForMember(dest => dest.SubTasks, opt => opt.MapFrom(src => src.SubTasks));

            CreateMap<TodoTask, SubTaskDto>();
        }
    }
}
