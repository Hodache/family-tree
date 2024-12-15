using FamilyTree.DAL.DTO;
using FamilyTree.BLL.DTO;

namespace FamilyTree.BLL.Interfaces;

public interface IFamilyTreeService
{
    // Создать сущность "Человек"
    Task CreatePersonAsync(PersonCreationDTO person);

    // Установить отношение родства (родитель, ребенок или супруг) и добавить сущности в дерево
    Task AddRelationAsync(RelationCreationDTO relation);

    // Вывести ближайших родственников (родителей и детей)
    Task<IEnumerable<PersonResponseDTO>> GetRelativesAsync(int personId);

    // Показать получившееся дерево
    Task<IEnumerable<PersonResponseDTO>> GetPersonsAsync();
    Task<IEnumerable<RelationResponseDTO>> GetRelationsAsync();

    // Вычислить возраст человека на момент рождения другого человека
    Task<int> CalculatePersonAgeAtBirth(int personId, int bornPersonId);

    // Создать новое дерево (очистить существующее)
    Task ClearTreeAsync();
}
