using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTO;
using webapi.Interfaces;

namespace webapi.BusinessRules
{
    public class PokemonBusinessRules : IPokemonBusinessRules
    {
        private readonly DataContext _context;
        public PokemonBusinessRules(DataContext context)
        {
            _context = context;
        }

        public bool PokemonExists(int id)
        {
            return _context.Pokemon.Any(p => p.Id == id);
        }

        public bool PokemonExists(PokemonDTO pokemonDTO)
        {
            var pokemon = _context.Pokemon.Where(p => p.Name.Trim().ToUpper() == pokemonDTO.Name.TrimEnd().ToUpper()).FirstOrDefault();
            if (pokemon == null)
            {
                pokemon = _context.Pokemon.Where(p => p.Id == pokemonDTO.Id).FirstOrDefault();
            }

            return pokemon != null;
        }
    }
}
