namespace CustomDelegats
{

    public static class V
    {
        public delegate void m();
        public delegate void m<in T1>(T1 arg1);
        public delegate void m<in T1, in T2>(T1 arg1, T2 arg2);
        public delegate void m<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
        public delegate void m<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        public delegate void m<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate void m<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public delegate void m<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        public delegate void m<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }
    public static class M
    {
        public delegate T1 m<out T1>();
        public delegate T1 m<out T1, in T2>(T2 arg2);
        public delegate T1 m<out T1, in T2, in T3>(T2 arg2, T3 arg3);
        public delegate T1 m<out T1, in T2, in T3, in T4>(T2 arg2, T3 arg3, T4 arg4);
        public delegate T1 m<out T1, in T2, in T3, in T4, in T5>(T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public delegate T1 m<out T1, in T2, in T3, in T4, in T5, in T6>(T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
        public delegate T1 m<out T1, in T2, in T3, in T4, in T5, in T6, in T7>(T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
        public delegate T1 m<out T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
        public delegate T1 m<out T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>(T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    }
}