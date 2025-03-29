using System.Threading;
using UnityEngine;
using System.Collections.Generic;
namespace CTSCancelLogic
{
    public static class CancelAndRestartTokens
    {
        public static CancellationTokenSource GetCancellationToken(GameObject target, Dictionary<GameObject, CancellationTokenSource> cancellationTokens)
        {
            if (cancellationTokens.TryGetValue(target, out var cts))
            {
                cts.Cancel();
                cancellationTokens.Remove(target);
            }
            var newCts = new CancellationTokenSource();
            cancellationTokens[target] = newCts;
            return newCts;
        }
        public static void CancelAllAnimations(Dictionary<GameObject, CancellationTokenSource> cancellationTokens)
        {
            foreach (var cts in cancellationTokens.Values)
            {
                cts.Cancel();
            }
            cancellationTokens.Clear();
        }

        public static Dictionary<GameObject, CancellationTokenSource> GiveDictinary(GameObject[]g, CancellationTokenSource[]tokens)
        {
            Dictionary<GameObject, CancellationTokenSource> dictionary = new Dictionary<GameObject, CancellationTokenSource>();
            for (int i = 0; i < g.Length; i++)
            {
                if(tokens.Length<=i){ dictionary.Add(g[i], tokens[i]);}
            }
            return dictionary;
        }
    }
}