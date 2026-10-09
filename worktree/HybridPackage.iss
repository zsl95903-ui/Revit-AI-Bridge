#define AppName "Revit AI"
#define AppVersion "5.0.0"
#define AppPublisher "Revit AI"
#define PayloadDir "$(RepoRoot)\hybrid-package\payload"
#define RevitYear "2027"

[Setup]
AppId={{A59E8ED1-6F1D-4F87-9F9C-4FB56B41D2A2}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={localappdata}\Programs\RevitAi
DefaultGroupName={#AppName}
DisableDirPage=no
DisableProgramGroupPage=yes
OutputDir=$(RepoRoot)\hybrid-package\dist
OutputBaseFilename=RevitAI-Hybrid-Setup-{#AppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayName={#AppName} {#AppVersion}
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "chinesesimplified"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#PayloadDir}\app\*"; DestDir: "{app}\app"; \
    Flags: ignoreversion recursesubdirs createallsubdirs

Source: "{#PayloadDir}\addins\RevitAi.HybridUI\*"; \
    DestDir: "{code:GetRevitAddinsPath|{#RevitYear}}\RevitAi.HybridUI"; \
    Flags: ignoreversion recursesubdirs createallsubdirs

Source: "{#PayloadDir}\addins\RevitAi.HybridUI.addin"; \
    DestDir: "{code:GetRevitAddinsPath|{#RevitYear}}"; \
    Flags: ignoreversion

Source: "{#PayloadDir}\extensions\*"; DestDir: "{app}\extensions"; \
    Flags: ignoreversion recursesubdirs createallsubdirs

[UninstallDelete]
Type: files; Name: "{code:GetRevitAddinsPath|{#RevitYear}}\RevitAi.addin"
Type: files; Name: "{code:GetRevitAddinsPath|{#RevitYear}}\RevitAi.HybridUI.addin"
Type: filesandordirs; Name: "{code:GetRevitAddinsPath|{#RevitYear}}\RevitAi.HybridUI"

[InstallDelete]
Type: files; Name: "{code:GetRevitAddinsPath|{#RevitYear}}\RevitAi.CodexBridge.addin"
Type: filesandordirs; Name: "{code:GetRevitAddinsPath|{#RevitYear}}\RevitAi.CodexBridge"

[Code]
function GetRevitAddinsPath(Param: String): String;
begin
  Result := ExpandConstant('{userappdata}\Autodesk\Revit\Addins\') + Param;
end;

procedure WriteLoaderManifest;
var
  Dir, FileName, Content, LoaderPath: String;
begin
  Dir := GetRevitAddinsPath('{#RevitYear}');
  if not DirExists(Dir) then
    ForceDirectories(Dir);

  LoaderPath := ExpandConstant('{app}\app\R{#RevitYear}\RevitAi.Loader.dll');
  FileName := AddBackslash(Dir) + 'RevitAi.addin';
  Content :=
    '<?xml version="1.0" encoding="utf-8"?>' + #13#10 +
    '<RevitAddIns>' + #13#10 +
    '  <AddIn Type="Application">' + #13#10 +
    '    <Name>RevitAi</Name>' + #13#10 +
    '    <Assembly>' + LoaderPath + '</Assembly>' + #13#10 +
    '    <FullClassName>RevitAi.Loader.ExternalApplication</FullClassName>' + #13#10 +
    '    <ClientId>3c9d0464-8643-5ffe-96e5-ab1769818209</ClientId>' + #13#10 +
    '    <VendorId>RevitAi</VendorId>' + #13#10 +
    '    <VendorDescription>RevitAi engine</VendorDescription>' + #13#10 +
    '    <VisibilityMode>AlwaysVisible</VisibilityMode>' + #13#10 +
    '    <AllowLoadIntoExistingSession>true</AllowLoadIntoExistingSession>' + #13#10 +
    '  </AddIn>' + #13#10 +
    '</RevitAddIns>' + #13#10;

  SaveStringToFile(FileName, Content, False);
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  AddinsDir: String;
begin
  if CurStep = ssInstall then
  begin
    AddinsDir := GetRevitAddinsPath('{#RevitYear}');
    DeleteFile(AddBackslash(AddinsDir) + 'RevitAi.CodexBridge.addin');
    DelTree(AddBackslash(AddinsDir) + 'RevitAi.CodexBridge', True, True, True);
  end;

  if CurStep = ssPostInstall then
    WriteLoaderManifest;
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
end;

