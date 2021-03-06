# lucky-mushroom-backend
ASP.NET Core backend for Lucky Mushroom app

## How to run
1. Compile source with `dotnet build` command or just download and unpack compiled release
2. Deploy database with `mysql dbWithLuckyMushroomTables < lucky_mushroom_database.sql` command (replace `dbWithLuckyMushroomTables` with your database name)
3. In folder where release was unpacked, create `appsettings.json` file with such content (all the values are for example)
   ```json
   {
     "ConnectionStrings": {
       "LuckyMushroomDatabase": "server=localhost;user=yourDbUserName;password=yourDbUserPassword;database=dbWithLuckyMushroomTables"
     }
   }
   ```
4. Run server with `dotnet run LuckyMushroom.dll` command

## API schema

- ```/api/articles [GET]```
    - URL parameters: GPS coorinates ```latitudeSeconds [int], longitudeSeconds [int]```
    - return: ```arrayOf({ articleTitle [string], articleText [string], articleId [int], gpsTags arrayOf({ latitudeSeconds [int], longitudeSeconds [int] }) })```
- ```/api/articles/add [POST]```
    - body paramters: ```{ articleTitle [string], articleText [string], gpsTags arrayOf({ latitudeSeconds [int], longitudeSeconds [int] }) }```
    - return: ```{ articleText [string], articleId [int], gpsTags arrayOf({ latitudeSeconds [int], longitudeSeconds [int] }) }```
- ```/api/articles/delete [DELETE]```
    - URL paramters: ```id [uint]```
    - return: ```{ articleText [string], articleId [int], gpsTags arrayOf({ latitudeSeconds [int], longitudeSeconds [int] })```

- ```/api/recognitionRequests [GET]```
    - non-required URL paramters: ```edibleStatusAlias [string], recognitionStatusAlias [string], startDateTime [DateTime], endDateTime [DateTime]```
    - return: ```arrayOf({ requestId [int], requestDatetime [datetime], edibleStatus { edibleStatusAlias [string], edibleStatusDescription [string], edibleStatusId [int] }, recognitionStatus { recognitionStatusAlias [string], recognitionStatusName [string], recognitionStatusId [int] }, requestPhotos arrayOf({ photoId [int], photoExtension [string], photoData [string] }) OR requestPhoto { photoId [int], photoExtension [string], photoData [string] } })```
    - comment: ```photoData``` is Base64 string
- ```/api/recognitionRequests/{id} [GET]```
    - URL parameters: ```id [int]```
    - return: ```{ requestId [int], requestDatetime [datetime], edibleStatus { edibleStatusAlias [string], edibleStatusDescription [string], edibleStatusId [int] }, recognitionStatus { recognitionStatusAlias [string], recognitionStatusName [string], recognitionStatusId [int] }, requestPhotos arrayOf({ photoId [int], photoExtension [string], photoData [string] }) OR requestPhoto { photoId [int], photoExtension [string], photoData [string] } }```
    - comment: ```photoData``` is Base64 string
- ```/api/recognitionRequests/addFew [POST]```
    - body parameters: ```{ requestDatetime [datetime], edibleStatus { edibleStatusAlias [string] }, recognitionStatus { recognitionStatusAlias [string] }, requestPhotos arrayOf({ photoExtension [string], photoData [string] }) }```
    - return: ```{ requestId [int], requestDatetime [datetime], edibleStatus { edibleStatusAlias [string], edibleStatusDescription [string], edibleStatusId [int] }, recognitionStatus { recognitionStatusAlias [string], recognitionStatusName [string], recognitionStatusId [int] }, requestPhotos arrayOf({ photoId [int], photoExtension [string], photoData [string] }) }```
    - comment: ```photoData``` is Base64 string
- ```/api/recognitionRequests/add [POST]```
    - body parameters: ```{ requestDatetime [datetime], edibleStatus { edibleStatusAlias [string] }, recognitionStatus { recognitionStatusAlias [string] }, requestPhoto { photoExtension [string], photoData [string] } }```
    - return: ```{ requestId [int], requestDatetime [datetime], edibleStatus { edibleStatusAlias [string], edibleStatusDescription [string], edibleStatusId [int] }, recognitionStatus { recognitionStatusAlias [string], recognitionStatusName [string], recognitionStatusId [int] }, requestPhoto { photoId [int], photoExtension [string], photoData [string] } }```
    - comment: ```photoData``` is Base64 string
- ```/api/recognitionRequests/delete/{id} [DELETE]```
    - URL parameters: ```id [int]```
    - return: ```{ requestId [int], requestDatetime [datetime] }```

- ```/api/users/signup [POST]```
    - body parameters: ```{ userMail [string], userPasswordHash [string] }```
    - comment: hash is SHA-512
    - return: ```{ userId [int], role { roleAlias [string], roleName [string] }, userCredentials { userMail [string] } }```, auth cookie
- ```/api/users/login [POST]```
    - body parameters: ```{ userMail [string], userPasswordHash [string] }```
    - comment: hash is SHA-512
    - return: ```{ userId [int], role { roleAlias [string], roleName [string] }, userCredentials { userMail [string] } }```, auth cookie
- ```/api/users/logout [POST]```
    - comment: auth cookie will be reset
- ```/api/users/delete [DELETE]```
    - comment: auth cookie will be reset
