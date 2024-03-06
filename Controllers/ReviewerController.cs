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
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IReviewerBusinessRule _reviewerBusinessRule;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IReviewerBusinessRule reviewerBusinessRule, IPokemonBusinessRules pokemonBusinessRules, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _reviewerBusinessRule = reviewerBusinessRule;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {

            var reviewers = _mapper.Map<List<ReviewerDTO>>(_reviewerRepository.GetReviewers());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviewers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int id)
        {
            if (!_reviewerBusinessRule.ReviewerExists(id))
            {
                return NotFound();
            }

            var reviewer = _mapper.Map<ReviewerDTO>(_reviewerRepository.GetReviewer(id));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviewer);

        }

        [HttpGet("review/{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerBusinessRule.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(reviews);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDTO reviewerDTO)
        {
            if (reviewerDTO == null) return BadRequest(ModelState);

            if (_reviewerBusinessRule.ReviewerExists(reviewerDTO))
            {
                ModelState.AddModelError("", "Reviewer already exists!");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var reviewer = _mapper.Map<Reviewer>(reviewerDTO);

            if (!_reviewerRepository.CreateReviewer(reviewer))
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
        public IActionResult UpdateReview(int id, [FromBody] ReviewerDTO reviewerDTO)
        {
            if (reviewerDTO == null || !ModelState.IsValid || id != reviewerDTO.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewerBusinessRule.ReviewerExists(id))
            {
                return NotFound();
            }

            var reviewer = _mapper.Map<Reviewer>(reviewerDTO);
            if (!_reviewerRepository.UpdateReviewer(reviewer))
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
        public IActionResult DeleteReviewer(int id)
        {
            if (!_reviewerBusinessRule.ReviewerExists(id))
            {
                return NotFound();
            }

            var reviewer = _reviewerRepository.GetReviewer(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.DeleteReviewer(reviewer))
            {
                ModelState.AddModelError("", "Something wrong happened while deleting!");
            }
            return NoContent();
        }
    }
}
