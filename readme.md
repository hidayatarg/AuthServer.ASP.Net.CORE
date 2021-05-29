Private Set : Helps to set a property via a method. You dont need private set while you set with a constructor.

### Docker MSSQL Server
Download docker image for sql server and activate it
```docker
docker pull microsoft/mssql-server-linux
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=ProductApi(!)' -e 'MSSQL_PID=Express' -p 1433:1433 --name=catalog microsoft/mssql-server-linux
docker start catalog

```

### Docker Containers Tricks
-   to list or check active containers `docker ps`
-   to list all containers `docker ps -a`
-   to start a container `docker start CONTAINER_ID`

### Migration
It will create the database with name of mentioned in API `AppSetting.json`


### Best Practices
Try catch are not used in controller there will be a middleware class to catch the exceptions.
