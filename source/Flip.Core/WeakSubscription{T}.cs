﻿namespace Flip
{
    using System;

    internal sealed class WeakSubscription<T> : IDisposable
    {
        private readonly WeakReference<IObserver<T>> _reference;
        private readonly IDisposable _subscription;

        public WeakSubscription(
            IObservable<T> observable, IObserver<T> observer)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            _reference = new WeakReference<IObserver<T>>(observer);
            _subscription = observable.Subscribe(OnNext, OnError, OnCompleted);
        }

        public void Dispose() => _subscription.Dispose();

        private void OnNext(T value)
        {
            IObserver<T> observer;
            if (_reference.TryGetTarget(out observer))
            {
                observer.OnNext(value);
            }
            else
            {
                _subscription.Dispose();
            }
        }

        private void OnError(Exception error)
        {
            IObserver<T> observer;
            if (_reference.TryGetTarget(out observer))
            {
                observer.OnError(error);
            }
            else
            {
                _subscription.Dispose();
            }
        }

        private void OnCompleted()
        {
            IObserver<T> observer;
            if (_reference.TryGetTarget(out observer))
            {
                observer.OnCompleted();
            }
            else
            {
                _subscription.Dispose();
            }
        }
    }
}
