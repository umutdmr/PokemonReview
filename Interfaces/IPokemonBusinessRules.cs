using webapi.DTO;

namespace webapi.Interfaces
{
    public interface IPokemonBusinessRules
    {
        bool PokemonExists(int id);
        bool PokemonExists(PokemonDTO pokemonDTO);
    }
}
