using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.DTOs.Review
{
    public class CreateReviewDto
    {
    public int ProductId { get; set; }
    public int Rating { get; set; }       
    public string? Comment { get; set; }
    }
}