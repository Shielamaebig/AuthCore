using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthCore.Data.Migrations;

namespace AuthCore.Dto
{
    public class DivisionsDto
    {
        public int Id { get; set; }
        public string? DateAdded { get; set; }
        public string? Name { get; set; }
        public int DepartmentsId { get; set; }
    }
}