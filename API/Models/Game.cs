using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Models
{
    [Table("games")]
    public class Game
    {
        [Key] 
        [Column("id")]
        public int Id { get; set; }
        [Column("team1")]
        public int Team1 { get; set; }
        [Column("team2")]
        public int Team2 { get; set; }
        
        [Column("goals1")]
        public int Goals1 { get; set; }
        [Column("goals2")]
        public int Goals2 { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("vars")]
        public JsonDocument? Vars { get; set; } 

        [Column("createdate")]
        public DateTime Createdate { get; set; }

        [Column("completeddate")]
        public DateTime CompletedDate { get; set; }
        
        [Column("modifydate")]
        public DateTime Modifydate { get; set; }
    }
}