using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ArtsieApi.Models;
public class ArtsieDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ArtCollectionName { get; set; } = null!;
    public string CommentsCollectionName { get; set; } = null!;
    public string UsersCollectionName { get; set; } = null!;
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

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
         [BsonElement("artId")]
    public string? ArtId { get; set; }

     [BsonElement("author")]
    public required string? Author { get; set; }
         [BsonElement("body")]
    public required string? Body { get; set; }
         [BsonElement("likes")]
    public int? Likes { get; set; }
}

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

     [BsonElement("username")]
    public required string? Username { get; set; }
}
public class Likes
{

     [BsonElement("inc_votes")]
    public required int? IncLikes { get; set; }
}