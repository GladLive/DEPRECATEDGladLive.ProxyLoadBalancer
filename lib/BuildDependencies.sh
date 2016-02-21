xbuild ./LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
mkdir -p Dependency\ Builds/LoggingServices/DLLs/
rsync -avv ./LoggingServices/src/LoggingServices/bin/Release/ Dependency\ Builds/LoggingServices/DLLs/

chmod +x ./GladNet2/lib/BuildDependencies.sh
cd GladNet2/lib/
./BuildDependencies.sh
cd ..
nuget restore GladNetV2.sln
xbuild GladNetV2.sln /p:Configuration=Release /p:Platform="Any CPU"
cd ..

mkdir -p Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Common/bin/Release/ Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Serializer/bin/Release/ Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Server.Common/bin/Release/ Dependency\ Builds/GladNet/DLLs/




chmod +x ./GladNet.PhotonServer/lib/BuildDependencies.sh
cd GladNet.PhotonServer/lib/
./BuildDependencies.sh
cd ..
nuget restore GladNet.PhotonServer.sln
xbuild GladNet.PhotonServer.sln /p:Configuration=Release /p:Platform="Any CPU"
cd ..

mkdir -p Dependency\ Builds/GladNet.PhotonServer/DLLs/
rsync -avv ./GladNet.PhotonServer/src/GladNet.PhotonServer.Common/bin/Release/ Dependency\ Builds/GladNet.PhotonServer/DLLs/
rsync -avv ./GladNet.PhotonServer/src/GladNet.PhotonServer.Server/bin/Release/ Dependency\ Builds/GladNet.PhotonServer/DLLs/





chmod +x ./GladNet.Serializer.Protobuf/lib/BuildDependencies.sh
cd GladNet.Serializer.Protobuf/lib/
./BuildDependencies.sh
cd ..
nuget restore GladNet.Serializer.Protobuf.sln
xbuild GladNet.Serializer.Protobuf.sln /p:Configuration=Release /p:Platform="Any CPU"
cd ..

mkdir -p Dependency\ Builds/GladNet.Serializer.Protobuf/DLLs/
rsync -avv ./GladNet.Serializer.Protobuf/src/GladNet.Serializer.Protobuf/bin/Release/ Dependency\ Builds/GladNet.Serializer.Protobuf/DLLs/





chmod +x ./GladLive.Common/lib/BuildDependencies.sh
cd GladLive.Common/lib/
./BuildDependencies.sh
cd ..
nuget restore GladLive.Common.sln
xbuild GladLive.Common.sln /p:Configuration=Release /p:Platform="Any CPU"
cd ..

mkdir -p Dependency\ Builds/GladLive.Common/DLLs/
rsync -avv ./GladLive.Common/src/GladLive.Common/bin/Release/ Dependency\ Builds/GladLive.Common/DLLs/





chmod +x ./GladLive.Server.Common/lib/BuildDependencies.sh
cd GladLive.Server.Common/lib/
./BuildDependencies.sh
cd ..
nuget restore GladLive.Server.Common.sln
xbuild GladLive.Server.Common.sln /p:Configuration=Release /p:Platform="Any CPU"
cd ..

mkdir -p Dependency\ Builds/GladLive.Server.Common/DLLs/
rsync -avv ./GladLive.Server.Common/src/GladLive.Server.Common/bin/Release/ Dependency\ Builds/GladLive.Server.Common/DLLs/






