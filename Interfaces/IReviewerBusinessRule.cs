using webapi.DTO;

namespace webapi.Interfaces
{
    public interface IReviewerBusinessRule
    {
        bool ReviewerExists(int id);
        bool ReviewerExists(ReviewerDTO reviewerDTO);
    }
}
