using ReactiveUI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Fly.ViewModels;

public class ViewModelBase : ReactiveObject
{
    /// <summary>
    /// Utility method to mimic MAUI MVVM's SetProperty.
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="PropertyChanging"/> event, updates the property with the new
    /// value, then raises the <see cref="PropertyChanged"/> event.
    /// This overload is much less efficient than <see cref="SetProperty{T}(ref T,T,string)"/> and it
    /// should only be used when the former is not viable (eg. when the target property being
    /// updated does not directly expose a backing field that can be passed by reference).
    /// For performance reasons, it is recommended to use a stateful callback if possible through
    /// the <see cref="SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string?)"/> whenever possible
    /// instead of this overload, as that will allow the C# compiler to cache the input callback and
    /// reduce the memory allocations. More info on that overload are available in the related XML
    /// docs. This overload is here for completeness and in cases where that is not applicable.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised
    /// if the current and new value for the target property are the same.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is <see langword="null"/>.</exception>
    protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return false;
        }

        this.RaisePropertyChanging(propertyName);

        callback(newValue);

        this.RaisePropertyChanged(propertyName);

        return true;
    }


    /// <summary>
    /// Utility method to mimic MAUI MVVM's SetProperty.
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="PropertyChanging"/> event, updates the property with the new
    /// value, then raises the <see cref="PropertyChanged"/> event.
    /// See additional notes about this overload in <see cref="SetProperty{T}(T,T,Action{T},string)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> or <paramref name="callback"/> are <see langword="null"/>.</exception>
    protected bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(callback);

        if (comparer.Equals(oldValue, newValue))
        {
            return false;
        }

        this.RaisePropertyChanging(propertyName);

        callback(newValue);

        this.RaisePropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Utility method to mimic MAUI MVVM's SetProperty.
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="PropertyChanging"/> event, updates the property with the new
    /// value, then raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised
    /// if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        // We duplicate the code here instead of calling the overload because we can't
        // guarantee that the invoked SetProperty<T> will be inlined, and we need the JIT
        // to be able to see the full EqualityComparer<T>.Default.Equals call, so that
        // it'll use the intrinsics version of it and just replace the whole invocation
        // with a direct comparison when possible (eg. for primitive numeric types).
        // This is the fastest SetProperty<T> overload so we particularly care about
        // the codegen quality here, and the code is small and simple enough so that
        // duplicating it still doesn't make the whole class harder to maintain.
        if (EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }

        this.RaisePropertyChanging(propertyName);

        field = newValue;

        this.RaisePropertyChanged(propertyName);

        return true;
    }


}
