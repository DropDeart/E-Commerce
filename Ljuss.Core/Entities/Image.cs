using Ljuss.Core.Common;
using Ljuss.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ljuss.Core.Entities
{
    public class Image : BaseEntity
    {
        public string OwnerID { get; set; }
        public ImageTypeEnum Type { get; set; }
        public string FileName { get; set; }
        public string? FileDescription { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes {  get; set; }
        public string FilePath { get; set; }
        public Guid? AttributeValueId { get; set; } 
        public AttributeValue? AttributeValue { get; set; }
        public bool IsMain { get; set; } = false;

        [NotMapped]
        public IFormFile File { get; set; }
    }
}
