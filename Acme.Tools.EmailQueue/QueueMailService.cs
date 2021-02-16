// <copyright file="QueueMailService.cs" company="Acme">
// Copyright (c) Acme. All rights reserved.
// </copyright>

namespace Acme.Tools.EmailQueue
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    using Acme.Core.Extensions;

    using Newtonsoft.Json;

    /// <summary>
    /// Service to enqueue mails into a database and process it when we want.
    /// </summary>
    public class QueueMailService
    {
        /// <summary>
        /// Avoid multiple concurrent call to emails.
        /// </summary>
        private static readonly object LockSendMails = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueMailService" /> class.
        /// </summary>
        /// <param name="connectionString">The name of the connection string used by this service.</param>
        /// <param name="adminEmail">The admin email used when sending mails with copy.</param>
        /// <param name="redirectEmail">if set to <c>true</c> [redirect email].</param>
        /// <param name="redirectionAdress">The redirection adress.</param>
        public QueueMailService(string connectionString, string adminEmail = null, bool redirectEmail = false, string redirectionAdress = null)
        {
            this.ConnectionString = connectionString;
            this.AdminEmail = adminEmail;
            this.CustomHeaders = new NameValueCollection();
            this.RedirectEmail = redirectEmail;
            this.RedirectionAdress = redirectionAdress;
        }

        /// <summary>
        /// Gets the admin email used when sending mails with copy.
        /// </summary>
        public string AdminEmail { get; }

        /// <summary>
        /// Gets the name of the connection string used by this service.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets the CustomHeaders to send when sending mail with smtp.
        /// </summary>
        /// <value>The CustomHeaders.</value>
        public NameValueCollection CustomHeaders { get; }

        /// <summary>
        /// Gets a value indicating whether [redirect email].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [redirect email]; otherwise, <c>false</c>.
        /// </value>
        public bool RedirectEmail { get; }

        /// <summary>
        /// Gets the redirection adress.
        /// </summary>
        /// <value>
        /// The redirection adress.
        /// </value>
        public string RedirectionAdress { get; }

        /// <summary>
        /// Enqueue an email and store it into the database.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="title">The title.</param>
        /// <param name="body">The body of the email.</param>
        /// <param name="recipient">The recipient of the email.</param>
        /// <param name="adminCopy">If we must send a copy to the admin.</param>
        /// <param name="replyTo">The adresse to use in reply to.</param>
        /// <param name="attachments">The optional attachments.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation, with the guid associated to the email.</returns>
        public async Task<Guid> EnqueueAsync(string sender, string title, string body, string recipient, bool adminCopy = false, string replyTo = null, params EmailAttachment[] attachments)
        {
            sender.ThrowIfNull(nameof(sender));
            title.ThrowIfNull(nameof(title));
            body.ThrowIfNull(nameof(body));
            recipient.ThrowIfNull(nameof(recipient));

            var mail = new Email();
            mail.Sender = sender;
            mail.Recipients = new List<string> { recipient };
            mail.ReplyTo = replyTo;
            mail.Subject = title;
            mail.Body = body;
            mail.AdminCopy = adminCopy;

            if (attachments != null)
            {
                mail.Attachments = JsonConvert.SerializeObject(attachments);
            }

            return await this.EnqueueAsync(mail);
        }

        /// <summary>
        /// Enqueue an email and store it into the database.
        /// </summary>
        /// <param name="mail">The mail to be enqueued.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation, with the guid associated to the email.</returns>
        public async Task<Guid> EnqueueAsync(Email mail)
        {
            var queuedEmail = new QueuedEmail();
            queuedEmail.Id = Guid.NewGuid();
            queuedEmail.Email = JsonConvert.SerializeObject(mail, Formatting.Indented);

            using (var context = new QueuedEmailContext(this.ConnectionString))
            {
                await context.QueuedEmails.AddAsync(queuedEmail);
                await context.SaveChangesAsync();

                return queuedEmail.Id;
            }
        }

        /// <summary>
        /// Send all mails in queue.
        /// </summary>
        /// <param name="client">Specify a custom smtp client to be used.</param>
        /// <param name="waitTime">Wait time between emails, in milliseconds.</param>
        public void SendQueuedMails(SmtpClient client = null, int waitTime = 0)
        {
            if (client == null)
            {
                client = new SmtpClient();
            }

            lock (LockSendMails)
            {
                using (var context = new QueuedEmailContext(this.ConnectionString))
                {
                    var queuedEmails = context.QueuedEmails.Where(x => x.Sent == null).ToList();
                    foreach (var queuedEmail in queuedEmails)
                    {
                        var email = JsonConvert.DeserializeObject<Email>(queuedEmail.Email);

                        var message = new MailMessage();
                        message.From = new MailAddress(email.Sender);
                        message.Subject = email.Subject;
                        message.Body = email.Body;
                        message.IsBodyHtml = true;

                        if (email.ReplyTo != null)
                        {
                            message.ReplyToList.Add(new MailAddress(email.ReplyTo));
                        }

                        if (email.Recipients != null)
                        {
                            foreach (var recipient in email.Recipients)
                            {
                                message.To.Add(new MailAddress(recipient));
                            }
                        }

                        if (email.RecipientsCopy != null)
                        {
                            foreach (var recipientCopy in email.RecipientsCopy)
                            {
                                message.CC.Add(new MailAddress(recipientCopy));
                            }
                        }

                        if (email.RecipientsBlindCopy != null)
                        {
                            foreach (var recipientBlindCopy in email.RecipientsBlindCopy)
                            {
                                message.Bcc.Add(new MailAddress(recipientBlindCopy));
                            }
                        }

                        if (email.AdminCopy)
                        {
                            message.To.Add(new MailAddress(this.AdminEmail));
                        }

                        if (this.RedirectEmail)
                        {
                            var originalAdress = string.Join(", ", message.To.Select(x => x.Address));
                            message.Subject += " (" + originalAdress + ")";

                            message.To.Clear();
                            message.To.Add(new MailAddress(this.RedirectionAdress));
                        }

                        if (email.Attachments != null)
                        {
                            var attachments = JsonConvert.DeserializeObject<EmailAttachment[]>(email.Attachments);
                            foreach (var attachment in attachments)
                            {
                                var a = new Attachment(new MemoryStream(attachment.FileData), attachment.FileName);
                                message.Attachments.Add(a);
                            }
                        }

                        foreach (var customHeaderKey in this.CustomHeaders.AllKeys)
                        {
                            message.Headers.Add(customHeaderKey, this.CustomHeaders[customHeaderKey]);
                        }

                        try
                        {
                            client.Send(message);
                            queuedEmail.Sent = DateTime.Now;
                            queuedEmail.ResultMessage = null;

                            if (waitTime != 0)
                            {
                                Thread.Sleep(waitTime);
                            }
                        }
                        catch (SmtpException e)
                        {
                            queuedEmail.ResultMessage = e.ToString();
                        }

                        context.SaveChanges();
                    }
                }
            }
        }
    }
}