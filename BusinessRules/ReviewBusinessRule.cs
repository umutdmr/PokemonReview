using webapi.Data;
using webapi.DTO;
using webapi.Interfaces;

namespace webapi.BusinessRules
{
    public class ReviewBusinessRule : IReviewBusinessRule
    {
        private readonly DataContext _context;
        public ReviewBusinessRule(DataContext context)
        {
            _context = context;
        }
        public bool ReviewExists(int id)
        {
            return _context.Reviews.Any(r => r.Id == id);

        }

        public bool ReviewExists(ReviewDTO reviewDTO)
        {
            var review = _context.Reviews.Where(r => r.Title.Trim().ToUpper() == reviewDTO.Title.Trim().ToUpper()).FirstOrDefault();
            if (review != null)
            {
                review = _context.Reviews.Where(r => r.Id == reviewDTO.Id).FirstOrDefault();
            }
            return review != null;
        }
    }
}
