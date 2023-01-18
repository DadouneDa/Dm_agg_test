#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DmAggregator/DmAggregator.csproj", "DmAggregator/"]
RUN dotnet restore "DmAggregator/DmAggregator.csproj"
COPY . .
WORKDIR "/src/DmAggregator"
RUN dotnet build "DmAggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DmAggregator.csproj" -c Release -o /app/publish

FROM base AS final
#
## Install OpenSSH and set the password for root to "Docker!". In this example, "apk add" is the install instruction for an Alpine Linux-based image.
#RUN apt-get update && apt-get install -y openssh-server \
     #&& echo "root:Docker!" | chpasswd 
#
## Copy the sshd_config file to the /etc/ssh/ directory
#COPY DmAggregator/SSHD/sshd_config /etc/ssh/
#
## Copy and configure the ssh_setup file
#RUN mkdir -p /tmp
#COPY DmAggregator/SSHD/ssh_setup.sh /tmp
#RUN chmod +x /tmp/ssh_setup.sh \
    #&& (sleep 1;/tmp/ssh_setup.sh 2>&1 > /dev/null)
#
## Open port 2222 for SSH access
#EXPOSE 2222
#
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["/bin/bash", "-c", "/usr/sbin/sshd && dotnet DmAggregator.dll"]
ENTRYPOINT ["dotnet", "DmAggregator.dll"]
