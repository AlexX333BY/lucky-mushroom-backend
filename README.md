# lucky-mushroom-backend
ASP.NET Core backend for Lucky Mushroom app

## API schema

- ```/api/articles [GET]```
    - URL parameters: GPS coorinates ```latitudeSeconds [int], longitudeSeconds [int]```
    - return: collection of ```(articleId [uint], articleText [string]```)
- ```/api/articles/add [POST]```
    - body paramters: ```text [string], latitude [int], longitude [int]```
    - return: ```articleId [uint], articleText [string]```
- ```/api/articles/delete [DELETE]```
    - URL paramters: ```id [uint]```
    - return: ```articleId [uint], articleText [string]```

- ```/api/recognitionRequests [GET]``` [TODO - add photos]
    - non-required URL paramters: ```edibleStatusAlias [string], recognitionStatusAlias [string]```
    - return: collection of (```requestId [uint], requestDatetime [datetime], edibleDescription [string], statusName [string]```)
- ```/api/recognitionRequests [GET]``` [TODO - add photos]
    - URL parameters: ```id [uint]```
    - return: ```requestId [uint], requestDatetime [datetime], edibleDescription [string], statusName [string]```
- ```/api/recognitionRequests/add [POST]``` [TODO]
- ```/api/recognitionRequests/delete [DELETE]```
    - URL parameters: ```id [uint]```
    - return: ```requestId [uint], requestDatetime [datetime], edibleDescription [string], statusName [string]```

- ```/api/users/signup [POST]```
    - body parameters: ```{ userMail [string], userPasswordHash [string] }```
    - comment: hash is SHA-512
    - return: ```{ userId [int], role { roleAlias [string], roleName [string] }, userCredentials { userMail [string } }```, auth cookie
- ```/api/users/signup [POST]```
    - body parameters: ```{ userMail [string], userPasswordHash [string] }```
    - comment: hash is SHA-512
    - return: ```{ userId [int], role { roleAlias [string], roleName [string] }, userCredentials { userMail [string } }```, auth cookie
- ```/api/users/logout [POST]```
    - comment: auth cookie will be reset
- ```/api/users/delete [DELETE]```
    - comment: auth cookie will be reset
