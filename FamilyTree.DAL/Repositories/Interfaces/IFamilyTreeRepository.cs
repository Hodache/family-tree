using FamilyTree.DAL.DTO;

namespace FamilyTree.DAL.Repositories.Interfaces;

public interface IFamilyTreeRepository
{
    Task AddPersonAsync(PersonDTO person);
    Task AddRelationAsync(RelationDTO relation);
    Task<IEnumerable<RelationDTO>> GetAllRelationsAsync();
    Task<IEnumerable<PersonDTO>> GetAllPersonsAsync();
    Task<PersonDTO?> GetPersonByIdAsync(int id);
    Task<IEnumerable<PersonDTO>> GetRelativesByIdAsync(int personId);
    Task ClearTreeAsync();
}
