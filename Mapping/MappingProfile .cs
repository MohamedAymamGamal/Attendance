using AutoMapper;
using Job.DTO;
using Job.Models;

namespace Job.Mapping
{
    public class MappingProfile : Profile
    {
      public MappingProfile() {

            CreateMap<Attendance, AttendanceDto>()
              .ForMember(dest => dest.UserName,
                  opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
              .ForMember(dest => dest.Department,
                  opt => opt.MapFrom(src => src.User != null ? src.User.Department : string.Empty))
              .ForMember(dest => dest.Status,
                  opt => opt.MapFrom(src => src.Status.ToString()))
              .ForMember(dest => dest.WorkedFormatted,
                  opt => opt.MapFrom(src =>
              $"{src.WorkedMinutes / 60}h {src.WorkedMinutes % 60}m"));

        }
    }
}
