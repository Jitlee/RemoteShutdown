;chinacnit.com
;remoteshutdown setup
;2012-10-09
;Jitlee.Wan

#define MyAppName "���ƽ���ƽ̨�ն�Զ�̿���"
#define OutputBaseFilename "���ƽ���ƽ̨�ն�Զ�̿��ư�װ����"
#define MyAppVersion "1.0"
#define MyAppVersionText "RemoteShutdown 1.0"
#define MyAppPublisher "CNIT, Inc."
#define MyAppCopyright "Copyright (C) 2012 CNIT. ALL Rights Reserved."
#define MyAppURL "http://tech.chinacnit.com/"
#define MyDirName "RemoteShutdown"


[Setup]
AppId=44C136F3-F62B-8152-ABC9-6502F28F02D1
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppVersion}
VersionInfoTextVersion={#MyAppVersionText}
VersionInfoCopyright={#MyAppCopyright}
AppCopyright={#MyAppCopyright}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
DefaultDirName={pf}\{#MyDirName}
DefaultGroupName={#MyAppName}
OutputBaseFilename={#OutputBaseFilename} 
OutputDir=./
DisableProgramGroupPage=yes
PrivilegesRequired=admin
ShowUndisplayableLanguages=yes

[Components]
Name: "server"; Description: "��������";
Name: "client"; Description: "�ͻ���"; Types: full compact custom;
Name: "client\tcp"; Description: "TCP�ͻ��ˣ��������÷�����IP"; Flags:exclusive
Name: "clinet\udp"; Description: "UDP�ͻ��ˣ��Զ���Ѱ������������ͬһ����)";  Flags:exclusive

[Files]
; Install MyProg-x64.exe if running in 64-bit mode (x64; see above),
; MyProg.exe otherwise.
Source: "dotNetFx40_Full_x86_x64.exe"; DestDir: "{tmp}"; Flags: ignoreversion

Source: "Server.exe"; DestDir: "{app}";Components:server;Source: "tcp\Client.exe"; DestDir: "{app}";Components:client\tcp;DestName:"tcp.exe"
Source: "udp\Client.exe"; DestDir: "{app}";Components:clinet\udp;DestName:"udp.exe"

[Run]

Filename: {tmp}\dotNetFx40_Full_x86_x64.exe; Parameters: "/q"; WorkingDir: "{tmp}"; StatusMsg: "���ڰ�װ .NET����, �˰�װ���̿��ܺܺ�ʱ,�벻Ҫ�رոó���..."; Check : VerifierFramework
Filename: {app}\server.exe;Description:"������������";Flags:postinstall;Components:server;
Filename: {app}\tcp.exe;Description:"�����ͻ���(TCP)";Flags:postinstall;Components:client\tcp; 
Filename: {app}\udp.exe;Description:"�����ͻ���(UDP)";Flags:postinstall;Components:clinet\udp;

[Icons]
Name: "{group}\��������"; Filename: "{app}\server.exe"; WorkingDir: "{app}" ;IconIndex: 0;Components:server;
Name: "{group}\�ͻ���(TCP)"; Filename: "{app}\tcp.exe"; WorkingDir: "{app}";IconIndex: 1;Components:client\tcp;
Name: "{group}\�ͻ���(UDP)"; Filename: "{app}\udp.exe"; WorkingDir: "{app}" ;IconIndex: 2;Components:clinet\udp;
Name: "{group}\ж��"; Filename: "{uninstallexe}"; IconIndex: 3;

[Languages]
Name: "zh"; MessagesFile: "compiler:Languages\ChineseSimple.isl"

[Code] 

function VerifierFramework():boolean; 
begin 
Result:=not fileExists(ExpandConstant('{win}') + '\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe'); 
end;