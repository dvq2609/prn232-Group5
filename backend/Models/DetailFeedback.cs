using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class DetailFeedback
{
    public int Id { get; set; }

    public int? DeliveryOnTime { get; set; }

    public int? ExactSame { get; set; }

    public int? Communication { get; set; }

    public int FeedbackId { get; set; }
}
