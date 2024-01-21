// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using LipFileManager;

namespace WebviewAppTest
{
    public class AppState
    {
        public LipFile LipFile { get; set; }
        public bool EditorView { get; set; }
        public bool SetupView { get; set; }
        public bool WordsView { get; set; }
        public string PhonemText { get; set; }
        public string EnglishText { get; set; }
    }
}
