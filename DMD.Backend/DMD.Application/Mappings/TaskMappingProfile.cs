using AutoMapper;

using DMD.Application.DTOs;
using DMD.Domain.Entities;

namespace DMD.Application.Mappings
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<TodoTask, TaskDto>()
                .ForMember(dest => dest.SubTasks, opt => opt.MapFrom(src => src.SubTasks));
        }
    }
}
