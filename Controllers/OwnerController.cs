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
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IOwnerBusinessRules _ownerBusinessRules;
        private readonly IPokemonBusinessRules _pokemonBusinessRules;
        private readonly ICountryBusinessRules _countryBusinessRules;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IOwnerBusinessRules ownerBusinessRules, IPokemonBusinessRules pokemonBusinessRules, ICountryBusinessRules countryBusinessRules, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _ownerBusinessRules = ownerBusinessRules;
            _pokemonBusinessRules = pokemonBusinessRules;
            _countryBusinessRules = countryBusinessRules;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetCountries()
        {

            var owners = _mapper.Map<List<OwnerDTO>>(_ownerRepository.GetOwners());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(owners);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int id)
        {
            if (!_ownerBusinessRules.OwnerExists(id))
            {
                return NotFound();
            }

            var owner = _mapper.Map<OwnerDTO>(_ownerRepository.GetOwner(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(owner);

        }

        [HttpGet("owner/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnerOfAPokemon(int pokeId)
        {
            if (!_pokemonBusinessRules.PokemonExists(pokeId))
            {
                return NotFound();
            }
            
            var owners = _mapper.Map<List<OwnerDTO>>(_ownerRepository.GetOwnerOfAPokemon(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(owners);
        }

        [HttpGet("pokemon/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerBusinessRules.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var pokemon = _mapper.Map<List<PokemonDTO>>(_ownerRepository.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDTO ownerDTO)
        {
            if (ownerDTO == null) return BadRequest(ModelState);

            if (_ownerBusinessRules.OwnerExists(ownerDTO))
            {
                ModelState.AddModelError("", "Owner already exists!");
                return StatusCode(422, ModelState);
            }
            if (!_countryBusinessRules.CountryExists(countryId))
            {
                ModelState.AddModelError("", "Country does not exist with given id!");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var owner = _mapper.Map<Owner>(ownerDTO);
            owner.Country = _countryRepository.GetCountry(countryId);
            

            if (!_ownerRepository.CreateOwner(owner))
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
        public IActionResult UpdateOwner(int id, [FromBody] OwnerDTO ownerDTO)
        {
            if (ownerDTO == null || !ModelState.IsValid || id != ownerDTO.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_ownerBusinessRules.OwnerExists(ownerDTO))
            {
                return NotFound();
            }

            var owner = _mapper.Map<Owner>(ownerDTO);
            if (!_ownerRepository.UpdateOwner(owner))
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
        public IActionResult DeleteOwner(int id)
        {
            if (!_ownerBusinessRules.OwnerExists(id))
            {
                return NotFound();
            }

            var owner = _ownerRepository.GetOwner(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_ownerRepository.DeleteOwner(owner))
            {
                ModelState.AddModelError("", "Something wrong happened while deleting!");
            }
            return NoContent();
        }

    }
}
