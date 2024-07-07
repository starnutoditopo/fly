using System;

namespace Fly.ViewModels;

public class DocumentInformation
{
    public DocumentInformation(string displayName, string bookmark)
    {
        ArgumentNullException.ThrowIfNull(displayName);
        ArgumentNullException.ThrowIfNull(bookmark);

        DisplayName = displayName;
        Bookmark = bookmark;
    }
    public string DisplayName { get; }
    public string Bookmark { get; }
}