using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReductionTypeAdd<T> {
    public T Op(T left, T right) {
       return left += right;
    }
}

public interface IParameter { }

public interface IModifier<T> {
    void Modify(T val);
}

public class AdditiveModifier<T> : IModifier<T> { }
public class MultiplicativeModifier : IModifier<T> { }

public class Parameter<T, ReductionType> : IParameter where ReductionType : IReductionType {
    public T Value { get {
            T ret = baseValue;
            foreach(IEmitter e in emitters) {
                ReductionType.Op(ret, (T) e.GetParameter(name));
            }
            foreach(SortedList<int, IModifier<T>> mod in modifiers.Values) {
                foreach(IModifier<T> m in mod.Values) {
                    m.Modify(ret);
                }
            }

            return ret;
        }
    }

    enum ModifierType {  Additive, Multiplicative };

    T baseValue;
    string name;
    string reductionType;
    SortedDictionary<ModifierType, SortedList<int, IModifier<T>>> modifiers = 
        new SortedDictionary<ModifierType, SortedList<int, IModifier<T>>>();

    List<IEmitter> emitters = null;
}

internal interface IReductionType {
}