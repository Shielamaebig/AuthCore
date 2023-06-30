using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthCore.Models
{
    public class Divisions
    {
        public int Id { get; set; }
        public string? DateAdded { get; set; }
        public string? Name { get; set; }
        public Departments? Departments { get; set; }
        public int DepartmentsId { get; set; }
    }
}