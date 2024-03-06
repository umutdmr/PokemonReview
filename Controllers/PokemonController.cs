using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using webapi.BusinessRules;
using webapi.DTO;
using webapi.Interfaces;
using webapi.Models;
using webapi.Repository;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController: Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IPokemonBusinessRules _pokemonBusinessRules;
        private readonly IOwnerBusinessRules _ownerBusinessRules;
        private readonly ICategoryBusinessRules _categoryBusinessRules;
        private readonly IMapper _mapper;
        public PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository, IPokemonBusinessRules pokemonBusinessRules, IOwnerBusinessRules ownerBusinessRules, ICategoryBusinessRules categoryBusinessRules, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
            _pokemonBusinessRules = pokemonBusinessRules;
            _ownerBusinessRules = ownerBusinessRules;
            _categoryBusinessRules = categoryBusinessRules;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {

            var pokemons = _mapper.Map<List<PokemonDTO>>(_pokemonRepository.GetPokemons());

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return Ok(pokemons);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int id)
        {
            if(!_pokemonBusinessRules.PokemonExists(id))
            {
                return NotFound();
            }

            var pokemon = _mapper.Map<PokemonDTO>(_pokemonRepository.GetPokemon(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemon);

        }

        [HttpGet("{id}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int id)
        {
            if (!_pokemonBusinessRules.PokemonExists(id))
            {
                return NotFound();
            }

            var rating = _pokemonRepository.GetPokemonRating(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(rating);
        }


        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery]  int categoryId, [FromBody] PokemonDTO pokemonDTO)
        {
            if (pokemonDTO == null) return BadRequest(ModelState);

            if (_pokemonBusinessRules.PokemonExists(pokemonDTO))
            {
                ModelState.AddModelError("", "Pokemon already exists!");
                return StatusCode(422, ModelState);
            }
            if (!_ownerBusinessRules.OwnerExists(ownerId))
            {
                ModelState.AddModelError("", "Owner does not exist with given id!");
                return StatusCode(422, ModelState);
            }
            if (!_categoryBusinessRules.CategoryExists(categoryId))
            {
                ModelState.AddModelError("", "Category does not exist with given id!");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pokemon = _mapper.Map<Pokemon>(pokemonDTO);


            if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemon))
            {
                ModelState.AddModelError("", "Something wrong happened while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created.");

        }

        [HttpPut("pokeId")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId, [FromQuery] int ownerId, [FromBody] PokemonDTO pokemonDTO)
        {
            if (pokemonDTO == null || !ModelState.IsValid || pokeId != pokemonDTO.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_pokemonBusinessRules.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var pokemon = _mapper.Map<Pokemon>(pokemonDTO);
            if (!_pokemonRepository.UpdatePokemon(pokemon))
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
        public IActionResult DeletePokemon(int id)
        {
            if (!_pokemonBusinessRules.PokemonExists(id))
            {
                return NotFound();
            }

            var pokemon = _pokemonRepository.GetPokemon(id);
            var reviews = _reviewRepository.GetReviewsOfAPokemon(id).ToList();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_pokemonRepository.DeletePokemon(pokemon))
            {
                ModelState.AddModelError("", "Something wrong happened while deleting!");
            }
            if (!_reviewRepository.DeleteReviews(reviews))
            {
                ModelState.AddModelError("", "Something wrong happened while deleting!");
            }
            return NoContent();
        }

    }
}
