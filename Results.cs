
namespace Api_Project.Models
{
    public class Person
    {
        public string Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int Workouts_Per_Week { get; set; }

    }
    public class ResponseResult
    {
        public string id { get; set; }
        public double BMI { get; set; }
        public int Maintenance_Calories{ get; set; }
        public int carbs { get; set; }
        public int fat { get; set; }
        public int protein { get; set; }


    }

    public class StorageObject
    {
        public string id { get; set; }  
        public Person person { get; set; }
        public ResponseResult result { get; set; }
        public ResponseResult CalculateResults(string id, Person person)
        {
            var Results = new ResponseResult();
            Results.id = id;
            var heightParts = person.Height.Split('.');

            int feet = int.Parse(heightParts[0]);
            int inches = int.Parse(heightParts[1]);

            var heightCm = ((feet * 12) + inches) * 2.54;

            double intensity;
            if(person.Workouts_Per_Week == 0)
            {
                intensity = 1.2;
            } else if (person.Workouts_Per_Week <= 3){
                intensity = 1.375;
            } else if (person.Workouts_Per_Week <= 5){
                intensity = 1.55;
            } else if (person.Workouts_Per_Week <= 7){
                intensity = 1.725;
            } else
            {
                throw new ArgumentException("Invalid Workouts Per Week");
            }
            switch (person.Gender.ToUpper())
            {
                case "MALE":
                    Results.Maintenance_Calories = (int)Math.Round(((88.4 + (13.4 * (person.Weight/2.205))) + (4.8 * heightCm) - (5.68 * person.Age))*intensity);
                    break;

                case "FEMALE":
                    Results.Maintenance_Calories = (int)Math.Round(((447.6 + (9.25 * (person.Weight / 2.205))) + (3.1 * heightCm) - (4.33 * person.Age))*intensity);
                    break;
                default:
                    throw new ArgumentException();
                    break;
            }
            Results.BMI = (person.Weight / (Math.Pow(((feet * 12) + inches), 2))) * 703;
            Results.carbs = (int)Math.Round((Results.Maintenance_Calories * .55) / 4);
            Results.fat = (int)Math.Round((Results.Maintenance_Calories * .275) / 9);
            Results.protein = (int)Math.Round(person.Weight * .8);

            return Results;

        }
    }

}
