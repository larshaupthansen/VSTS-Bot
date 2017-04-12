﻿//———————————————————————————————
// <copyright file=”name of this file, i.e. RootDialogTests.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the RootDialog.
// </summary>
//———————————————————————————————

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vsar.TSBot.Dialogs;

namespace Vsar.TSBot.UnitTests
{
    [TestClass]
    public class RootDialogTests : TestsBase<DialogFixture>
    {
        public RootDialogTests() : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task EchoDialog()
        {
            var toBot = Fixture.CreateMessage();
            toBot.From.Id = Guid.NewGuid().ToString();
            toBot.Text = "Test";

            var root = new RootDialog();

            using (var container = Fixture.Build(root))
            {
                var toUser = await Fixture.GetResponse(container, root, toBot);

                toUser.Text.Should().Contain("4").And.Contain("Test");
            }
        }
    }
}