namespace FootballLeague.Shared
{
    public class Constants
    {
        public const int WinPoints = 3;
        public const int DrawPoints = 1;
        public const int LosePoints = 0;

        // Success Messages 
        public const string GetSuccessMessage = "Successfully retrieved ${0}.";
        public const string PostSuccessMessage = "Successfully created ${0}.";
        public const string UpdateSuccessMessage = "Successfully updated ${0}.";
        public const string DeleteSuccessMessage = "Successfully deleted ${0}.";

        // Error Messages
        public const string InvalidIdErrorMessage = "No ${0} with the provided ID exists.";
        public const string InvalidNameErrorMessage = "No ${0} with the provided name exists.";
        public const string EntityAlreadyExistsErrorMessage = "There is already a ${0} with the provided ${1}.";
        public const string InvalidHomeTeamNameErrorMessage = "Team with the provided Home name does not exist.";
        public const string InvalidAwayTeamNameErrorMessage = "Team with the provided Away name does not exist.";
    }
}
