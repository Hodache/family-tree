﻿namespace FamilyTree.DAL.DTO;

public class RelationDTO
{
    public int Id { get; set; }
    public int FromPersonId { get; set; }
    public int ToPersonId { get; set; }
    public int Type { get; set; }
}
