namespace Utils;
public class Helper{

    public static bool IsTestEnvironment()
    {
        return Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") == "true";
    }

}
