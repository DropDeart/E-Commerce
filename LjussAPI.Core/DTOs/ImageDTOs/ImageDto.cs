using Ljuss.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LjussAPI.Application.DTOs.ImageDTOs
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string? FileDescription { get; set; }
        public string FileUrl { get; set; }
        public ImageTypeEnum Type { get; set; }
        public Guid? AttributeValueId { get; set; }
        public bool IsMain { get; set; }
    }
}
