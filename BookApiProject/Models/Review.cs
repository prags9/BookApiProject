using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(200,MinimumLength =10, ErrorMessage ="Headline should be between 10 and 200")]
        public string Headline { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 50, ErrorMessage = "Review Text should be between 50 and 200")]
        public string ReviewText { get; set; }
        [Range(1,5, ErrorMessage = "Rating should be between 1 and 5 stars")]
        public int Rating { get; set; }
        public virtual Reviewer Reviewer { get; set; }
        public virtual Book Book { get; set; }
    }
}
