// <copyright file="QueuedEmail.cs" company="Acme">
// Copyright (c) Acme. All rights reserved.
// </copyright>

namespace Acme.Tools.EmailQueue
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Represent an email into the queue.
    /// </summary>
    [Table("QueuedEmails", Schema = "acme")]
    internal class QueuedEmail
    {
        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        /// <value>The Email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the id of the mail.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the result message.
        /// </summary>
        public string ResultMessage { get; set; }

        /// <summary>
        /// Gets or sets the sent date.
        /// Null if the mail has not been sent.
        /// </summary>
        public DateTime? Sent { get; set; }
    }
}