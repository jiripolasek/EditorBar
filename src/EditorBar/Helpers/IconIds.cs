﻿// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using Microsoft.VisualStudio.Imaging;

namespace JPSoftworks.EditorBar.Helpers;

internal static class IconIds
{
    public const int None = KnownImageIds.Blank;

    // semantic icons
    public const int AbstractMember = KnownImageIds.DialogTemplate;
    public const int AsyncMember = KnownImageIds.Thread;
    public const int Generic = KnownImageIds.MarkupXML;
    public const int ExtensionMethod = KnownImageIds.ExtensionMethod;
    public const int StaticMember = KnownImageIds.OverlayTimelineMarkBlack;
    public const int InstanceMember = KnownImageIds.BuildQueue;
    public const int DefaultInterfaceImplementation = KnownImageIds.AddInterface;

    public const int ReadonlyField = KnownImageIds.Field
                                     | (KnownImageIds.OverlayLock << OverlayShift)
                                     | FullOverlayMask;

    public const int ReadonlyProperty = KnownImageIds.Property
                                        | (KnownImageIds.OverlayLock << OverlayShift)
                                        | FullOverlayMask;

    public const int ReadonlyMethod = KnownImageIds.Method
                                      | (KnownImageIds.OverlayLock << OverlayShift)
                                      | FullOverlayMask;

    public const int ReadonlyType = KnownImageIds.Type | (KnownImageIds.OverlayLock << OverlayShift) | FullOverlayMask;
    public const int RefMember = KnownImageIds.DraggedCurrentInstructionPointer;
    public const int InitonlyProperty = KnownImageIds.Property | (KnownImageIds.New << OverlayShift);
    public const int AutoProperty = KnownImageIds.PropertyShortcut;
    public const int VolatileField = KnownImageIds.SetProactiveCaching;
    public const int AbstractClass = KnownImageIds.AbstractClass;
    public const int SealedClass = KnownImageIds.ClassSealed;
    public const int SealedMethod = KnownImageIds.MethodSealed;
    public const int SealedEvent = KnownImageIds.EventSealed;
    public const int SealedProperty = KnownImageIds.PropertySealed;
    public const int RequiredMember = KnownImageIds.StatusRequired;
    public const int PublicConstructor = KnownImageIds.MethodPublic;
    public const int ProtectedConstructor = KnownImageIds.MethodProtected;
    public const int InternalConstructor = KnownImageIds.MethodInternal;
    public const int PrivateConstructor = KnownImageIds.MethodPrivate;
    public const int PartialClass = KnownImageIds.ClassShortcut;
    public const int PartialInterface = KnownImageIds.InterfaceShortcut;
    public const int PartialStruct = KnownImageIds.ValueTypeShortcut;
    public const int ImplicitConversion = KnownImageIds.MacroPublic;
    public const int ExplicitConversion = KnownImageIds.MacroShortcut;
    public const int LocalFunction = KnownImageIds.MethodSnippet;
    public const int LocalVariable = KnownImageIds.LocalVariable;
    public const int Namespace = KnownImageIds.Namespace;
    public const int Class = KnownImageIds.Class;
    public const int ValueType = KnownImageIds.ValueType;
    public const int Enum = KnownImageIds.Enumeration;
    public const int EnumFlags = KnownImageIds.SetField;
    public const int Interface = KnownImageIds.Interface;
    public const int Delegate = KnownImageIds.Delegate;
    public const int Field = KnownImageIds.Field;
    public const int Event = KnownImageIds.Event;
    public const int Method = KnownImageIds.Method;
    public const int Constructor = KnownImageIds.Method;

    public const int Destructor
        = KnownImageIds.Method | (KnownImageIds.OverlayOffline << OverlayShift) | FullOverlayMask;

    public const int EnumField = KnownImageIds.EnumerationItemPublic;
    public const int GenericDefinition = KnownImageIds.Template;
    public const int Region = KnownImageIds.Numeric;
    public const int Unsafe = KnownImageIds.HotSpot;
    public const int Obsoleted = KnownImageIds.Timeout;
    public const int UnavailableSymbol = KnownImageIds.StatusWarning;
    public const int LambdaExpression = KnownImageIds.PartitionFunction;
    public const int ParenthesizedExpression = KnownImageIds.NamedSet;
    public const int Argument = KnownImageIds.Parameter;
    public const int Attribute = KnownImageIds.FormPostBodyParameterNode;
    public const int Return = KnownImageIds.Return;
    public const int Value = KnownImageIds.Field;
    public const int If = KnownImageIds.If;
    public const int Else = KnownImageIds.GoToNextUncovered;
    public const int TryCatch = KnownImageIds.TryCatch;
    public const int Using = KnownImageIds.TransactedReceiveScope;
    public const int DoWhile = KnownImageIds.DoWhile;
    public const int While = KnownImageIds.While;
    public const int Switch = KnownImageIds.FlowSwitch;
    public const int SwitchSection = KnownImageIds.FlowDecision;
    public const int PublicSymbols = KnownImageIds.ModulePublic;
    public const int ProtectedSymbols = KnownImageIds.ModuleProtected;
    public const int InternalSymbols = KnownImageIds.ModuleInternal;
    public const int PrivateSymbols = KnownImageIds.ModulePrivate;
    public const int PublicPropertyMethod = KnownImageIds.ObjectPublic;
    public const int ProtectedPropertyMethod = KnownImageIds.ObjectProtected;
    public const int InternalPropertyMethod = KnownImageIds.ObjectInternal;
    public const int PrivatePropertyMethod = KnownImageIds.ObjectPrivate;
    public const int OverrideEvent = KnownImageIds.ModifyEvent;
    public const int OverrideProperty = KnownImageIds.ModifyProperty;
    public const int OverrideMethod = KnownImageIds.ModifyMethod;
    public const int VirtualMember = KnownImageIds.Online;
    public const int ExplicitInterfaceEvent = KnownImageIds.RenameEvent;
    public const int ExplicitInterfaceProperty = KnownImageIds.RenameProperty;
    public const int ExplicitInterfaceMethod = KnownImageIds.RenameMethod;
    public const int Constant = KnownImageIds.Constant;
    public const int FunctionPointer = KnownImageIds.CallReturnInstructionPointer;

    // user interface icons
    public const int Filter = KnownImageIds.Filter;
    public const int FilterCustomized = KnownImageIds.EditFilter;
    public const int ClearFilter = KnownImageIds.DeleteFilter;
    public const int Add = KnownImageIds.Add;
    public const int Remove = KnownImageIds.Cancel;
    public const int Search = KnownImageIds.Search;
    public const int RunTest = KnownImageIds.RunTest;
    public const int DebugTest = KnownImageIds.DebugSelection;
    public const int Project = KnownImageIds.CSProjectNode;
    public const int File = KnownImageIds.CSSourceFile;
    public const int FileEmpty = KnownImageIds.CSFile;
    public const int FileLocations = KnownImageIds.DocumentCollection;
    public const int GlobalNamespace = KnownImageIds.Home;
    public const int Module = KnownImageIds.Module;
    public const int Headings = KnownImageIds.Flag;
    public const int Heading1 = KnownImageIds.FlagDarkRed;
    public const int Heading2 = KnownImageIds.FlagRed;
    public const int Heading3 = KnownImageIds.FlagGreen;
    public const int Heading4 = KnownImageIds.FlagTurquoise;
    public const int Heading5 = KnownImageIds.FlagPurple;
    public const int Quotation = KnownImageIds.TextArea;
    public const int UnorderedList = KnownImageIds.BulletList;
    public const int OrderedList = KnownImageIds.OrderedList;
    public const int ReferencedXmlDoc = KnownImageIds.GoToNextComment;
    public const int ExceptionXmlDoc = KnownImageIds.StatusInvalidOutline;
    public const int RemarksXmlDoc = KnownImageIds.CommentGroup;
    public const int ExampleXmlDoc = KnownImageIds.EnableCode;
    public const int SeeAlsoXmlDoc = KnownImageIds.Next;
    public const int DeclaredVariables = KnownImageIds.AddVariable;
    public const int ReadVariables = KnownImageIds.ExternalVariableValue;
    public const int WrittenVariables = KnownImageIds.PromoteVariable;
    public const int RefVariables = KnownImageIds.GlobalVariable;
    public const int TypeAndDelegate = KnownImageIds.EntityContainer;
    public const int ReturnValue = KnownImageIds.ReturnValue;
    public const int AnonymousType = KnownImageIds.GroupByType;
    public const int ParameterCandidate = KnownImageIds.ParameterWarning;

    public const int SymbolCandidate
        = KnownImageIds.Field | (KnownImageIds.OverlayWarning << OverlayShift) | FullOverlayMask;

    public const int InterfaceImplementation = KnownImageIds.ImplementInterface;
    public const int Disposable = KnownImageIds.InterfacePublic | (KnownImageIds.Brush << OverlayShift);
    public const int Discard = KnownImageIds.HiddenFile;
    public const int BaseTypes = KnownImageIds.ParentChild;
    public const int MethodOverloads = KnownImageIds.MethodSet;
    public const int XmlDocComment = KnownImageIds.Comment;
    public const int TypeParameters = KnownImageIds.TypeDefinition;
    public const int OpCodes = KnownImageIds.Binary;
    public const int Comment = KnownImageIds.CommentCode;
    public const int Uncomment = KnownImageIds.UncommentCode;
    public const int Rename = KnownImageIds.Rename;
    public const int Refactoring = KnownImageIds.Refactoring;
    public const int ReorderParameters = KnownImageIds.ReorderParameters;
    public const int ExtractInterface = KnownImageIds.ExtractInterface;
    public const int ExtractMethod = KnownImageIds.ExtractMethod;
    public const int EncapsulateField = KnownImageIds.EncapsulateField;
    public const int AddBraces = KnownImageIds.AddNamespace;
    public const int AsToCast = KnownImageIds.ReportingAction;
    public const int DuplicateMethodDeclaration = KnownImageIds.MethodSet;
    public const int DeleteMethod = KnownImageIds.MethodMissing;
    public const int DeleteType = KnownImageIds.ClassMissing;
    public const int DeleteProperty = KnownImageIds.PropertyMissing;
    public const int DeleteEvent = KnownImageIds.EventMissing;
    public const int DeleteField = KnownImageIds.FieldMissing;
    public const int DeleteCondition = KnownImageIds.DeleteClause;
    public const int MergeCondition = KnownImageIds.GroupByClause;
    public const int NestCondition = KnownImageIds.AddChildNode;
    public const int SplitCondition = KnownImageIds.UngroupClause;
    public const int MultiLine = KnownImageIds.WordWrap;
    public const int MultiLineList = KnownImageIds.DataList;
    public const int InterpolatedString = KnownImageIds.Quote;
    public const int SwapOperands = KnownImageIds.SwitchSourceOrTarget;
    public const int ToggleValue = KnownImageIds.ToggleButton;
    public const int InvertOperator = KnownImageIds.Operator;
    public const int UseStaticField = KnownImageIds.LinkFile;
    public const int AddXmlDoc = KnownImageIds.AddComment;
    public const int TagCode = KnownImageIds.MarkupTag;
    public const int TagXmlDocSee = KnownImageIds.Next;
    public const int TagXmlDocPara = KnownImageIds.ParagraphHardReturn;
    public const int TagBold = KnownImageIds.Bold;
    public const int TagItalic = KnownImageIds.Italic;
    public const int TagUnderline = KnownImageIds.Underline;
    public const int TagHyperLink = KnownImageIds.HyperLink;
    public const int TagStrikeThrough = KnownImageIds.StrikeThrough;
    public const int TagHighlight = KnownImageIds.Highlighter;
    public const int TagSubscript = KnownImageIds.Subscript;
    public const int TagSuperscript = KnownImageIds.Superscript;
    public const int Marks = KnownImageIds.FlagGroup;
    public const int MarkSymbol = KnownImageIds.Flag;
    public const int UnmarkSymbol = KnownImageIds.FlagOutline;
    public const int ToggleBreakpoint = KnownImageIds.BreakpointEnabled;
    public const int ToggleBookmark = KnownImageIds.Bookmark;
    public const int Watch = KnownImageIds.Watch;
    public const int DeleteBreakpoint = KnownImageIds.DeleteBreakpoint;
    public const int RunToCursor = KnownImageIds.GoToLast;
    public const int SetNextStatement = KnownImageIds.GoToNextInList;
    public const int EditSelection = KnownImageIds.CustomActionEditor;
    public const int FormatSelection = KnownImageIds.FormatSelection;
    public const int FormatDocument = KnownImageIds.FormatDocument;
    public const int PartialDocumentCount = KnownImageIds.OpenDocumentFromCollection;
    public const int DeclarationModifier = KnownImageIds.ControlAltDel;
    public const int LineOfCode = KnownImageIds.Code;
    public const int InstanceProducer = KnownImageIds.NewItem;
    public const int GoToDefinition = KnownImageIds.GoToDefinition;
    public const int QuickAction = KnownImageIds.IntellisenseLightBulb;
    public const int SelectCode = KnownImageIds.BlockSelection;
    public const int SelectBlock = KnownImageIds.MatchBrace;
    public const int SelectText = KnownImageIds.RectangleSelection;
    public const int SelectAll = KnownImageIds.SelectAll;
    public const int Open = KnownImageIds.Open;
    public const int OpenFolder = KnownImageIds.OpenFolder;
    public const int OpenWithVisualStudio = KnownImageIds.VisualStudio;
    public const int OpenWithCmd = KnownImageIds.Console;
    public const int Load = KnownImageIds.FolderOpened;
    public const int SaveAs = KnownImageIds.SaveAs;
    public const int Copy = KnownImageIds.Copy;
    public const int Cut = KnownImageIds.Cut;
    public const int Paste = KnownImageIds.Paste;
    public const int Delete = KnownImageIds.Cancel;
    public const int Undo = KnownImageIds.Undo;
    public const int Duplicate = KnownImageIds.CopyItem;
    public const int Find = KnownImageIds.QuickFind;
    public const int FindNext = KnownImageIds.FindNext;
    public const int Replace = KnownImageIds.QuickReplace;
    public const int FindInFile = KnownImageIds.FindInFile;
    public const int ReplaceInFolder = KnownImageIds.ReplaceInFolder;
    public const int SearchWebSite = KnownImageIds.OpenWebSite;
    public const int CustomizeWebSearch = KnownImageIds.Edit;
    public const int SurroundWith = KnownImageIds.AddSnippet;
    public const int WrapText = KnownImageIds.CorrelationScope;
    public const int CustomizeWrapText = KnownImageIds.Edit;
    public const int ToggleParentheses = KnownImageIds.MaskedTextBox;
    public const int NewGuid = KnownImageIds.NewNamedSet;
    public const int IncrementNumber = KnownImageIds.Counter;
    public const int JoinLines = KnownImageIds.Join;
    public const int SortLines = KnownImageIds.SortAscending;
    public const int DeleteEmptyLines = KnownImageIds.IgnoreTrimWhiteSpace;
    public const int TrimTrailingSpaces = KnownImageIds.EditRowRight;
    public const int Unindent = KnownImageIds.DecreaseIndent;
    public const int Indent = KnownImageIds.IncreaseIndent;
    public const int StageSelectedRange = KnownImageIds.Add;
    public const int RevertSelectedRange = KnownImageIds.Undo;
    public const int ListMembers = KnownImageIds.ListMembers;
    public const int MissingImplementation = KnownImageIds.StatusInvalid;
    public const int SymbolAnalysis = KnownImageIds.DimensionBrowserView;
    public const int FindReference = KnownImageIds.ReferencedDimension;
    public const int FindReferencingSymbols = KnownImageIds.ShowBuiltIns;
    public const int FindReferrers = KnownImageIds.ShowCallerGraph;
    public const int FindTypeReferrers = KnownImageIds.ShowReferencedElements;
    public const int FindDerivedTypes = KnownImageIds.ParentChildAttribute;
    public const int FindImplementations = KnownImageIds.ImplementInterface;
    public const int FindSymbolsWithName = KnownImageIds.DisplayName;
    public const int FindOverloads = KnownImageIds.OverloadBehavior;
    public const int FindMethodsMatchingSignature = KnownImageIds.ClassMethodReference;
    public const int FindParameterAssignment = KnownImageIds.InputParameter;
    public const int GoToReturnType = KnownImageIds.GoToDeclaration;
    public const int GoToType = KnownImageIds.Type;
    public const int GoToMember = KnownImageIds.Next;
    public const int GoToSymbol = KnownImageIds.FindSymbol;
    public const int GoToDeclaration = KnownImageIds.GoToDeclaration;
    public const int Capitalize = KnownImageIds.Font;
    public const int Uppercase = KnownImageIds.IncreaseFontSize;
    public const int Lowercase = KnownImageIds.DecreaseFontSize;
    public const int HtmlEncode = KnownImageIds.ReloadXML;
    public const int UrlEncode = KnownImageIds.DynamicWebSite;
    public const int EntityDecode = KnownImageIds.HanCharacter;
    public const int EditMatches = KnownImageIds.EditAssociation;
    public const int ShowClassificationInfo = KnownImageIds.HighlightText;
    public const int TogglePinning = KnownImageIds.Unpin;
    public const int Pin = KnownImageIds.Pin;
    public const int Unpin = KnownImageIds.Unpin;
    public const int Close = KnownImageIds.Close;
    public const int SyntaxTheme = KnownImageIds.ColorPalette;
    public const int Opacity = KnownImageIds.FillOpacity;
    public const int CustomizeStyle = KnownImageIds.StyleBlock;
    public const int PickColor = KnownImageIds.ColorDialog;
    public const int Brightness = KnownImageIds.Brightness;
    public const int Reset = KnownImageIds.EmptyBucket;
    public const int ResetTheme = KnownImageIds.CleanData;
    public const int Cpu = KnownImageIds.Processor;
    public const int Memory = KnownImageIds.Memory;
    public const int Drive = KnownImageIds.HardDrive;
    public const int Network = KnownImageIds.Network;
    public const int Question = KnownImageIds.StatusHelp;
    public const int HiddenInfo = KnownImageIds.StatusHidden;
    public const int Info = KnownImageIds.StatusInformation;
    public const int Suggestion = KnownImageIds.StatusAlert;
    public const int SyntaxError = KnownImageIds.StatusError;
    public const int SevereWarning = KnownImageIds.StatusWarning;
    public const int Warning = KnownImageIds.StatusWarningOutline;
    public const int Error = KnownImageIds.StatusInvalid;
    public const int Stop = KnownImageIds.StatusNo;

    // symbol usage icons
    public const int UseToWrite = KnownImageIds.Writeable;
    public const int UseToWriteNull = KnownImageIds.EmptyContainer;
    public const int UseAsTypeParameter = KnownImageIds.CPPMarkupXML;
    public const int UseToCast = KnownImageIds.ReportingAction;
    public const int UseToCatch = KnownImageIds.TryCatch;
    public const int UseAsDelegate = KnownImageIds.DelegateShortcut;
    public const int AttachEvent = KnownImageIds.AddEvent;
    public const int DetachEvent = KnownImageIds.EventMissing;
    public const int TriggerEvent = KnownImageIds.Event;

    // Overlay
    private const int OverlayShift = 16;

    private const int ImageMask = (1 << OverlayShift) - 1;
    private const int FullOverlayMask = 1 << ((OverlayShift * 2) - 1);
    private const int OverlayMask = ImageMask ^ (1 << (OverlayShift - 1));

    public static bool HasOverlay(this int iconId)
    {
        return (uint)iconId > 1 << OverlayShift;
    }

    public static int MakeOverlayImage(int iconId, int overlayId, bool fullOverlay)
    {
        return iconId | ((overlayId & OverlayMask) << OverlayShift) | (fullOverlay ? FullOverlayMask : 0);
    }

    public static bool IsOverlay(this int overlayId)
    {
        return (overlayId & (1 << (OverlayShift - 1))) != 0;
    }

    public static (int, int, bool) DeconstructIconOverlay(this int iconId)
    {
        return (iconId & ImageMask, (iconId >> OverlayShift) & OverlayMask, (iconId & FullOverlayMask) != 0);
    }
}