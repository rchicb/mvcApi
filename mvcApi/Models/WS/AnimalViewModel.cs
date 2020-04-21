using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mvcApi.Models.WS
{
    public class AnimalViewModel
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public int Patas { get; set; }
        [Required]
        public string Token { get; set; }
    }

    public class EditAnimalViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public int Patas { get; set; }
        [Required]
        public string Token { get; set; }
    }
    public class DeleteAnimalViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Token { get; set; }
    }
}