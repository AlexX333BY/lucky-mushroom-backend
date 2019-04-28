# lucky-mushroom-backend
ASP.NET Core backend for Lucky Mushroom app

## API schema

- ```/api/articles [GET]```
    - URL parameters: GPS coorinates ```latitudeSeconds [int], longitudeSeconds [int]```
    - return: ```arrayOf({ articleText [string], articleId [int], gpsTags arrayOf({ latitudeSeconds [int], longitudeSeconds [int] }) })```
- ```/api/articles/add [POST]```
    - body paramters: ```{ articleText [string], gpsTags arrayOf({ latitudeSeconds [int], longitudeSeconds [int] }) }```
    - return: ```{ articleText [string], articleId [int], gpsTags arrayOf({ latitudeSeconds [int], longitudeSeconds [int] }) }```
- ```/api/articles/delete [DELETE]```
    - URL paramters: ```id [uint]```
    - return: ```{ articleText [string], articleId [int], gpsTags arrayOf({ latitudeSeconds [int], longitudeSeconds [int] })```

- ```/api/recognitionRequests [GET]```
    - non-required URL paramters: ```edibleStatusAlias [string], recognitionStatusAlias [string]```
    - return: ```arrayOf({ requestId [int], requestDatetime [datetime], edibleStatus { edibleStatusAlias [string], edibleStatusDescription [string], edibleStatusId [int] }, recognitionStatus { recognitionStatusAlias [string], recognitionStatusName [string], recognitionStatusId [int] }, requestPhotos arrayOf({ photoId [int], photoExtension [string], photoData [string] }) })```
    - comment: ```photoData``` is Base64 string
- ```/api/recognitionRequests/{id} [GET]```
    - URL parameters: ```id [int]```
    - return: ```{ requestId [int], requestDatetime [datetime], edibleStatus { edibleStatusAlias [string], edibleStatusDescription [string], edibleStatusId [int] }, recognitionStatus { recognitionStatusAlias [string], recognitionStatusName [string], recognitionStatusId [int] }, requestPhotos arrayOf({ photoId [int], photoExtension [string], photoData [string] }) }```
    - comment: ```photoData``` is Base64 string
- ```/api/recognitionRequests/add [POST]```
    - body parameters: ```{ requestDatetime [datetime], edibleStatus { edibleStatusAlias [string] }, recognitionStatus { recognitionStatusAlias [string] }, requestPhotos arrayOf({ photoExtension [string], photoData [string] }) }```
    - return: ```{ requestId [int], requestDatetime [datetime], edibleStatus { edibleStatusAlias [string], edibleStatusDescription [string], edibleStatusId [int] }, recognitionStatus { recognitionStatusAlias [string], recognitionStatusName [string], recognitionStatusId [int] }, requestPhotos arrayOf({ photoId [int], photoExtension [string], photoData [string] }) }```
    - comment: ```photoData``` is Base64 string
- ```/api/recognitionRequests/delete/{id} [DELETE]```
    - URL parameters: ```id [int]```
    - return: ```{ requestId [int], requestDatetime [datetime] }```

- ```/api/users/signup [POST]```
    - body parameters: ```{ userMail [string], userPasswordHash [string] }```
    - comment: hash is SHA-512
    - return: ```{ userId [int], role { roleAlias [string], roleName [string] }, userCredentials { userMail [string] } }```, auth cookie
- ```/api/users/signup [POST]```
    - body parameters: ```{ userMail [string], userPasswordHash [string] }```
    - comment: hash is SHA-512
    - return: ```{ userId [int], role { roleAlias [string], roleName [string] }, userCredentials { userMail [string] } }```, auth cookie
- ```/api/users/logout [POST]```
    - comment: auth cookie will be reset
- ```/api/users/delete [DELETE]```
    - comment: auth cookie will be reset
