## artsie API 🖼️

An API that returns data from MongoDB on art, associated comments and users.

### Host this api locally
1. Clone this repo

```
git clone https://github.com/noepse/artsie-api.git
```

2. Navigate into the created folder

```
cd artsie-api
```

3. Create a [MongoDB Atlas Cluster](https://www.mongodb.com/docs/drivers/csharp/current/quick-start/) and note down your connection string

```
"mongodb+srv://<username>:<password>@cluster0.abc.mongodb.net/?retryWrites=true&w=majority"
```

3. Create an appsettings.Development.json file at the top level of the art-api folder and add the following. Replace the empty string under ConnectionString with your connection string. 

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ArtsieDatabase": {
    "ConnectionString": "[your connection string here]",
    "DatabaseName": "artsie",
    "ArtCollectionName": "art",
    "UsersCollectionName": "users",
    "CommentsCollectionName": "comments"
},
"AllowedHosts": "*" 
}

```

4. Create a .gitignore file in the root folder and add the following. This ensures your connection string is not added to future commits

```
art-api/appsettings.Development.json
```

5. Ensure your MongoDB database and collection names are set up to match the names outlined in your appsettings.json and appsettings.Development.json files. If you wish to populate the database with some default data, some data has been provided in the data folder. You can easily add this data directly using the [MongoDB Atlas UI](https://www.mongodb.com/docs/atlas/atlas-ui/documents/).

6. You are now ready to host the API locally. Ensure you are located in the root folder and run the following command

```
dotnet run --project art-api
```

Your terminal will display the URL where you can access the API e.g. http://localhost:(yourportnumber)
You can now view the endpoint documentation at http://localhost:(yourportnumber)/swagger


### Minimum requirements

1. .NET - version ^8.0 [download](https://dotnet.microsoft.com/en-us/download)

Check to see if .NET is already installed

```
dotnet --version
```

### Future plans
- host api and documentation online

### Contact

Feel free to reach out via any of the routes below.

- Fill out this [online form](https://simranamin.com/#contact)
- Connect with me on [LinkedIn](https://www.linkedin.com/in/simran-amin/)

### Useful resources
- build a .NET API with MongoDB - [tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-8.0)
- create Swagger documentation - [article](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-8.0)