﻿using SV20T1020275.DomainModels;

namespace SV20T1020275.Web.Models
{
    public class OrderDetailModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}
