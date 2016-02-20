call SubmoduleInit.bat < nul

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" .\LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\LoggingServices\src\LoggingServices\bin\Release" ".\Dependency Builds\LoggingServices\DLLs\"





nuget.exe restore GladNet2\GladNetV2.sln
CD .\GladNet2\Lib
call BuildDependencies.bat < nul
CD ..

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" GladNetV2.sln /p:Configuration=Release /p:Platform="Any CPU"

CD ..

xcopy  /R /E /Y /q ".\GladNet2\src\GladNet.Common\bin\Release" ".\Dependency Builds\GladNet\DLLs\"
xcopy  /R /E /Y /q ".\GladNet2\src\GladNet.Serializer\bin\Release" ".\Dependency Builds\GladNet\DLLs\"
xcopy  /R /E /Y /q ".\GladNet2\src\GladNet.Server.Common\bin\Release" ".\Dependency Builds\GladNet\DLLs\"






nuget.exe restore GladNet.PhotonServer\GladNet.PhotonServer.sln
CD .\GladNet.PhotonServer\Lib
call BuildDependencies.bat < nul
CD ..

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" GladNet.PhotonServer.sln /p:Configuration=Release /p:Platform="Any CPU"

CD ..

xcopy  /R /E /Y /q ".\GladNet.PhotonServer\src\GladNet.PhotonServer.Common\bin\Release" ".\Dependency Builds\GladNet.PhotonServer\DLLs\"
xcopy  /R /E /Y /q ".\GladNet.PhotonServer\src\GladNet.PhotonServer.Server\bin\Release" ".\Dependency Builds\GladNet.PhotonServer\DLLs\"





nuget.exe restore GladNet.Serializer.Protobuf\GladNet.Serializer.Protobuf.sln
CD .\GladNet.Serializer.Protobuf\Lib
call BuildDependencies.bat < nul
CD ..

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" GladNet.Serializer.Protobuf.sln /p:Configuration=Release /p:Platform="Any CPU"

CD ..

xcopy  /R /E /Y /q ".\GladNet.Serializer.Protobuf\src\GladNet.Serializer.Protobuf\bin\Release" ".\Dependency Builds\GladNet.Serializer.Protobuf\DLLs\"





nuget.exe restore GladLive.Common\GladLive.Common.sln
CD .\GladLive.Common\Lib
call BuildDependencies.bat < nul
CD ..

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" GladLive.Common.sln /p:Configuration=Release /p:Platform="Any CPU"

CD ..

xcopy  /R /E /Y /q ".\GladLive.Common\src\GladLive.Common\bin\Release" ".\Dependency Builds\GladLive.Common\DLLs\"





nuget.exe restore GladLive.Server.Common\GladLive.Server.Common.sln
CD .\GladLive.Server.Common\Lib
call BuildDependencies.bat < nul
CD ..

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" GladLive.Server.Common.sln /p:Configuration=Release /p:Platform="Any CPU"

CD ..

xcopy  /R /E /Y /q ".\GladLive.Server.Common\src\GladLive.Server.Common\bin\Release" ".\Dependency Builds\GladLive.Server.Common\DLLs\"





PAUSE