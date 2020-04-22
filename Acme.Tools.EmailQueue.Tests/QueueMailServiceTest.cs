// <copyright file="QueueMailServiceTest.cs" company="Acme">
// Copyright (c) Acme. All rights reserved.
// </copyright>

namespace Acme.Tools.EmailQueue.Tests
{
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
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task EnqueueOk()
        {
            var service = new QueueMailService("Data Source=.;Initial Catalog=acme-queue-mail-tests;Integrated Security=True");
            await service.EnqueueAsync("acme@yopmail.com", "Unit Test : EnqueueOk", "This is a unit test !", "acme@yopmail.com");
        }
    }
}