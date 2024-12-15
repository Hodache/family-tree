namespace FamilyTree.BLL.DTO;

public class RelationCreationDTO
{
    public int FromPersonId { get; set; }
    public int ToPersonId { get; set; }
    public int Type { get; set; }
}
