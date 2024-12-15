using FamilyTree.DAL.DTO;
using FamilyTree.DAL.Entities;
using FamilyTree.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.DAL.Repositories.Database;

public class FamilyTreeRepository : IFamilyTreeRepository
{
    private readonly FamilyTreeDatabaseContext _context;

    public FamilyTreeRepository(FamilyTreeDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddPersonAsync(PersonDTO person)
    {
        Person personEntity = new()
        {
            Name = person.Name,
            BirthDate = person.BirthDate,
            Gender = (Gender)person.Gender
        };

        await _context.Persons.AddAsync(personEntity);
        await _context.SaveChangesAsync();
    }

    public async Task AddRelationAsync(RelationDTO relation)
    {
        var relationEntity = new Relation
        {
            FromPersonId = relation.FromPersonId,
            ToPersonId = relation.ToPersonId,
            Type = (RelationType)relation.Type
        };

        await _context.Relations.AddAsync(relationEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<PersonDTO?> GetPersonByIdAsync(int id)
    {
        var person = await _context.Persons
            .FirstOrDefaultAsync(p => p.Id == id);

        return person == null
            ? null
            : new PersonDTO
            {
                Id = person.Id,
                Name = person.Name,
                BirthDate = person.BirthDate,
                Gender = (int)person.Gender
            };
    }

    public async Task<IEnumerable<PersonDTO>> GetAllPersonsAsync()
    {
        var persons = await _context.Persons
           .ToListAsync();

        return persons.Select(p => new PersonDTO
        {
            Id = p.Id,
            Name = p.Name,
            BirthDate = p.BirthDate,
            Gender = (int)p.Gender
        });
    }

    public async Task<IEnumerable<RelationDTO>> GetAllRelationsAsync()
    {
        var relations = await _context.Relations
            .ToListAsync();

        return relations.Select(r => new RelationDTO
        {
            Id = r.Id,
            FromPersonId = r.FromPersonId,
            ToPersonId = r.ToPersonId,
            Type = (int)r.Type
        });
    }

    public async Task<IEnumerable<PersonDTO>> GetRelativesByIdAsync(int personId)
    {
        var relations = await _context.Relations
            .Where(r => r.FromPersonId == personId && (r.Type == RelationType.Parent || r.Type == RelationType.Child))
            .ToListAsync();

        List<PersonDTO> relatives = new();
        foreach (var relation in relations)
        {
            var person = await _context.Persons
                .FirstOrDefaultAsync(p => p.Id == relation.ToPersonId);
            relatives.Add(new PersonDTO
            {
                Id = person.Id,
                Name = person.Name,
                BirthDate = person.BirthDate,
                Gender = (int)person.Gender
            });
        }

        return relatives;
    }

    public async Task ClearTreeAsync()
    {
        //_context.Persons.RemoveRange(_context.Persons);
        _context.Relations.RemoveRange(_context.Relations);
        await _context.SaveChangesAsync();
    }
}
