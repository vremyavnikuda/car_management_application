using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace document_management.Models.ViewModels
{
    public class DocumentUploadViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, введите название документа")]
        [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
        [Display(Name = "Название")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Пожалуйста, выберите тип документа")]
        [Display(Name = "Тип документа")]
        public required string DocumentType { get; set; }

        [Required(ErrorMessage = "Пожалуйста, выберите статус")]
        [Display(Name = "Статус")]
        public required string Status { get; set; }

        [Required(ErrorMessage = "Пожалуйста, выберите файл")]
        [Display(Name = "Файл")]
        public IFormFile? File { get; set; }

        [Display(Name = "Описание изменений")]
        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string? ChangeDescription { get; set; }
    }
} 