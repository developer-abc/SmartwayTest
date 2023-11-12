using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmartwayTest.Model
{
    [Index(nameof(UserId))]
    public class FileModel
    {
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        public string FileName { get; set; }
        public int UserId { get; set; }
        public double Progress { get; set; } = 0;
        public byte[] FileContent { get; set; } = new byte[0];
        [JsonIgnore]
        public virtual GroupModel GroupModel { get; set; }
        [NotMapped]
        public string GroupId => GroupModel.Id;
        [JsonIgnore]
        public string? DownloadToken { get; set; }
    }
}
