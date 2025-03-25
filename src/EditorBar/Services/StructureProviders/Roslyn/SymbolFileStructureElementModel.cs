// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Reflection;
using System.Text;
using JPSoftworks.EditorBar.Helpers;
using JPSoftworks.EditorBar.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text.Editor;

namespace JPSoftworks.EditorBar.Services.StructureProviders.Roslyn;

public record SymbolFileStructureElementModel : FileStructureElementModel
{
    private static readonly string CtorNameAlt = $" ({ConstructorInfo.ConstructorName})";
    private static readonly string TypeCtorNameAlt = $" ({ConstructorInfo.TypeConstructorName})";

    internal static readonly SymbolDisplayFormat SymbolDisplayFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameOnly,
        SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
        SymbolDisplayMemberOptions.IncludeExplicitInterface,
        SymbolDisplayDelegateStyle.NameOnly,
        SymbolDisplayExtensionMethodStyle.Default,
        SymbolDisplayParameterOptions.IncludeExtensionThis | SymbolDisplayParameterOptions.IncludeType,
        SymbolDisplayPropertyStyle.NameOnly,
        SymbolDisplayLocalOptions.None,
        SymbolDisplayKindOptions.None,
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral |
        SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
    );

    internal static readonly SymbolDisplayFormat MethodDisplayFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameOnly,
        SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
        SymbolDisplayMemberOptions.IncludeExplicitInterface | SymbolDisplayMemberOptions.IncludeParameters,
        SymbolDisplayDelegateStyle.NameOnly,
        SymbolDisplayExtensionMethodStyle.Default,
        SymbolDisplayParameterOptions.IncludeExtensionThis | SymbolDisplayParameterOptions.IncludeType,
        SymbolDisplayPropertyStyle.NameOnly,
        SymbolDisplayLocalOptions.None,
        SymbolDisplayKindOptions.None,
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral |
        SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
    );

    internal static readonly SymbolDisplayFormat IndexerSymbolDisplayFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameOnly,
        SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
        SymbolDisplayMemberOptions.IncludeExplicitInterface | SymbolDisplayMemberOptions.IncludeParameters,
        SymbolDisplayDelegateStyle.NameOnly,
        SymbolDisplayExtensionMethodStyle.Default,
        SymbolDisplayParameterOptions.IncludeExtensionThis | SymbolDisplayParameterOptions.IncludeType,
        SymbolDisplayPropertyStyle.NameOnly,
        SymbolDisplayLocalOptions.None,
        SymbolDisplayKindOptions.None,
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral |
        SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
    );

    internal static readonly SymbolDisplayFormat ParameterFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        parameterOptions: SymbolDisplayParameterOptions.IncludeType |
                          SymbolDisplayParameterOptions.IncludeExtensionThis |
                          SymbolDisplayParameterOptions.IncludeOptionalBrackets,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters |
                         SymbolDisplayGenericsOptions.IncludeVariance,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                              SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral |
                              SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName |
                              SymbolDisplayMiscellaneousOptions.UseSpecialTypes);


    public ISymbol? TargetSymbol { get; init; }

    public Location Location { get; init; }

    public SymbolFileStructureElementModel(ISymbol targetSymbol, Location location, string? secondaryName = null)
    {
        this.TargetSymbol = targetSymbol;
        this.Location = location;
        this.SecondaryName = secondaryName;
        this.ImageMoniker = VsImageHelper.GetImageMonikers(targetSymbol.GetImageId());
        this.NavigationAction = this.NavigationAction2;

        this.PrimaryName = targetSymbol switch
        {
            // add special handling for constructor and static constructor and finalizer
            // - primary name =
            //   - containing type name + ".ctor" or ".cctor" for static constructor
            //   - ~ClassName for finalizer
            IMethodSymbol { MethodKind: MethodKind.Constructor } methodSymbol =>
                methodSymbol.ContainingType!.Name + CtorNameAlt,

            IMethodSymbol { MethodKind: MethodKind.StaticConstructor } staticConstructorSymbol =>
                staticConstructorSymbol.ContainingType!.Name + TypeCtorNameAlt,

            IMethodSymbol { MethodKind: MethodKind.Destructor } destructorSymbol =>
                "~" + destructorSymbol.ContainingType!.Name + " (finalizer)",

            // if method is top-level main method, return "Main" + parameters as a hint to developer
            IMethodSymbol
                {
                    MethodKind: MethodKind.Ordinary, Name: FileStructureHelper.TopLevelStatementMainMethodName
                } method
                => Strings.TopLevelStatements! + FormatMethodParameters(method.Parameters),

            IMethodSymbol { ExplicitInterfaceImplementations: { Length: > 0 } explicitImplementations } =>
                explicitImplementations[0].ContainingType!.Name + "." + explicitImplementations[0].Name,

            // indexer needs special handling
            IPropertySymbol { IsIndexer: true } indexerSymbol => indexerSymbol.Name,

            _ => targetSymbol.ToDisplayString(SymbolDisplayFormat)
        };

        // Secondary name:
        // - namespace for named type
        // - parameter types for methods (in format "Type1, Type2, ...")
        // - type for properties & fields
        // - type for events

        this.SecondaryName ??= EvaluateSecondary(targetSymbol);

        // Search text: primary name + secondary name
        this.SearchText = this.PrimaryName;
        if (!string.IsNullOrWhiteSpace(this.SecondaryName!))
        {
            this.SearchText += " " + this.SecondaryName;
        }
    }

    private async void NavigationAction2(IWpfTextView obj)
    {
        if (this.TargetSymbol != null)
        {
            await Navigator.NavigateToAnchorAsync(new SymbolAnchorPoint(this.TargetSymbol, this.Location));
        }
    }

    private static string? EvaluateSecondary(ISymbol targetSymbol)
    {
        // if targetsymbol is delegate: return delegate signature
        try
        {
            return targetSymbol switch
            {
                INamedTypeSymbol { TypeKind: TypeKind.Delegate } namedType => FormatMethodParameters(namedType
                    .DelegateInvokeMethod?.Parameters) ?? "",
                INamedTypeSymbol namedType => namedType.ContainingNamespace?.ToDisplayString(),
                IMethodSymbol method => FormatMethodParameters(method.Parameters),
                IPropertySymbol { IsIndexer: true } indexer => FormatMethodParameters(indexer.Parameters, true),
                IPropertySymbol property => property.Type.ToDisplayString(),
                IFieldSymbol field => FormatField(field),
                IEventSymbol @event => @event.Type.ToDisplayString(),
                _ => null
            };
        }
        catch (Exception ex)
        {
            ex.Log();
            return null;
        }
    }

    private static string? FormatField(IFieldSymbol field)
    {
        // if field is a constant return type = value
        if (field.HasConstantValue)
        {
            // if field is enum member return value
            return field.ContainingType?.TypeKind == TypeKind.Enum
                ? FormatEnumValue(field)
                : FormatConstantValue(field);
        }

        return field.Type.ToDisplayString();
    }

    private static string FormatConstantValue(IFieldSymbol field)
    {
        return $"{field.Type.ToDisplayString()} = {field.ConstantValue}";
    }

    private static string? FormatEnumValue(IFieldSymbol field)
    {
        var expression = PrintFieldInitializer(field);
        if (expression != null)
        {
            return expression + "\u00A0=\u00A0" + field.ConstantValue;
        }

        return field.ConstantValue?.ToString();
    }

    private static string? PrintFieldInitializer(IFieldSymbol fieldSymbol)
    {
        // Get the syntax references for the field
        var syntaxReference = fieldSymbol.DeclaringSyntaxReferences.FirstOrDefault();
        // Get the syntax node
        var syntaxNode = syntaxReference?.GetSyntax();

        return syntaxNode switch
        {
            VariableDeclaratorSyntax { Initializer: not null } variableDeclarator => variableDeclarator.Initializer
                .Value.ToString(),
            EnumMemberDeclarationSyntax { EqualsValue: not null } enumDeclaration => enumDeclaration.EqualsValue.Value
                .ToString(),
            _ => null
        };
    }

    private static string? FormatMethodParameters(
        ICollection<IParameterSymbol>? parameterSymbols,
        bool squareBrackets = false)
    {
        // If the method has parameters, format them as a comma-separated list
        // Includes the parameter type and default value (if present), ignores parameter names
        // e.g., "(int, bool, string? = null)"

        if (parameterSymbols?.Count == 0)
        {
            return null;
        }

        var parameters = new StringBuilder();
        parameters.Append(squareBrackets ? "[" : "(");

        parameters.Append(string.Join(", ", parameterSymbols.Select(p => p.ToDisplayString(ParameterFormat))));

        parameters.Append(squareBrackets ? "]" : ")");
        return parameters.ToString();
    }
}