// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using JPSoftworks.EditorBar.Helpers;
using Microsoft;

namespace JPSoftworks.EditorBar.Services.StructureProviders;

/// <summary>
/// Represents a type in the text document.
/// </summary>
public class TypeModel : BaseStructureModel, IEquatable<TypeModel>
{
    /// <summary>Gets the fully qualified name of the type, including its namespace but not its assembly.</summary>
    /// <returns>
    /// The fully qualified name of the type, including its namespace but not its assembly.
    /// </returns>
    public string FullName { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="TypeModel" /> class.
    /// </summary>
    /// <param name="displayName">The display name of the type.</param>
    /// <param name="imageMoniker">The image moniker associated with the type.</param>
    /// <param name="anchorPoint">The anchor point indicating the location of the type.</param>
    /// <param name="fullName">The fully qualified name of the type.</param>
    public TypeModel(string displayName, StackedImageMoniker imageMoniker, AnchorPoint anchorPoint, string fullName)
        : base(displayName, imageMoniker, anchorPoint)
    {
        Requires.NotNullOrWhiteSpace(fullName, nameof(fullName));

        this.FullName = fullName;
    }

    /// <inheritdoc />
    public bool Equals(TypeModel? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && this.FullName == other.FullName;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((TypeModel)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), this.FullName);
    }

    /// <summary>
    /// Compares two TypeModel instances for equality.
    /// </summary>
    /// <param name="left">The first instance to compare for equality.</param>
    /// <param name="right">The second instance to compare for equality.</param>
    /// <returns>True if both instances are equal; otherwise, false.</returns>
    public static bool operator ==(TypeModel? left, TypeModel? right)
    {
        return Equals(left!, right!);
    }

    /// <summary>
    /// Compares two TypeModel instances for inequality. Returns true if they are not equal, false otherwise.
    /// </summary>
    /// <param name="left">The first instance to compare for inequality.</param>
    /// <param name="right">The second instance to compare for inequality.</param>
    /// <returns>A boolean indicating whether the two instances are not equal.</returns>
    public static bool operator !=(TypeModel? left, TypeModel? right)
    {
        return !Equals(left!, right!);
    }
}