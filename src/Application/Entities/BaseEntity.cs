using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Entities;

public abstract class BaseEntity
{
    //[Key]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    //public DateTime InsertTime { get; set; }

    //Partition Key
    [JsonPropertyName("pk")]
    public string Pk => Id;

    //SortKey
    [JsonPropertyName("sk")]
    public string Sk => Pk;
}