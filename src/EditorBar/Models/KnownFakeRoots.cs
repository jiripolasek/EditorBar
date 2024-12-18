// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.IO;

namespace JPSoftworks.EditorBar.Models;

public static class KnownFakeRoots
{
    public static KnownFakeRoot[] FakeRoots { get; }

    static KnownFakeRoots()
    {
        FakeRoots =
        [
            new KnownFakeRoot("Temp", Path.GetTempPath()),
            //new KnownFakeRoot("Roaming App Data", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
            //new KnownFakeRoot("Local AppData", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)),
            //new KnownFakeRoot("User Profile", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)),
            //new KnownFakeRoot("Program Data", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)),
            //new KnownFakeRoot("System32", Environment.GetFolderPath(Environment.SpecialFolder.System)),
            //new KnownFakeRoot("Windows", Environment.GetFolderPath(Environment.SpecialFolder.Windows))
        ];
    }
}