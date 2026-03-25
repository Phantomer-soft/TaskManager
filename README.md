###  TaskManager API 
Kullanıcıların hesap oluşturup bu hesaba giriş yapabildiği, hesap içerisinde projeler oluşturup projelere görev atayabildği, farklı kullanıcıları ekleyebildiği bir REST API

### Teknolojiler 
- .NET 8
- PostgreSQL
- JWT
- Serilog
- EfCore
### Docker İle Kurulum
1) Repoyu Klonla => git clone https://github.com/Phantomer-soft/TaskManager.git
2) Konteyner build et =>  docker build -t taskmanager . 
3) Konteyneri çalıştır => docker run taskmanager
4) Konteyner id bulunur => docker ps a 
5) Tarayıcıda =>  konteynerid:8080/swagger
6) Kontrol edilebilir 


### KURULUM 

1) Repoyu Klonla   =>   git clone https://github.com/Phantomer-soft/TaskManager.git
2) Repoya Gir
3) appsettings.json ayarla

========ÖRNEK APPSETTINGS.JSON===============
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": 
  {
    "PostgresConnection":"Host=localhost;Port=5432;Database=taskDb;Username=taskmanageradmin;Password=1Task1Manager1!"
  },
  "Jwt_Token":
  {
    "SecretKey" : " Son derece guvenlikli super gizli bir sifre olsa bile cozulebilir",
    "Issuer": "TaskManagerApi",
    "Audience": "TaskManagerClient",
    "ExpirationMins": 10,
    "RefreshTokenExpirationHours": 24
  }
}
==============================================


4) Migration oluştur     =>     dotnet ef migrations add CreateDbFirst
5) Migration uygula      =>     dotnet ef database update
6) Çalıştır              =>     dotnet run 

### Endpointler

POSTMAN DAVETİ : https://yty51757-1399428.postman.co/workspace/Yt-Yt's-Workspace~bc27a9f5-a1db-4dba-84af-553a828622f9/collection/49965296-7d3fb0fc-ec11-4e50-84d3-4ce982248332?action=share&source=copy-link&creator=49965296

Tüm korumalı endpointler `Authorization: Bearer <token>` header'ı gerektirir.
Register,Login,Refresh dışındaki tüm endpointler korumalıdır.

### Kullanıcı İşlemleri

| POST  `/api/User/Register` | Yeni kullanıcı kaydı
| POST  `/api/User/Login` | Giriş yap, token al 
| POST  `/api/User/RefreshToken` | Access token yenile 
| PUT   `/api/User/UpdatePassword` | Şifre güncelle 
| POST   `/api/User/LogoutAll` | Tüm oturumları kapat 

### Proje İşlemleri

| POST    `/api/Project` | Yeni proje oluştur 
| GET     `/api/Project` | Projeleri listele
| GET     `/api/Project/ProjectInfo/{projectId}` | Proje detayını getir 
| PUT     `/api/Project/{projectId}` | Proje güncelle 
| DELETE  `/api/Project/{projectId}` | Proje sil
| POST    `/api/Project/adduser`     | Projeye katılımcı ekle
| GET     `/api/project/join{pincode}` | Projeye katıl

### Görev İşlemleri

| POST  `/api/Project/{projectId}/Tasks` | Görev ekle (liste) 
| PUT  `/api/Project/{taskId}/Tasks` | Görev güncelle
| DELETE  `/api/Project/{taskId}/Tasks` | Görev sil 

## Örnek İstekler

### Kayıt

```http
POST /api/User/Register
Content-Type: application/json

{
  "userName": "testuser",
  "password": "Test123.",
  "email": "test@example.com",
  "firstName": "Test",
  "lastName": "User"
}
```

### Giriş

```http
POST /api/User/Login
Content-Type: application/json

{
  "userName": "testuser",
  "password": "Test123."
}
```

### Proje Oluştur

```http
POST /api/Project
Authorization: Bearer <token>
Content-Type: application/json

{
  "header": "Proje Başlığı",
  "description": "Proje açıklaması"
}
```

### Görev Ekle

```http
POST /api/Project/{projectId}/Tasks
Authorization: Bearer <token>
Content-Type: application/json

[
  { "description": "Görev 1" },
  { "description": "Görev 2" }
]
```

### Token Yenile

```http
POST /api/User/RefreshToken
Content-Type: application/json

{
  "refreshToken": "refresh_token_degeri"
}
```

## Notlar

- Access token süresi kısadır, süresi dolunca `/RefreshToken` endpoint'i ile yenilenebilir
- Şifre değiştirildiğinde tüm refresh tokenlar geçersiz kılınır
- Proje silme işlemi yalnızca proje sahibi tarafından yapılabilir
- Görev ekleme ve güncelleme işlemleri `Admin` ve `Participant` rolüne sahip kullanıcılar tarafından yapılabilir
- Katılımcı ekleme işlemleri sadece `Admin` rolüne sahip kullanıcılar tarafından yapılabilir

