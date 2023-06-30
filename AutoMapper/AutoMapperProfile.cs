using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AuthCore.Models;
using AuthCore.Dto;

namespace AuthCore.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DepartmentsDto, Departments>();
            CreateMap<DivisionsDto, Divisions>();
        }
    }
}