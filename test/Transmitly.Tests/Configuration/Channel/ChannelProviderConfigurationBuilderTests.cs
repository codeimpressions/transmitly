﻿// ﻿﻿Copyright (c) Code Impressions, LLC. All Rights Reserved.
//  
//  Licensed under the Apache License, Version 2.0 (the "License")
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using Moq;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;

namespace Transmitly.Channel.Configuration.Tests
{
	[TestClass()]
	public class ChannelProviderConfigurationBuilderTests
	{
		private readonly Action<IChannelProviderRegistration> fail_addConfirm = (provider) =>
		{
			Assert.Fail();
		};
		private readonly Action<IChannelProviderDeliveryReportRequestAdaptorRegistration> fail_addAdaptorConfirm = (provider) =>
		{
			Assert.Fail();
		};

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		[TestMethod]
		public void NewShouldRequireConfigurationBuilder()
		{
			Assert.ThrowsException<ArgumentNullException>(() => new ChannelProviderConfigurationBuilder(null, (opt) => { }, (opt) => { }));
		}

		[TestMethod]
		public void NewShouldRequireAddMethod()
		{
			Assert.ThrowsException<ArgumentNullException>(() => new ChannelProviderConfigurationBuilder(new CommunicationsClientBuilder(), null, null));
		}

		[TestMethod()]
		[DataRow("")]
		[DataRow(" ")]
		[DataRow(null)]
		public void AddShouldGuardEmptyOrWhitespaceChannelProviderId(string value)
		{
			var client = new Mock<IChannelProviderClient<object>>();
			var builder = new CommunicationsClientBuilder();
			var providerBuilder = new ChannelProviderConfigurationBuilder(builder, fail_addConfirm, fail_addAdaptorConfirm);
			Assert.ThrowsException<ArgumentNullException>(() => providerBuilder.Add(value, client.Object.GetType(), typeof(object), null));
		}

		[TestMethod]
		public void AddShouldGuardAgainstNullClient()
		{
			var builder = new CommunicationsClientBuilder();
			var providerBuilder = new ChannelProviderConfigurationBuilder(builder, fail_addConfirm, fail_addAdaptorConfirm);
			Assert.ThrowsException<ArgumentNullException>(() => providerBuilder.Add("test", null, typeof(object), null));
		}

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

		[TestMethod]
		public void AddMethodShouldBeUsedOnAdd()
		{
			var expectedId = "unit-test";
			void addConfirm(IChannelProviderRegistration provider)
			{
				Assert.IsNotNull(provider);
				Assert.AreEqual(expectedId, provider.Id);
			}
			var client = new Mock<IChannelProviderClient<object>>();
			var builder = new CommunicationsClientBuilder();
			var providerBuilder = new ChannelProviderConfigurationBuilder(builder, addConfirm, fail_addAdaptorConfirm);
			providerBuilder.Add(expectedId, client.Object.GetType(), typeof(object), null);
		}
	}
}