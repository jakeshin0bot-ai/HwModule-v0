[Setup]
AppName=HwModule
AppVersion=0.1.0
AppPublisher=Impacsys
DefaultDirName={pf}\HwModule
DefaultGroupName=HwModule
OutputDir=Output
OutputBaseFilename=HwModule-v0-Setup
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin

[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"

[Tasks]
Name: "desktopicon"; Description: "바탕화면 바로가기 만들기"; GroupDescription: "추가 아이콘:"; Flags: unchecked
Name: "autostart";   Description: "Windows 시작 시 자동 실행";   GroupDescription: "시작 옵션:";     Flags: unchecked

[Files]
Source: "bin\Release\HwModule.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\*.dll";        DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "bin\Release\*.config";     DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\HwModule v0";          Filename: "{app}\HwModule.exe"
Name: "{group}\제거";                  Filename: "{uninstallexe}"
Name: "{commondesktop}\HwModule v0";   Filename: "{app}\HwModule.exe"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "HwModule"; ValueData: "{app}\HwModule.exe"; Flags: uninsdeletevalue; Tasks: autostart

[Run]
Filename: "{app}\HwModule.exe"; Description: "HwModule v0 시작"; Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "taskkill"; Parameters: "/F /IM HwModule.exe"; Flags: runhidden
