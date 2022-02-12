using System;
using System.ComponentModel.DataAnnotations;

namespace Supervaga.Models
{
    public class Candidate
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public Guid ApplicationId { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public Guid AdId { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public bool IsCV { get; set; }
    }
}