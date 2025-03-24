// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.VisualStudio.Imaging.Interop;

namespace JPSoftworks.EditorBar.Helpers;

/// <summary>
/// Represents either a single <see cref="ImageMoniker" /> or
/// an array of stacked <see cref="ImageMoniker" /> instances.
/// </summary>
/// <summary>
/// Represents either:
/// - A single <see cref="ImageMoniker" />
/// - A pair (2) <see cref="ImageMoniker" /> objects
/// - Or an array of 3 or more <see cref="ImageMoniker" /> objects
/// </summary>
public readonly struct StackedImageMoniker : IEquatable<StackedImageMoniker>
{
    /// <summary>
    /// Equality comparer for <see cref="ImageMoniker" />. More efficient than default struct
    /// equality check.
    /// </summary>
    private static readonly ImageMonikerEqualityComparer ImageMonikerDefaultComparer = new();

    /// <summary>
    /// Represents an empty <see cref="StackedImageMoniker" />.
    /// </summary>
    public static readonly StackedImageMoniker Empty = default;

    private enum StorageMode
    {
        Empty,
        Single,
        Pair,
        Array
    }

    private readonly ImageMoniker _moniker1;
    private readonly ImageMoniker _moniker2;
    private readonly ImageMoniker[]? _monikerArray;
    private readonly StorageMode _mode;

    #region Constructors

    /// <summary>
    /// Initializes a <see cref="StackedImageMoniker" /> with a single moniker.
    /// </summary>
    public StackedImageMoniker(ImageMoniker moniker)
    {
        this._moniker1 = moniker;
        this._moniker2 = default;
        this._monikerArray = null;
        this._mode = Equals(moniker, default(ImageMoniker)) ? StorageMode.Empty : StorageMode.Single;
    }

    /// <summary>
    /// Initializes a <see cref="StackedImageMoniker" /> with exactly two monikers.
    /// </summary>
    public StackedImageMoniker(ImageMoniker moniker1, ImageMoniker moniker2)
    {
        this._moniker1 = moniker1;
        this._moniker2 = moniker2;
        this._monikerArray = null;
        this._mode = StorageMode.Pair;
    }

    /// <summary>
    /// Initializes a <see cref="StackedImageMoniker" /> with an array of monikers.
    /// If the array length is 1 or 2, we automatically store them in the optimized fields.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="monikers" /> is null or empty.
    /// </exception>
    public StackedImageMoniker(ImageMoniker[] monikers)
    {
        if (monikers is null || monikers.Length == 0)
        {
            throw new ArgumentException("Must provide at least one moniker.", nameof(monikers));
        }

        if (monikers.Length == 1)
        {
            this._moniker1 = monikers[0];
            this._moniker2 = default;
            this._monikerArray = null;
            this._mode = StorageMode.Single;
        }
        else if (monikers.Length == 2)
        {
            this._moniker1 = monikers[0];
            this._moniker2 = monikers[1];
            this._monikerArray = null;
            this._mode = StorageMode.Pair;
        }
        else
        {
            this._moniker1 = default;
            this._moniker2 = default;
            this._monikerArray = monikers;
            this._mode = StorageMode.Array;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if this <see cref="StackedImageMoniker" /> contains exactly one moniker.
    /// </summary>
    public bool IsSingle => this._mode == StorageMode.Single;

    /// <summary>
    /// True if this <see cref="StackedImageMoniker" /> contains exactly two monikers.
    /// </summary>
    public bool IsPair => this._mode == StorageMode.Pair;

    /// <summary>
    /// True if this <see cref="StackedImageMoniker" /> contains an array of three or more monikers.
    /// </summary>
    public bool IsArray => this._mode == StorageMode.Array;

    /// <summary>
    /// Indicates whether the current storage mode is empty. Returns true if the storage mode is set to Empty.
    /// </summary>
    public bool IsEmpty => this._mode == StorageMode.Empty;

    /// <summary>
    /// Gets the single moniker (if <see cref="IsSingle" /> is true); otherwise throws.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public ImageMoniker Single
    {
        get
        {
            if (!this.IsSingle)
            {
                throw new InvalidOperationException(
                    $"This {nameof(StackedImageMoniker)} does not contain a single moniker.");
            }

            return this._moniker1;
        }
    }

    /// <summary>
    /// Gets the first moniker in the pair (if <see cref="IsPair" /> is true); otherwise throws.
    /// </summary>
    /// <exception cref="InvalidOperationException">This image moniker stack doesn't contain a pair of image monikers.</exception>
    public ImageMoniker First
    {
        get
        {
            if (!this.IsPair)
            {
                throw new InvalidOperationException($"This {nameof(StackedImageMoniker)} does not contain a pair.");
            }

            return this._moniker1;
        }
    }

    /// <summary>
    /// Gets the second moniker in the pair (if <see cref="IsPair" /> is true); otherwise throws.
    /// </summary>
    /// <exception cref="InvalidOperationException">This image moniker stack doesn't contain a pair of image monikers.</exception>
    public ImageMoniker Second
    {
        get
        {
            if (!this.IsPair)
            {
                throw new InvalidOperationException($"This {nameof(StackedImageMoniker)} does not contain a pair.");
            }

            return this._moniker2;
        }
    }

    /// <summary>
    /// Gets the array of monikers if <see cref="IsArray" /> is true.
    /// Otherwise returns a 1-element or 2-element array (depending on whether
    /// this <see cref="StackedImageMoniker" /> contains a single or pair).
    /// </summary>
    public ImageMoniker[] Monikers =>
        this._mode switch
        {
            StorageMode.Single => [this._moniker1],
            StorageMode.Pair => [this._moniker1, this._moniker2],
            StorageMode.Array => this._monikerArray!, // We know _monikerArray is not null in Array mode.
            StorageMode.Empty => [],
            _ => throw new InvalidOperationException($"Unexpected storage mode '{this._mode}'.")
        };

    #endregion

    #region Implicit Conversions

    /// <summary>
    /// Implicitly converts a single <see cref="ImageMoniker" /> into a <see cref="StackedImageMoniker" />.
    /// </summary>
    public static implicit operator StackedImageMoniker(ImageMoniker moniker)
    {
        return new StackedImageMoniker(moniker);
    }

    /// <summary>
    /// Implicitly converts a pair of <see cref="ImageMoniker" /> into a <see cref="StackedImageMoniker" />.
    /// </summary>
    public static implicit operator StackedImageMoniker((ImageMoniker, ImageMoniker) pair)
    {
        return new StackedImageMoniker(pair.Item1, pair.Item2);
    }

    /// <summary>
    /// Implicitly converts an array of <see cref="ImageMoniker" /> into a <see cref="StackedImageMoniker" />.
    /// </summary>
    public static implicit operator StackedImageMoniker(ImageMoniker[] monikers)
    {
        return new StackedImageMoniker(monikers);
    }

    #endregion

    /// <inheritdoc />
    public bool Equals(StackedImageMoniker other)
    {
        if (this._mode != other._mode)
        {
            return true;
        }

        return this._mode switch
        {
            StorageMode.Empty => true,
            StorageMode.Single => Equals(this.Single, other.Single),
            StorageMode.Pair => Equals(this.First, other.First) && Equals(this.Second, other.Second),
            StorageMode.Array => this.Monikers.SequenceEqual(other.Monikers, ImageMonikerDefaultComparer),
            _ => throw new InvalidOperationException($"Unexpected storage mode '{this._mode}'.")
        };
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is StackedImageMoniker other && this.Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this._mode switch
        {
            StorageMode.Empty => 0,
            StorageMode.Single => HashCode.Combine(this.Single),
            StorageMode.Pair => HashCode.Combine(this.First, this.Second),
            StorageMode.Array => this.Monikers.Aggregate(0, HashCode.Combine),
            _ => throw new InvalidOperationException($"Unexpected storage mode '{this._mode}'.")
        };
    }

    /// <summary>
    /// Compares two instances of <see cref="StackedImageMoniker" />  for equality.
    /// </summary>
    /// <param name="left">The first instance to compare for equality.</param>
    /// <param name="right">The second instance to compare for equality.</param>
    /// <returns>True if both instances are equal; otherwise, false.</returns>
    public static bool operator ==(StackedImageMoniker left, StackedImageMoniker right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares two instances of StackedImageMoniker for inequality.
    /// </summary>
    /// <param name="left">The first instance to compare for inequality.</param>
    /// <param name="right">The second instance to compare for inequality.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(StackedImageMoniker left, StackedImageMoniker right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this._mode switch
        {
            StorageMode.Empty => "Empty",
            StorageMode.Single => $"Single: [{this._moniker1}]",
            StorageMode.Pair => $"Pair: [{this._moniker1}], [{this._moniker2}]",
            StorageMode.Array => $"Array ({this._monikerArray!.Length} monikers)",
            _ => "Unknown"
        };
    }

    class ImageMonikerEqualityComparer : IEqualityComparer<ImageMoniker>
    {
        public bool Equals(ImageMoniker x, ImageMoniker y)
        {
            return x.Id == y.Id && x.Guid.Equals(y.Guid);
        }

        public int GetHashCode(ImageMoniker obj)
        {
            return HashCode.Combine(obj.Guid, obj.Id);
        }
    }
}