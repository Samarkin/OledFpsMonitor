using NUnit.Framework;
using System;

namespace Samarkin.Oled.Tests
{
	[TestFixture]
	public class OledClientTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Consturctor_WhenAddressIsHostName_BuildsCorrectUri()
		{
			var client = new OledClient("localhost");
			Assert.That(client.RootUri, Is.EqualTo(new Uri("http://localhost/api")));
		}

		[Test]
		public void Constructor_WhenAddressIsComplex_BuildsCorrectUri()
		{
			var client = new OledClient("http://raspberrypi.local:6533/oled");
			Assert.That(client.RootUri, Is.EqualTo(new Uri("http://raspberrypi.local:6533/oled/api")));
		}
	}
}