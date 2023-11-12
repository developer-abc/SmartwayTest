using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmartwayTest.Model
{
    [Index(nameof(UserId))]
    public class GroupModel
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public double Progress => Files.Average(x => x.Progress);
        [JsonIgnore]
        public virtual List<FileModel> Files { get; set; }
        [NotMapped]
        public IEnumerable<string> FileIds => Files.Select(f => f.Id);
        [JsonIgnore]
        public string? DownloadToken { get; set; }
    }
}
