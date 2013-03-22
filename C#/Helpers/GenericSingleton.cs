///From : http://sanity-free.com/132/generic_singleton_pattern_in_csharp.html
public class Singleton<T> where T : class {
    static object SyncRoot = new object( );
    static T instance;
    public static T Instance {
        get {
            if ( instance == null ) {
                lock ( SyncRoot ) {
                    if ( instance == null ) {
                        ConstructorInfo ci = typeof( T ).GetConstructor( BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null );
                        if ( ci == null ) { throw new InvalidOperationException( "class must contain a private constructor" ); }
                        instance = (T)ci.Invoke( null );
                    }
                }
            }
            return instance;
        }
    }
}