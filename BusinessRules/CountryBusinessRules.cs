using webapi.Data;
using webapi.DTO;
using webapi.Interfaces;

namespace webapi.BusinessRules
{
    public class CountryBusinessRules : ICountryBusinessRules
    {
        private readonly DataContext _context;
        public CountryBusinessRules(DataContext context)
        {
            _context = context;
        }
        public bool CountryExists(int id)
        {
            return _context.Countries.Any(c => c.Id == id);

        }

        public bool CountryExists(CountryDTO countryDTO)
        {

            var country = _context.Countries.Where(c => c.Name.Trim().ToUpper() == countryDTO.Name.TrimEnd().ToUpper()).FirstOrDefault();
            if (country == null)
            {
                country = _context.Countries.Where(c => c.Id == countryDTO.Id).FirstOrDefault();
            }

            return country != null;
            
        }
    }
}
