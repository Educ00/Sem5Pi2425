namespace Sem5Pi2425.Domain.Categories
{
    public class CreatingCategoryDto
    {
        public string Description { get; set; }


        public CreatingCategoryDto(string description)
        {
            this.Description = description;
        }
    }
}