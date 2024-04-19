using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ArtsieApi.Models;
public class ArtsieDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ArtCollectionName { get; set; } = null!;
}
public class Art
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string? Id { get; set; }

     [BsonElement("name")]
    public required string? Name { get; set; }
         [BsonElement("artist")]
    public required string? Artist { get; set; }
         [BsonElement("description")]
    public required string? Description { get; set; }
    public string? ImgUrl { get; set; }
}