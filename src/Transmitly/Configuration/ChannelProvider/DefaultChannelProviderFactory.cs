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

using System.Runtime.InteropServices;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Exceptions;

namespace Transmitly.Channel.Configuration
{
	/// <summary>
	/// Default channel provider factory
	/// </summary>
	sealed class DefaultChannelProviderFactory(IEnumerable<IChannelProviderRegistration> registrations) : BaseChannelProviderFactory(registrations)
	{
		public override Task<IChannelProviderClient> ResolveClientAsync(IChannelProviderRegistration channelProvider)
		{
			IChannelProviderClient? resolvedClient;
			if (channelProvider.ClientType.GetConstructors().Length == 0)
				throw new CommunicationsException($"Cannot create an instance of {channelProvider.ClientType}. No public constructors");

			if (channelProvider.Configuration == null)
			{
				
				resolvedClient = Activator.CreateInstance(channelProvider.ClientType, channelProvider.ClientType.GetConstructors()[0].GetParameters().Select(x=>Activator.CreateInstance(x.ParameterType)).ToArray()) as IChannelProviderClient;
			}
			else
				resolvedClient = Activator.CreateInstance(channelProvider.ClientType, channelProvider.Configuration) as IChannelProviderClient;
			
			return Task.FromResult(Guard.AgainstNull(resolvedClient));
		}
	}
}