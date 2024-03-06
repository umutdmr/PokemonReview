using webapi.Data;
using webapi.DTO;
using webapi.Interfaces;

namespace webapi.BusinessRules
{
    public class ReviewerBusinessRule : IReviewerBusinessRule
    {
        private readonly DataContext _context;
        public ReviewerBusinessRule(DataContext context)
        {
            _context = context;
        }
        public bool ReviewerExists(int id)
        {
            return _context.Reviewers.Any(r => r.Id == id);

        }

        public bool ReviewerExists(ReviewerDTO reviewerDTO)
        {
            var reviewer = _context.Reviewers.Where(r => r.FirstName.Trim().ToUpper() == reviewerDTO.FirstName.TrimEnd().ToUpper()).FirstOrDefault();
            if (reviewer == null)
            {
                reviewer = _context.Reviewers.Where(r => r.Id == reviewerDTO.Id).FirstOrDefault();
            }

            return reviewer != null;
        }
    }
}
