database command:
dotnet ef database drop --context ArticleContext --project Heysundue.csproj --force //刪除
dotnet ef migrations add "名字" --context ArticleContext --project Heysundue.csproj 
dotnet ef database update --context ArticleContext --project Heysundue.csproj //更新資料庫
