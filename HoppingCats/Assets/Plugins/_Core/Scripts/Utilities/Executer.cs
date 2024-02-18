using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace LateExe
{
    public class Executer
    {
        private readonly object @object;
        private readonly MonoBehaviour mb;
        public Executer(object obj)
        {
            @object = obj;
            mb = @object as MonoBehaviour;
        }

        public InvokeId ExecuteSequences(params Executable[] executables)
        {
            return new InvokeId(mb.StartCoroutine(CallExecutable(executables)));
        }

        private IEnumerator CallExecutable(Executable[] executables)
        {
            for (int i = 0; i < executables.Length; i++)
            {
                yield return new WaitForSeconds(executables[i].delay);
                executables[i].action.Invoke();
            }
        }

        public InvokeId DelayExecuteByFrame(int frame, Action lambda)
        {
            return new InvokeId(mb.StartCoroutine(DelayedByFrame(frame, lambda)));
        }

        public InvokeId DelayExecute(float seconds, params Action[] lambdas)
        {
            return new InvokeId(mb.StartCoroutine(Delayed(seconds, lambdas)));
        }

        public InvokeId DelayExecute(float seconds, Action<object[]> lambda, params object[] parameters)
        {
            return new InvokeId(mb.StartCoroutine(Delayed(seconds, lambda, parameters)));
        }

        public InvokeId DelayExecute(float seconds, string methodName)
        {
            foreach (MethodInfo method in @object.GetType().GetMethods())
            {
                if (method.Name == methodName)
                    return new InvokeId(mb.StartCoroutine(Delayed(seconds, method)));
            }
            return null;
        }

        public InvokeId DelayExecute(float seconds, string methodName, params object[] parameters)
        {
            foreach (MethodInfo method in @object.GetType().GetMethods())
            {
                if (method.Name == methodName)
                    return new InvokeId(mb.StartCoroutine(Delayed(seconds, method, parameters)));
            }
            return null;
        }

        public InvokeId DelayExecuteUnscale(float seconds, params Action[] lambdas)
        {
            return new InvokeId(mb.StartCoroutine(DelayedUnscale(seconds, lambdas)));
        }

        public InvokeId ConditionExecute(Func<bool> condition, string methodName, params object[] parameters)
        {
            foreach (MethodInfo method in @object.GetType().GetMethods())
            {
                if (method.Name == methodName)
                    return new InvokeId(mb.StartCoroutine(Delayed(condition, method, parameters)));
            }
            return null;
        }

        public InvokeId ConditionExecute(Func<bool> condition, Action<object[]> lambda, params object[] parameters)
        {
            return new InvokeId(mb.StartCoroutine(Delayed(condition, lambda, parameters)));
        }

        public void StopExecute(InvokeId id)
        {
            if (id != null) mb.StopCoroutine(id.coroutine);
        }

        public void StopAllExecute() => mb.StopAllCoroutines();

        private IEnumerator Delayed(float DelayInSeconds, Action<object[]> lambda, params object[] parameters)
        {
            yield return new WaitForSeconds(DelayInSeconds);
            lambda.Invoke(parameters);
            yield return null;
        }

        private IEnumerator Delayed(float seconds, MethodInfo method)
        {
            yield return new WaitForSeconds(seconds);
            method.Invoke(@object, new object[0]);
            yield return null;
        }

        private IEnumerator Delayed(float seconds, params Action[] lambdas)
        {
            yield return new WaitForSeconds(seconds);
            for (int i = 0; i < lambdas.Length; i++)
            {
                lambdas[i].Invoke();
            }
            yield return null;
        }

        private IEnumerator DelayedUnscale(float seconds, params Action[] lambdas)
        {
            yield return new WaitForSecondsRealtime(seconds);
            for (int i = 0; i < lambdas.Length; i++)
            {
                lambdas[i].Invoke();
            }
            yield return null;
        }

        private IEnumerator DelayedByFrame(int frame, params Action[] lambdas)
        {
            do { yield return null; } while (--frame > 0);

            for (int i = 0; i < lambdas.Length; i++)
            {
                lambdas[i].Invoke();
            }

            yield return null;
        }

        private IEnumerator Delayed(float DelayInSeconds, MethodInfo method, params object[] parameters)
        {
            yield return new WaitForSeconds(DelayInSeconds);
            method.Invoke(@object, parameters);
            yield return null;
        }

        private IEnumerator Delayed(Func<bool> condition, Action<object[]> lambda, params object[] parameters)
        {
            yield return new WaitUntil(condition);
            lambda.Invoke(parameters);
            yield return null;
        }

        private IEnumerator Delayed(Func<bool> condition, MethodInfo method, params object[] parameters)
        {
            yield return new WaitUntil(condition);
            method.Invoke(@object, parameters);
            yield return null;
        }

    }

    public class InvokeId
    {
        public readonly Coroutine coroutine;
        public InvokeId(Coroutine coroutine)
        {
            this.coroutine = coroutine;
        }
    }

    public class Executable
    {
        public float delay;
        public Action action;

        public Executable(float delay, Action action)
        {
            this.delay = delay;
            this.action = action;
        }
    }
}
