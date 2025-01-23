namespace MicroServices.Shared.Models
{
    public static class StaticMessages
    {
        public static string InternalServerError => "Ooh no! An unforeseen error has occured! Please try again in a few minutes";
        public static string ServiceUnreachable => "The requested service is currently unreachable";
        public static string ResourceNotFound => "The requested resource was not found";
        public static string Forbidden => "You are not authorized to access this module";
    }
}