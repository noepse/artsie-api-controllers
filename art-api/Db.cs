namespace Artsie.DB;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;

public record Art
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Artist { get; set; }
    public string? Description { get; set; }
    public string? ImgUrl { get; set; }
}

public class ArtDB
{
    private static List<Art> _art = new List<Art>()
   {
     new Art{ Id=1, Name="Cleopatra decorating the Tomb of Mark Anthony", Artist="Angelica Kauffman", Description="The scene portrays Cleopatra in a moment of mourning and love, as she decorates the tomb of Mark Antony, her lover and ally, who had committed suicide after being defeated by Octavian (later Emperor Augustus) in the Battle of Actium in 31 BC. Cleopatra, with a look of sorrow and determination, is shown placing a wreath on Antony's tomb, surrounded by mourners and attendants." },
     new Art{ Id=2, Name="Wanderer above the Sea of Fog", Artist = "Caspar David Friedrich", Description="The painting depicts a solitary figure standing atop a rocky precipice, gazing out over a vast, misty landscape of mountains and fog-covered valleys. The figure, often interpreted as a representation of the Romantic wanderer or the sublime individual, stands with his back to the viewer, his face obscured, and his identity ambiguous. He is dressed in a dark overcoat and wears a wide-brimmed hat, adding to the sense of mystery and anonymity. The painting is often interpreted as a meditation on the human experience of the sublime—the overwhelming sense of awe, terror, and transcendence inspired by nature's grandeur. "},
     new Art{ Id=3, Name="The Great Day of His Wrath", Artist = "John Martin", Description="In this painting, Martin depicts a cataclysmic scene of divine judgment and apocalyptic destruction. The painting shows a vast landscape engulfed in chaos and devastation, with fire, brimstone, and volcanic eruptions wreaking havoc upon the earth. In the foreground, terrified figures, including women, men, and children, flee in panic from the impending doom, while others kneel in prayer or despair. The sky is darkened by swirling clouds, lightning bolts, and ominous storm clouds, heightening the sense of dread and impending doom."}
   };

    public static Art? GetArtById(int id)
    {
    var art = _art.SingleOrDefault(art => art.Id == id);
    return art;
    }
}

public record Comment
{
    public int Id { get; set; }
    public int? ArtId { get; set; }
    public string? Author { get; set; }
    public string? Body { get; set; }

    public int? Likes { get; set; }

}

public class CommentsDB
{

    private static List<Comment> _comments = new List<Comment>()
   {
     new Comment{ Id=1, ArtId=1, Author="froggie", Body="Nice art!" },
   };


    public static List<Comment>? GetCommentsByArtId(int id)
    {
        return _comments.FindAll(comment => comment.ArtId == id).ToList();
    }

    public static Comment CreateComment(Comment comment)
    {
        _comments.Add(comment);
        return comment;
    }

    public static Comment UpdateComment(Comment update)
    {
        _comments = _comments.Select(comment =>
        {
            if (comment.Id == update.Id)
            {
                comment.Likes += update.Likes;
            }
            return comment;
        }).ToList();
        return update;
    }

    public static void RemoveComment(int id)
    {
        _comments = _comments.FindAll(comment => comment.Id != id).ToList();
    }

}
public record User
{
    public int Id { get; set; }
    public string? Username { get; set; }

}

public class UsersDB
{

    private static List<User> _users = new List<User>()
   {
     new User{ Id=1, Username="froggie"},
   };

       public static User? GetUserById(int id)
    {
    var user = _users.SingleOrDefault(user => user.Id == id);
    return user;
    }
}