using FamilyTree.BLL.Interfaces;
using FamilyTree.BLL.DTO;
using FamilyTree.DAL.DTO;
using FamilyTree.DAL.Repositories.Interfaces;

namespace FamilyTree.BLL.Services;

public class FamilyTreeService : IFamilyTreeService
{
    private readonly IFamilyTreeRepository _repository;

    public FamilyTreeService(IFamilyTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task CreatePersonAsync(PersonCreationDTO person)
    {
        if (person.Gender < 0 || person.Gender > 1)
        {
            throw new ArgumentException("Invalid relation type");
        }

        await _repository.AddPersonAsync(new PersonDTO
        {
            Name = person.Name,
            BirthDate = person.BirthDate ?? DateTime.Today,
            Gender = person.Gender
        });
    }

    public async Task AddRelationAsync(RelationCreationDTO relation)
    {
        if (relation.Type < 0 || relation.Type > 1)
        {
            throw new ArgumentException("Invalid relation type");
        }

        if (relation.FromPersonId == relation.ToPersonId)
        {
            throw new ArgumentException("Same person");
        }

        var fromPerson = await _repository.GetPersonByIdAsync(relation.FromPersonId);
        if (fromPerson == null)
        {
            throw new ArgumentException("Person not found");
        }

        var toPerson = await _repository.GetPersonByIdAsync(relation.ToPersonId);
        if (toPerson == null)
        {
            throw new ArgumentException("Person not found");
        }

        var relations = await _repository.GetAllRelationsAsync();
        switch (relation.Type)
        {
            case 0:
                if (fromPerson.Gender == toPerson.Gender)
                {
                    throw new ArgumentException("Same gender");
                }

                if (relations.Any(r => (r.FromPersonId == fromPerson.Id || r.ToPersonId == toPerson.Id) && r.Type == 0))
                {
                    throw new ArgumentException("Already have spouse");
                }

                await _repository.AddRelationAsync(new RelationDTO
                {
                    FromPersonId = relation.FromPersonId,
                    ToPersonId = relation.ToPersonId,
                    Type = relation.Type
                });
                await _repository.AddRelationAsync(new RelationDTO
                {
                    FromPersonId = relation.ToPersonId,
                    ToPersonId = relation.FromPersonId,
                    Type = relation.Type
                });
                break;
            case 1:
                var partnerId = relations.FirstOrDefault(r => r.FromPersonId == fromPerson.Id && r.Type == 0)?.ToPersonId;
                if (partnerId == null)
                {
                    throw new ArgumentException("Doesn't have a partner");
                }

                var partner = await _repository.GetPersonByIdAsync(partnerId.Value);
                if (fromPerson.BirthDate.Year - toPerson.BirthDate.Year >= 0 || partner.BirthDate.Year - toPerson.BirthDate.Year >= 0)
                {
                    throw new ArgumentException("Parent born after child");
                }

                if (relations.Any(r => r.FromPersonId != fromPerson.Id && r.ToPersonId == toPerson.Id && r.Type == 1))
                {
                    throw new ArgumentException("Already have parent");
                }

                await _repository.AddRelationAsync(new RelationDTO
                {
                    FromPersonId = relation.FromPersonId,
                    ToPersonId = relation.ToPersonId,
                    Type = relation.Type
                });

                await _repository.AddRelationAsync(new RelationDTO
                {
                    FromPersonId = partnerId.Value,
                    ToPersonId = relation.ToPersonId,
                    Type = relation.Type
                });

                await _repository.AddRelationAsync(new RelationDTO
                {
                    FromPersonId = relation.ToPersonId,
                    ToPersonId = relation.FromPersonId,
                    Type = 2
                });

                await _repository.AddRelationAsync(new RelationDTO
                {
                    FromPersonId = relation.ToPersonId,
                    ToPersonId = partnerId.Value,
                    Type = 2
                });

                break;
        }
    }

    public async Task<IEnumerable<PersonResponseDTO>> GetRelativesAsync(int personId)
    {
        var person = await _repository.GetPersonByIdAsync(personId);
        if (person == null)
        {
            throw new ArgumentException("Person not found");
        }

        var relatives = await _repository.GetRelativesByIdAsync(personId);

        return relatives.Select(p => new PersonResponseDTO
        {
            Id = p.Id,
            Name = p.Name,
            BirthDate = p.BirthDate,
            Gender = p.Gender,
        });
    }

    public async Task<IEnumerable<PersonResponseDTO>> GetPersonsAsync()
    {
        var persons = await _repository.GetAllPersonsAsync();
        return persons.Select(p => new PersonResponseDTO
        {
            Id = p.Id,
            Name = p.Name,
            BirthDate = p.BirthDate,
            Gender = p.Gender,
        });
    }

    public async Task<IEnumerable<RelationResponseDTO>> GetRelationsAsync()
    {
        var relations = await _repository.GetAllRelationsAsync();
        return relations.Select(p => new RelationResponseDTO
        {
            Id = p.Id,
            FromPersonId = p.FromPersonId,
            ToPersonId = p.ToPersonId,
            Type = p.Type
        });
    }

    public async Task<int> CalculatePersonAgeAtBirth(int personId, int bornPersonId)
    {
        if (personId == bornPersonId)
        {
            throw new ArgumentException("The same person is specified twice");
        }

        var person = await _repository.GetPersonByIdAsync(personId);
        var bornPerson = await _repository.GetPersonByIdAsync(bornPersonId);

        if (person == null || bornPerson == null) 
        {
            throw new ArgumentException("Person not found");
        }

        var age = (bornPerson.BirthDate - person.BirthDate).Days / 365;

        if (age <= 0)
        {
            throw new ArgumentException("Person born after the second person");
        }

        return age;
    }

    public async Task ClearTreeAsync()
    {
        await _repository.ClearTreeAsync();
    }
}
