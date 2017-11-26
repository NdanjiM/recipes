using Microsoft.AspNetCore.Mvc;
using RecipesApplication.Database;
using RecipesApplication.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipesApplication
{
    [Route("api/[controller]")]
    public class RecipesController : Controller
    {
        [Route("recipes")]
        [HttpGet]
        public IActionResult GetAllRecipes()
        {
            RecipesDal db = new RecipesDal();
            
            var recipes = db.GetAllRecipesAsync(); 
            return Ok(recipes.Result);
        }

        [Route("recipe")]
        public IActionResult GetRecipe(string title)
        {
            RecipesDal db = new RecipesDal();
            RecipesModel recipe = db.GetRecipeAsync(title).Result;
            return Ok(recipe);
        }

        [Route("addRecipe")]
        [HttpPost]
        public IActionResult AddRecipe(string userId, RecipesModel recipe)
        {
          RecipesDal db = new RecipesDal();
          var user = db.AddRecipeAsync(userId, recipe);
          return Ok(user);
        }

    [Route("updateRecipe")]
    [HttpPost]
    public IActionResult UpateRecipe(string userId, RecipesModel recipe)
    {
      RecipesDal db = new RecipesDal();
      var updated = db.UpdateRecipeAsync(userId, recipe);
      return Ok(updated.Result);
    }

    [Route("DeleteRecipe")]
    [HttpPost]
    public IActionResult AddUser(string userId, RecipesModel recipe)
    {
      RecipesDal db = new RecipesDal();
      var deleted = db.RemoveRecipeAsync(userId, recipe);
      return Ok(deleted.Result);
    }

    [Route("addUser")]
        [HttpPost]
        public IActionResult AddUser(User newUser)
        {
            RecipesDal db = new RecipesDal();
            var user = db.InsertUser(newUser);
            return Ok(user);
        }

        [Route("login")]
        [HttpPost]
        public IActionResult VerifyUser(User user)
        {
          RecipesDal db = new RecipesDal();
          var isUser = db.VerifyUserAsync(user);
          return Ok();
        }
    }
}
