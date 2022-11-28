using System.ComponentModel.DataAnnotations;

namespace Application.Entities;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime InsertTime { get; set; }
}