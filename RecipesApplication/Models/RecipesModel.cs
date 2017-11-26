using MongoDB.Bson;
using System.Collections.Generic;

namespace RecipesApplication.Models
{
    public class RecipesModel
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public List<Ingredients> Ingredients { get; set; }
        public List<Steps> Method { get; set; }
        public int UserId { get; set; }
    }

    public class Steps
    {
        public int StepNumber { get; set; }
        public string Description { get; set; }
    }

    public class Ingredients
    {
        public string Description { get; set; }
        public string Amount { get; set; }
    }
}
