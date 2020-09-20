using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApi.Model
{
    public class TaskItem
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long TaskItemId { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Notes { get; set; }
        public string Comment { get; set; }

        [ForeignKey("UserId")]
        public long UserId { get; set; }
    }
}
