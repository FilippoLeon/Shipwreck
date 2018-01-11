public interface IObserver<T> where T: class {
    T Emitter { set; get; }
    void HandleEvent(string signal, object[] args);
    void HandleEvent(string signal);
}