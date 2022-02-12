using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Supervaga.Models
{
    public class Ad
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MinLength(10, ErrorMessage = "Este campo deve conter no mínimo 10 caracteres")]
        [MaxLength(1024, ErrorMessage = "Este campo deve conter no máximo 1024 caracteres")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Address { get; set; }
        public string AudienceGender { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public DateTime ExpiresAt { get; set; }

        public User User { get; set; }
        public bool IsFreelance { get; set; }


        [Required(ErrorMessage = "Este campo é obrigatório")]
        public IList<Advantage> Advantages { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public IList<Requirement> Requirements { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public float SalaryOffer { get; set; }
    }

    public class Requirement
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid AdId { get; set; }

        [Required]
        public string Title { get; set; }
    }

    public class Advantage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid AdId { get; set; }

        [Required]
        public string Title { get; set; }
    }
}