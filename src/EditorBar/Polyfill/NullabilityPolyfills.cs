﻿// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

// This was copied from https://github.com/dotnet/coreclr/blob/60f1e6265bd1039f023a82e0643b524d6aaf7845/src/System.Private.CoreLib/shared/System/Diagnostics/CodeAnalysis/NullableAttributes.cs
// and updated to have the scope of the attributes be internal.

// ReSharper disable All

#pragma warning disable CA1019 // Define accessors for attribute arguments
#nullable enable

namespace System.Diagnostics.CodeAnalysis;
#if !NETCOREAPP

/// <summary>Specifies that null is allowed as an input even if the corresponding type disallows it.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
internal sealed class AllowNullAttribute : Attribute
{
}

/// <summary>Specifies that null is disallowed as an input even if the corresponding type allows it.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
internal sealed class DisallowNullAttribute : Attribute
{
}

/// <summary>Specifies that an output may be null even if the corresponding type disallows it.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property |
                AttributeTargets.ReturnValue)]
internal sealed class MaybeNullAttribute : Attribute
{
}

/// <summary>Specifies that an output will not be null even if the corresponding type allows it.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property |
                AttributeTargets.ReturnValue)]
internal sealed class NotNullAttribute : Attribute
{
}

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue" />, the parameter may be null even if the
/// corresponding type disallows it.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class MaybeNullWhenAttribute : Attribute
{
    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }

    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter may be null.
    /// </param>
    public MaybeNullWhenAttribute(bool returnValue)
    {
        this.ReturnValue = returnValue;
    }
}

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue" />, the parameter will not be null even if the
/// corresponding type allows it.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class NotNullWhenAttribute : Attribute
{
    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }

    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    public NotNullWhenAttribute(bool returnValue)
    {
        this.ReturnValue = returnValue;
    }
}

/// <summary>Specifies that the output will be non-null if the named parameter is non-null.</summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue,
    AllowMultiple = true)]
internal sealed class NotNullIfNotNullAttribute : Attribute
{
    /// <summary>Gets the associated parameter name.</summary>
    public string ParameterName { get; }

    /// <summary>Initializes the attribute with the associated parameter name.</summary>
    /// <param name="parameterName">
    /// The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
    /// </param>
    public NotNullIfNotNullAttribute(string parameterName)
    {
        this.ParameterName = parameterName;
    }
}

/// <summary>Applied to a method that will never return under any circumstance.</summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute
{
}

/// <summary>Specifies that the method will not return if the associated Boolean parameter is passed the specified value.</summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class DoesNotReturnIfAttribute : Attribute
{
    /// <summary>Gets the condition parameter value.</summary>
    public bool ParameterValue { get; }

    /// <summary>Initializes the attribute with the specified parameter value.</summary>
    /// <param name="parameterValue">
    /// The condition parameter value. Code after the method will be considered unreachable by diagnostics if the argument
    /// to the associated parameter matches this value.
    /// </param>
    public DoesNotReturnIfAttribute(bool parameterValue)
    {
        this.ParameterValue = parameterValue;
    }
}

#endif

#if !NETCOREAPP || NETCOREAPP3_1

/// <summary>
/// Specifies that the method or property will ensure that the listed field and property members have not-null
/// values.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullAttribute : Attribute
{
    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }

    /// <summary>Initializes the attribute with a field or property member.</summary>
    /// <param name="member">
    /// The field or property member that is promised to be not-null.
    /// </param>
    public MemberNotNullAttribute(string member)
    {
        this.Members = new[] { member };
    }

    /// <summary>Initializes the attribute with the list of field and property members.</summary>
    /// <param name="members">
    /// The list of field and property members that are promised to be not-null.
    /// </param>
    public MemberNotNullAttribute(params string[] members)
    {
        this.Members = members;
    }
}

/// <summary>
/// Specifies that the method or property will ensure that the listed field and property members have not-null
/// values when returning with the specified return value condition.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullWhenAttribute : Attribute
{
    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }

    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }

    /// <summary>Initializes the attribute with the specified return value condition and a field or property member.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    /// <param name="member">
    /// The field or property member that is promised to be not-null.
    /// </param>
    public MemberNotNullWhenAttribute(bool returnValue, string member)
    {
        this.ReturnValue = returnValue;
        this.Members = new[] { member };
    }

    /// <summary>Initializes the attribute with the specified return value condition and list of field and property members.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    /// <param name="members">
    /// The list of field and property members that are promised to be not-null.
    /// </param>
    public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
    {
        this.ReturnValue = returnValue;
        this.Members = members;
    }
}

#endif