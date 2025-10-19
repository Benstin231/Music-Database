using System.ComponentModel.DataAnnotations;

namespace s1121735_Final_Project.Models
{
    public class Songs
    {
        [Key]
        public int SongID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Artist { get; set; }

        [StringLength(100)]
        public string Album { get; set; }

        [StringLength(50)]
        public string Genre { get; set; }

        public int? Duration { get; set; }

        public int? ReleaseYear { get; set; }

        [StringLength(255)]
        public string ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
