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
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IReviewBusinessRule _reviewBusinessRule;
        private readonly IReviewerBusinessRule _reviewerBusinessRule;
        private readonly IPokemonBusinessRules _pokemonBusinessRules;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, IReviewBusinessRule reviewBusinessRule, IReviewerBusinessRule reviewerBusinessRule, IPokemonBusinessRules pokemonBusinessRules, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
            _reviewBusinessRule = reviewBusinessRule;
            _pokemonBusinessRules = pokemonBusinessRules;
            _reviewerBusinessRule = reviewerBusinessRule;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {

            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewRepository.GetReviews());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int id)
        {
            if (!_reviewBusinessRule.ReviewExists(id))
            {
                return NotFound();
            }

            var review = _mapper.Map<ReviewDTO>(_reviewRepository.GetReview(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(review);

        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            if(!_pokemonBusinessRules.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewRepository.GetReviewsOfAPokemon(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokemonId, [FromBody] ReviewDTO reviewDTO)
        {
            if (reviewDTO == null) return BadRequest(ModelState);

            if (_reviewBusinessRule.ReviewExists(reviewDTO))
            {
                ModelState.AddModelError("", "Review already exists!");
                return StatusCode(422, ModelState);
            }
            if (!_pokemonBusinessRules.PokemonExists(pokemonId))
            {
                ModelState.AddModelError("", "Pokemon does not exist with given id!");
                return StatusCode(422, ModelState);
            }
            if (_reviewerBusinessRule.ReviewerExists(reviewerId))
            {
                ModelState.AddModelError("", "Reviewer does not exist with given id!");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var review = _mapper.Map<Review>(reviewDTO);
            review.Pokemon = _pokemonRepository.GetPokemon(pokemonId);
            review.Reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if (!_reviewRepository.CreateReview(review))
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
        public IActionResult UpdateReview(int id, [FromBody] ReviewDTO reviewDTO)
        {
            if (reviewDTO == null || !ModelState.IsValid || id != reviewDTO.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewBusinessRule.ReviewExists(id))
            {
                return NotFound();
            }

            var review = _mapper.Map<Review>(reviewDTO);
            if (!_reviewRepository.UpdateReview(review))
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
        public IActionResult DeleteReview(int id)
        {
            if (!_reviewBusinessRule.ReviewExists(id))
            {
                return NotFound();
            }

            var review = _reviewRepository.GetReview(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.DeleteReview(review))
            {
                ModelState.AddModelError("", "Something wrong happened while deleting!");
            }
            return NoContent();
        }


        [HttpDelete("/DeleteReviewByReviewer/reviewerId")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewByReviewer(int reviewerId)
        {
            if (!_reviewerBusinessRule.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId).ToList();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.DeleteReviews(reviews))
            {
                ModelState.AddModelError("", "Something wrong happened while deleting!");
            }
            return NoContent();
        }
    }
}
