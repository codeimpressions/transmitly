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

using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;

namespace Transmitly.Channel.Configuration
{
	///<inheritdoc/>
	internal class ChannelProvider(string providerId, IChannelProviderClient communicationClientType, Func<IAudienceAddress, bool> supportAudienceAddress, params string[]? supportedChannels) : IChannelProvider
	{
		private readonly IChannelProviderClient _client = Guard.AgainstNull(communicationClientType);
		private readonly Func<IAudienceAddress, bool> _supportedAudienceAddress = Guard.AgainstNull(supportAudienceAddress);
		private readonly string[] _supportedChannels = supportedChannels ?? [];

		///<inheritdoc/>
		public ChannelProvider(string providerId, IChannelProviderClient communicationClientType, params string[]? supportedChannels)
			: this(providerId, communicationClientType, (audienceAddress) => true, supportedChannels)
		{

		}

		///<inheritdoc/>
		public string Id { get; } = Guard.AgainstNullOrWhiteSpace(providerId);

		///<inheritdoc/>
		public IChannelProviderClient GetClient()
		{
			return _client;
		}

		///<inheritdoc/>
		public bool SupportAudienceAddress(IAudienceAddress audienceAddress)
		{
			return _supportedAudienceAddress(audienceAddress);
		}

		///<inheritdoc/>
		public bool SupportsChannel(string channel)
		{
			if (_supportedChannels.Length == 0)
				return true;
			return Array.Exists(_supportedChannels, a => string.Equals(a, channel, StringComparison.OrdinalIgnoreCase));
		}
	}
}