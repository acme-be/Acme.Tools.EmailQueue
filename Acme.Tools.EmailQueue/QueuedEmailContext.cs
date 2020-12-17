// <copyright file="QueuedEmailContext.cs" company="Acme">
// Copyright (c) Acme. All rights reserved.
// </copyright>

namespace Acme.Tools.EmailQueue
{
    using System;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represent a databse context for the queued emails.
    /// </summary>
    internal class QueuedEmailContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedEmailContext" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string used by this context.</param>
        public QueuedEmailContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the name of the connection string used by this service.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets or sets the QueuedEmails.
        /// </summary>
        /// <value>The QueuedEmails.</value>
        public DbSet<QueuedEmail> QueuedEmails { get; set; }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}