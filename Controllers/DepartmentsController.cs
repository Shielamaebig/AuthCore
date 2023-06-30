using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthCore.Data;
using AuthCore.Dto;
using AuthCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public DepartmentsController(ApplicationDbContext db, IMapper mapper)
        {
            this._db = db;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDepartments()
        {
            if (_db.Departments == null)
            {
                return NotFound();
            }
            var depts = await _db.Departments.ToListAsync();
            return Ok(depts);
        }

        [HttpPost]
        public async Task<ActionResult> PostDepartment(DepartmentsDto departmentsDto)
        {
            var departmentInDb = _mapper.Map<Departments>(departmentsDto);
            departmentsDto.DateAdded = DateTime.Now.ToString("MMMM dd yyyy hh:mm tt");
            departmentInDb.DateAdded = DateTime.Now.ToString("MMMM dd yyyy hh:mm tt");


            _db.Departments.Add(departmentInDb);
            await _db.SaveChangesAsync();
            return Created($"/{departmentInDb.Id}", departmentInDb);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Departments>> GetbyIdDepartment(int id)
        {
            if (_db.Departments == null)
            {
                return NotFound();
            }
            var depts = await _db.Departments.FindAsync(id);

            if (depts == null)
            {
                return NotFound();
            }
            return depts;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateDepartment(int id, DepartmentsDto departmentsDto)
        {
            var department = await _db.Departments.SingleOrDefaultAsync(x => x.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            department.Id = departmentsDto.Id;
            department.DateAdded = DateTime.Now.ToString("MMMM dd yyyy hh:mm tt");
            departmentsDto.DateAdded = DateTime.Now.ToString("MMMM dd yyyy hh:mm tt");
            department.Name = departmentsDto.Name;
            _db.Departments.Update(department);
            _db.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            var departmentInDb = await _db.Departments.SingleOrDefaultAsync(x => x.Id == id);
            if (departmentInDb == null)
            {
                return NotFound();
            }
            _db.Departments.Remove(departmentInDb);
            _db.SaveChanges();
            return Ok();
        }
    }
}