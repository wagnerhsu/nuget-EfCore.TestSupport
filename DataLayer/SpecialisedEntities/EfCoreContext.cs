﻿// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.SpecialisedEntities.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.SpecialisedEntities
{
    public class SpecializedDbCotext : DbContext
    {
        public DbSet<BookSummary> BookSummaries { get; set; }
        public DbSet<OrderInfo> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }


        public SpecializedDbCotext(DbContextOptions<SpecializedDbCotext> options)      
            : base(options) {}

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookSummaryConfig());
            modelBuilder.ApplyConfiguration(new BookDetailConfig());
            modelBuilder.ApplyConfiguration(new OrderInfoConfig());
            modelBuilder.ApplyConfiguration(new PaymentConfig());
        }
    }
}
