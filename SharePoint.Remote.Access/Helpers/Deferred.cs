using System;
using System.Collections.Generic;

namespace SharePoint.Remote.Access.Helpers
{
    public interface IPromise
    {
        IPromise Done(Action callback);

        IPromise Fail(Action callback);

        IPromise Always(Action callback);

        bool IsRejected { get; }

        bool IsResolved { get; }

        bool IsFulfilled { get; }
    }

    public interface IPromise<out TResolve, out TReject> : IPromise
    {
        IPromise<TResolve, TReject> Done(Action<TResolve> callback);

        IPromise<TResolve, TReject> Done(IEnumerable<Action<TResolve>> callbacks);

        IPromise<TResolve, TReject> Fail(Action<TReject> callback);

        IPromise<TResolve, TReject> Fail(IEnumerable<Action<TReject>> callbacks);

        IPromise<TResolve, TReject> Always(Action<TResolve, TReject> callback);

        IPromise<TResolve, TReject> Always(IEnumerable<Action<TResolve, TReject>> callbacks);
    }

    public interface IPromise<out T> : IPromise<T, T>
    {
    }

    public class Deferred : Deferred<object, Exception>
    {
        // generic object
    }

    public class Deferred<T> : Deferred<T, T>
    {
        // generic object
    }

    public class Deferred<TResolve, TReject> : IPromise<TResolve, TReject>
    {
        private readonly List<Callback> _callbacks = new List<Callback>();

        private TResolve _argResolve;
        private TReject _argReject;

        public static IPromise When(IEnumerable<IPromise> promises)
        {
            int count = 0;
            var masterPromise = new Deferred();

            foreach (IPromise promise in promises)
            {
                count++;
                promise.Fail(() => masterPromise.Reject());
                promise.Done(() =>
                {
                    count--;
                    if (0 == count)
                    {
                        masterPromise.Resolve();
                    }
                });
            }

            return masterPromise;
        }

        public static IPromise When(object d)
        {
            var masterPromise = new Deferred();
            masterPromise.Resolve();
            return masterPromise;
        }

        public static IPromise When(Deferred d)
        {
            return d.Promise();
        }

        public static IPromise<TResolve, TReject> When(Deferred<TResolve, TReject> d)
        {
            return d.Promise();
        }

        public IPromise<TResolve, TReject> Promise()
        {
            return this;
        }

        public IPromise Always(Action callback)
        {
            if (IsResolved || IsRejected)
                callback();
            else
                _callbacks.Add(new Callback(callback, Callback.Condition.Always, false));
            return this;
        }

        public IPromise<TResolve, TReject> Always(Action<TResolve, TReject> callback)
        {
            if (IsResolved || IsRejected)
                callback(_argResolve, _argReject);
            else
                _callbacks.Add(new Callback(callback, Callback.Condition.Always, true));
            return this;
        }

        public IPromise<TResolve, TReject> Always(IEnumerable<Action<TResolve, TReject>> callbacks)
        {
            foreach (Action<TResolve, TReject> callback in callbacks)
                this.Always(callback);
            return this;
        }

        public IPromise Done(Action callback)
        {
            if (IsResolved)
                callback();
            else
                _callbacks.Add(new Callback(callback, Callback.Condition.Success, false));
            return this;
        }

        public IPromise<TResolve, TReject> Done(Action<TResolve> callback)
        {
            if (IsResolved)
                callback(_argResolve);
            else
                _callbacks.Add(new Callback(callback, Callback.Condition.Success, true));
            return this;
        }

        public IPromise<TResolve, TReject> Done(IEnumerable<Action<TResolve>> callbacks)
        {
            foreach (Action<TResolve> callback in callbacks)
                this.Done(callback);
            return this;
        }

        public IPromise Fail(Action callback)
        {
            if (IsRejected)
                callback();
            else
                _callbacks.Add(new Callback(callback, Callback.Condition.Fail, false));
            return this;
        }

        public IPromise<TResolve, TReject> Fail(Action<TReject> callback)
        {
            if (IsRejected)
                callback(_argReject);
            else
                _callbacks.Add(new Callback(callback, Callback.Condition.Fail, true));
            return this;
        }

        public IPromise<TResolve, TReject> Fail(IEnumerable<Action<TReject>> callbacks)
        {
            foreach (Action<TReject> callback in callbacks)
                this.Fail(callback);
            return this;
        }

        public bool IsRejected { get; protected set; }

        public bool IsResolved { get; protected set; }

        public bool IsFulfilled
        {
            get { return IsRejected || IsResolved; }
        }

        public IPromise Reject()
        {
            if (IsRejected || IsResolved) // ignore if already rejected or resolved
                return this;
            IsRejected = true;
            DequeueCallbacks(Callback.Condition.Fail);
            return this;
        }

        public Deferred<TResolve, TReject> Reject(TReject argReject)
        {
            if (IsRejected || IsResolved) // ignore if already rejected or resolved
                return this;
            IsRejected = true;
            this._argReject = argReject;
            DequeueCallbacks(Callback.Condition.Fail);
            return this;
        }

        public IPromise Resolve()
        {
            if (IsRejected || IsResolved) // ignore if already rejected or resolved
                return this;
            this.IsResolved = true;
            DequeueCallbacks(Callback.Condition.Success);
            return this;
        }

        public Deferred<TResolve, TReject> Resolve(TResolve arg)
        {
            if (IsRejected || IsResolved) // ignore if already rejected or resolved
                return this;
            this.IsResolved = true;
            this._argResolve = arg;
            DequeueCallbacks(Callback.Condition.Success);
            return this;
        }

        private void DequeueCallbacks(Callback.Condition cond)
        {
            foreach (Callback callback in _callbacks)
            {
                if (callback.Cond == cond || callback.Cond == Callback.Condition.Always)
                {
                    if (callback.IsReturnValue)
                    {
                        if (callback.Cond == Callback.Condition.Success)
                        {
                            callback.Del.DynamicInvoke(_argResolve);
                        }
                        else if (callback.Cond == Callback.Condition.Fail)
                        {
                            callback.Del.DynamicInvoke(_argReject);
                        }
                        else
                        {
                            callback.Del.DynamicInvoke(_argResolve, _argReject);
                        }
                    }
                    else
                    {
                        callback.Del.DynamicInvoke();
                    }
                }
            }
            _callbacks.Clear();
        }
    }

    internal class Callback
    {
        public enum Condition { Always, Success, Fail };

        public Callback(Delegate del, Condition cond, bool returnValue)
        {
            Del = del;
            Cond = cond;
            IsReturnValue = returnValue;
        }

        public bool IsReturnValue { get; private set; }

        public Delegate Del { get; private set; }

        public Condition Cond { get; private set; }
    }
}