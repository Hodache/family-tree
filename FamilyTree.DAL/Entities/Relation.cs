namespace FamilyTree.DAL.Entities;

public enum RelationType
{
    Spouse, // Супруг/Супруга
    Parent, // Родитель
    Child  // Ребенок
}

public class Relation
{
    public int Id { get; set; }
    public int FromPersonId { get; set; }
    public int ToPersonId { get; set; }
    public RelationType Type { get; set; }
}
