using CustomDelegats;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace CustomInvoke
{
    public static class CastomInvoke
    {
        public static async Task Invoke(V.m M, int duration) { await Task.Delay(duration); M.Invoke(); }
        public static async Task Invoke<T>(V.m<T> M, T param, int duration) { await Task.Delay(duration); M.Invoke(param); }
        public static async Task Invoke<T, T1>(V.m<T, T1> M, T param, T1 param2, int duration) { await Task.Delay(duration); M.Invoke(param, param2); }   
        public static async Task Invoke<T, T1, T2>(V.m<T, T1, T2> M, T param, T1 param2,T2 param3, int duration) { await Task.Delay(duration); M.Invoke(param, param2, param3); }  
        public static async Task Invoke<T, T1, T2, T3>(V.m<T, T1, T2, T3> M, T param, T1 param2,T2 param3,T3 param4, int duration) { await Task.Delay(duration); M.Invoke(param, param2, param3, param4); }  
        public static async Task Invoke<T, T1, T2, T3, T4>(V.m<T, T1, T2, T3, T4> M, T param, T1 param2,T2 param3,T3 param4, T4 param5, int duration) { await Task.Delay(duration); M.Invoke(param, param2, param3, param4, param5); }  
        public static async Task Invoke<T, T1, T2, T3, T4, T5>(V.m<T, T1, T2, T3, T4, T5> M, T param, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, int duration) { await Task.Delay(duration); M.Invoke(param, param2, param3, param4, param5, param6); }  
        public static async Task Invoke<T, T1, T2, T3, T4, T5, T6>(V.m<T, T1, T2, T3, T4, T5,T6> M, T param, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, int duration) { await Task.Delay(duration); M.Invoke(param, param2, param3, param4, param5, param6, param7); }  
        public static async Task Invoke<T, T1, T2, T3, T4, T5, T6, T7>(V.m<T, T1, T2, T3, T4, T5,T6,T7> M, T param, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, T7 param8, int duration) { await Task.Delay(duration); M.Invoke(param, param2, param3, param4, param5, param6, param7, param8); }  
        
        public static async Task<T> Invoke<T>(M.m<T> M, int duration) { await Task.Delay(duration);return M.Invoke(); }
        public static async Task<T> Invoke<T, T1>(M.m<T, T1> M, T1 param2, int duration) { await Task.Delay(duration);return M.Invoke(param2); }   
        public static async Task<T> Invoke<T, T1, T2>(M.m<T, T1, T2> M, T1 param2,T2 param3, int duration) { await Task.Delay(duration);return M.Invoke(param2, param3); }  
        public static async Task<T> Invoke<T, T1, T2, T3>(M.m<T, T1, T2, T3> M, T1 param2,T2 param3,T3 param4, int duration) { await Task.Delay(duration);return M.Invoke(param2, param3, param4); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4>(M.m<T, T1, T2, T3, T4> M, T1 param2,T2 param3,T3 param4, T4 param5, int duration) { await Task.Delay(duration);return M.Invoke(param2, param3, param4, param5); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4, T5>(M.m<T, T1, T2, T3, T4, T5> M, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, int duration) { await Task.Delay(duration);return M.Invoke(param2, param3, param4, param5, param6); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4, T5, T6>(M.m<T, T1, T2, T3, T4, T5,T6> M, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, int duration) { await Task.Delay(duration);return M.Invoke(param2, param3, param4, param5, param6, param7); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4, T5, T6, T7>(M.m<T, T1, T2, T3, T4, T5,T6,T7> M, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, T7 param8, int duration) { await Task.Delay(duration);return M.Invoke(param2, param3, param4, param5, param6, param7, param8); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4, T5, T6, T7, T8>(M.m<T, T1, T2, T3, T4, T5,T6,T7 , T8> M, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, T7 param8, T8 param9 , int duration) { await Task.Delay(duration);return M.Invoke(param2, param3, param4, param5, param6, param7, param8, param9); }  
        
        public static async Task Invoke(V.m M, int duration,CancellationTokenSource ct) { await Task.Delay(duration); if(!ct.IsCancellationRequested){M.Invoke();} }
        public static async Task Invoke<T>(V.m<T> M, T param, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){M.Invoke(param);} }
        public static async Task Invoke<T, T1>(V.m<T, T1> M, T param, T1 param2, int duration,CancellationTokenSource ct) { await Task.Delay(duration); if(!ct.IsCancellationRequested){M.Invoke(param, param2);} }   
        public static async Task Invoke<T, T1, T2>(V.m<T, T1, T2> M, T param, T1 param2,T2 param3, int duration,CancellationTokenSource ct) { await Task.Delay(duration); if(!ct.IsCancellationRequested){M.Invoke(param, param2, param3);} }  
        public static async Task Invoke<T, T1, T2, T3>(V.m<T, T1, T2, T3> M, T param, T1 param2,T2 param3,T3 param4, int duration,CancellationTokenSource ct) { await Task.Delay(duration); if(!ct.IsCancellationRequested){M.Invoke(param, param2, param3, param4);} }  
        public static async Task Invoke<T, T1, T2, T3, T4>(V.m<T, T1, T2, T3, T4> M, T param, T1 param2,T2 param3,T3 param4, T4 param5, int duration,CancellationTokenSource ct) { await Task.Delay(duration); if(!ct.IsCancellationRequested){M.Invoke(param, param2, param3, param4, param5);} }  
        public static async Task Invoke<T, T1, T2, T3, T4, T5>(V.m<T, T1, T2, T3, T4, T5> M, T param, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, int duration,CancellationTokenSource ct) { await Task.Delay(duration); if(!ct.IsCancellationRequested){M.Invoke(param, param2, param3, param4, param5, param6);} }  
        public static async Task Invoke<T, T1, T2, T3, T4, T5, T6>(V.m<T, T1, T2, T3, T4, T5,T6> M, T param, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){M.Invoke(param, param2, param3, param4, param5, param6, param7);} }  
        public static async Task Invoke<T, T1, T2, T3, T4, T5, T6, T7>(V.m<T, T1, T2, T3, T4, T5,T6,T7> M, T param, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, T7 param8, int duration,CancellationTokenSource ct) { await Task.Delay(duration); if(!ct.IsCancellationRequested){M.Invoke(param, param2, param3, param4, param5, param6, param7, param8);} }  
        
        public static async Task<T> Invoke<T>(M.m<T> M, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke();} return Activator.CreateInstance<T>(); }
        public static async Task<T> Invoke<T, T1>(M.m<T, T1> M, T1 param2, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke(param2);} return Activator.CreateInstance<T>(); }   
        public static async Task<T> Invoke<T, T1, T2>(M.m<T, T1, T2> M, T1 param2,T2 param3, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke(param2, param3);} return Activator.CreateInstance<T>(); }  
        public static async Task<T> Invoke<T, T1, T2, T3>(M.m<T, T1, T2, T3> M, T1 param2,T2 param3,T3 param4, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke(param2, param3, param4);} return Activator.CreateInstance<T>(); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4>(M.m<T, T1, T2, T3, T4> M, T1 param2,T2 param3,T3 param4, T4 param5, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke(param2, param3, param4, param5);} return Activator.CreateInstance<T>();; }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4, T5>(M.m<T, T1, T2, T3, T4, T5> M, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke(param2, param3, param4, param5, param6);} return Activator.CreateInstance<T>(); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4, T5, T6>(M.m<T, T1, T2, T3, T4, T5,T6> M, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke(param2, param3, param4, param5, param6, param7);} return Activator.CreateInstance<T>(); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4, T5, T6, T7>(M.m<T, T1, T2, T3, T4, T5,T6,T7> M, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, T7 param8, int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke(param2, param3, param4, param5, param6, param7, param8);} return Activator.CreateInstance<T>(); }  
        public static async Task<T> Invoke<T, T1, T2, T3, T4, T5, T6, T7, T8>(M.m<T, T1, T2, T3, T4, T5,T6,T7 , T8> M, T1 param2,T2 param3,T3 param4, T4 param5, T5 param6, T6 param7, T7 param8, T8 param9 , int duration,CancellationTokenSource ct) { await Task.Delay(duration);if(!ct.IsCancellationRequested){ return M.Invoke(param2, param3, param4, param5, param6, param7, param8, param9);} return Activator.CreateInstance<T>(); }  
    }
}