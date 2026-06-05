using AutoMapper;
using WMS.Application.DTOs;
using WMS.Domain.Models;

namespace WMS.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null))
                .ForMember(d => d.RoleName, o => o.MapFrom(s => s.Role != null ? s.Role.RoleName : null));

            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();

            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            CreateMap<Attendance, AttendanceDto>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee != null ? s.Employee.FirstName + " " + s.Employee.LastName : null));

            CreateMap<Leave, LeaveDto>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee != null ? s.Employee.FirstName + " " + s.Employee.LastName : null));

            CreateMap<CreateLeaveDto, Leave>();

            CreateMap<Project, ProjectDto>()
                .ForMember(d => d.ClientName, o => o.MapFrom(s => s.Client != null ? s.Client.ClientName : null));
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();

            CreateMap<Client, ClientDto>();
            CreateMap<CreateClientDto, Client>();
            CreateMap<UpdateClientDto, Client>();

            CreateMap<ProjectAllocation, AllocationDto>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee != null ? s.Employee.FirstName + " " + s.Employee.LastName : null))
                .ForMember(d => d.ProjectName, o => o.MapFrom(s => s.Project != null ? s.Project.ProjectName : null));

            CreateMap<CreateAllocationDto, ProjectAllocation>();

            CreateMap<Announcement, AnnouncementDto>()
                .ForMember(d => d.CreatedByName, o => o.MapFrom(s => s.CreatedByEmployee != null ? s.CreatedByEmployee.FirstName + " " + s.CreatedByEmployee.LastName : null))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedDate));
            CreateMap<CreateAnnouncementDto, Announcement>();
            CreateMap<UpdateAnnouncementDto, Announcement>();

            CreateMap<AuditLog, AuditLogDto>();
        }
    }
}
