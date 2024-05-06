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

using System.Collections.Generic;
using Transmitly.Channel.Configuration;
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.Configuration
{
	/// <summary>
	/// Creates a new instance of <see cref="BaseChannelProviderFactory"/>
	/// </summary>
	/// <param name="registrations">Enumerable of registered channel providers.</param>
	/// <exception cref="ArgumentNullException">When the registrations is null</exception>
	/// 
	public abstract class BaseChannelProviderFactory(IEnumerable<IChannelProviderRegistration> registrations) : IChannelProviderFactory
	{
		private readonly List<IChannelProviderRegistration> _registrations = Guard.AgainstNull(registrations).ToList();
		protected IReadOnlyCollection<IChannelProviderRegistration> Registrations => _registrations.AsReadOnly();

		///<inheritdoc/>
		public virtual Task<IReadOnlyCollection<IChannelProviderRegistration>> GetAllAsync()
		{
			return Task.FromResult(Registrations);
		}

		public abstract Task<IChannelProviderClient?> ResolveClientAsync(IChannelProviderRegistration channelProvider, IChannelProviderClientRegistration channelProviderClientRegistration);

		///<inheritdoc/>
		public abstract Task<IChannelProviderDeliveryReportRequestAdaptor> ResolveDeliveryReportRequestAdaptorAsync(IDeliveryReportRequestAdaptorRegistration channelProviderDeliveryReportRequestAdaptor);

		///<inheritdoc/>
		public virtual Task<IReadOnlyCollection<IDeliveryReportRequestAdaptorRegistration>> GetAllDeliveryReportRequestAdaptorsAsync()
		{
			var adaptors = _registrations.SelectMany(m => m.DeliveryReportRequestAdaptorRegistrations).ToList().AsReadOnly();
			return Task.FromResult((IReadOnlyCollection<IDeliveryReportRequestAdaptorRegistration>)adaptors);
		}

		
	}
}