# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj và restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ code và build
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Expose port mặc định của .NET 8 (thường là 8080)
EXPOSE 8080

# Sửa lỗi chữ tiếng Nhật "--バランスfrom" thành "--from"
COPY --from=build /app/out .

# Thay đổi tên file DLL cho đúng với project của anh (backend.dll)
ENTRYPOINT ["dotnet", "backend.dll"]
