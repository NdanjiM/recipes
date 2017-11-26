using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RecipesApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipesApplication.Database
{
    public class RecipesDal
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public RecipesDal()
        {
            var credential = MongoCredential.CreateCredential("recipes-app-db", "RecipesAdmin", "8@=cYy5yrk4UPBvs");

            var mongoClientSettings = new MongoClientSettings
            {
              Server = new MongoServerAddress("ds042687.mlab.com", 42687),
              Credentials = new List<MongoCredential> { credential }
            };

            _client = new MongoClient(mongoClientSettings);
            _database = _client.GetDatabase("recipes-app-db");
        }

        public bool InsertUser(User user)
        {
            var document = new BsonDocument{
                {"name",user.Name },
                {"surname", user.Surname },
                {"username", user.Username},
                {"password",user.Password }
            };

            var collection = _database.GetCollection<BsonDocument>("Users");
            collection.InsertOne(document);

            return true;
        }

    public async System.Threading.Tasks.Task<bool> VerifyUserAsync(User user)
    {
      var collection = _database.GetCollection<BsonDocument>("Users");

      var filter = new BsonDocument("Username", user.Username);

      var result = await collection.Find(filter).ToListAsync();

      var returnedUser = BsonSerializer.Deserialize<User>(result.FirstOrDefault());

      if (returnedUser == null)
        return false;
      
      return returnedUser.Password==user.Password;
    }

    public async System.Threading.Tasks.Task AddRecipeAsync(string username, RecipesModel recipe)
        {
            var userId = GetUserId(username);
            recipe.UserId = userId;

            BsonDocument newRecipe = ExtractRecipe(recipe);

            var collection = _database.GetCollection<BsonDocument>("Recipes");
            await collection.InsertOneAsync(newRecipe);
        }

    public async System.Threading.Tasks.Task<bool> RemoveRecipeAsync(string username, RecipesModel recipe)
    {
          var userId = GetUserId(username);

          var collection = _database.GetCollection<BsonDocument>("Recipes");

          var builder = Builders<BsonDocument>.Filter;
          var filter = builder.Eq("UserId", userId) & builder.Eq("Title", recipe.Title);

          var result = await collection.DeleteManyAsync(filter);

        return result.IsAcknowledged;
    }

    public async System.Threading.Tasks.Task<bool> UpdateRecipeAsync(string username, RecipesModel recipe)
    {
      var userId = GetUserId(username);

      var collection = _database.GetCollection<BsonDocument>("Recipes");

      var builder = Builders<BsonDocument>.Filter;
      var filter = builder.Eq("UserId", userId) & builder.Eq("Title", recipe.Title);

      var update = Builders<BsonDocument>.Update
          .Set("Title", recipe.Title)
          .Set("Notes", recipe.Notes)
          .Set("Ingredients", recipe.Ingredients.ToBsonDocument())
          .Set("Method", recipe.Method.ToBsonDocument());

      var result = await collection.UpdateOneAsync(filter, update);

      return result.IsAcknowledged;
    }

    public async System.Threading.Tasks.Task<List<string>> GetAllRecipesAsync()
    {
      var collection = _database.GetCollection<BsonDocument>("Recipes");
      List<string> recipes = new List<string>();

      using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(new BsonDocument()))
      {
        while (await cursor.MoveNextAsync())
        {
          IEnumerable<BsonDocument> batch = cursor.Current;
          foreach (BsonDocument document in batch)
          {
            var recipe = document.GetValue("Title");
            recipes.Add(Convert.ToString(recipe));
          }
        }
      }

      return recipes;
    }

    internal async System.Threading.Tasks.Task<RecipesModel> GetRecipeAsync(string title)
    {
      var collection = _database.GetCollection<BsonDocument>("Recipes");
      
      var filter = new BsonDocument("Title", title);

      var result = await collection.Find(filter).ToListAsync();

      var recipe = BsonSerializer.Deserialize<RecipesModel>(result.FirstOrDefault());

      return recipe;
    }

        private int GetUserId(string username)
        {
            var collection = _database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
            var result = collection.Find(filter).ToList();
            if (result == null || result.Count == 0) return -1;
            else return Convert.ToInt32(result[0].GetValue("UserId"));
        }

        private static BsonDocument ExtractRecipe(RecipesModel recipe)
        {
            var method = new List<BsonDocument>();
            foreach (var s in recipe.Method)
            {
                var step = new BsonDocument
                {
                    {"Step", s.StepNumber},
                    {"Description", s.Description }
                };
                method.Add(step);
            }

            var ingredients = new List<BsonDocument>();
            foreach (var i in recipe.Ingredients)
            {
                var ingredient = new BsonDocument
                {
                    {"Description",i.Description },
                    {"Amount",i.Amount }
                };
            }

            var newRecipe = new BsonDocument
            {
                {"Title",recipe.Title },
                {"Method", new BsonArray(method.ToArray()) },
                {"Ingredients", new BsonArray(ingredients.ToArray())},
                {"Notes", recipe.Notes },
                {"UserId", recipe.UserId }
            };
            return newRecipe;
        }
  }
}
