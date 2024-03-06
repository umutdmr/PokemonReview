using webapi.Data;
using webapi.DTO;
using webapi.Interfaces;

namespace webapi.BusinessRules
{
    public class CategoryBusinessRules : ICategoryBusinessRules
    {
        private readonly DataContext _context;
        public CategoryBusinessRules(DataContext context)
        {
            _context = context;
        }

        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }

        public bool CategoryExists(CategoryDTO categoryDTO)
        {
            var category = _context.Categories.Where(c => c.Name.Trim().ToUpper() == categoryDTO.Name.TrimEnd().ToUpper()).FirstOrDefault();
            if (category == null)
            {
                category = _context.Categories.Where(c => c.Id == categoryDTO.Id).FirstOrDefault();
            }

            return category != null;
        }
    }
}
