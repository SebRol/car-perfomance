using System;
using System.Collections.Generic;
using System.Linq;

namespace perf
{
   public interface IEventBus
   {
      // Subscribes to the specified event type with the specified action
      // param "action": The Action to invoke when an event of this type is published
      // Returns A "SubscriptionToken" to be used when calling "Unsubscribe"
      SubscriptionToken Subscribe<TEventBase>(Action<TEventBase> action) where TEventBase : EventBase;

      // Unsubscribe from the Event type related to the specified "SubscriptionToken"
      // param = "token" The "SubscriptionToken" received from calling the Subscribe method
      void Unsubscribe(SubscriptionToken token);

      // Publishes the specified event to any subscribers for the "TEventBase" event type
      // param = "eventItem" Event to publish
      void Publish<TEventBase>(TEventBase eventItem) where TEventBase : EventBase;

      // Publishes the specified event to any subscribers for the "TEventBase" event type asychronously
      // This is a wrapper call around the synchronous  method as this method is naturally synchronous (CPU Bound)
      // param = "eventItem" Event to publish
      void PublishAsync<TEventBase>(TEventBase eventItem) where TEventBase : EventBase;

      // Publishes the specified event to any subscribers for the "TEventBase" event type asychronously
      // and 
      // This is a wrapper call around the synchronous  method as this method is naturally synchronous (CPU Bound)
      // param = "eventItem" Event to publish
      // param = "callback" "AsyncCallback" that is called on completion
      void PublishAsync<TEventBase>(TEventBase eventItem, AsyncCallback callback) where TEventBase : EventBase;
   }

   public interface ISubscription
   {
      // Token returned to the subscriber
      SubscriptionToken SubscriptionToken { get; }

      // Publish to the subscriber
      // param = "eventBase" 
      void Publish(EventBase eventBase);
   }

   public abstract class EventBase
   {
   }

   public class PayloadEvent<TPayload> : EventBase
   {
      public TPayload Payload { get; protected set; }

      public PayloadEvent(TPayload payload)
      {
         Payload = payload;
      }
   }

   public class LocalisationChangeEvent : EventBase
   {
   }
   
   public class HelpAppStoreEvent : EventBase
   {
   }

   public class SubscriptionToken
   {
      internal SubscriptionToken(Type eventItemType)
      {
         _uniqueTokenId = Guid.NewGuid();
         _eventItemType = eventItemType;
      }

      public Guid Token { get { return _uniqueTokenId; } }
      public Type EventItemType { get { return _eventItemType; } }

      private readonly Guid _uniqueTokenId;
      private readonly Type _eventItemType;
   }

   internal class Subscription<TEventBase> : ISubscription where TEventBase : EventBase
   {
      public SubscriptionToken SubscriptionToken { get { return _subscriptionToken; } }

      public Subscription(Action<TEventBase> action, SubscriptionToken token)
      {
         if(action == null)
            throw new ArgumentNullException("action");

         if(token == null)
            throw new ArgumentNullException("token");

         _action = action;
         _subscriptionToken = token;
      }

      public void Publish(EventBase eventItem)
      {
         if (!(eventItem is TEventBase))
            throw new ArgumentException("Event Item is not the correct type.");

         _action.Invoke(eventItem as TEventBase);
      }

      readonly Action<TEventBase> _action;
      readonly SubscriptionToken _subscriptionToken;
   }

   public class EventBus : IEventBus
   {
      static EventBus _instance;

      public EventBus()
      {
         _subscriptions = new Dictionary<Type, List<ISubscription>>();
      }

      public static EventBus GetInstance()
      {
         if (_instance == null)
         {
            _instance = new EventBus();
         }

         return _instance;
      }

      // Subscribes to the specified event type with the specified action
      // param = "action" The Action to invoke when an event of this type is published
      // Returns A "SubscriptionToken" to be used when calling "Unsubscribe" 
      public SubscriptionToken Subscribe<TEventBase>(Action<TEventBase> action) where TEventBase : EventBase
      {
         if (action == null)
            throw new ArgumentNullException("action");

         lock (SubscriptionsLock)
         {
            if (!_subscriptions.ContainsKey(typeof(TEventBase)))
               _subscriptions.Add(typeof(TEventBase), new List<ISubscription>());

            var token = new SubscriptionToken(typeof(TEventBase));
            _subscriptions[typeof(TEventBase)].Add(new Subscription<TEventBase>(action, token));

            return token;
         }
      }

      // Unsubscribe from the Event type related to the specified "SubscriptionToken"
      // param = "token" The "SubscriptionToken" received from calling the Subscribe method
      public void Unsubscribe(SubscriptionToken token)
      {
         if (token == null)
            throw new ArgumentNullException("token");

         lock (SubscriptionsLock)
         {
            if (_subscriptions.ContainsKey(token.EventItemType))
            {
               var allSubscriptions = _subscriptions[token.EventItemType];
               var subscriptionToRemove = allSubscriptions.FirstOrDefault(x => x.SubscriptionToken.Token == token.Token);
               if (subscriptionToRemove != null)
                  _subscriptions[token.EventItemType].Remove(subscriptionToRemove);
            }
         }
      }

      // Publishes the specified event to any subscribers for the "TEventBase" event type
      // param = "eventItem" Event to publish
      public void Publish<TEventBase>(TEventBase eventItem) where TEventBase : EventBase
      {
         if (eventItem == null)
            throw new ArgumentNullException("eventItem");

         List<ISubscription> allSubscriptions = new List<ISubscription>();
         lock (SubscriptionsLock)
         {
            if (_subscriptions.ContainsKey(typeof(TEventBase)))
               allSubscriptions = _subscriptions[typeof(TEventBase)];
         }

         foreach (var subscription in allSubscriptions)
         {
            try
            {
               subscription.Publish(eventItem);
            }
            catch (Exception exception)
            {
               Debug.LogToFileMethod(exception.ToString());
            }
         }
      }

      // Publishes the specified event to any subscribers for the "TEventBase" event type asychronously
      // This is a wrapper call around the synchronous  method as this method is naturally synchronous (CPU Bound)
      // param = "eventItem" Event to publish
      public void PublishAsync<TEventBase>(TEventBase eventItem) where TEventBase : EventBase
      {
         PublishAsyncInternal(eventItem, null);
      }

      // Publishes the specified event to any subscribers for the "TEventBase" event type asychronously
      // This is a wrapper call around the synchronous  method as this method is naturally synchronous (CPU Bound)
      // param = "eventItem" Event to publish
      // param = "callback" "AsyncCallback" that is called on completion
      public void PublishAsync<TEventBase>(TEventBase eventItem, AsyncCallback callback) where TEventBase : EventBase
      {
         PublishAsyncInternal(eventItem, callback);
      }

      void PublishAsyncInternal<TEventBase>(TEventBase eventItem, AsyncCallback callback) where TEventBase : EventBase
      {
         Action publishAction = () => Publish(eventItem);
         publishAction.BeginInvoke(callback, null);
      }

      readonly Dictionary<Type, List<ISubscription>> _subscriptions;
      static readonly object SubscriptionsLock = new object();
   }
}
