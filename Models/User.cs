using System.ComponentModel.DataAnnotations;

namespace Supervaga.Models
{
    public class User
    {
        [Key]
        public System.Guid Id { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MinLength(4, ErrorMessage = "A senha deve ser no mínimo de 4 caracteres")]
        [MaxLength(22, ErrorMessage = "A senha deve ser no máximo de 20 caracteres")]
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Biography { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Email { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public string Provider { get; set; }
        public string Cv { get; set; }
        public string Tag { get; set; }

        public string Role { get; set; }
    }
}