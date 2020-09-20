# ProinterBackendTestProject
This repo contains all code necessary to implement the provided task (DB &amp; Backend)



### Prerequirements

#### 1. Redis setup

* Install Redis using the provided installer  (`Redis-x64-3.0.504.msi`) or via [this](https://github.com/MicrosoftArchive/redis/releases/download/win-3.0.504/Redis-x64-3.0.504.msi)  link
* Open Windows Services and run Redis service or open the location where Redis was installed (`C:\Program Files\Redis`) and start the server by executing `redis-server.exe` 

#### 2. DB Setup

* Execute `script.sql` 

   

### Backend app

⚠Make sure that you have **ASP.NET and web development** Workload installed in Visual Studio. 

```markdown
| Request |         Route        |                   Description                   |
|:-------:|:--------------------:|:-----------------------------------------------:|
|   POST  | api/user/register    | Creates user                                    |
|   POST  | api/user/login       | Login user. Returns JWT                         |
|   GET   | api/user             | Returns list of users                           |
|   GET   | api/user/{id}        | Returns user with the specified {id}            |
|   POST  | api/user/logout/{id} | Log out user. Returns revoked JWT ID            |
|  PATCH  | api/user/{id}        | Updates user with the specified body parameters |
|  DELETE | api/user/{id}        | Deletes user with the specified {id}            |
|   GET   | api/user/tasks       | Returns all tasks for active user               |
|         |                      |                                                 |
|   POST  | api/taskitems        | Creates task                                    |
|   GET   | api/taskitems/{id}   | Returns task with the specified {id}            |
|  PATCH  | api/taskitems/{id}   | Updates task with the specified body parameters |
|  DELETE | api/taskitems/{id}   | Deletes task with the specified {id}            |
```



