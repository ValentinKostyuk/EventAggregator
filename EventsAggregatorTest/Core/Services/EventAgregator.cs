﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EventsAggregator.Core.Services
{
	public class EventAggregator
	{
		private readonly Dictionary<Type, IList> _subscriber;

		public EventAggregator()
		{
			_subscriber = new Dictionary<Type, IList>();
		}

		public void Publish<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
		{
			Type argumentsType = typeof(TEventArgs);
			if (_subscriber.ContainsKey(argumentsType))
			{
				IList subscriptions = new List<ISubscription<TEventArgs>>(_subscriber[argumentsType].Cast<Subscription<TEventArgs>>());

				foreach (ISubscription<TEventArgs> subscription in subscriptions)
				{
					subscription.Handler(sender, eventArgs);
				}
			}
		}

		public Subscription<TEventArgs> Subscribe<TEventArgs>(AggregatorEventHandler<TEventArgs> handler) where TEventArgs : EventArgs
		{
			Type argumentsType = typeof(TEventArgs);
			IList subscriptions;
			var subscrtionDetail = new Subscription<TEventArgs>(handler, this);

			if (!_subscriber.TryGetValue(argumentsType, out subscriptions))
			{
				subscriptions = new List<ISubscription<TEventArgs>> {subscrtionDetail};
				_subscriber.Add(argumentsType, subscriptions);
			}
			else
			{
				subscriptions.Add(subscrtionDetail);
			}

			return subscrtionDetail;
		}

		public void UnSbscribe<TEventArgs>(Subscription<TEventArgs> subscription) where TEventArgs : EventArgs
		{
			Type argumentsType = typeof(TEventArgs);
			if (_subscriber.ContainsKey(argumentsType))
			{
				_subscriber[argumentsType].Remove(subscription);
			}
		}

	}
}
