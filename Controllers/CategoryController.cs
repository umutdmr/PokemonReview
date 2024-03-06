using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;
using webapi.Interfaces;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryBusinessRules _categoryBusinessRules;
        private readonly IMapper _mapper;


        public CategoryController(ICategoryRepository categoryRepository, ICategoryBusinessRules categoryBusinessRules, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _categoryBusinessRules = categoryBusinessRules;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {

            var categories = _mapper.Map<List<CategoryDTO>>(_categoryRepository.GetCategories());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(categories);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int id)
        {
            if (!_categoryBusinessRules.CategoryExists(id))
            {
                return NotFound();
            }

            var category = _mapper.Map<CategoryDTO>(_categoryRepository.GetCategory(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(category);

        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategory(int categoryId)
        {
            if (!_categoryBusinessRules.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var pokemon = _mapper.Map<List<PokemonDTO>>(_categoryRepository.GetPokemonByCategory(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            if (categoryDTO == null) return BadRequest(ModelState);

            if (_categoryBusinessRules.CategoryExists(categoryDTO))
            {
                ModelState.AddModelError("", "Category already exists!");
                return StatusCode(422, ModelState);
            }
            if(!ModelState.IsValid) 
            { 
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(categoryDTO);

            if (!_categoryRepository.CreateCategory(category))
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
        public IActionResult UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
        {
            if (categoryDTO == null || !ModelState.IsValid || id != categoryDTO.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_categoryBusinessRules.CategoryExists(id))
            {
                return NotFound();
            }

            var category = _mapper.Map<Category>(categoryDTO);
            if (!_categoryRepository.UpdateCategory(category))
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
            if (!_categoryBusinessRules.CategoryExists(id))
            {
                return NotFound();
            }

            var category = _categoryRepository.GetCategory(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("", "Something wrong happened while deleting!");
            }
            return NoContent();
        }

    }
}
