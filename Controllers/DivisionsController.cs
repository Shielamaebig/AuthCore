using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthCore.Data;
using AutoMapper;
using AuthCore.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthCore.Models;

namespace AuthCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public DivisionsController(ApplicationDbContext db, IMapper mapper)
        {
            this._db = db;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDivisions()
        {
            if (_db.Divisions == null)
            {
                return NotFound();
            }
            var divs = await _db.Divisions.Include(x => x.Departments).ToListAsync();

            return Ok(divs);
        }

        [HttpPost]
        public async Task<ActionResult> SaveDivision(DivisionsDto divisionDto)
        {
            var divisionInDb = _mapper.Map<Divisions>(divisionDto);
            divisionDto.DateAdded = DateTime.Now.ToString("MMMM dd yyyy hh:mm tt");
            divisionInDb.DateAdded = DateTime.Now.ToString("MMMM dd yyyy hh:mm tt");

            _db.Divisions.Add(divisionInDb);
            await _db.SaveChangesAsync();

            return Created($"/{divisionInDb.Id}", divisionInDb);
        }
    }
}