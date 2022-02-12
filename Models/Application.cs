using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supervaga.Models
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public Guid AdId { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateTime ExpiresAt { get; set; }
        public IList<Candidate> Candidates { get; set; }

        public Ad Ad { get; set; }
    }
}