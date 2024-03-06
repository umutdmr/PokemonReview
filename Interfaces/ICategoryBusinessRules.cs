using webapi.DTO;

namespace webapi.Interfaces
{
    public interface ICategoryBusinessRules
    {
        bool CategoryExists(int id);
        bool CategoryExists(CategoryDTO categoryDTO);
    }
}
