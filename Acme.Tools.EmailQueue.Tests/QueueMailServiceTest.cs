// <copyright file="QueueMailServiceTest.cs" company="Acme">
// Copyright (c) Acme. All rights reserved.
// </copyright>

namespace Acme.Tools.EmailQueue.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;

    using Xunit;

    /// <summary>
    /// Test the QueueMailService.
    /// </summary>
    public class QueueMailServiceTest
    {
        /// <summary>
        /// Test the Enqueue method.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task EnqueueOk()
        {
            var service = new QueueMailService("Data Source=.;Initial Catalog=acme-queue-mail-tests;Integrated Security=True");
            await service.EnqueueAsync("acme@yopmail.com", "Unit Test : EnqueueOk", "This is a unit test !", "acme@yopmail.com");
        }

        /// <summary>
        /// Test the sending of the mail queue.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task SendQueuedMailsOk()
        {
            var service = new QueueMailService("Data Source=.;Initial Catalog=acme-queue-mail-tests;Integrated Security=True");

            var smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            smtpClient.PickupDirectoryLocation = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(smtpClient.PickupDirectoryLocation);

            await service.EnqueueAsync("acme@yopmail.com", "Unit Test : EnqueueOk", "This is a unit test !", "acme@yopmail.com");
            service.SendQueuedMails(smtpClient);

            Assert.NotEmpty(Directory.GetFiles(smtpClient.PickupDirectoryLocation));

            Directory.Delete(smtpClient.PickupDirectoryLocation, true);
        }
    }
}