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

namespace Transmitly.Tests
{
	[TestClass()]
	public class CommunicationsClientBuilderTests
	{
		[TestMethod]
		public void NewConfigurationBuilderShouldInitializeRegistrationObjects()
		{
			var builder = new CommunicationsClientBuilder();
			Assert.IsNotNull(builder.Pipeline);
			//Assert.IsNotNull(builder.AudienceResolver);
			Assert.IsNotNull(builder.TemplateEngine);
			Assert.IsNotNull(builder.ChannelProvider);
		}

		[TestMethod]
		public void CreateClientThrowsIfCalledMoreThanOnce()
		{
			var configuration = new CommunicationsClientBuilder();
			configuration.BuildClient();
			Assert.ThrowsException<InvalidOperationException>(() => configuration.BuildClient());
		}
		class Test1 : IChannelProviderClient<object>
		{
			public IReadOnlyCollection<string>? RegisteredEvents => throw new NotImplementedException();

			public Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(object communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
			{
				return Task.FromResult<IReadOnlyCollection<IDispatchResult?>>([]);
			}
		}
		class Test2 : IChannelProviderClient<UnitTestCommunication>
		{

			public IReadOnlyCollection<string>? RegisteredEvents => throw new NotImplementedException();

			public Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(UnitTestCommunication communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
			{
				return Task.FromResult<IReadOnlyCollection<IDispatchResult?>>([new DispatchResult(true, nameof(Test2))]);
			}

			public Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(object communication, IDispatchCommunicationContext communicationContext, CancellationToken cancellationToken)
			{
				return DispatchAsync((UnitTestCommunication)communication, communicationContext, cancellationToken);
			}
		}
		[TestMethod]
		public async Task ChannelProviderShouldBeAbleToRegisterMultipleChannelProviderClientsWithSameProviderId()
		{

			string ExpectedSendResultMessage = nameof(Test2);
			const string expectedId = "test";
			var providerObject = new Mock<IChannelProviderClient<object>>();
			var providerUnitTest = new Mock<IChannelProviderClient<UnitTestCommunication>>();


			var client = new CommunicationsClientBuilder()
				.ChannelProvider.Add<Test1, object>(expectedId)
				.ChannelProvider.Add<Test2, UnitTestCommunication>(expectedId)
				.AddPipeline("test", options =>
				{
					options.AddChannel(new UnitTestChannel("unit-test-address").HandlesAddressStartsWith("object"));
					options.AddChannel(new UnitTestChannel("unit-test-address"));
				})
				.BuildClient();

			var result = await client.DispatchAsync(expectedId, "unit-test-address-to", new { });
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Results.Count);
			Assert.AreEqual(ExpectedSendResultMessage, result.Results?.Single()?.ResourceId);
		}
	}
}