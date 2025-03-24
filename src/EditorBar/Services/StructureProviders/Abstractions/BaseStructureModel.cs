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
/// Base class representing a structural model.
/// </summary>
public abstract class BaseStructureModel : IEquatable<BaseStructureModel>
{
    /// <summary>
    /// Gets or initializes the stacked image moniker for this model.
    /// </summary>
    public StackedImageMoniker ImageMoniker { get; init; }

    /// <summary>
    /// Gets the name of this model.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the anchor point that indicates the location of this model.
    /// </summary>
    public AnchorPoint AnchorPoint { get; }

    /// <summary>
    /// Gets a value indicating whether this model can have child elements.
    /// </summary>
    public bool CanHaveChildren { get; init; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseStructureModel" /> class.
    /// </summary>
    /// <param name="displayName">The name of the structure.</param>
    /// <param name="imageMoniker">The stacked image moniker.</param>
    /// <param name="anchorPoint">The anchor point.</param>
    protected BaseStructureModel(string displayName, StackedImageMoniker imageMoniker, AnchorPoint anchorPoint)
    {
        Requires.NotNullOrWhiteSpace(displayName, nameof(displayName));
        Requires.NotNull(anchorPoint, nameof(anchorPoint));

        this.DisplayName = displayName;
        this.ImageMoniker = imageMoniker;
        this.AnchorPoint = anchorPoint;
    }

    /// <summary>
    /// Determines whether this instance is equal to another <see cref="BaseStructureModel" /> instance.
    /// </summary>
    /// <param name="other">The other instance to compare.</param>
    /// <returns>True if both instances are equal; otherwise, false.</returns>
    public bool Equals(BaseStructureModel? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.AnchorPoint.FilePath == other.AnchorPoint.FilePath
               && this.DisplayName == other.DisplayName
               && Equals(this.AnchorPoint, other.AnchorPoint);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
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

        return this.Equals((BaseStructureModel)obj);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>An integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.AnchorPoint.FilePath, this.DisplayName, this.AnchorPoint);
    }

    /// <summary>
    /// Determines whether two <see cref="BaseStructureModel" /> instances are equal.
    /// </summary>
    /// <param name="left">The left instance.</param>
    /// <param name="right">The right instance.</param>
    /// <returns>True if both instances are equal; otherwise, false.</returns>
    public static bool operator ==(BaseStructureModel? left, BaseStructureModel? right)
    {
        return Equals(left!, right!);
    }

    /// <summary>
    /// Determines whether two <see cref="BaseStructureModel" /> instances are not equal.
    /// </summary>
    /// <param name="left">The left instance.</param>
    /// <param name="right">The right instance.</param>
    /// <returns>True if both instances are not equal; otherwise, false.</returns>
    public static bool operator !=(BaseStructureModel? left, BaseStructureModel? right)
    {
        return !Equals(left!, right!);
    }
}