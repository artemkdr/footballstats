using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Models
{
    [Table("teams")]
    public class Team
    {
        [Key] 
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("players")]
        public string[]? Players { get; set; } 
        [Column("createdate")]
        public DateTime Createdate { get; set; }
        [Column("modifydate")]
        public DateTime Modifydate { get; set; }
    }
}