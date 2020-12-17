// <copyright file="Email.cs" company="Acme">
// Copyright (c) Acme. All rights reserved.
// </copyright>

namespace Acme.Tools.EmailQueue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represent an email into the queue.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Gets or sets a value indicating whether the mail should be sent to the admin.
        /// </summary>
        public bool AdminCopy { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the recipient.
        /// </summary>
        public List<string> Recipients { get; set; }

        /// <summary>
        /// Gets or sets the recipient that will receive a blind copy.
        /// </summary>
        public List<string> RecipientsBlindCopy { get; set; }

        /// <summary>
        /// Gets or sets the recipient that will receive a copy.
        /// </summary>
        public List<string> RecipientsCopy { get; set; }

        /// <summary>
        /// Gets or sets the use to reply to.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the subject of the mail.
        /// </summary>
        public string Subject { get; set; }
    }
}