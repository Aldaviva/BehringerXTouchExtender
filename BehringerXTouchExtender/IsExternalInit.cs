using System.ComponentModel;

// ReSharper disable once CheckNamespace - this is the exact namespace required by this type to fix broken record compilation for .NET Standard 2.0
namespace System.Runtime.CompilerServices;

/// <summary>
/// <para>Needed to make .NET Standard 2.0 stop breaking the build when a record type with auto-initialized property parameters is defined.</para>
/// <para>From <see href="https://stackoverflow.com/a/62656145/979493"/></para>
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal class IsExternalInit;