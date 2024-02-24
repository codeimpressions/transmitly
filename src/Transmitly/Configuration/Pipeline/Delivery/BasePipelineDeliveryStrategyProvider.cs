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

using Transmitly.Channel.Configuration;
using Transmitly.ChannelProvider;

namespace Transmitly.Delivery
{
	public abstract class BasePipelineDeliveryStrategyProvider
	{
		public abstract Task<IReadOnlyCollection<IDispatchResult?>> DispatchAsync(IReadOnlyCollection<ChannelChannelProviderGroup> sendingGroups, IDispatchCommunicationContext context, CancellationToken cancellationToken);

		protected async Task<IReadOnlyCollection<IDispatchResult?>> DispatchCommunicationAsync(IChannel channel, IChannelProvider provider, IDispatchCommunicationContext context, CancellationToken cancellationToken)
		{

			var internalContext = new DispatchCommunicationContext(context, channel, provider)
			{
				RecipientAudiences = FilterRecipientAddresses(channel, provider, context.RecipientAudiences)
			};

			var communication = await GetChannelCommunicationAsync(channel, internalContext);
			IReadOnlyCollection<IDispatchResult?> results;
			if (context.Settings.IsDeliveryEnabled)
			{
				try
				{
					if (!provider.CommunicationType.IsInstanceOfType(communication))
						return [];

					var client = Guard.AgainstNull(await provider.ClientInstance());
					var method = typeof(IChannelProviderClient<>).MakeGenericType(provider.CommunicationType).GetMethod(nameof(IChannelProviderClient.DispatchAsync));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
					var comm = method.Invoke(client, [communication, internalContext, cancellationToken]);
					Guard.AgainstNull(comm);

#pragma warning restore CS8602 // Dereference of a possibly null reference.
					results = await ((Task<IReadOnlyCollection<IDispatchResult?>>)comm).ConfigureAwait(false);//await client.DispatchAsync(, internalContext, cancellationToken);

					if (results == null || results.Count == 0)
						return [];

					return results.Where(r => r != null).Select(r => new DispatchResult(r!, provider.Id, channel.Id)).ToList();
				}
				catch (Exception ex)
				{
					//TODO: Fire dispatch error event, log
					return [new DispatchResult(DispatchStatus.Error, provider.Id, channel.Id) { Exception = ex }];
				}
			}
			else
			{
				results = [new DispatchNotEnabledResult()];
				//context.DispatchResults.Add(result);
			}
			return results;
		}
		public T CastInto<T>(object obj)
		{
			return (T)obj;
		}

		private static IReadOnlyCollection<IAudience> FilterRecipientAddresses(IChannel channel, IChannelProvider provider, IReadOnlyCollection<IAudience> audiences)
		{
			return audiences.Select(x =>
			   new AudienceRecord(
				   x.Addresses.Where(a =>
						   channel.SupportsAudienceAddress(a) && provider.SupportAudienceAddress(a)
					   )
				   )
			   ).ToList();
		}

		protected virtual async Task<object> GetChannelCommunicationAsync(IChannel channel, IDispatchCommunicationContext context)
		{
			return await channel.GenerateCommunicationAsync(context);
		}
	}
}