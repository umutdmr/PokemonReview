using webapi.DTO;

namespace webapi.Interfaces
{
    public interface IReviewBusinessRule
    {
        bool ReviewExists(int id);
        bool ReviewExists(ReviewDTO reviewDTO);
    }
}
