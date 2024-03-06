using webapi.Data;
using webapi.DTO;
using webapi.Interfaces;

namespace webapi.BusinessRules
{
    public class OwnerBusinessRules : IOwnerBusinessRules
    {
        private readonly DataContext _context;
        public OwnerBusinessRules(DataContext context)
        {
            _context = context;
        }
        public bool OwnerExists(int id)
        {
            return _context.Owners.Any(o => o.Id == id);
        }

        public bool OwnerExists(OwnerDTO ownerDTO)
        {
            var owner = _context.Owners.Where(o => o.FirstName.Trim().ToUpper() == ownerDTO.FirstName.TrimEnd().ToUpper()).FirstOrDefault();
            if (owner == null)
            {
                owner = _context.Owners.Where(o => o.Id == ownerDTO.Id).FirstOrDefault();
            }

            return owner != null;
        }
    }
}
