using webapi.DTO;

namespace webapi.Interfaces
{
    public interface ICountryBusinessRules
    {
        bool CountryExists(int id);
        bool CountryExists(CountryDTO countryDTO);
    }
}
