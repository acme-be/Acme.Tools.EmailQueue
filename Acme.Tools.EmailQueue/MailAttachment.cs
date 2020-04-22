// <copyright file="MailAttachment.cs" company="Acme">
// Copyright (c) Acme. All rights reserved.
// </copyright>

namespace Acme.Tools.EmailQueue
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represent an attachment to the email.
    /// </summary>
    public class MailAttachment
    {
        /// <summary>
        /// Gets or sets the file binary data.
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }
    }
}