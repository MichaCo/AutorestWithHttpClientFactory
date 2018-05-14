# Client Generation
To generate the swagger client, run the following from command line (within the projects directory)

```
autorest --csharp --clear-output-folder --input-file=http://petstore.swagger.io/v2/swagger.json --override-client-name=PetStoreClient --add-credentials
```
