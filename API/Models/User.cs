using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Models
{
    [Table("users")]
    public class User
    {
        [Key] 
        [Column("username")]
        public string? Username { get; set; }
        
        [Column("password")]
        [JsonIgnore]
        public string? Password { get; set; }
        
        [Column("vars")]
        public JsonDocument? Vars { get; set; } 
        
        [Column("createdate")]
        public DateTime CreateDate { get; set; }
        
        [Column("modifydate")]
        public DateTime ModifyDate { get; set; }

        [Column("status", TypeName = "text")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus Status { get; set; }
    }

    public enum UserStatus 
    {
        Active,
        Deleted
    }
}