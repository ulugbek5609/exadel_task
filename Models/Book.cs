using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;



using System.ComponentModel.DataAnnotations;

namespace Exadel_task.Models
{
    public class Book
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("title")]
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } = null!;

        [BsonElement("author")]
        [Required(ErrorMessage = "Author is required.")]
        [MaxLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
        public string Author { get; set; } = null!;

        [BsonElement("publicationYear")]
        [Range(1900, 2026, ErrorMessage = "Publication year must be between 1900 and 2026.")]
        public int PublicationYear { get; set; }

        [BsonElement("views")]
        [Range(0, int.MaxValue, ErrorMessage = "Views count must be 0 or greater.")]
        public int Views { get; set; } = 0;

        [BsonElement("isDeleted")]
        public bool IsDeleted { get; set; } = false;
    }
}


    // internal class Program
    // {
    //     public static void Main(string[] args)
    //     {
    //         BsonClassMap.RegisterClassMap<Book>(cm =>
    //         {
    //             cm.AutoMap();
    //             cm.MapIdProperty(c => c.Id)
    //                 .SetIdGenerator(StringObjectIdGenerator.Instance)
    //                 .SetSerializer(new StringSerializer(BsonType.ObjectId));
    //         });
    //     }
    // }

