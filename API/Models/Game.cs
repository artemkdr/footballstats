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

        [Column("status", TypeName = "text")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GameStatus Status { get; set; }

        [Column("vars")]
        public JsonDocument? Vars { get; set; } 

        [Column("createdate")]
        public DateTime CreateDate { get; set; }

        [Column("completedate")]
        public DateTime? CompleteDate { get; set; }
        
        [Column("modifydate")]
        public DateTime ModifyDate { get; set; }
    }

    public enum GameStatus 
    {
        NotStarted,
        Playing,
        Completed,
        Cancelled
    }
}