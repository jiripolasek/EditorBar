// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Windows.Media;
using Color = System.Drawing.Color;

namespace JPSoftworks.EditorBar.ViewModels;

/// <summary>
/// Represents a breadcrumb model with a generic type.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
public class BreadcrumbModel<TModel> : BreadcrumbModel
{
    /// <summary>
    /// Gets the model associated with the breadcrumb.
    /// </summary>
    public TModel Model { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BreadcrumbModel{TModel}" /> class.
    /// </summary>
    /// <param name="model">The model associated with the breadcrumb.</param>
    /// <param name="text">The text of the breadcrumb.</param>
    /// <param name="background">The background brush of the breadcrumb.</param>
    /// <param name="foreground">The foreground brush of the breadcrumb.</param>
    public BreadcrumbModel(TModel model, string text, Brush background, Brush foreground) : base(text, background,
        foreground)
    {
        this.Model = model;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BreadcrumbModel{TModel}" /> class.
    /// </summary>
    /// <param name="model">The model associated with the breadcrumb.</param>
    /// <param name="text">The text of the breadcrumb.</param>
    /// <param name="background">The background color of the breadcrumb.</param>
    /// <param name="foreground">The foreground color of the breadcrumb.</param>
    public BreadcrumbModel(TModel model, string text, Color background, Color foreground) : base(text, background,
        foreground)
    {
        this.Model = model;
    }
}