using webapi.DTO;

namespace webapi.Interfaces
{
    public interface IOwnerBusinessRules
    {
        bool OwnerExists(int id);
        bool OwnerExists(OwnerDTO ownerDTO);

    }
}
