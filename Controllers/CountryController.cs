using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using webapi.BusinessRules;
using webapi.DTO;
using webapi.Interfaces;
using webapi.Models;
using webapi.Repository;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICountryBusinessRules _countryBusinessRules;
        private readonly IOwnerBusinessRules _ownerBusinessRules;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, ICountryBusinessRules countryBusinessRules, IOwnerBusinessRules ownerBusinessRules, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _countryBusinessRules = countryBusinessRules;
            _ownerBusinessRules = ownerBusinessRules;
            _mapper = mapper;   
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {

            var countries = _mapper.Map<List<CountryDTO>>(_countryRepository.GetCountries());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(countries);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int id)
        {
            if (!_countryBusinessRules.CountryExists(id))
            {
                return NotFound();
            }

            var country = _mapper.Map<CountryDTO>(_countryRepository.GetCountry(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(country);

        }

        [HttpGet("country/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryByOwner(int ownerId)
        {
            if (!_ownerBusinessRules.OwnerExists(ownerId))
            {
                return NotFound();
            }
            var country = _mapper.Map<CountryDTO>(_countryRepository.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(country);
        }

        [HttpGet("owner/{countryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwnersFromACountry(int countryId)
        {

            if (!_countryBusinessRules.CountryExists(countryId))
            {
                return NotFound();
            }

            var owners = _mapper.Map<List<OwnerDTO>>(_countryRepository.GetOwnersFromACountry(countryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(owners);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDTO countryDTO)
        {
            if (countryDTO == null) return BadRequest(ModelState);

            if (_countryBusinessRules.CountryExists(countryDTO))
            {
                ModelState.AddModelError("", "Country already exists!");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var country = _mapper.Map<Country>(countryDTO);

            if (!_countryRepository.CreateCountry(country))
            {
                ModelState.AddModelError("", "Something wrong happened while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created.");

        }

        [HttpPut("id")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int id, [FromBody] CountryDTO countryDTO)
        {
            if (countryDTO == null || !ModelState.IsValid || id != countryDTO.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_countryBusinessRules.CountryExists(id))
            {
                return NotFound();
            }

            var country = _mapper.Map<Country>(countryDTO);
            if (!_countryRepository.UpdateCountry(country))
            {
                ModelState.AddModelError("", "Something wrong happened while updating!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("id")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int id)
        {
            if (!_countryBusinessRules.CountryExists(id))
            {
                return NotFound();
            }

            var country = _countryRepository.GetCountry(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.DeleteCountry(country))
            { 
                ModelState.AddModelError("", "Something wrong happened while deleting!");
            }
            return NoContent();
        }

    }
}
